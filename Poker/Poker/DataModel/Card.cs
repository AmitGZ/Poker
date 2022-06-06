using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerClassLibrary
{
    public class Card
    {
        public string Id { get; set; }
        public CardSuit Suit { get; set; }
        public CardValue Value { get; set; }
        public string UserId { get; set; }
        public string RoomId { get; set; }

        public override string ToString()
        {
            string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            string[] suits =  { "C", "H", "S", "D" };
            return values[(int)Value] + suits[(int)Suit];
        }
    }

  
    public enum CardSuit
    {
        Clubs,
        Hearts,
        Spades,
        Diamonds
    }
    public enum CardValue
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }
}