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
        private readonly string _botUser;
        PokerContext _db;

        //private SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=Poker;Integrated Security=True");
        public PokerHub(PokerContext db)
        {
            _db = db;
            _botUser = "notification";   //notification user name          
        }

        //public override Task OnDisconnectedAsync(Exception exception)
        //{
        //    if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
        //    {
        //        //removing connection
        //        _connections.Remove(Context.ConnectionId);

        //        //messaging user's group
        //        Clients.Group(userConnection.Room).SendAsync(
        //            "ReceiveMessage", _botUser, $"{userConnection.User} has left");

        //        //signaling user's group
        //        SendUsersConnected(userConnection.Room);
        //    }
        //    return base.OnDisconnectedAsync(exception);
        //}

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

        //public async Task JoinRoom(string room)
        //{
        //    //storing old room to update
        //    string old_room = _connections[Context.ConnectionId].Room;
            
        //    //if clicked new room 
        //    if (room == "new")
        //        room = _connections[Context.ConnectionId].User + "'s Room";

        //    //updating groups
        //    _connections[Context.ConnectionId].Room = room;
        //    await Groups.AddToGroupAsync(Context.ConnectionId, room);
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, old_room);

        //    //sending join message
        //    await Clients.Group(room)
        //        .SendAsync(
        //        "ReceiveMessage", _botUser, $"{_connections[Context.ConnectionId].User} " +
        //        $"has joined {_connections[Context.ConnectionId].Room}"
        //        );

        //    //updating old room and new room
        //    await SendUsersConnected(old_room);
        //    await SendUsersConnected(_connections[Context.ConnectionId].Room);

        //    //updating rooms available
        //    await SendRoomsAvailable();
        //}

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

        //public Task SendUsersConnected(string room)
        //{
        //    //getting all room users
        //    var users = _connections.Values.Where(c => c.Room == room).Select(c => c.User);

        //    //sending the user his current room
        //    Clients.Group(room).SendAsync("ReceivePage", room);
            
        //    //updating all players in the room
        //    return Clients.Group(room).SendAsync("UsersInRoom", users);
        //}
        //public Task SendRoomsAvailable()
        //{
        //    //filtering to get all Distinct rooms excluding Lobby
        //    var users = _connections.Values;

        //    List<String> list = new List<String>();
        //    for (int i = 0; i < users.Count(); i++)
        //        list.Add(users.ElementAt(i).Room);

        //    list = list.Distinct().Where(x => x != "Lobby").ToList();

        //    //updating Lobby rooms
        //    return Clients.Group("Lobby").SendAsync("ReceiveRooms", list);
        //}
    }
}
