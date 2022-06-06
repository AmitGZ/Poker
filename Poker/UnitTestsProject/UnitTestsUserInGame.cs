using Poker.DataModel;
using System;
using Xunit;

namespace UnitTestsProject
{
    // This Tests class deals with UserInRoom class methods
    // The Only method that we can define unit tests is ResetUser()
    public class UnitTestsUserInGame
    {
        // -------------Tests for ResetUser() method----------------
        // 1
        [Fact]
        public void ResetUser_IsActiveTrue_Void()
        {
            // Gaol: Invoke the method ResetUser() and check if IsActive == true

            // Arrange
            var userInGame = new UserInGame();

            // Act
            userInGame.ResetUser();

            // Assert
            Assert.True(userInGame.IsActive);
        }

        // 2
        [Fact]
        public void ResetUser_IsWinnerFalse_Void()
        {
            // Gaol: Invoke the method ResetUser() and check if IsWinner == false

            // Arrange
            var userInGame = new UserInGame();

            // Act
            userInGame.ResetUser();

            // Assert
            Assert.False(userInGame.IsWinner);
        }

        // 3
        [Fact]
        public void ResetUser_IsMoneyInTurnZero_Void()
        {
            // Gaol: Invoke the method ResetUser() and check if MoneyInTurn == 0

            // Arrange
            var userInGame = new UserInGame();

            // Act
            userInGame.ResetUser();

            // Assert
            Assert.Equal(0, userInGame.MoneyInTurn);
        }

        // 4
        [Fact]
        public void ResetUser_IsPlayedThisTurnFalse_Void()
        {
            // Gaol: Invoke the method ResetUser() and check if PlayedThisTurn == false

            // Arrange
            var userInGame = new UserInGame();

            // Act
            userInGame.ResetUser();

            // Assert
            Assert.False(userInGame.PlayedThisTurn);
        }

        // 5
        [Fact]
        public void ResetUser_IsBestHandNull_Void()
        {
            // Gaol: Invoke the method ResetUser() and check if BestHand == null

            // Arrange
            var userInGame = new UserInGame();

            // Act
            userInGame.ResetUser();

            // Assert
            Assert.Null(userInGame.BestHand);
        }
    }
}
