using PokerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestsProject
{
    // This class deals with the unit tests to the methods implemented in the class Room
    public class UnitTestsRoom
    {
        // -------------Tests for AddUser() method----------
        // 1
        [Fact]
        public void AddUser_IsRoomFull_ReturnFalse()
        {
            // Goal: Invoke the method AddUser() when the room is full
            // (Users.Count = 5). Checks if the function returns false
            // as needed

            // Arrange
            var room = new PokerClassLibrary.Room();
            var user = new PokerClassLibrary.User(); // User to add
            var user_in_game = new Poker.DataModel.UserInGame(); // User to add
            bool result; // Contains the boolean value that return from method

            // Act
            user_in_game.Room = room;
            // Fill the room by adding 5 users room
            room.Users.Add(user_in_game);
            room.Users.Add(user_in_game);
            room.Users.Add(user_in_game);
            room.Users.Add(user_in_game);
            room.Users.Add(user_in_game);

            result = room.AddUser(user, 1000);
            
            // Assert
            Assert.False(result);
        }
    }
}
