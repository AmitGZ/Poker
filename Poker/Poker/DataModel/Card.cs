using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerClassLibrary
{
    public enum CardSuit
    {
        Clubs,
        Hearts,
        Spades,
        Diamonds
    }
    public class Card
    {
        public string Id { get; set; }
        public CardSuit Suit { get; set; }
        public int Number { get; set; }
        public string UserId { get; set; }
        public string RoomId { get; set; }
    }
}