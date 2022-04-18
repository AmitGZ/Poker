using System.Collections.Generic;

namespace Poker.ClassLibrary
{
    public class MyHashTable<T>
    {
        private struct item<Struct_T>
        {
            public int key;
            public T value;
        }

        private List<item<T>?> table;

        public MyHashTable(int number_of_items)
        {
            table = new List<item<T>?>();
            for(int i = 0; i < number_of_items; i++)
                table.Add(null);
        }

        public void Add(int index,T value)
        { 
            // TODO : add a new player into the hash table
        }

        public void Remove(T value)
        {
            // TODO : remove a new player from the hash table
        }

        public int Count()
        { return table.Count; }

    }
}
