using PokerClassLibrary;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Poker.DataModel.Dto
{
    public class RoomDto
    {
        public int _id{ get; set; }
        public string _name { get; set; }
        public int _numberOfPlayers { get; set; }
        public List<User> _players { get; set; }

        public RoomDto(Room room, List<User> users)
        {
            _id = room._id;
            _name = room._name;
            _players = users.Where(u => u._roomId == room._id).ToList();
            _numberOfPlayers = _players.Count;
        }
    }
}
