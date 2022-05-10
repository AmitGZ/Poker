using PokerClassLibrary;
using System.Collections.Generic;

namespace Poker.DataModel.Dto
{
    public class LobbyDto
    {
        public string Username { get; set; }
        public double Money    { get; set; }
        public List<RoomFromLobbyDto> Rooms { get; set; }

        public LobbyDto(User user,List<Room> rooms)
        {
            this.Username = user.Username;
            this.Money = user.UserMoney;
            Rooms = new List<RoomFromLobbyDto>();
            foreach(var room in rooms)
            {
                RoomFromLobbyDto roomFromLobbyDto = new RoomFromLobbyDto(room);
                Rooms.Add(roomFromLobbyDto);
            }
        }
    }
}
