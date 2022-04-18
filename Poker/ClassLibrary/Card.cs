using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.ClassLibrary
{
    public class Card
    {
        public enum CardType { Clubs, Spades, Hearts, Diamonds }
        public CardType Type { get; set; }  
        public int Number { get; set; }
        public static Stack<Card> generateDeck()
        {
            List<Card> cards = new List<Card>();
            for (int i = 2; i <= 14; i++)
                foreach (CardType type in Enum.GetValues(typeof(CardType)))
                    cards.Add(new Card()
                    {
                        Type = type,
                        Number = i,
        }) ;

            cards = cards.Distinct().OrderBy(x => System.Guid.NewGuid().ToString()).ToList();
            return new Stack<Card>(cards);
        }
    }
}
