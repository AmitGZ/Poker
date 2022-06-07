using PokerClassLibrary;
using Xunit;

namespace UnitTestsProject
{
    // This tests class deals with the unit tests to the functions implemented in the class Room
    // All the function in Room return Boolean value:
    // 1. AddUser(User,Money)
    // 2. RemoveUser(User)
    // 3. Call(UserInGame)
    // 4. Raise(UserInGame,amount)
    // 5. Check(UserInGame)
    // 6. ResetGame()
    public class UnitTestsRoom
    {
        // -------------Tests for AddUser(User,Money) method----------

        // 1
        [Fact]
        public void AddUser_IsRoomFull_ReturnFalse()
        {
            // Goal: Invoke the function AddUser() when the room is full (Users.Count = 5).
            // Checks if the function returns false as needed.

            // Arrange
            var room = new PokerClassLibrary.Room();
            var user = new PokerClassLibrary.User(); // User to add
            var user_in_game = new Poker.DataModel.UserInGame(); // User to add manually
            bool result; // Contains the boolean value that return from function

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

        // 2
        [Fact]
        public void AddUser_IsAdded_ReturnTrue()
        {
            // Goal: Invoke the function AddUser when the room is not full.
            // If the return value is true, than the user was added to the room.

            // Arrange
            var room = new PokerClassLibrary.Room();
            var user = new PokerClassLibrary.User(); // User to add
            bool result; // Contains the boolean value that return from function

            // Act
            result = room.AddUser(user, 1000); // Add user to empty room

            // Assert
            Assert.True(result);
        }

        // -------------Tests for RemoveUser(User) method----------

        // 1
        [Fact]
        public void RemoveUser_GameStateIsStopped_ReturnFalse()
        {
            // Goal: Invoke the function RemoveUser when Stage = Stopped.
            // This means that the remove process didn't occur, Should return false.

            // Arrange
            var room = new PokerClassLibrary.Room(); // Room with no users
            var user = new PokerClassLibrary.User(); // User to Add
            // Referenced to UserInGame of user
            var user_in_game = new Poker.DataModel.UserInGame();
            bool result; // Contains the boolean value that return from function

            // Act
            // Connect UserInGame of user to default values definef by the constructor
            user.UserInGame = user_in_game;
            room.Stage = GameStage.Stopped; // Stop the Game
            result = room.RemoveUser(user); // Should set result to false

            // Assert
            Assert.False(result);
        }

        // 2
        [Fact]
        public void RemoveUser_GameStateIsFinished_ReturnFalse()
        {
            // Goal: Invoke the function RemoveUser when Stage = Finished.
            // This means that the remove process didn't occur, Should return false.

            // Arrange
            var room = new PokerClassLibrary.Room(); // Room with no users
            var user = new PokerClassLibrary.User(); // User to Add
            // Referenced to UserInGame of user
            var user_in_game = new Poker.DataModel.UserInGame();
            bool result; // Contains the boolean value that return from function

            // Act
            // Connect UserInGame of user to default values definef by the constructor
            user.UserInGame = user_in_game;
            room.Stage = GameStage.Finished; // Finish the Game

            result = room.RemoveUser(user); // Should set result to false

            // Assert
            Assert.False(result);
        }

        
        // 3 
        [Fact]
        public void RemoveUser_IsRemoveSucceeded_ReturnTrue()
        {
            // Goal: Invoke the method RemoveUser when gameRunning=false.
            // This means that the remove process didn't occur.

            // Arrange
            var room = new PokerClassLibrary.Room(); // Room with no users
            var user1 = new PokerClassLibrary.User(); // User to Add
            var user2 = new PokerClassLibrary.User(); // User to Add
            bool result; // Contains the boolean value that return from function

            // Act
            // Attatch UserInGame of user to object
            user1.UserInGame = new Poker.DataModel.UserInGame(); 
            user2.UserInGame = new Poker.DataModel.UserInGame();
            room.AddUser(user1, user1.Money); // Adding the user to the room
            room.AddUser(user2, user2.Money); // Adding the user to the room
            user1.UserInGame.Room = room;
            user2.UserInGame.Room = room;
            room.StartGame();
            result = room.RemoveUser(user1); // Should set result to false
                        
            // Assert
            Assert.Equal(1, room.Users.Count);
        }


        // -------------Tests for EndGame() method----------

        // 1
        [Fact]
        public void EndGame_IsGameStageFinished_ReturnTrue()
        {
            // Goal: Invoke the function EndGame to check if the Game was ended.
            // This means that the function should return true

            // Arrange
            var room = new PokerClassLibrary.Room();
            var user1 = new PokerClassLibrary.User(); // User to Add
            var user2 = new PokerClassLibrary.User(); // User to Add
            bool result; // Contains the boolean value that return from function

            // Act
            user1.UserInGame = new Poker.DataModel.UserInGame();
            user2.UserInGame = new Poker.DataModel.UserInGame();
            room.AddUser(user1, user1.Money); // Adding the user to the room
            room.AddUser(user2, user2.Money); // Adding the user to the room
            user1.UserInGame.Room = room;
            user2.UserInGame.Room = room;
            room.StartGame();
            result = room.Fold(user1.UserInGame); // Should set result to true
            
            // Assert
            Assert.False(result);
            Assert.True(user2.UserInGame.IsWinner); // User 2 is the winner
            Assert.Equal(2,room.Users.Count); // In the room but IsActive == false
            Assert.Equal(GameStage.Finished, room.Stage);
        }


        // -------------Tests for StartGame() method----------

        // 1
        [Fact]
        public void StartGame_IsGameStarted_ReturnTrue()
        {
            // Goal: Invoke the function StartGame to check .
            // This means that the function should return true

            // Arrange
            var room = new PokerClassLibrary.Room();
            var user1 = new PokerClassLibrary.User(); // User to Add
            var user2 = new PokerClassLibrary.User(); // User to Add
            bool result; // Contains the boolean value that return from function

            // Act
            user1.UserInGame = new Poker.DataModel.UserInGame();
            user2.UserInGame = new Poker.DataModel.UserInGame();
            room.AddUser(user1, user1.Money); // Adding the user to the room
            room.AddUser(user2, user2.Money); // Adding the user to the room
            user1.UserInGame.Room = room;
            user2.UserInGame.Room = room;
            result = room.StartGame(); // Should set result to true

            // Assert
            Assert.Equal(GameStage.Preflop, room.Stage);
            Assert.Equal(0,room.Pot);
            Assert.Equal(2, user1.UserInGame.Cards.Count);
            Assert.Equal(2, user2.UserInGame.Cards.Count);
            Assert.True(result);
        }

        // -------------Tests for Raise(UserInGame,amount) method----------
        
        // 1
        [Fact]
        public void Raise_IsCanRaise_ReturnTrue()
        {
            // Goal: Check if the raise can be done by UserInGame because amount > MoneyInTable

            // Arrange
            var room = new PokerClassLibrary.Room();
            var user1 = new PokerClassLibrary.User(); // User to Add
            var user2 = new PokerClassLibrary.User(); // User to Add
            bool result; // Contains the boolean value that return from function

            // Act
            user1.UserInGame = new Poker.DataModel.UserInGame();
            user2.UserInGame = new Poker.DataModel.UserInGame();
            room.AddUser(user1, user1.Money); // Adding the user to the room
            room.AddUser(user2, user2.Money); // Adding the user to the room
            user1.UserInGame.Room = room;
            user2.UserInGame.Room = room;
            room.StartGame(); // Should set result to true
            user1.UserInGame.MoneyInTable = 50;
            result = room.Raise(user1.UserInGame, 100);

            // Assert
            Assert.True(result);
        }

        // -------------Tests for Check(UserInGame) method----------
        // 1 
        [Fact]
        public void Check_InvalidOperation_ReturnFalse()
        {
            // Goal: Check if the raise can be done by UserInGame because amount > MoneyInTable

            // Arrange
            var room = new PokerClassLibrary.Room();
            var user = new PokerClassLibrary.User();
            var user_in_game = new Poker.DataModel.UserInGame();
            bool result;

            // Act
            //user.UserInGame = user_in_game;
            //result = room.AddUser(user, 1000);
            room.TurnStake = 1; // Set to positive number
            result = room.Check(user_in_game);

            // Assert
            Assert.False(result);
        }

        // -------------Tests for ResetGame() method----------
        // 1 
        [Fact]
        public void ResetGame_IsPotZero_Void()
        {
            // Goal: Invoke the function ResetGame and check if the pot == 0

            // Arrange
            var room = new PokerClassLibrary.Room();

            // Act
            room.ResetGame();

            // Assert
            Assert.Equal(0,room.Pot);
        }

        // 2
        [Fact]
        public void ResetGame_IsGameStageStopped_Void()
        {
            // Goal: Invoke the function ResetGame and check if the GameStage == Stopped

            // Arrange
            var room = new PokerClassLibrary.Room();

            // Act
            room.ResetGame();

            // Assert
            Assert.Equal(GameStage.Stopped, room.Stage);
        }

        // 3
        [Fact]
        public void ResetGame_IsTurnStakeZero_Void()
        {
            // Goal: Invoke the function ResetGame and check if the TurnStake == 0

            // Arrange
            var room = new PokerClassLibrary.Room();

            // Act
            room.ResetGame();

            // Assert
            Assert.Equal(0, room.TurnStake);
        }
    }
}
