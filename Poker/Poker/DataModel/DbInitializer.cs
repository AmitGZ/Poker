using Poker;
using PokerClassLibrary;
using System;
using System.Linq;

namespace Poker.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PokerContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

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
    }
}