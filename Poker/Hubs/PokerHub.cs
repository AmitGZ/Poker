using Microsoft.AspNetCore.SignalR;
using Nancy.Json;
using Poker.ClassLibrary;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poker.Hubs
{
    public class PokerHub : Hub
    {
        GameEngine gameEngine = new GameEngine();
        readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public override async Task OnConnectedAsync()
        {
            //game logic
            //gameEngine.AddPlayer(Context.ConnectionId);

            //sending Status
            await SendGameStatus();
            await base.OnConnectedAsync();
        }

        public async Task EndTurn()
        {
            //game logic
            //turnCount += 1;
            //turnCount %= GameEngine.Players.Count;
            //GameEngine.SetPlaying(turnCount);

            //sending to everybody Status and starting game
            await SendGameStatus();
        }


        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            //game logic
            gameEngine.RemovePlayerById(Context.ConnectionId);
            //if (gameEngine.Players.Count == 1)
            //    gameEngine.isRunning = false;

            //sending to everyone
            await SendGameStatus();
            await base.OnDisconnectedAsync(exception);
        }

        //sending everybody the current Status
        public async Task SendGameStatus()
        {
            //System.Object[] Status = { gameStatus, GameEngine.Players };
            //string StatusJSON = serializer.Serialize(Status);

            //await Clients.All.SendAsync("ReceiveStatus", StatusJSON);
        }
    }
}