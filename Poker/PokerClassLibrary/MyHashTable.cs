using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.ClassLibrary
{
    public class MyHashTable
    {
        private List<Player> table_members;
        public MyHashTable(int number_of_items)
        {
            table_members = new List<Player>();
            for (int i = 0; i < number_of_items; i++)
                table_members.Add(null); // default is null in external classes
        }
        private bool isEmpty()
        {
            // True : all items arr null.
            // False: there is NOT null item.
            return table_members.Where(p => p != null).Count() == 0;
        }
        private bool isFull()
        {
            // True : number_of_items from the Builder is equal to
            //        the number of Not null elements. Can't find null element.
            // False: there is at least 1 null element in the Hash Table.
            return (table_members.Count() == table_members.Where(p => p != null).Count());
        }
        public bool Add(int index, Player value)
        {
            if (isFull())
                return false;
            table_members[index] = value;
            return true;
        }
        public void Remove(string player_id)
        {
            int index = table_members.FindIndex(p => p.Id == player_id);
            table_members[index] = null;
        }
        public void setEveryonePlaying()
        {
            foreach (Player player in table_members.Where(p => p.isInGame == false))
                player.isInGame = true;
        }
        public int Count()
        { return table_members.Count; }
        public Player FindAdjacentPlayer(Player current_player)
        {
            int current_player_index = table_members.FindIndex(p => p.Id == current_player.Id);
            int tmp_player_index = current_player_index;
            do
            {
                tmp_player_index = (tmp_player_index + 1) % table_members.Count;
                if (table_members[tmp_player_index] != null)
                    break;
            } while (tmp_player_index != current_player_index);
            return table_members[tmp_player_index];
        }
    }
}