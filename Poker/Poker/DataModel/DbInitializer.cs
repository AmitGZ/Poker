using PokerClassLibrary;
using System.Collections.Generic;
using System.Linq;

namespace Poker.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PokerContext context)
        {
            context.Database.EnsureCreated();

            // Removing all Cards
            foreach (var c in context.Cards)
            {
                context.Cards.Remove(c);
            }

            // Removing all Users
            foreach (var u in context.Users)
            {
                context.Users.Remove(u);
            }

            // Adding New Users
            var Users = new User[]
            {
            new User{Username="Amit",    Password="1234",      Money=1000},
            new User{Username="Ofek",    Password="1234",      Money=1000},
            new User{Username="Golan",   Password="1234",      Money=1000},
            new User{Username="Ofir",    Password="1234",      Money=1000},
            new User{Username="Yan",     Password="Li",        Money=1000},
            new User{Username="Peggy",   Password="Justice",   Money=1000},
            new User{Username="Laura",   Password="Norman",    Money=1000},
            new User{Username="Nino",    Password="Olivetto",  Money=1000}
            };
            foreach (User s in Users)
            {
                context.Users.Add(s);
            }
            context.SaveChanges();
            
            // Removing all Rooms
            foreach (var r in context.Rooms)
            {
                context.Rooms.Remove(r);
            }

            // Adding New Rooms
            var Rooms = new Room[]
            {
            // new Room{Name = "Test" }
            };
            
            foreach (Room r in Rooms)
            {
                context.Rooms.Add(r);
            }

            context.SaveChanges();
        }
    }
}