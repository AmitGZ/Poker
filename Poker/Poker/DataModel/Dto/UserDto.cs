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
        public int _money { get; set; }
        public string? _roomId { get; set; }
        public short? _position { get; set; }

        public UserDto(User user)
        {
            this._username = user._username;
            this._money = user._money;
            this._roomId = user._roomId;
            this._position = user._position;
        }
    }
}
