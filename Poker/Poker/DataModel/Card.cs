using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerClassLibrary
{
    public class Card
    {
        public int _id { get; set; }
        public enum CardType { Clubs, Spades, Hearts, Diamonds }
        public CardType _type { get; set; }
        public int _number { get; set; }
        public int _roomId { get; set; }
    }
}