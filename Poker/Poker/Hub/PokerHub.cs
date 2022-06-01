using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using Poker;
using System.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using PokerClassLibrary;
using System.Diagnostics;
using Poker.DataModel.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Poker.DataModel;

namespace Poker.Hubs
{
    public class PokerHub : Hub
    {
        private PokerContext DbContext;
        private readonly IDictionary<string, string> ConnectionIds;

        //private SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=Poker;Integrated Security=True");
        public PokerHub(PokerContext dbContext, IDictionary<string, string> connectionIds)
        {
            DbContext = dbContext;
            ConnectionIds = connectionIds;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionIds.Remove(Context.ConnectionId);

            // TODO handle disconnect
            //LeaveRoom();
            //DbContext.SaveChanges();
            return Task.CompletedTask;
        }

        //TODO not working
        public Task SignOut()
        {
            Context.Abort();
            return Task.CompletedTask;
        }

        public Task SignIn(string username, string password)
        {
            // Checking if user exists and verifying
            User user = DbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || user.Password != password)
            {
                Clients.Client(Context.ConnectionId).SendAsync("Alert", "Invalid username or password");
                return null;
            }

            ConnectionIds.Add(Context.ConnectionId, user.Username);  // Adding to connectionIds list

            SendLobbyStatus();                                       // Sending Lobby status

            SendUserStatus(user);                                    // Sending User's status

            if (user.UserInGame != null)
            {
                Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == user.UserInGame.RoomId);
                if (room != null)
                    SendRoomStatus(room);                            // Sending Room status
            }

            DbContext.SaveChanges();
            return Task.CompletedTask;
        }


        public Task JoinRoom(string roomId, int enterMoney)
        {
            // Getting the user, room, and players in room
            User user = GetUserByConnectionId();
            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == roomId);
            if (user == null || room == null) 
                return null;                 // Verifying user and room exist 

            // Trying to add user to room
            if (!room.AddUser(DbContext, user, enterMoney))
            {
                Clients.Clients(GetUserConnections(user)).SendAsync("Alert", "Room is full!");
                return null;
            }

            SendRoomStatus(room);             // Sending everyone in the room the status

            SendLobbyStatus();                // Sending everyone in lobby status

            SendUserStatus(user);             // Sending User's status

            return Task.CompletedTask;
        }

        public Task LeaveRoom()
        {
            // Getting the user, room, and players in room
            User user = GetUserByConnectionId();
            if (user == null || user.UserInGame == null)
                return null;                             // Verifying user and room exist
            
            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == user.UserInGame.RoomId);
            if (room == null) return null;               // Verifying room exists

            room.Fold(DbContext, user.UserInGame);       // Folding player

            // Returning player to lobby
            room.Users.Remove(user.UserInGame);
            user.Money += (int)user.UserInGame.MoneyInTable;
            DbContext.SaveChanges();

            if (room.Users.Count() > 1)
                room.StartGame(DbContext);

            // Sending everyone in the room the status
            List<User> playersInRoom = DbContext.Users.Where(u => u.UserInGame.RoomId == room.Id).ToList();
            if (playersInRoom.Count() == 0)
            {
                DbContext.Rooms.Remove(room);
                DbContext.SaveChanges();
            }
            else
            {
                SendRoomStatus(room);
            }
            
            SendLobbyStatus();                // Sending everyone in lobby status

            SendUserStatus(user);             // Sending User's status

            return Task.CompletedTask;
        }

        public Task CreateRoom(string roomName, int enterMoney)
        {
            Room room = new Room() { Name = roomName };
            DbContext.Rooms.Add(room);
            DbContext.SaveChanges();
            JoinRoom(room.Id, enterMoney);
            return Task.CompletedTask;
        }

        public Task SendMessage(string message)
        {
            // Getting the user, room, and players in room
            User user = GetUserByConnectionId();
            if (user == null) return null;    // Verifying user exists

            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == user.UserInGame.RoomId);
            if (room == null) return null;    // Verifying room exists

            //sending a message to all users in current room
            room.Users.ForEach(u=>
                Clients.Clients(GetUserConnections(u.User)).SendAsync("ReceiveMessage", user.Username, message)
            );
            return Task.CompletedTask;
        }

        // "Synchrnoenous" fold received from fold button
        public Task Fold()
        {
            // Getting the user, room, and players in room
            User user = GetUserByConnectionId();
            if (user == null) return null;    // Verifying user exists

            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == user.UserInGame.RoomId);
            if (room == null) return null;    // Verifying room exists

            if (room.TalkingPosition != user.UserInGame.Position)
                return null;                  // Validating it's the player's turn

            room.Fold(DbContext, user.UserInGame);

            if (room.Users.Count() > 1)       // Starting new game
                room.StartGame(DbContext);

            SendUserStatus(user);             // Sending User's status

            SendRoomStatus(room);             // Sending everyone in the room the status

            return Task.CompletedTask;
        }

        public Task Call()
        {
            // Getting the user, room, and players in room
            User user = GetUserByConnectionId();
            if (user == null) return null;    // Verifying user exists

            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == user.UserInGame.RoomId);
            if (room == null) return null;    // Verifying room exists

            if (room.TalkingPosition != user.UserInGame.Position)
                return null;                  // Validating it's the player's turn

            room.Call(DbContext, user.UserInGame);

            SendUserStatus(user);             // Sending User's status

            SendRoomStatus(room);             // Sending everyone in the room the status

            return Task.CompletedTask;
        }

        public Task Raise(int amount)
        {
            // Getting the user, room, and players in room
            User user = GetUserByConnectionId();
            if (user == null) return null;    // Verifying user exists

            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == user.UserInGame.RoomId);
            if (room == null) return null;    // Verifying room exists

            if (room.TalkingPosition != user.UserInGame.Position)
                return null;                  // Validating it's the player's turn

            room.Raise(DbContext, user.UserInGame, amount);

            SendUserStatus(user);             // Sending User's status

            SendRoomStatus(room);             // Sending everyone in the room the status

            return Task.CompletedTask;
        }

        public Task Check()
        {
            // Getting the user, room, and players in room
            User user = GetUserByConnectionId();
            if (user == null) return null;    // Verifying user exists

            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == user.UserInGame.RoomId);
            if (room == null) return null;    // Verifying room exists

            if (room.TalkingPosition != user.UserInGame.Position)
                return null;                  // Validating it's the player's turn

            room.Check(DbContext, user.UserInGame);

            SendUserStatus(user);             // Sending User's status

            SendRoomStatus(room);             // Sending everyone in the room the status

            return Task.CompletedTask;
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
            Clients.Clients(userConnectionIds).SendAsync("UserStatus", new UserDto(user));
        }

        private List<string> GetUserConnections(User user)
        {
            return ConnectionIds.Where(d => d.Value == user.Username).Select(d => d.Key).ToList();
        }

        private void SendLobbyStatus()
        {
            Clients.All.SendAsync("AllRoomsStatus", new LobbyDto(DbContext.Rooms.ToList()));
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
    }
}
