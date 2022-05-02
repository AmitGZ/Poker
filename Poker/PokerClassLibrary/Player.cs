using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.ClassLibrary
{
    public class Player
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool isInGame { get; set; }

        public List<Card> Cards = new List<Card>();

        internal void DealPlayerCards()
        {
            throw new NotImplementedException();
        }
    }
}