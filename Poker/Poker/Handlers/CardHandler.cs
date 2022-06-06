using PokerClassLibrary;
using System;
using System.Collections.Generic;

namespace Poker.Handlres
{
    // This class use for implementing global methods for card
    public static class CardHandler
    {
        // This function creates and shuffle a deck of cards
        public static List<Card> GenerateShuffledDeck()
        {
            // Generating deck
            List<Card> tmpDeck = new List<Card>();
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 13; j++)
                {
                    tmpDeck.Add(new Card() { Suit = (CardSuit)i, Value = (CardValue)j });
                }
            }

            // Shuffling Deck
            int n = tmpDeck.Count;
            Random rng = new Random();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = tmpDeck[k];
                tmpDeck[k] = tmpDeck[n];
                tmpDeck[n] = value;
            }
            return tmpDeck;
        }
    }
}
