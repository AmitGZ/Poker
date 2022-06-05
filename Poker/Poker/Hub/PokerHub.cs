using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using PokerClassLibrary;
using Poker.DataModel.Dto;
using Poker.DataModel;
using Microsoft.AspNetCore.SignalR;
using System;
using LinqToDB;
using System.Diagnostics;
using BluffinMuffin.HandEvaluator;

namespace Poker.Hubs
{
    public class PokerHub : Hub
    {
        private readonly PokerContext DbContext;
        private readonly IDictionary<string, string> ConnectionIds;
        private static readonly IDictionary<string, System.Timers.Timer> Timers = new Dictionary<string, System.Timers.Timer>();
        private readonly IHubContext<PokerHub> HubContext;
        private readonly int TimerInterval = Constants.TurnTime * 1000;

        public PokerHub
        (
            PokerContext dbContext,
            IDictionary<string, string> connectionIds,
            IHubContext<PokerHub> hubContext
        )
        {
            DbContext = dbContext;
            ConnectionIds = connectionIds;
            HubContext = hubContext;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionIds.Remove(Context.ConnectionId);

            // TODO handle disconnect
            return Task.CompletedTask;
        }

        //TODO not working
        public Task SignOut()
        {
            return Task.CompletedTask;
        }

        public async Task SignIn(string username, string password)
        {
            // Checking if user exists and verifying
            User user = DbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || user.Password != password)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("Alert", "Invalid username or password");
                return;
            }

            ConnectionIds.Add(Context.ConnectionId, user.Username);  // Adding to connectionIds list

            SendLobbyStatus();                                       // Sending Lobby status

            SendUserStatus(user);                                    // Sending User's status

            if (user.UserInGame != null)
            {
                SendRoomStatus(user.UserInGame.Room);                // Sending Room status
            }

