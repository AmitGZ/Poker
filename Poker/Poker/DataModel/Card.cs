using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerClassLibrary
{
    public class Card
    {
        public int Id { get; set; }
        public enum CardType { Clubs, Spades, Hearts, Diamonds }
        public CardType Type { get; set; }
        public int Number { get; set; }
        public int RoomId { get; set; }
    }
}