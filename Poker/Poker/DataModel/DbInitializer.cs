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

            // Look for any Users.
            if (!context.Users.Any())
            {
                // Adding New Users
                var Users = new User[]
                {
                new User{Username="Amit",    Password="1234",     UserMoney=123},
                new User{Username="Ofek",    Password="1234",     UserMoney=123},
                new User{Username="Arturo",  Password="Anand",    UserMoney=123},
                new User{Username="Gytis",   Password="Barzdukas",UserMoney=123},
                new User{Username="Yan",     Password="Li",       UserMoney=123},
                new User{Username="Peggy",   Password="Justice",  UserMoney=123},
                new User{Username="Laura",   Password="Norman",   UserMoney=123},
                new User{Username="Nino",    Password="Olivetto", UserMoney=123}
                };
                foreach (User s in Users)
                {
                    context.Users.Add(s);
                }

                context.SaveChanges();
            }

            foreach (var r in context.Rooms)
            {
                context.Rooms.Remove(r);
            }
            context.SaveChanges();
            // Look for any Rooms.
            if (!context.Rooms.Any())
            {
                // Adding New Rooms
                var Rooms = new Room[]
                {
                new Room{RoomName = "Test2", RoomId = 456, Players = new List<UserInRoom>()}
                };
                foreach (Room r in Rooms)
                {
                    context.Rooms.Add(r);
                }

                context.SaveChanges();
            }
        }
    }
}