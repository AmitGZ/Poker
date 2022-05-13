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
        public double Money { get; set; }

        public UserDto(User user)
        {
            this.Username = user.Username;
            this.Money = user.UserMoney;
        }
    }
}
