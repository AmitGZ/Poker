using PokerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.DataModel.Dto
{
    public class LobbyDto
    {
        public List<LobbyRoomDto> Rooms { get; set; }

        public LobbyDto(List<Room> rooms)
        {
            Rooms = new List<LobbyRoomDto>();
            foreach (var room in rooms)
            {
                LobbyRoomDto lobbyRoomDto = new LobbyRoomDto()
                {
                    Id = room.Id,
                    Name = room.Name,
                    NumberOfPlayers = (short)room.Users.Count()
                };
                Rooms.Add(lobbyRoomDto);
            }
        }
    }

    public class LobbyRoomDto
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public short NumberOfPlayers { get; set; }
    }
}
