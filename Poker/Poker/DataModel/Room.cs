﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using BluffinMuffin.HandEvaluator;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poker.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace PokerClassLibrary
{
    public enum GameStage
    {
        Stopped,
        Preflop,
        Flop,
        Turn,
        River
    }
    public partial class Room
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual List<Card> CardsOnTable { get; set; }
        public virtual List<Pot> Pots { get; set; }
        public virtual List<UserInGame> Users { get; set; }
        public short TalkingPosition { get; set; }
        public short DealerPosition { get; set; }
        public int Pot { get; set; }
        public int TurnStake { get; set; }
        public GameStage Stage { get; set; }
        public int BigBlind { get; set; }

        public Room()
        {
            Users = new List<UserInGame>();
            CardsOnTable = new List<Card>();
            Pots = new List<Pot>();
            Pot = 0;
            TurnStake = 0;
            Stage = GameStage.Stopped;
            DealerPosition = 0;
            TalkingPosition = 0;
            BigBlind = 10;
        }

        public bool AddUser(PokerContext context, User user, int enterMoney)
        {
            if (Users.Count == 5)
            {
                return false;
            }

            // Getting available position
            List<short> positions = Users.Select(p => p.Position).ToList();

            short pos = 0;
            for (; pos < 5; pos++)
                if (!positions.Contains(pos))
                    break;

            // Adding the player to the room
            user.Money -= enterMoney;
            user.UserInGame = new UserInGame(enterMoney, pos);
            Users.Add(user.UserInGame);
            context.SaveChanges();

            // If enough players start game
            if (Users.Count() == 2 && Stage == GameStage.Stopped)
                StartGame(context);

            return true;
        }

        public bool EndGame(PokerContext context)
        {

            // TODO reset pot

            // Changing the dealer
            if (Users.Count() > 0)
            {
                int dealerIndex = Users.OrderBy(u => u.Position).ToList().FindIndex(u => u.Position == DealerPosition);
                DealerPosition = Users.OrderBy(u => u.Position).Select(u=>u.Position).ToList().ElementAt((dealerIndex + 1) % Users.Count());
            }

            EvaluatedCardHolder<IStringCardsHolder> Winners = CheckWinner(context);

            // Resetting table
            Users.ToList().ForEach(u => u.Cards.ToList().ForEach(c => u.Cards.Remove(c))); // Removing cards on user
            CardsOnTable.ToList().ForEach(c => CardsOnTable.Remove(c));                    // Removing cards on table
            Users.ForEach(u => u.IsActive = false);                                        // Setting inactive
            Stage = GameStage.Stopped;                                                     // Setting game stopped
            context.SaveChanges();

            return true;
        }

        public bool StartGame(PokerContext context)
        {
            // Setting initial bet
            TurnStake = BigBlind;

            // Getting list of all player positions
            List<short> positions = Users.OrderBy(u =>u.Position).Select(u => u.Position).ToList();

            // Setting new talking position
            TalkingPosition = positions.ElementAt((positions.IndexOf(DealerPosition) + 3) % positions.Count());

            // Getting talking user
            UserInGame talklingUser = Users.FirstOrDefault(u => u.Position == TalkingPosition);

            // Set everyone active
            Users.ForEach(u => u.IsActive = true);

            // Dealing cards 
            List<Card> tmpDeck = GenerateShuffledDeck();

            int cardIdx;
            for (cardIdx = 0; cardIdx < 5; cardIdx++)
            {
                CardsOnTable.Add(tmpDeck.ElementAt(cardIdx)); // Adding table cards
            }
            Users.ForEach(u => {                              // Adding two cards to each player
                u.Cards.Add(tmpDeck.ElementAt(cardIdx++));
                u.Cards.Add(tmpDeck.ElementAt(cardIdx++));
                }
            );

            // Setting Stage and pot back to 0
            Stage = GameStage.Preflop;
            Pot = 0;

            // Updating database
            context.SaveChanges();

            return true;
        }

        public bool Fold(PokerContext context,UserInGame userInGame)
        {
            if(userInGame.Position == TalkingPosition)
            {
                FinishTurn(context, userInGame);
            }
            userInGame.IsActive = false;                                         // Setting inactive
            userInGame.Cards.ToList().ForEach(c => userInGame.Cards.Remove(c));  // Returning cards

            // Getting list of all player positions
            List<short> activePositions = Users.Where(u => u.IsActive == true).Select(u => u.Position).ToList();

            if (activePositions.Count() == 1)
            {
                // TODO Set next player the winner

                EndGame(context);            // End game

                if (Users.Count() > 1)       // Starting new game
                    StartGame(context);
            }
            context.SaveChanges();
            return true;
        }

        public bool Call(PokerContext context, UserInGame userInGame)
        {
            // User has enough money
            if (TurnStake <= userInGame.MoneyInTable)
            {
                userInGame.MoneyInTurn += TurnStake;
                userInGame.MoneyInTable -= TurnStake;
            }
            else
            {
                // Open new pot
            }

            FinishTurn(context, userInGame);
            return true;
        }

        public bool Raise(PokerContext context, UserInGame userInGame, int amount)
        {
            // Validating user can raise
            if (userInGame.MoneyInTable < amount)
                return false;

            TurnStake += (int)amount;
            userInGame.MoneyInTable -= (int)amount;
            userInGame.MoneyInTurn += (int)amount;

            //going another Stage
            Users.Where(u => u.IsActive == true).ToList().ForEach(u => u.PlayedThisTurn = false);

            FinishTurn(context, userInGame);
            return true;
        }
        public bool Check(PokerContext context, UserInGame userInGame)
        {
            if (TurnStake > 0)
            {
                return false; // Invalid operation
            }
            FinishTurn(context, userInGame);
            return true;
        }

        private bool FinishTurn(PokerContext context, UserInGame userInGame)
        {
            // Getting list of all player positions
            List<short> activePositions = Users.Where(u => u.IsActive == true).OrderBy(u => u.Position).Select(u => u.Position).ToList();

            // Setting player as already played
            userInGame.PlayedThisTurn = true;

            // Check if everyone played this turn
            if (Users.Where(u => u.IsActive == true && u.PlayedThisTurn == false).Count() == 0)
            {
                // Start new Stage
                Users.ForEach(u => u.PlayedThisTurn = false);
                Stage++;
                TurnStake = 0;
                Users.ForEach(u =>
                {
                    Pot += u.MoneyInTurn;
                    u.MoneyInTurn = 0;
                });

                // Checking if game ended
                if (Stage > GameStage.River)
                {
                    // End game
                    EndGame(context);

                    // Start new game 
                    if (Users.Count() >= 2)
                    {
                        StartGame(context);
                    }
                }
            }

            // Setting new talking position
            TalkingPosition = activePositions.ElementAt((activePositions.IndexOf(TalkingPosition) + 1) % activePositions.Count());

            // Updating database
            context.SaveChanges();

            return true;
        }

        public EvaluatedCardHolder<IStringCardsHolder> CheckWinner(PokerContext context)
        {
            // Getting list of all active players
            IStringCardsHolder[] activePlayers = Users.Where(u => u.IsActive == true).ToArray();
            return HandEvaluators.Evaluate(activePlayers).ElementAt(0).ElementAt(0);
        }
        //HandEvaluators.Evaluate(players).Where(t=> t.Key == 1).ElementAt(0).ElementAt(0)
        private List<Card> GenerateShuffledDeck() // TODO move somewhere else
        {
            // Generating deck
            List<Card> tmpDeck = new List<Card>(); 
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 13; j++)
                {
                    tmpDeck.Add(new Card() { Suit = (CardSuit)i, Value = (CardValue)j });
                }
            }

            // Shuffling Deck
            int n = tmpDeck.Count;           
            Random rng = new Random();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = tmpDeck[k];
                tmpDeck[k] = tmpDeck[n];
                tmpDeck[n] = value;
            }
            return tmpDeck;
        } 
    }
}