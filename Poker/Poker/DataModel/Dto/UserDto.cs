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
        public bool IsWinner { get; set; }

        public UserDto(User user, Boolean isPrivate = false)
        {
            Username = user.Username;
            Money = user.Money;
            if (user.UserInGame != null)
            {
                MoneyInTable = user.UserInGame.MoneyInTable;
                RoomId = user.UserInGame.Room.Id;
                Position = user.UserInGame.Position;
                Cards = (isPrivate) ? new List<Card>() : user.UserInGame.Cards;
                MoneyInTurn = user.UserInGame.MoneyInTurn;
                IsActive = user.UserInGame.IsActive;
                IsWinner = user.UserInGame.IsWinner;
            }
            else
            {
                // TODO remove somehow
                MoneyInTable = 0;
                RoomId = null;
                Position = 0;
                Cards = new List<Card>();
                MoneyInTurn = 0;
                IsActive = false;
                IsWinner = false;
            }
        }
    }
}
