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

namespace Poker.Hubs
{
    public class PokerHub : Hub
    {
        PokerContext _db;

        //private SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=Poker;Integrated Security=True");
        public PokerHub(PokerContext db)
        {
            _db = db;        
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            LeaveRoom();
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
            User user = _db.Users.FirstOrDefault(u => u._username == username);
            if (user == null || user._password != password)
            {
                Clients.Client(Context.ConnectionId).SendAsync("SignInStatus", false);
                return null;
            }

            // Setting user's ConnectionId
            user._connectionId = Context.ConnectionId;

            // Sending successful sign in 
            Clients.Client(Context.ConnectionId).SendAsync("SignInStatus", true);

            // Sending room status
            Clients.Client(Context.ConnectionId).SendAsync("AllRoomsStatus", new LobbyDto(_db.Rooms.ToList(), _db.Users.ToList()));
            
            // Sending User's status
            Clients.Client(Context.ConnectionId).SendAsync("UserStatus", new UserDto(user));

            _db.SaveChanges();
            return Task.CompletedTask;
        }


        public Task JoinRoom(string roomId, int enterMoney)
        {
            // Getting the user, room, and players in room
            User user = _db.Users.FirstOrDefault(u => u._connectionId == Context.ConnectionId);
            Room room = _db.Rooms.FirstOrDefault(r => r._id == roomId);

            // Verifying room and user exist
            if (user == null || room == null)
                return null;

            // Adding the player to the room
            user._roomId = roomId;
            user._money -= enterMoney;
            user._moneyInTable = enterMoney;
            _db.SaveChanges();

            // TODO check if room is empty, and delete

            LobbyDto lobbyDto = new LobbyDto(_db.Rooms.ToList(), _db.Users.ToList());
            RoomDto roomDto = lobbyDto._rooms.FirstOrDefault(r => r._id == roomId);

            // Sending everyone in the room the status
            List<User> playersInRoom = _db.Users.Where(u => u._roomId == roomId).ToList();
            Clients.Clients(playersInRoom.Select(p => p._connectionId)).SendAsync("RoomStatus", roomDto);
            
            // Sending everyone new rooms status
            Clients.All.SendAsync("AllRoomsStatus", lobbyDto);

            // Sending User's status
            Clients.Client(Context.ConnectionId).SendAsync("UserStatus", new UserDto(user));

            return Task.CompletedTask;
        }

        public Task LeaveRoom()
        {
            // Getting the user, room, and players in room
            User user = _db.Users.FirstOrDefault(u => u._connectionId == Context.ConnectionId);
            Room room = _db.Rooms.FirstOrDefault(r => r._id == user._roomId);

            // Verifying room and user exist
            if (user == null || room == null)
                return null;

            // Returning player to lobby
            user._roomId = null;
            user._money += (int)user._moneyInTable;
            _db.SaveChanges();

            LobbyDto lobbyDto = new LobbyDto(_db.Rooms.ToList(), _db.Users.ToList());
            RoomDto roomDto = lobbyDto._rooms.FirstOrDefault(r => r._id == room._id);

            // Sending everyone in the room the status
            List<User> playersInRoom = _db.Users.Where(u => u._roomId == room._id).ToList();
            Clients.Clients(playersInRoom.Select(p => p._connectionId)).SendAsync("RoomStatus", roomDto);

            // Sending everyone new rooms status
            Clients.All.SendAsync("AllRoomsStatus", lobbyDto);

            // Sending User's status
            Clients.Client(Context.ConnectionId).SendAsync("UserStatus", new UserDto(user));
            Clients.Client(Context.ConnectionId).SendAsync("SignInStatus", true);

            return Task.CompletedTask;
        }

        public Task CreateRoom(string roomName, int enterMoney)
        {
            Room room = new Room() { _name = roomName };
            _db.Rooms.Add(room);
            _db.SaveChanges();
            JoinRoom(room._id, enterMoney);
            return Task.CompletedTask;
        }

        public Task SendMessage(string message)
        {
            // Getting the user, room, and players in room
            User user = _db.Users.FirstOrDefault(u => u._connectionId == Context.ConnectionId);
            Room room = _db.Rooms.FirstOrDefault(r => r._id == user._roomId);
            List<User> playersInRoom = _db.Users.Where(u => u._roomId == user._roomId).ToList();

            // Verifying room and user exist
            if (user == null || room == null)
                return null;

            //sending a message to all users in current room
            Clients.Clients(playersInRoom.Select(p => p._connectionId)).SendAsync("ReceiveMessage", user._username, message);
            
            return Task.CompletedTask;
        }
    }
}
