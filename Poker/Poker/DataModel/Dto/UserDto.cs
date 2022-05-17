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
        public string? RoomId { get; set; }
        public short? Position { get; set; }

        public UserDto(User user)
        {
            this.Username = user.Username;
            this.Money = user.Money;
            this.RoomId = user.RoomId;
            this.Position = user.Position;
        }
    }
}
