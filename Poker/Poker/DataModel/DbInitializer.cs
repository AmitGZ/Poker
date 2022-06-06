using BluffinMuffin.HandEvaluator;
using Poker.DataModel;
using PokerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PokerContext context)
        {

            List<CalculateUser> users = new List<CalculateUser>();
            users.Add(new CalculateUser());
            users.Add(new CalculateUser());

            string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            string[] suits = { "C", "H", "S", "D" };
            List<string> combinations =  new List<string>();
            for (var value = 0; value < 13; value++)
            {
                for (var suit = 0; suit < 4; suit++)
                {
                    combinations.Add(values[value] + suits[suit]);
                }
            }

            for (var mycard1 = 0; mycard1 < 52; mycard1++)
            {
                for (var mycard2 = mycard1 + 1; mycard2 < 52; mycard2++)
                {
                    for (var tablecard1 = mycard2 + 1; tablecard1 < 52; tablecard1++)
                    {
                        for (var tablecard2 = tablecard1 + 1; tablecard2 < 52; tablecard2++)
                        {
                            for (var tablecard3 = tablecard2 + 1; tablecard3 < 52; tablecard3++)
                            {
                                for (var tablecard4 = tablecard3 + 1; tablecard4 < 52; tablecard4++)
                                {
                                    for (var tablecard5 = tablecard4 + 1; tablecard5 < 52; tablecard5++)
                                    {
                                        for (var opponentcard1 = tablecard5 + 1; opponentcard1 < 52; opponentcard1++)
                                        {
                                            for (var opponentcard2 = opponentcard1 + 1; opponentcard2 < 52; opponentcard2++)
                                            {
                                                List<string> communityCards = new List<string>(new string[] { "JC", "JS", "JD", "JH", "10C" });
                                                users[0].playerCards = new List<string>(new string[] { "4S", "KC" });
                                                users[1].playerCards = new List<string>(new string[] { "4C", "5C" });
                                                users[0].communityCards = communityCards;
                                                users[1].communityCards = communityCards;
                                                HandEvaluators.Evaluate(users, communityCards);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
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

    public class CalculateUser : IStringCardsHolder
    {
        public List<string> playerCards;
        public List<string> communityCards;
        IEnumerable<string> IStringCardsHolder.PlayerCards => playerCards;
        IEnumerable<string> IStringCardsHolder.CommunityCards => communityCards;
    }
}