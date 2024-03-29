﻿using Laboratorio_7_OOP_201902.Cards;
using Laboratorio_7_OOP_201902.Enums;
using Laboratorio_7_OOP_201902.Static;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Laboratorio_7_OOP_201902
{
    public class Game
    {
        //Constantes
        private const int DEFAULT_CHANGE_CARDS_NUMBER = 3;
        private string Direct = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        //Atributos
        private Player[] players;
        private Player activePlayer;
        private List<Deck> decks;
        private List<SpecialCard> captains;
        private Board boardGame;
        internal int turn;

        //Constructor
        public Game()
        {
            Random random = new Random();
            decks = new List<Deck>();
            captains = new List<SpecialCard>();
            AddDecks();
            AddCaptains();
            players = new Player[2] { new Player(), new Player() };
            ActivePlayer = Players[random.Next(2)];
            boardGame = new Board();
            //Add board to players
            players[0].Board = boardGame;
            players[1].Board = boardGame;
            turn = 0;
        }
        //Propiedades
        public Player[] Players
        {
            get
            {
                return this.players;
            }
        }
        public Player ActivePlayer
        {
            get
            {
                return this.activePlayer;
            }
            set
            {
                activePlayer = value;
            }
        }
        public List<Deck> Decks
        {
            get
            {
                return this.decks;
            }
        }
        public List<SpecialCard> Captains
        {
            get
            {
                return this.captains;
            }
        }
        public Board BoardGame
        {
            get
            {
                return this.boardGame;
            }
        }


        //Metodos
        public bool CheckIfEndGame()
        {
            if (players[0].LifePoints == 0 || players[1].LifePoints == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetWinner()
        {
            if (players[0].LifePoints == 0 && players[1].LifePoints > 0)
            {
                return 1;
            }
            else if (players[1].LifePoints == 0 && players[0].LifePoints > 0)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        public int GetRoundWinner()
        {
            if (Players[0].GetAttackPoints()[0] == Players[1].GetAttackPoints()[0])
            {
                return -1;
            }
            else
            {
                int winner = Players[0].GetAttackPoints()[0] > Players[1].GetAttackPoints()[0] ? 0 : 1;
                return winner;
            }
        }
        public void Play()
        {
            int userInput = 0;
            int userInput1 = 0;
            int userInput2 = 0;
            int contChanges = 0;
            int firstOrSecondUser = ActivePlayer.Id == 0 ? 0 : 1;
            int winner = -1;
            bool bothPlayersPlayed = false;


            while (turn < 4 && !CheckIfEndGame())
            {
                bool drawCard = false;
                bool endCicle = true;
                bool auxCicle = true;
                if (File.Exists("SavePlayers.bin") || auxCicle)
                {
                    Visualization.ShowListOptions(new List<string> { "Start New Game", "Load Game" }, "Wanna load a game or star a new one???");
                    userInput2 = Visualization.GetUserInput(1);
                    if (userInput2 == 0)
                    {
                        for (int _ = 0; _ < Players.Length; _++)
                        {
                            ActivePlayer = Players[firstOrSecondUser];
                            Visualization.ClearConsole();
                            //Mostrar mensaje de inicio
                            Visualization.ShowProgramMessage($"Player {ActivePlayer.Id + 1} select Deck and Captain:");
                            //Preguntar por deck
                            Visualization.ShowDecks(this.Decks);
                            userInput = Visualization.GetUserInput(this.Decks.Count - 1);
                            Deck deck = new Deck();
                            deck.Cards = new List<Card>(Decks[userInput].Cards);
                            ActivePlayer.Deck = deck;
                            //Preguntar por capitan
                            Visualization.ShowCaptains(Captains);
                            userInput = Visualization.GetUserInput(this.Captains.Count - 1);
                            ActivePlayer.ChooseCaptainCard(new SpecialCard(Captains[userInput].Name, Captains[userInput].Type, Captains[userInput].Effect));
                            //Asignar mano
                            ActivePlayer.FirstHand();
                            //Mostrar mano
                            while (endCicle)
                            {
                                Visualization.ShowHand(ActivePlayer.Hand);
                                //Mostar opciones, cambiar carta o pasar
                                Visualization.ShowListOptions(new List<string>() { "Change Card", "Show card info", "Pass" }, "Change 3 cards or ready to play:");
                                userInput = Visualization.GetUserInput(2);
                                if (userInput == 0)
                                {
                                    if (contChanges < 3)
                                    {
                                        Visualization.ClearConsole();
                                        Visualization.ShowProgramMessage($"Player {ActivePlayer.Id + 1} change cards:");
                                        Visualization.ShowHand(ActivePlayer.Hand);
                                        for (int i = 0; i < DEFAULT_CHANGE_CARDS_NUMBER; i++)
                                        {
                                            Visualization.ShowProgramMessage($"Input the numbers of the cards to change (max {DEFAULT_CHANGE_CARDS_NUMBER}). To stop enter -1");
                                            userInput = Visualization.GetUserInput(ActivePlayer.Hand.Cards.Count, true);
                                            if (userInput == -1)
                                            {
                                                break;
                                            }
                                            ActivePlayer.ChangeCard(userInput);
                                            contChanges += 1;
                                            Visualization.ShowHand(ActivePlayer.Hand);
                                        }
                                        Visualization.ClearConsole();
                                    }
                                    else
                                    {
                                        Visualization.ShowProgramMessage("Already Changed the max number of cards");
                                        Visualization.ShowProgramMessage("Press any key to continue");
                                        Visualization.PressToContinue();
                                        Visualization.ClearConsole();
                                    }
                                }
                                if (userInput == 1)
                                {
                                    Visualization.ClearConsole();
                                    Visualization.ShowHand(ActivePlayer.Hand);
                                    Visualization.ShowProgramMessage("Input the number of the card you want to see the info. To cancel enter -1");
                                    userInput1 = Visualization.GetUserInput(ActivePlayer.Hand.Cards.Count, true);
                                    if (userInput1 != -1)
                                    {

                                        if (ActivePlayer.Hand.Cards[userInput1] is CombatCard)
                                        {
                                            CombatCard aux = ActivePlayer.Hand.Cards[userInput1] as CombatCard;
                                            Visualization.ShowProgramMessage("Name: " + aux.GetCharacteristics()[0]);
                                            Visualization.ShowProgramMessage("Type: " + aux.GetCharacteristics()[1]);
                                            Visualization.ShowProgramMessage("Effect: " + aux.GetCharacteristics()[2]);
                                            Visualization.ShowProgramMessage("AttackPoints: " + aux.GetCharacteristics()[3]);
                                            Visualization.ShowProgramMessage("Hero: " + aux.GetCharacteristics()[4]);
                                            Visualization.ShowProgramMessage("");
                                            Visualization.ShowProgramMessage("Press any key to continue");
                                            Visualization.PressToContinue();

                                        }
                                        else
                                        {
                                            SpecialCard aux = ActivePlayer.Hand.Cards[userInput1] as SpecialCard;
                                            Visualization.ShowProgramMessage("Name: " + aux.GetCharacteristics()[0]);
                                            Visualization.ShowProgramMessage("Type: " + aux.GetCharacteristics()[1]);
                                            Visualization.ShowProgramMessage("Effect: " + aux.GetCharacteristics()[2]);
                                            Visualization.ShowProgramMessage("");
                                            Visualization.ShowProgramMessage("Press any key to continue");
                                            Visualization.PressToContinue();
                                        }
                                    }
                                    Visualization.ClearConsole();
                                }
                                if (userInput == 2)
                                {
                                    firstOrSecondUser = ActivePlayer.Id == 0 ? 1 : 0;
                                    endCicle = false;
                                }
                            }
                            endCicle = true;
                            contChanges = 0;
                        }
                        Save();
                        turn += 1;
                        auxCicle = false;
                    }
                    else
                    {
                        Load();
                    }
                }
                while (true)
                {
                    ActivePlayer = Players[firstOrSecondUser];
                    //Obtener lifePoints
                    int[] lifePoints = new int[2] { Players[0].LifePoints, Players[1].LifePoints };
                    //Obtener total de ataque:
                    int[] attackPoints = new int[2] { Players[0].GetAttackPoints()[0], Players[1].GetAttackPoints()[0] };
                    //Mostrar tablero, mano y solicitar jugada
                    Visualization.ClearConsole();
                    Visualization.ShowBoard(boardGame, ActivePlayer.Id, lifePoints, attackPoints);
                    //Robar carta
                    if (!drawCard)
                    {
                        ActivePlayer.DrawCard();
                        drawCard = true;
                    }
                    Visualization.ShowHand(ActivePlayer.Hand);
                    Visualization.ShowListOptions(new List<string> { "Play Card", "Show Card Info", "Pass" }, $"Make your move player {ActivePlayer.Id + 1}:");
                    userInput = Visualization.GetUserInput(2);
                    if (userInput == 0)
                    {
                        //Si la carta es un buff solicitar a la fila que va.
                        Visualization.ShowProgramMessage($"Input the number of the card to play. To cancel enter -1");
                        userInput = Visualization.GetUserInput(ActivePlayer.Hand.Cards.Count, true);
                        if (userInput != -1)
                        {
                            if (ActivePlayer.Hand.Cards[userInput].Type == EnumType.buff)
                            {
                                Visualization.ShowListOptions(new List<string> { "Melee", "Range", "LongRange" }, $"Choose row to buff {ActivePlayer.Id}:");
                                int cardId = userInput;
                                userInput = Visualization.GetUserInput(2);
                                if (userInput == 0)
                                {
                                    ActivePlayer.PlayCard(cardId, EnumType.buffmelee);
                                }
                                else if (userInput == 1)
                                {
                                    ActivePlayer.PlayCard(cardId, EnumType.buffrange);
                                }
                                else
                                {
                                    ActivePlayer.PlayCard(cardId, EnumType.bufflongRange);
                                }
                            }
                            else
                            {
                                ActivePlayer.PlayCard(userInput);
                            }
                        }
                        //Revisar si le quedan cartas, si no le quedan obligar a pasar.
                        if (ActivePlayer.Hand.Cards.Count == 0)
                        {
                            firstOrSecondUser = ActivePlayer.Id == 0 ? 1 : 0;
                            break;
                        }
                    }
                    if (userInput == 1)
                    {
                        Visualization.ClearConsole();
                        Visualization.ShowHand(ActivePlayer.Hand);
                        Visualization.ShowProgramMessage("Input the number of the card you want to see the info. To cancel enter -1");
                        userInput1 = Visualization.GetUserInput(ActivePlayer.Hand.Cards.Count, true);
                        if (userInput1 != -1)
                        {

                            if (ActivePlayer.Hand.Cards[userInput1] is CombatCard)
                            {
                                CombatCard aux = ActivePlayer.Hand.Cards[userInput1] as CombatCard;
                                Visualization.ShowProgramMessage("Name: " + aux.GetCharacteristics()[0]);
                                Visualization.ShowProgramMessage("Type: " + aux.GetCharacteristics()[1]);
                                Visualization.ShowProgramMessage("Effect: " + aux.GetCharacteristics()[2]);
                                Visualization.ShowProgramMessage("AttackPoints: " + aux.GetCharacteristics()[3]);
                                Visualization.ShowProgramMessage("Hero: " + aux.GetCharacteristics()[4]);
                                Visualization.ShowProgramMessage("");
                                Visualization.ShowProgramMessage("Press any key to continue");
                                Visualization.PressToContinue();

                            }
                            else
                            {
                                SpecialCard aux = ActivePlayer.Hand.Cards[userInput1] as SpecialCard;
                                Visualization.ShowProgramMessage("Name: " + aux.GetCharacteristics()[0]);
                                Visualization.ShowProgramMessage("Type: " + aux.GetCharacteristics()[1]);
                                Visualization.ShowProgramMessage("Effect: " + aux.GetCharacteristics()[2]);
                                Visualization.ShowProgramMessage("");
                                Visualization.ShowProgramMessage("Press any key to continue");
                                Visualization.PressToContinue();
                            }
                        }
                        Visualization.ClearConsole();
                    }
                    if (userInput == 2)
                    {
                        firstOrSecondUser = ActivePlayer.Id == 0 ? 1 : 0;
                        break;
                    }

                }
                //Cambiar al oponente si no ha jugado
                if (!bothPlayersPlayed)
                {
                    bothPlayersPlayed = true;
                    drawCard = false;
                }
                //Si ambos jugaron obtener el ganador de la ronda, reiniciar el tablero y pasar de turno.
                else
                {
                    winner = GetRoundWinner();
                    if (winner == 0)
                    {
                        Players[1].LifePoints -= 1;
                    }
                    else if (winner == 1)
                    {
                        Players[0].LifePoints -= 1;
                    }
                    else
                    {
                        Players[0].LifePoints -= 1;
                        Players[1].LifePoints -= 1;
                    }
                    bothPlayersPlayed = false;
                    turn += 1;
                    //Destruir Cartas
                    BoardGame.DestroyCards();
                }
            }
            //Definir cual es el ganador.
            winner = GetWinner();
            if (winner == 0)
            {
                Visualization.ShowProgramMessage($"Player 1 is the winner!");
            }
            else if (winner == 1)
            {
                Visualization.ShowProgramMessage($"Player 2 is the winner!");
            }
            else
            {
                Visualization.ShowProgramMessage($"Draw!");
            }

        }
        public void AddDecks()
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent + @"\Files\Decks.txt";
            StreamReader reader = new StreamReader(path);
            int deckCounter = 0;
            List<Card> cards = new List<Card>();


            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string [] cardDetails = line.Split(",");

                if (cardDetails[0] == "END")
                {
                    decks[deckCounter].Cards = new List<Card>(cards);
                    deckCounter += 1;
                }
                else
                {
                    if (cardDetails[0] != "START")
                    {
                        if (cardDetails[0] == nameof(CombatCard))
                        {
                            cards.Add(new CombatCard(cardDetails[1], (EnumType) Enum.Parse(typeof(EnumType),cardDetails[2]), cardDetails[3], Int32.Parse(cardDetails[4]), bool.Parse(cardDetails[5])));
                        }
                        else
                        {
                            cards.Add(new SpecialCard(cardDetails[1], (EnumType)Enum.Parse(typeof(EnumType), cardDetails[2]), cardDetails[3]));
                        }
                    }
                    else
                    {
                        decks.Add(new Deck());
                        cards = new List<Card>();
                    }
                }

            }
            
        }
        public void AddCaptains()
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent + @"\Files\Captains.txt";
            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] cardDetails = line.Split(",");
                captains.Add(new SpecialCard(cardDetails[1], (EnumType)Enum.Parse(typeof(EnumType), cardDetails[2]), cardDetails[3]));
            }
        }

        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("SavePlayers.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this.players);
            stream.Close();

            IFormatter formatte = new BinaryFormatter();
            Stream stream1 = new FileStream("SaveActivePlayer.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream1, this.activePlayer);
            stream1.Close();

            IFormatter formatterD = new BinaryFormatter();
            Stream stream2 = new FileStream("SaveDecks.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream2, this.decks);
            stream2.Close();

            IFormatter formatterB = new BinaryFormatter();
            Stream stream3 = new FileStream("SaveBoard.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream3, this.boardGame);
            stream3.Close();

            IFormatter formatterC = new BinaryFormatter();
            Stream stream4 = new FileStream("SaveCaptains.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream4, this.captains);
            stream4.Close();
        }
        public void Load()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("SavePlayers.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            this.players = (Player[])formatter.Deserialize(stream);
            stream.Close();


            Stream stream1 = new FileStream("SaveActivePlayer.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            this.activePlayer = (Player)formatter.Deserialize(stream1);
            stream1.Close();


            Stream strea2 = new FileStream("SaveDecks.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            this.decks = (List<Deck>)formatter.Deserialize(strea2);
            strea2.Close();

            Stream strea3 = new FileStream("SaveCaptains.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            this.captains = (List<SpecialCard>)formatter.Deserialize(strea3);
            strea3.Close();

            Stream strea4 = new FileStream("SaveBoard.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            this.boardGame = (Board)formatter.Deserialize(strea4);
            strea4.Close();
        }
    }
}
