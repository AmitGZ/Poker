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
        PokerContext Db;

        //private SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=Poker;Integrated Security=True");
        public PokerHub(PokerContext db)
        {
            Db = db;        
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
            User user = Db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || user.Password != password)
            {
                Clients.Client(Context.ConnectionId).SendAsync("Alert", "Invalid username or password");
                return null;
            }

            // Setting user's ConnectionId
            user.ConnectionId = Context.ConnectionId;

            // Sending room status
            Clients.Client(Context.ConnectionId).SendAsync("AllRoomsStatus", new LobbyDto(Db.Rooms.Include(room => room.Users).ToList()));
            
            // Sending User's status
            Clients.Client(Context.ConnectionId).SendAsync("UserStatus", new UserDto(user));

            Db.SaveChanges();
            return Task.CompletedTask;
        }


        public Task JoinRoom(string roomId, int enterMoney)
        {
            // Getting the user, room, and players in room
            User user = Db.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            Room room = Db.Rooms.FirstOrDefault(r => r.Id == roomId);

            // Verifying room and user exist
            if (user == null || room == null)
                return null;


            if (room.Users.Count == 5)
            {
                Clients.Client(Context.ConnectionId).SendAsync("Alert", "Room is full!");
                return null;
            }

            // Getting available position
            List<short?> positions = room.Users.Select(p => p.Position).ToList();
            short pos = 0;
            for ( ;pos<5; pos++)
                if (!positions.Contains(pos))
                    break;

            // Adding the player to the room
            user.RoomId = roomId;
            user.Money -= enterMoney;
            user.MoneyInTable= enterMoney;
            user.Position = pos;
            room.Users.Add(user);
            Db.SaveChanges();

            LobbyDto lobbyDto = new LobbyDto(Db.Rooms.ToList());
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
            User user = Db.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user == null) return null;

            Room room = Db.Rooms.FirstOrDefault(r => r.Id == user.RoomId);

            // Verifying room and user exist
            if (room == null) return null;

            // Returning player to lobby
            user.RoomId = null;
            user.Money += (int)user.MoneyInTable;
            Db.SaveChanges();

            // Sending everyone in the room the status
            List<User> playersInRoom = Db.Users.Where(u => u.RoomId == room.Id).ToList();
            if (playersInRoom.Count() == 0)
            {
                Db.Rooms.Remove(room);
                Db.SaveChanges();
            }
            else
            {
                // Sending everyone in the room the status
                Clients.Clients(playersInRoom.Select(p => p.ConnectionId)).SendAsync("RoomStatus", new RoomDto(room));
            }
            // Sending everyone new rooms status
            Clients.All.SendAsync("AllRoomsStatus", new LobbyDto(Db.Rooms.ToList()));

            // Sending User's status
            Clients.Client(Context.ConnectionId).SendAsync("UserStatus", new UserDto(user));

            return Task.CompletedTask;
        }

        public Task CreateRoom(string roomName, int enterMoney)
        {
            Room room = new Room() { Name = roomName };
            Db.Rooms.Add(room);
            Db.SaveChanges();
            JoinRoom(room.Id, enterMoney);
            return Task.CompletedTask;
        }

        public Task SendMessage(string message)
        {
            // Getting the user, room, and players in room
            User user = Db.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            Room room = Db.Rooms.FirstOrDefault(r => r.Id == user.RoomId);

            // Verifying room and user exist
            if (user == null || room == null)
                return null;

            //sending a message to all users in current room
            Clients.Clients(room.Users.Select(p => p.ConnectionId)).SendAsync("ReceiveMessage", user.Username, message);
            
            return Task.CompletedTask;
        }
    }
}
