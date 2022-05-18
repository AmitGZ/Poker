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

// Change LobbyDto
// Put User inside Player

namespace Poker.Hubs
{
    public class PokerHub : Hub
    {
        PokerContext DbContext;

        //private SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=Poker;Integrated Security=True");
        public PokerHub(PokerContext dbContext)
        {
            this.DbContext = dbContext;        
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
            User user = DbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || user.Password != password)
            {
                Clients.Client(Context.ConnectionId).SendAsync("Alert", "Invalid username or password");
                return null;
            }

            // Setting user's ConnectionId
            user.ConnectionId = Context.ConnectionId;

            // Sending room status .Include(room => room.Users)
            Clients.Client(Context.ConnectionId).SendAsync("AllRoomsStatus", new LobbyDto(DbContext.Rooms.ToList()));
            
            // Sending User's status
            Clients.Client(Context.ConnectionId).SendAsync("UserStatus", new UserDto(user));

            DbContext.SaveChanges();
            return Task.CompletedTask;
        }


        public Task JoinRoom(string roomId, int enterMoney)
        {
            // Getting the user, room, and players in room
            User user = DbContext.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == roomId);

            // Verifying room and user exist
            if (user == null || room == null)
                return null;

            // Trying to add user to room
            if (!room.AddUser(DbContext, user, enterMoney))
            {
                Clients.Client(Context.ConnectionId).SendAsync("Alert", "Room is full!");
                return null;
            }

            LobbyDto lobbyDto = new LobbyDto(DbContext.Rooms.ToList());
            RoomDto roomDto = lobbyDto.Rooms.FirstOrDefault(r => r.Id == roomId);

            // Sending everyone in the room the status
            Clients.Clients(room.Users.Select(p => p.ConnectionId)).SendAsync("RoomStatus", roomDto);
            
            // Sending everyone new rooms status
            Clients.All.SendAsync("AllRoomsStatus", lobbyDto);

            // Sending User's status
            Clients.Client(Context.ConnectionId).SendAsync("UserStatus", new UserDto(user));

            return Task.CompletedTask;
        }

        public Task LeaveRoom()
        {
            // Getting the user, room, and players in room
            User user = DbContext.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user == null) return null;

            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == user.RoomId);

            // Verifying room and user exist
            if (room == null) return null;

            // Returning player to lobby
            room.Users.Remove(user);
            user.Money += (int)user.MoneyInTable;
            DbContext.SaveChanges();

            // Sending everyone in the room the status
            List<User> playersInRoom = DbContext.Users.Where(u => u.RoomId == room.Id).ToList();
            if (playersInRoom.Count() == 0)
            {
                DbContext.Rooms.Remove(room);
                DbContext.SaveChanges();
            }
            else
            {
                // Sending everyone in the room the status
                Clients.Clients(playersInRoom.Select(p => p.ConnectionId)).SendAsync("RoomStatus", new RoomDto(room));
            }
            // Sending everyone new rooms status
            Clients.All.SendAsync("AllRoomsStatus", new LobbyDto(DbContext.Rooms.ToList()));

            // Sending User's status
            Clients.Client(Context.ConnectionId).SendAsync("UserStatus", new UserDto(user));

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
            User user = DbContext.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == user.RoomId);

            // Verifying room and user exist
            if (user == null || room == null)
                return null;

            //sending a message to all users in current room
            Clients.Clients(room.Users.Select(p => p.ConnectionId)).SendAsync("ReceiveMessage", user.Username, message);
            
            return Task.CompletedTask;
        }

        public Task ReceiveAction(string action, int? amount = null)
        {
            // Getting the user, room, and players in room
            User user = DbContext.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            Room room = DbContext.Rooms.FirstOrDefault(r => r.Id == user.RoomId);

            // Validating it's the player's turn
            if (room.TalkingPosition != user.Position)
                return null;

            room.ReceiveAction(DbContext, action);

            //sending a message to all users in current room
            Clients.Clients(room.Users.Select(p => p.ConnectionId)).SendAsync("RoomStatus", new RoomDto(room));

            return Task.CompletedTask;
        }
    }
}
