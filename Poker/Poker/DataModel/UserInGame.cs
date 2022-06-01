using PokerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.DataModel
{
    public class UserInGame
    {
        public string Id { get; set; }
        public int MoneyInTable { get; set; }
        public int MoneyInTurn { get; set; }
        public bool IsActive { get; set; }
        public bool PlayedThisTurn { get; set; }
        public short Position { get; set; }
        public string? RoomId { get; set; }
        public string Username { get; set; } // FK User
        public virtual User User { get; set; }
        public virtual List<Card>? Cards { get; set; }
        public UserInGame()
        {
            Cards = new List<Card>();
            MoneyInTable = 0;
            MoneyInTurn = 0;
            PlayedThisTurn = false;
            Position = 0;
            IsActive = false;
            RoomId = null;
        }
        public UserInGame(int enterMoney, short pos)
        {
            Cards = new List<Card>();
            MoneyInTable = enterMoney;
            MoneyInTurn = 0;
            PlayedThisTurn = false;
            Position = pos;
            IsActive = false;
        }
    }
}
