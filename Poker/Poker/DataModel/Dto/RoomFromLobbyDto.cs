using PokerClassLibrary;

namespace Poker.DataModel.Dto
{
    public class RoomFromLobbyDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public int NumberOfPlayers { get; set; }

        public RoomFromLobbyDto(Room room)
        {
            RoomId = room.RoomId;
            RoomName = room.RoomName;
            NumberOfPlayers = room.Players.Count;
        }
    }
}
