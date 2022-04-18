using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Poker.ClassLibrary
{
    public class GameEngine
    {
        public Queue<Player> Players = new Queue<Player>(); 
        public Stack<Card> Stack = null;
        public List<Card> Deck = new List<Card>();
        public Player Dealer { get; set; }
        public bool isRunning = false;

        internal void AddPlayer(string connectionId)
        {
            Player NewPlayer = new Player()
            {
                Id = connectionId,
                UserName = "Amit",// need to read from DB 
                isInGame = false
            };
            Players.Enqueue(NewPlayer);
        }

        //public Player GetPlayerById(string Id)
        //{
        //    return Players.SingleOrDefault(p => p.Id == Id);
        //}
        public void RemovePlayerById(string Id)
        {
            Players = new Queue<Player>(Players.Where(p => p.Id != Id));
        }

        public void StartGame()
        {
            // Put All Players In Game
            foreach (Player player in Players.Where(p => p.isInGame == false))
                player.isInGame = true;

            SwitchDealer();

            Stack = Card.generateDeck();

            DealTheCards();
        }
        private void DealTheCards()
        {
            while (Players.Peek().Id != Dealer.Id)
            {
                Player player = Players.Dequeue();
                player.Cards.Add(Stack.Pop());
                player.Cards.Add(Stack.Pop());
                Players.Enqueue(player);
            }
        }

        private void GoToDealerPlus(int x)
        {
            // Go To Dealer
            while (Players.Peek().Id != Dealer.Id)
                Players.Enqueue(Players.Dequeue());

            for (int i = 0; i < x; i++)
                Players.Enqueue(Players.Dequeue());
        }
        private void SwitchDealer()
        {
            GoToDealerPlus(1);
            Dealer = Players.Peek(); 
        }
    }
}
