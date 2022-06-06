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

            SendUserStatus(user);                                    // Sending User status

            SendLobbyStatus();                                       // Sending Lobby status

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
            bool roomFull = room.AddUser(user, enterMoney);
            await DbContext.SaveChangesAsync();
            if (!roomFull) 
            {
                await DbContext.SaveChangesAsync();
                await Clients.Clients(GetUserConnections(user)).SendAsync("Alert", "Room is full!");
                return;
            }

            // If enough players start game
            if ( room.Users.Count() == 2 && room.Stage == GameStage.Stopped )
            {
                room.StartGame();
                ResetTimer(room.Id);
            }

            await DbContext.SaveChangesAsync();

            SendRoomStatus(room);             // Sending everyone in the room the status

            SendLobbyStatus();                // Sending everyone in lobby status
        }

        public async Task LeaveRoom()
        {
            // Getting the user and room
            User user = GetUserByConnectionId();
            if (user == null || user.UserInGame == null)
                return;                             // Verifying user and room exist

            Room room = user.UserInGame.Room;

            bool gameRunning = room.RemoveUser(user);
            await DbContext.SaveChangesAsync();
            if(!gameRunning)
            {
                // Game Ended
                await HandleEndedGame(room);
            }

            // Sending everyone in the room the status
            if (room.Users.Count() == 0)
            {
                RemoveTimer(room.Id);
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

        public async Task ReceiveFold()
        {
            // Getting the user and room
            User user = GetUserByConnectionId();
            if (user == null || user.UserInGame == null)
                return;                             // Verifying user and room exist

            Room room = user.UserInGame.Room;

            if (room.TalkingPosition != user.UserInGame.Position)
                return;                  // Validating it's the player's turn

            ResetTimer(room.Id);

            bool gameRunning = room.Fold(user.UserInGame);
            DbContext.SaveChanges();
            if (!gameRunning)
            {
                // Game Ended
                await HandleEndedGame(room);
            }

            SendRoomStatus(room);             // Sending everyone in the room the status
        }

        public async Task ReceiveCall()
        {
            // Getting the user and room
            User user = GetUserByConnectionId();
            if (user == null || user.UserInGame == null)
                return;                             // Verifying user and room exist

            Room room = user.UserInGame.Room;

            if (room.TalkingPosition != user.UserInGame.Position)
                return;                  // Validating it's the player's turn

            ResetTimer(room.Id);

            bool gameRunning = room.Call(user.UserInGame);
            DbContext.SaveChanges();
            if (!gameRunning)
            {
                // Game ended
                await HandleEndedGame(room);
            }

            SendRoomStatus(room);             // Sending everyone in the room the status
        }

        public async Task ReceiveRaise(int amount)
        {
            // Getting the user and room
            User user = GetUserByConnectionId();
            if (user == null || user.UserInGame == null)
                return;                  // Verifying user and room exist

            Room room = user.UserInGame.Room;

            if (room.TalkingPosition != user.UserInGame.Position)
                return;                  // Validating it's the player's turn

            ResetTimer(room.Id);

            bool gameRunning = room.Raise(user.UserInGame, amount);
            DbContext.SaveChanges();
            if (!gameRunning)
            {
                // Game ended
                await HandleEndedGame (room);
            }

            SendRoomStatus(room);             // Sending everyone in the room the status
        }

        public async Task ReceiveCheck()
        {
            // Getting the user and room
            User user = GetUserByConnectionId();
            if (user == null || user.UserInGame == null)
                return;                   // Verifying user and room exist

            Room room = user.UserInGame.Room;

            if (room.TalkingPosition != user.UserInGame.Position)
                return;                  // Validating it's the player's turn

            ResetTimer(room.Id);

            bool gameRunning = room.Check(user.UserInGame);
            DbContext.SaveChanges();
            if (!gameRunning)
            {
                // Game ended
                await HandleEndedGame(room);
            }

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

                if (room.Fold(talkingUser))
                {
                    // Game Ended
                    await HandleEndedGame(room);
                    ResetTimer(room.Id);
                }
            }
            else if(room.Stage == GameStage.Finished)
            {
                System.Timers.Timer timer;
                if (!Timers.TryGetValue(room.Id, out timer)) return;   // Verifying timer exist

                room.ResetGame();
                if (room.Users.Count() > 1)
                {
                    room.StartGame();
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

        private void RemoveTimer(string roomId)
        {
            System.Timers.Timer timer;
            if (!Timers.TryGetValue(roomId, out timer)) return;   // Verifying timer exist
            timer.Dispose();
            ConnectionIds.Remove(roomId);
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
            await DbContext.SaveChangesAsync();
        }
    }
}
