using PokerClassLibrary;
using System;
using System.Collections.Generic;

namespace Poker.DataModel.Dto
{
    public class LobbyDto
    {
        public List<RoomDto> Rooms { get; set; }

        public LobbyDto(List<Room> rooms)
        {
            Rooms = new List<RoomDto>();
            foreach(var room in rooms)
            {
                RoomDto roomDto = new RoomDto(room);
                Rooms.Add(roomDto);
            }
        }
    }
}
