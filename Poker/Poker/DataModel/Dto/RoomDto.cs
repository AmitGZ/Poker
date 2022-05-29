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
        public List<UserDto> Users { get; set; }
        public List<Card> CardsOnTable { get; set; }
        public short TalkingPosition { get; set; }
        public short DealerPosition { get; set; }
        public int Pot { get; set; }
        public GameStage Stage { get; set; }
        public int TurnStake { get; set; }

        public RoomDto(Room room, User user)
        {
            Id = room.Id;
            Name = room.Name;
            Users = new List<UserDto>();
            Users.Add(new UserDto(user, false));
            room.Users.Where(u => u.Username != user.Username).ToList().ForEach(us => Users.Add(new UserDto(us, true)));
            NumberOfPlayers = room.Users.Count;
            TalkingPosition = room.TalkingPosition;
            Pot = room.Pot;
            Stage = room.Stage;
            TurnStake = room.TurnStake;
            CardsOnTable = ((int)room.Stage < 2) ? new List<Card>() : room.CardsOnTable.GetRange(0, (int)room.Stage + 1);
            DealerPosition = room.DealerPosition;
        }
    }
}
