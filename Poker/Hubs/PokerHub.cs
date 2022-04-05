using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nancy.Json;
using Poker.ClassLibrary;


//add exception for GetById


namespace Poker.Hubs
{
    public class PokerHub : Hub
    {
        //Game Variables
        static Status gameStatus = Status.NotPlaying;
        static int turnCount = 0;
        readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public override async Task OnConnectedAsync()
        {
            //game logic
            Player NewPlayer = new Player(Context.ConnectionId, "Amit", Status.NotPlaying);
            Users.Players.Add(NewPlayer);
            if (gameStatus == Status.NotPlaying && Users.Players.Count >= 2)
            {
                gameStatus = Status.Playing;
                Users.SetPlaying(0);
            }

            //sending Status
            await SendGameStatus();
            await base.OnConnectedAsync();
        }

        public async Task EndTurn()
        {
            //game logic
            turnCount += 1;
            turnCount %= Users.Players.Count;
            Users.SetPlaying(turnCount);

            //sending to everybody Status and starting game
            await SendGameStatus();
        }


        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            //game logic
            Users.RemoveById(Context.ConnectionId);
            if (Users.Players.Count == 1)
                gameStatus = Status.NotPlaying;

            //sending to everyone
            await SendGameStatus();
            await base.OnDisconnectedAsync(exception);
        }

        //sending everybody the current Status
        public async Task SendGameStatus()
        {
            System.Object[] Status = { gameStatus, Users.Players };
            string StatusJSON = serializer.Serialize(Status);

            await Clients.All.SendAsync("ReceiveStatus", StatusJSON);
        }
    }
}