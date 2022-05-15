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

            // Removing all Users
            foreach (var u in context.Users)
            {
                context.Users.Remove(u);
            }

            // Adding New Users
            var Users = new User[]
            {
            new User{_username="Amit",    _password="1234",      _money=1000},
            new User{_username="Ofek",    _password="1234",      _money=1000},
            new User{_username="Arturo",  _password="Anand",     _money=1000},
            new User{_username="Gytis",   _password="Barzdukas", _money=1000},
            new User{_username="Yan",     _password="Li",        _money=1000},
            new User{_username="Peggy",   _password="Justice",   _money=1000},
            new User{_username="Laura",   _password="Norman",    _money=1000},
            new User{_username="Nino",    _password="Olivetto",  _money=1000}
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
            new Room{_name = "Test" }
            };

            foreach (Room r in Rooms)
            {
                context.Rooms.Add(r);
            }
            context.SaveChanges();

        }
    }
}