            await DbContext.SaveChangesAsync();
        }

        public async Task JoinRoom(string roomId, int enterMoney)
        {
            // Getting the user, room, and players in room
            User user = GetUserByConnectionId();
            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == roomId);
            if (user == null || room == null) 
                return;                 // Verifying user and room exist 

            // Trying to add user to room
            if (!(await room.AddUser(DbContext, user, enterMoney))) 
            {
                await Clients.Clients(GetUserConnections(user)).SendAsync("Alert", "Room is full!");
                return;
            }

            // If enough players start game
            if ( room.Users.Count() == 2 && room.Stage == GameStage.Stopped )
            {
                await room.StartGame(DbContext);
                ResetTimer(room.Id);
            }

            SendRoomStatus(room);             // Sending everyone in the room the status

            SendLobbyStatus();                // Sending everyone in lobby status

            SendUserStatus(user);             // Sending User's status
        }

        public async Task LeaveRoom()
        {
            // Getting the user and room
            User user = GetUserByConnectionId();
            if (user == null || user.UserInGame == null)
                return;                             // Verifying user and room exist

            Room room = user.UserInGame.Room;

            if (room.Stage != GameStage.Stopped)
                if (!(await room.Fold(DbContext, user.UserInGame))) // Folding player
                {
                    // Game Ended
                    await HandleEndedGame (room);
                }

            // Returning player to lobby
            room.Users.Remove(user.UserInGame);
            user.Money += (int)user.UserInGame.MoneyInTable;
            await DbContext.SaveChangesAsync();

            // Sending everyone in the room the status
            List<User> playersInRoom = DbContext.Users.Where(u => u.UserInGame.Room.Id == room.Id).ToList();
            if (playersInRoom.Count() == 0)
            {
                DbContext.Rooms.Remove(room);
                await DbContext.SaveChangesAsync();
            }
            else
            {
                SendRoomStatus(room);
            }
            
            SendLobbyStatus();                // Sending everyone in lobby status

            SendUserStatus(user);             // Sending User's status
        }

        public async Task CreateRoom(string roomName, int enterMoney)
        {
            Room room = new Room() { Name = roomName };
            DbContext.Rooms.Add(room);
            await DbContext.SaveChangesAsync();
            await JoinRoom(room.Id, enterMoney);
            CreateTurnTimer(room.Id);
        }

        public Task SendMessage(string message)
        {
            // Getting the user and room
            User user = GetUserByConnectionId();
            if (user == null || user.UserInGame == null)
                return null;                             // Verifying user and room exist

            Room room = user.UserInGame.Room;

            //sending a message to all users in current room
            room.Users.ForEach(u=>
                Clients.Clients(GetUserConnections(u.User)).SendAsync("ReceiveMessage", user.Username, message)
            );
            return Task.CompletedTask;
        }

        // "Synchrnoenous" fold received from fold button
        public async Task Fold()
        {
            // Getting the user and room
            User user = GetUserByConnectionId();
            if (user == null || user.UserInGame == null)
                return;                             // Verifying user and room exist

            Room room = user.UserInGame.Room;

            ResetTimer(room.Id);

            if (room.TalkingPosition != user.UserInGame.Position)
                return;                  // Validating it's the player's turn

            if(!(await room.Fold(DbContext, user.UserInGame)))
            {
                // Game Ended
                await HandleEndedGame(room);
            }

            SendUserStatus(user);             // Sending User's status

            SendRoomStatus(room);             // Sending everyone in the room the status
        }

        public async Task Call()
        {
            // Getting the user and room
            User user = GetUserByConnectionId();
            if (user == null || user.UserInGame == null)
                return;                             // Verifying user and room exist

            Room room = user.UserInGame.Room;

            ResetTimer(room.Id);

            if (room.TalkingPosition != user.UserInGame.Position)
                return;                  // Validating it's the player's turn

            if(!(await room.Call(DbContext, user.UserInGame)))
            {
                // Game ended
                await HandleEndedGame (room);
            }

            SendUserStatus(user);             // Sending User's status

            SendRoomStatus(room);             // Sending everyone in the room the status
        }

        public async Task Raise(int amount)
        {
            // Getting the user and room
            User user = GetUserByConnectionId();
            if (user == null || user.UserInGame == null)
                return;                  // Verifying user and room exist

            Room room = user.UserInGame.Room;

            ResetTimer(room.Id);

            if (room.TalkingPosition != user.UserInGame.Position)
                return;                  // Validating it's the player's turn

            if(!(await room.Raise(DbContext, user.UserInGame, amount)))
            {
                // Game ended
                await HandleEndedGame (room);
            }

            SendUserStatus(user);             // Sending User's status

            SendRoomStatus(room);             // Sending everyone in the room the status
        }

        public async Task Check()
        {
            // Getting the user and room
            User user = GetUserByConnectionId();
            if (user == null || user.UserInGame == null)
                return;                   // Verifying user and room exist

            Room room = user.UserInGame.Room;

            ResetTimer(room.Id);

            if (room.TalkingPosition != user.UserInGame.Position)
                return;                  // Validating it's the player's turn

            if(!(await room.Check(DbContext, user.UserInGame)))
            {
                // Game ended
                await HandleEndedGame(room);
            }

            SendUserStatus(user);             // Sending User's status

            SendRoomStatus(room);             // Sending everyone in the room the status
        }

        private void SendRoomStatus(Room room)
        {
            // Sending everyone in the room the status
            foreach (UserInGame u in room.Users)
            {
                List<string> tmpUserConnectionIds = GetUserConnections(u.User);
                Clients.Clients(tmpUserConnectionIds).SendAsync("RoomStatus", new RoomDto(room, u.User));
                SendUserStatus(u.User);
            }
        }

        private void SendUserStatus(User user)
        {
            List<string> userConnectionIds = GetUserConnections(user);
            HubContext.Clients.Clients(userConnectionIds).SendAsync("UserStatus", new UserDto(user));
        }

        private List<string> GetUserConnections(User user)
        {
            return ConnectionIds.Where(d => d.Value == user.Username).Select(d => d.Key).ToList();
        }

        private async void SendLobbyStatus()
        {
            await Clients.All.SendAsync("AllRoomsStatus", new LobbyDto(DbContext.Rooms.ToList()));
        }

        private User GetUserByConnectionId()
        {
            string username;
            if (!ConnectionIds.TryGetValue(Context.ConnectionId, out username))
            {
                return null;                 // Verifying room and user exist
            }
            return (DbContext.Users.FirstOrDefault(u => u.Username == username));
        }

        private System.Timers.Timer CreateTurnTimer(string roomId)
        {
            // TODO delete timer with room
            var timer = new System.Timers.Timer();
            timer.Interval = TimerInterval;
            timer.Elapsed += async (sender, e) => await TimerElapsed(sender, e, roomId);
            Timers.Add(roomId, timer);
            return timer;
        }

        private async Task TimerElapsed(object sender, System.Timers.ElapsedEventArgs e, string roomId)
        {
            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null) return; // Validating room exists
            
            if (room.Stage != GameStage.Stopped && room.Stage != GameStage.Finished)
            {
                UserInGame talkingUser = room.Users.Where(u => u.Position == room.TalkingPosition).FirstOrDefault();
                if (talkingUser == null)
                    return;

                if (!(await room.Fold(DbContext, talkingUser)))
                {
                    // Game Ended
                    await HandleEndedGame(room);
                    ResetTimer(room.Id);
                    room.Stage = GameStage.Finished;
                }
            }
            else if(room.Stage == GameStage.Finished)
            {
                System.Timers.Timer timer;
                if (!Timers.TryGetValue(room.Id, out timer)) return;   // Verifying timer exist

                room.ResetGame();
                if (room.Users.Count() > 1)
                {
                    await room.StartGame(DbContext);
                    ResetTimer(room.Id);
                }
            }

            // Sending everyone in the room the status
            foreach (UserInGame u in room.Users)
            {
                List<string> tmpUserConnectionIds = GetUserConnections(u.User);
                await HubContext.Clients.Clients(tmpUserConnectionIds).SendAsync("RoomStatus", new RoomDto(room, u.User));
                SendUserStatus(u.User);
            }
        }

        private void ResetTimer(string roomId)
        {
            System.Timers.Timer timer;
            if (!Timers.TryGetValue(roomId, out timer)) return;   // Verifying timer exist

            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null) return;                             // Verifying room exists

            timer.Stop();
            if (room.Stage != GameStage.Stopped)
            {
                timer.Start();
            }
        }

        private async Task HandleEndedGame(Room room)
        {
            ResetTimer(room.Id);
            UserInGame winner = room.Users.FirstOrDefault(u => u.IsWinner == true);
            foreach (UserInGame u in room.Users)
            {
                List<string> tmpUserConnectionIds = GetUserConnections(u.User);
                // await HubContext.Clients.Clients(tmpUserConnectionIds).SendAsync("Alert", (winner.Username + " " + winner.BestHand));
            }
            room.Stage = GameStage.Finished;
            await DbContext.SaveChangesAsync();
            return;
        }
    }
}
