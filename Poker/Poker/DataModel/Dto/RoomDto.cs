using PokerClassLibrary;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Poker.DataModel.Dto
{
    public class RoomDto
    {
        public string Id{ get; set; }
        public string Name { get; set; }
        public int NumberOfPlayers { get; set; }
        public List<User> Users { get; set; }
        public List<Card> CardsOnTable { get; set; }
        public short TalkingPosition { get; set; }
        public int Pot { get; set; }
        public GameStage Stage { get; set; }

        public RoomDto(Room room)
        {
            Id = room.Id;
            Name = room.Name;
            Users = room.Users.ToList();
            NumberOfPlayers = room.Users.Count;
            TalkingPosition = room.TalkingPosition;
            Pot = room.Pot;
            Stage = room.Stage;
            CardsOnTable = room.CardsOnTable;
        }
    }
}
