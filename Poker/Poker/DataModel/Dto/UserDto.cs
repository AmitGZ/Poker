using PokerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.DataModel.Dto
{
    public class UserDto
    {
        public string Username { get; set; }
        public int Money { get; set; }
        public int MoneyInTable { get; set; }
        public string? RoomId { get; set; }
        public short? Position { get; set; }
        public List<Card> Cards { get; set; }
        public int MoneyInTurn { get; set; }
        public bool IsActive { get; set; }

        public UserDto(User user)
        {
            Username = user.Username;
            Money = user.Money;
            MoneyInTable = user.MoneyInTable;
            RoomId = user.RoomId;
            Position = user.Position;
            Cards = user.Cards;
            MoneyInTurn = user.MoneyInTurn;
            IsActive = user.IsActive;
        }
    }
}
