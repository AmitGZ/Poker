using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.ClassLibrary
{
    public enum Status { Playing, NotPlaying, Waiting };
    public class Player
    {
        public string Id { get; set; }
        public string UserName;
        public Status Status;

        public Player(string id, string userName, Status status)
        {
            this.Id = id;
            this.UserName = userName;
            this.Status = status;
        }
    }
}
