using BluffinMuffin.HandEvaluator;
using PokerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.DataModel
{
    public class UserInGame : IStringCardsHolder
    {
        public string Id { get; set; }
        public int MoneyInTable { get; set; }
        public int MoneyInTurn { get; set; }
        public bool IsActive { get; set; }
        public bool PlayedThisTurn { get; set; }
        public short Position { get; set; }
        public virtual Room Room { get; set; }
        public string Username { get; set; } // FK User
        public virtual User User { get; set; }
        public virtual List<Card>? Cards { get; set; }
        public Boolean IsWinner { get; set; }
        public String BestHand { get; set; }

        IEnumerable<string> IStringCardsHolder.PlayerCards => PlayerCards();

        IEnumerable<string> IStringCardsHolder.CommunityCards => CommunityCards();

        private IEnumerable<string> PlayerCards()
        {
            List<string> formattedCards = new List<string>();
            foreach (Card card in Cards)
                formattedCards.Add(card.ToString());
            return formattedCards;
        }

        private IEnumerable<string> CommunityCards()
        {
            List<string> formattedCards = new List<string>();
            foreach(Card card in Room.CardsOnTable)
                formattedCards.Add(card.ToString());
            return formattedCards;
        }

        public UserInGame()
        {
            Cards = new List<Card>();
            MoneyInTable = 0;
            MoneyInTurn = 0;
            PlayedThisTurn = false;
            Position = 0;
            IsActive = false;
            Room = null;
            IsWinner = false;
            BestHand = null;
        }
        public UserInGame(int enterMoney, short pos)
        {
            Cards = new List<Card>();
            MoneyInTable = enterMoney;
            MoneyInTurn = 0;
            PlayedThisTurn = false;
            Position = pos;
            IsActive = false;
            IsWinner = false;
            BestHand = null;
        }
    }
}
