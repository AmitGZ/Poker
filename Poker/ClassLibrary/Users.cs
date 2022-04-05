using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Poker.ClassLibrary
{
    public static class Users
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
                Players[i].Status = Status.NotPlaying;
            Players[index].Status = Status.Playing;
        }
    }
}
