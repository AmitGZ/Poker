using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using Poker;
using System.Linq;
using System;
using PokerClassLibrary;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Poker.Hubs
{
    public class PokerHub : Hub
    {
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;
        private SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=Poker;Integrated Security=True");
        
        public PokerHub(IDictionary<string, UserConnection> connections)
        {
            _botUser = "notification";   //notification user name          
            _connections = connections; // all connections
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                //removing connection
                _connections.Remove(Context.ConnectionId);
                
                //messaging user's group
                Clients.Group(userConnection.Room).SendAsync(
                    "ReceiveMessage", _botUser, $"{userConnection.User} has left");

                //signaling user's group
                SendUsersConnected(userConnection.Room);
            }
            return base.OnDisconnectedAsync(exception);
        }

        //TODO not working
        public Task SignOut()
        {
            Context.Abort();
            return Task.CompletedTask;
        }

        public async Task JoinGame(UserConnection userConnection)
        {
            con.Open();
            SqlCommand insert = new SqlCommand("INSERT INTO Player (userName,userMoney) VALUES('amit',999)", con);
            SqlCommand com = new SqlCommand("Select * from Player;", con);
            insert.ExecuteNonQuery();
            var tmp = com.ExecuteScalar().ToString() ;
            con.Close();

            //initializing user at _connections
            _connections[Context.ConnectionId] = userConnection;

            //adding to Room (Lobby)
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            //sending everybody join message
            await Clients.Group(userConnection.Room).SendAsync(
                "ReceiveMessage",
                _botUser, $"{userConnection.User} has joined {userConnection.Room}"
                );

            //updating all the room that someone connected
            await SendUsersConnected(userConnection.Room);

            //updating rooms available in Lobby
            await SendRoomsAvailable();
        }

        public async Task JoinRoom(string room)
        {
            //storing old room to update
            string old_room = _connections[Context.ConnectionId].Room;
            
            //if clicked new room 
            if (room == "new")
                room = _connections[Context.ConnectionId].User + "'s Room";

            //updating groups
            _connections[Context.ConnectionId].Room = room;
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, old_room);

            //sending join message
            await Clients.Group(room)
                .SendAsync(
                "ReceiveMessage", _botUser, $"{_connections[Context.ConnectionId].User} " +
                $"has joined {_connections[Context.ConnectionId].Room}"
                );

            //updating old room and new room
            await SendUsersConnected(old_room);
            await SendUsersConnected(_connections[Context.ConnectionId].Room);

            //updating rooms available
            await SendRoomsAvailable();
        }

        public async Task SendMessage(string message)
        {
            //sending a message to all users in current room
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", userConnection.User, message);
            }
        }

        public Task SendUsersConnected(string room)
        {
            //getting all room users
            var users = _connections.Values.Where(c => c.Room == room).Select(c => c.User);

            //sending the user his current room
            Clients.Group(room).SendAsync("ReceivePage", room);
            
            //updating all players in the room
            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }
        public Task SendRoomsAvailable()
        {
            //filtering to get all Distinct rooms excluding Lobby
            var users = _connections.Values;

            List<String> list = new List<String>();
            for (int i = 0; i < users.Count(); i++)
                list.Add(users.ElementAt(i).Room);

            list = list.Distinct().Where(x => x != "Lobby").ToList();

            //updating Lobby rooms
            return Clients.Group("Lobby").SendAsync("ReceiveRooms", list);
        }
    }
}
