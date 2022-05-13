using PokerClassLibrary;
using System.Collections.Generic;
using System;

namespace Poker.DataModel.Dto
{
    public class RoomFromLobbyDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public int NumberOfPlayers { get; set; }

        public List<User> Players { get; set; }

        public RoomFromLobbyDto(Room room)
        {
            NumberOfPlayers = room.Players.Count;
            RoomId = room.RoomId;
            RoomName = room.RoomName;
            Players = room.Players;
        }
    }
}
