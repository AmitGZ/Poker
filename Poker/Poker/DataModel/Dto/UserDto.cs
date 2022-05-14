using PokerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.DataModel.Dto
{
    public class UserDto
    {
        public string _username { get; set; }
        public double _money { get; set; }

        public UserDto(User user)
        {
            this._username = user._username;
            this._money = user._money;
        }
    }
}
