using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CSharpGameExample
{
    public class Game
    {
        static string activeScene = "Room1";
        const ConsoleColor defaultColor = ConsoleColor.White;
        static List<Room> Rooms = new List<Room>();

        public static void StartGame()
        {
            Console.Title = "CSharp Adventure Time";
            string titleScreen = @"
       __   ____  _  _  ____  __ _  ____  _  _  ____  ____             ___   __   _  _  ____  __    
      / _\ (    \/ )( \(  __)(  ( \(_  _)/ )( \(  _ \(  __)           / __) / _\ ( \/ )(  __)(_ \_  
     /    \ ) D (\ \/ / ) _) /    /  )(  ) \/ ( )   / ) _)           ( (_ \/    \/ \/ \ ) _)   \__) 
     \_/\_/(____/ \__/ (____)\_)__) (__) \____/(__\_)(____)           \___/\_/\_/\_)(_/(____)       
       ___  ____   __   ____                                                  _  _  __   _  _  ____ 
      / __)(  _ \ / _\ (  _ \                                                ( \/ )/  \ / )( \(  _ \
     ( (_ \ )   //    \ ) _ (                                                 )  /(  O )) \/ ( )   /
      \___/(__\_)\_/\_/(____/                                                (__/  \__/ \____/(__\_)
                                              ____  ____  __  ____  __ _  ____                      
                                             (  __)(  _ \(  )(  __)(  ( \(    \                     
                                              ) _)  )   / )(  ) _) /    / ) D (                       
                                             (__)  (__\_)(__)(____)\_)__)(____/       ";
            Console.WriteLine(titleScreen);
            Console.WriteLine("Press Any Key to Start");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("Adventure Game!");
            Console.WriteLine("Time to explore some shit~!");
            NameCharacter();
            Item wallet = new Item("Wallet", "It's your lucky wallet! Has a bear on it.");
            Character.Inventory.Add(wallet);

            string[] roomFiles = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "RoomData"));
            foreach (string roomFile in roomFiles)
            {
                string roomJson = File.ReadAllText(roomFile);
                Room newRoom = JsonConvert.DeserializeObject<Room>(roomJson);
                Rooms.Add(newRoom);
            }

            Console.Clear();
            Menu(Rooms.Find(room => room.Label == activeScene));
        }

        public static void NameCharacter()
        {
            Console.WriteLine("\nEnter player name: ");
            string playerName = Console.ReadLine();
            if (!String.IsNullOrEmpty(playerName))
            {
                Character.playerName = playerName;
            }
            Console.WriteLine($"\nWelcome to the jungle, {Character.playerName}!");
        }
        static void Dialog(string message, ConsoleColor color = defaultColor)
        {
            Console.ForegroundColor = color;
            Console.Write("\n" + message);
            Console.ResetColor();
        }
        static string Choice()
        {
            string userInput = "";
            while (String.IsNullOrEmpty(userInput))
            {
                Console.Write("\nWhat do you do?: ");
                userInput = Console.ReadLine().ToLower();
                if (userInput.StartsWith("go "))
                {
                    userInput = userInput.Substring(3);
                }
            }
            return userInput;
        }
        static bool Run
        { get; set; } = true;

        static bool Goal
        { get; set; } = false;

        public static void Menu(Room currentRoom)
        {
            Dialog(currentRoom.Description);

            string userInput = Choice();
            string[] getVerbs = { "get", "take", "possess", "steal", "pickup" };
            string[] readVerbs = { "read", "look", "check" };
            string[] talkVerbs = { "talk", "speak", "chat" };

            if (userInput.Contains(" ") && userInput.Split().Length <= 2)
            {
                string actionVerb = userInput.Split(" ")[0];
                string actionItem = userInput.Split(" ")[1];
                if (actionVerb == "use")
                {
                    if (Character.Inventory.Exists(inventoryItem => inventoryItem.Name == actionItem))
                    {
                        string uaPhrase = $"use {actionItem}";
                        EvalUniqueActions(currentRoom.UniqueActions, uaPhrase, actionItem, actionVerb);
                    }
                    else
                    {
                        Dialog($"You don't possess {actionItem}.");
                    }
                }
                else if (getVerbs.Contains(actionVerb))
                {
                    string uaPhrase = $"get {actionItem}";
                    EvalUniqueActions(currentRoom.UniqueActions, uaPhrase, actionItem, actionVerb);
                }
                else if (actionVerb == "open")
                {
                    string uaPhrase = $"open {actionItem}";
                    EvalUniqueActions(currentRoom.UniqueActions, uaPhrase, actionItem, actionVerb);
                }
                else if (readVerbs.Contains(actionVerb))
                {
                    string uaPhrase = $"read {actionItem}";
                    EvalUniqueActions(currentRoom.UniqueActions, uaPhrase, actionItem, actionVerb);
                }
                else if (talkVerbs.Contains(actionVerb))
                {
                    string uaPhrase = $"talk {actionItem}";
                    EvalUniqueActions(currentRoom.UniqueActions, uaPhrase, actionItem, actionVerb);
                }
                else
                {
                    Dialog("I don't know what that's supposed to mean. Try a simpler 1-2 word command?");
                }
            }

            else
            {
                switch (userInput.Substring(0, 1))
                {
                    case "n":
                        {
                            Move(currentRoom.Exits, "north");
                            break;
                        }
                    case "s":
                        {
                            Move(currentRoom.Exits, "south");
                            break;
                        }
                    case "e":
                        {
                            Move(currentRoom.Exits, "east");
                            break;
                        }
                    case "w":
                        {
                            Move(currentRoom.Exits, "west");
                            break;
                        }
                    case "i":
                        {
                            Dialog("You rummage through your backpack and find the following:\n");
                            foreach (Item possessedItem in Character.Inventory)
                            {
                                Dialog($"- {possessedItem.Name}", ConsoleColor.Cyan);
                            }
                            Dialog("Also, here are all your appendages you still have!\n");
                            foreach (string bodyPart in Character.bodyParts)
                            {
                                Dialog($"- {bodyPart}", ConsoleColor.Cyan);
                            }
                            break;
                        }
                    case "q":
                        {
                            Run = false;
                            break;
                        }
                    default:
                        {
                            Dialog("I don't know what that's supposed to mean. Try a simpler 1-2 word command?");
                            break;
                        }
                }
            }
        }


        public static void EndGame()
        {
            if (Goal == true)
            {
                Console.WriteLine("Well dang, you won!");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("You leave the dungeon without success. The world will never be rid of COVID-19 now...");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                //Run = true;
                //Play();
            }
        }

        public static void Play()
        {
            StartGame();
            while (Run == true)
            {
                Menu(Rooms.Find(room => room.Label == activeScene));
            }
            EndGame();
        }

        public static void Move(string[] exits, string direction)
        {
            if (exits.Any(exit => exit.StartsWith(direction)))
            {
                Console.Clear();
                string nextRoom = Array.Find(exits, room => room.StartsWith(direction));
                activeScene = "Room" + nextRoom.Split("_")[1];
            }
            else
            {
                Dialog("No way to travel that direction.");
            }
        }

        public static void RunUniqueAction(string roomText, string roomConsequence)
        {
            Dialog(roomText);
            if (roomConsequence != "none")
            {
                Character.SetFlags(roomConsequence);
            }
        }

        public static void EvalUniqueActions(List<UniqueAction> roomUniqueActions, string uaMatchPhrase, string uaActionItem, string uaActionVerb)
        {
            if (roomUniqueActions.Exists(ua => ua.Action == uaMatchPhrase))
            {
                List<UniqueAction> roomUAs = roomUniqueActions.FindAll(ua => ua.Action == uaMatchPhrase);
                foreach (UniqueAction roomUA in roomUAs)
                {
                    if (roomUA.Flag != "none" && Character.Flags.Contains(roomUA.Flag))
                    {
                        RunUniqueAction(roomUA.Text, roomUA.Consequence);
                        break;
                    }
                    else if (roomUA.Flag == "none")
                    {
                        RunUniqueAction(roomUA.Text, roomUA.Consequence);
                        break;
                    }
                }
            }
            else
            {
                Dialog($"There is no {uaActionItem} to {uaActionVerb}.");
            }
        }

    }
}
