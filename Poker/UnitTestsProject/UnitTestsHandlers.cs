using Poker.Handlres;
using PokerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestsProject
{
    // This tests class deals with the functions implemented in class CardHandler
    // The only function in this CardHandler is GenerateShuffledDeck
    public class UnitTestsHandlers
    {
        // -------------Tests for GenerateShuffledDeck() method----------
        // 1
        [Fact]
        public void GenerateShuffledDeck_IsCountCorrect_ReturnDeckOfCards()
        {
            // Goal: Invoke the method GenerateShuffledDeck() and check if the deck (List) contains 52 cards

            // Arrange
            List<Card> cards = new List<Card>();

            // Act
            cards = CardHandler.GenerateShuffledDeck();

            // Assert
            Assert.Equal(52, cards.Count);
        }
    }
}
