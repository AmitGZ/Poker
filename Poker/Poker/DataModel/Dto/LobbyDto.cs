using PokerClassLibrary;
using System;
using System.Collections.Generic;

namespace Poker.DataModel.Dto
{
    public class LobbyDto
    {
        public List<RoomFromLobbyDto> Rooms { get; set; }

        public LobbyDto(List<Room> rooms)
        {
            Rooms = new List<RoomFromLobbyDto>();
            foreach(var room in rooms)
            {
                RoomFromLobbyDto roomFromLobbyDto = new RoomFromLobbyDto(room);
                Rooms.Add(roomFromLobbyDto);
            }
        }
    }
}
