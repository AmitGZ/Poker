using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nancy.Json;


//add exception getById

public class Player
{
    public string id;
    public string user_name;
    public string status;

    public Player(string id, string UserName, string status)
    {
        this.id = id;
        this.user_name = UserName;
        this.status = status;
    }
}

public static class ConnectedUser
{
    public static List<Player> Players = new List<Player>();

    public static void removeById(string id)
    {
        Players.Remove(getById(id));
    }
    public static Player getById(string id)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].id == id)
                return Players[i];
        }
        return null;
    }

    public static void setPlaying(int index)
    {
        for (int i = 0; i < Players.Count; i++)
            Players[i].status = "Waiting";
        Players[index].status = "Playing";
    }
}

namespace Poker.Hubs
{
    public class PokerHub : Hub
    {
        //Game Variables
        static string GameStatus = "Not Playing";
        static int turn_count = 0;
        JavaScriptSerializer serializer = new JavaScriptSerializer();


        public override async Task OnConnectedAsync()
        {
            //game logic
            Player NewPlayer = new Player(Context.ConnectionId, "Amit", "Not Playing");
            ConnectedUser.Players.Add(NewPlayer);
            if (GameStatus == "Not Playing" && ConnectedUser.Players.Count >= 2)
            {
                GameStatus = "Playing";
                ConnectedUser.setPlaying(0);
            }

            //sending status
            await sendGameStatus();
            await base.OnConnectedAsync();
        }

        public async Task endTurn()
        {
            //game logic
            turn_count += 1;
            turn_count %= ConnectedUser.Players.Count;
            ConnectedUser.setPlaying(turn_count);

            //sending to everybody status and starting game
            await sendGameStatus();
        }


        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            //game logic
            ConnectedUser.removeById(Context.ConnectionId);
            if (ConnectedUser.Players.Count == 1)
                GameStatus = "Not Playing";

            //sending to everyone
            await sendGameStatus();
            await base.OnDisconnectedAsync(exception);
        }

        //sending to everybody the current status
        public async Task sendGameStatus()
        {
            System.Object[] status = { GameStatus, ConnectedUser.Players };
            string statusJSON = serializer.Serialize(status);

            await Clients.All.SendAsync("ReceiveStatus", statusJSON);

        }
    }
}