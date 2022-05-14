using PokerClassLibrary;
using System;
using System.Collections.Generic;

namespace Poker.DataModel.Dto
{
    public class LobbyDto
    {
        public List<RoomDto> _rooms { get; set; }

        public LobbyDto(List<Room> rooms, List<User> users)
        {
            _rooms = new List<RoomDto>();
            foreach(var room in rooms)
            {
                RoomDto roomDto = new RoomDto(room, users);
                _rooms.Add(roomDto);
            }
        }
    }
}
