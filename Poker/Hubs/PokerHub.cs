using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nancy.Json;


//add exception for GetById

public class Player
{
    public string Id { get; set; }
    public string UserName;
    public string Status;

    public Player(string id, string userName, string status)
    {
        this.Id = id;
        this.UserName = userName;
        this.Status = status;
    }
}

public static class ConnectedUser
{
    public static List<Player> Players = new List<Player>();

    public static void RemoveById(string id)
    {
        Players.Remove(GetById(id));
    }
    public static Player GetById(string id)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Id == id)
                return Players[i];
        }
        return null;
    }

    public static void SetPlaying(int index)
    {
        for (int i = 0; i < Players.Count; i++)
            Players[i].Status = "Waiting";
        Players[index].Status = "Playing";
    }
}

namespace Poker.Hubs
{
    public class PokerHub : Hub
    {
        //Game Variables
        static string gameStatus = "NotPlaying";
        static int turnCount = 0;
        readonly JavaScriptSerializer serializer = new JavaScriptSerializer();


        public override async Task OnConnectedAsync()
        {
            //game logic
            Player NewPlayer = new Player(Context.ConnectionId, "Amit", "NotPlaying");
            ConnectedUser.Players.Add(NewPlayer);
            if (gameStatus == "NotPlaying" && ConnectedUser.Players.Count >= 2)
            {
                gameStatus = "Playing";
                ConnectedUser.SetPlaying(0);
            }

            //sending Status
            await SendGameStatus();
            await base.OnConnectedAsync();
        }

        public async Task EndTurn()
        {
            //game logic
            turnCount += 1;
            turnCount %= ConnectedUser.Players.Count;
            ConnectedUser.SetPlaying(turnCount);

            //sending to everybody Status and starting game
            await SendGameStatus();
        }


        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            //game logic
            ConnectedUser.RemoveById(Context.ConnectionId);
            if (ConnectedUser.Players.Count == 1)
                gameStatus = "NotPlaying";

            //sending to everyone
            await SendGameStatus();
            await base.OnDisconnectedAsync(exception);
        }

        //sending to everybody the current Status
        public async Task SendGameStatus()
        {
            System.Object[] Status = { gameStatus, ConnectedUser.Players };
            string StatusJSON = serializer.Serialize(Status);

            await Clients.All.SendAsync("ReceiveStatus", StatusJSON);

        }
    }
}