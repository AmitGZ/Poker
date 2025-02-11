﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using BluffinMuffin.HandEvaluator;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poker.DataModel;
using Poker.Handlres;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PokerClassLibrary
{
    static class Constants
    {
        public const int TurnTime = 20;
        public const int BigBlind = 10;
    }
    public enum GameStage
    {
        Stopped,
        Preflop,
        Flop,
        Turn,
        River,
        Finished
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
        public int TurnTime{ get; set; }

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
            BigBlind = Constants.BigBlind;
            TurnTime = Constants.TurnTime;
        }

        public bool AddUser(User user, int enterMoney)
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

            return true;
        }

        public bool RemoveUser(User user)
        {
            bool gameRunning = false;
            if (Stage != GameStage.Stopped && Stage != GameStage.Finished)
            {
                gameRunning = Fold(user.UserInGame);
            }

            // Returning player to lobby
            Users.Remove(user.UserInGame);
            user.Money += (int)user.UserInGame.MoneyInTable;

            return gameRunning;
        }

        private bool EndGame()
        {
            // Changing the dealer
            if (Users.Count() > 0)
            {
                int dealerIndex = Users.OrderBy(u => u.Position).ToList().FindIndex(u => u.Position == DealerPosition);
                DealerPosition = Users.OrderBy(u => u.Position).Select(u=>u.Position).ToList().ElementAt((dealerIndex + 1) % Users.Count());
            }

            UserInGame winner = CheckWinner();

            // Resetting table
            winner.MoneyInTable += Pot; // Giving money to winner
            Pot = 0;
            Stage = GameStage.Finished;

            return true;
        }

        public bool StartGame()
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
            Users.ForEach(u =>  u.ResetUser());

            // Dealing cards 
            List<Card> tmpDeck = CardHandler.GenerateShuffledDeck();

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

            return true;
        }

        public bool Fold(UserInGame userInGame)
        {
            userInGame.IsActive = false;                                         // Setting inactive
            userInGame.Cards.ToList().ForEach(c => userInGame.Cards.Remove(c));  // Returning cards

            return FinishTurn(userInGame);
        }

        public bool Call(UserInGame userInGame)
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

            return FinishTurn(userInGame);
        }

        public bool Raise(UserInGame userInGame, int amount)
        {
            // Validating user can raise
            if (userInGame.MoneyInTable < amount)
                return true;

            TurnStake += (int)amount;
            userInGame.MoneyInTable -= (int)amount;
            userInGame.MoneyInTurn += (int)amount;

            // Going another Stage
            Users.Where(u => u.IsActive == true).ToList().ForEach(u => u.PlayedThisTurn = false);

            return FinishTurn(userInGame);
        }

        public bool Check(UserInGame userInGame)
        {
            if (TurnStake > 0)
            {
                return false; // Invalid operation
            }
            return FinishTurn(userInGame);
        }

        private bool FinishTurn(UserInGame userInGame)
        {
            // Getting list of all player positions
            List<short> activePositions = Users.Where(u => u.IsActive == true).OrderBy(u => u.Position).Select(u => u.Position).ToList();
            if (activePositions.Count() <= 1)
            {
                // Ending game
                EndGame();
                return false;
            }

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
                    EndGame();                 // Ending game
                    return false;
                }
            }

            // Setting new talking position
            TalkingPosition = activePositions.ElementAt((activePositions.IndexOf(TalkingPosition) + 1) % activePositions.Count());

            return true;
        }

        public UserInGame CheckWinner()
        {
            // Getting list of all active players
            IStringCardsHolder[] activePlayers = Users.Where(u => u.IsActive == true).ToArray();

            //  Evaluating winner
            EvaluatedCardHolder<IStringCardsHolder> winner = HandEvaluators.Evaluate(activePlayers).ElementAt(0).ElementAt(0);
            
            // Setting winner
            ((UserInGame)(winner.CardsHolder)).IsWinner = true;
            ((UserInGame)(winner.CardsHolder)).BestHand = winner.Evaluation.ToString();
            
            return (UserInGame)(winner.CardsHolder);    // Returning winner
        }

        public void ResetGame()
        {
            this.CardsOnTable = new List<Card>();
            this.Pot = 0;
            this.Pots = new List<Pot>();
            this.Stage = GameStage.Stopped;
            this.TurnStake = 0;
            this.Users.ForEach(u => u.ResetUser());
        }
    }
}