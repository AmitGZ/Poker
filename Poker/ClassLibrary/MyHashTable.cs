using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.ClassLibrary
{
    public class MyHashTable<T>
    {
        private List<T> table;

        public MyHashTable(int number_of_items)
        {
            table = new List<T>();
            for(int i = 0; i < number_of_items; i++)
                table.Add(default); // default is null in external classes
        }

        private bool isEmpty()
        {
            // True : all items arr null.
            // False: there is NOT null item.
            return table.Where(t => t != null).Count() == 0;
        }
        private bool isFull()
        {
            // True : number_of_items from the Builder is equal to
            //        the number of Not null elements. Can't find null element.
            // False: there is at least 1 null element in the Hash Table.
            return (table.Count() == table.Where(t => t != null).Count());
        }
        public bool Add(int index,T value)
        {
            if(isFull())
                return false;
            int availble_index = nextAvialbleIndex(index);
            table[availble_index] = value;
            return true;
        }
        private int nextAvialbleIndex(int index)
        {
            // TODO : reurn the next null cell index
            throw new NotImplementedException();
        }

        public void Remove(T value)
        { 
            // TODO : remove a new player from the hash table
        }

        public int Count()
        { return table.Count; }

    }
}
