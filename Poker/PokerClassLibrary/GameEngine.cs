using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokerClassLibrary
{
    public class GameEngine
    {
        const int MAX_AMOUNT_OF_PLAYERS = 6;

        public MyHashTable Players = new MyHashTable(MAX_AMOUNT_OF_PLAYERS);
        public Stack<Card> Stack = null;
        public List<Card> Deck = new List<Card>();
        public Player Dealer { get; set; }
        public bool isRunning = false;

        internal void AddPlayer(string connectionId, int location)
        {
            // Each player have his location on the current table
            // Each player have his unique Id
            Player NewPlayer = new Player()
            {
                Id = connectionId,
                UserName = "Amit",// need to read from DB 
                isInGame = false
            };
            Players.Add(location, NewPlayer);
        }
        public void RemovePlayerById(string Id)
        {
            Players.Remove(Id);
        }
        public void StartGame()
        {
            Players.setEveryonePlaying();
            SwitchDealer();
            Stack = Card.generateDeck();
            DealTheCards();
        }
        private void DealTheCards()
        {
            Player tmp = Dealer;
            do
            {
                tmp.Cards.Add(Stack.Pop());
                tmp.Cards.Add(Stack.Pop());
                tmp = Players.FindAdjacentPlayer(tmp);
            } while (tmp.Id != Dealer.Id);
        }
        private void SwitchDealer()
        {
            Dealer = Players.FindAdjacentPlayer(Dealer);
        }
    }
}