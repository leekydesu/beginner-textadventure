using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices.ComTypes;

namespace CSharpGameExample
{
    public class Game
    {
        static string activeScene = "Room1";
        static string currentRoomLabel = "Blank";
        const ConsoleColor defaultColor = ConsoleColor.White;
        static List<Room> Rooms = new List<Room>();
        static List<Item> Items = new List<Item>();
        static Room AllRoom = new Room();

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

            string[] roomFiles = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "RoomData"));
            foreach (string roomFile in roomFiles)
            {
                string roomJson = File.ReadAllText(roomFile);
                Room newRoom = JsonConvert.DeserializeObject<Room>(roomJson);
                Rooms.Add(newRoom);
            }

            string[] itemFiles = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "ItemData"));
            foreach (string itemFile in itemFiles)
            {
                string itemJson = File.ReadAllText(itemFile);
                Item newItem = JsonConvert.DeserializeObject<Item>(itemJson);
                Items.Add(newItem);
            }

            Item wallet = Items.Find(item => item.Name == "Wallet");
            Character.Inventory.Add(wallet);

            Console.Clear();
            Game.AllRoom = Rooms.Find(room => room.Label == "RoomAll");
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
            Console.WriteLine("You can use the '?' command to get help if needed once you're in.");
            Console.WriteLine("\n~Press any key to begin~");
            Console.ReadKey();
        }
        static void Dialog(string message, ConsoleColor color = defaultColor)
        {
            Console.ForegroundColor = color;
            Console.Write($"\n{message}");
            Console.ResetColor();
        }
        static string Choice()
        {
            string userInput = "";
            while (String.IsNullOrEmpty(userInput))
            {
                Console.Write("\n>> ");
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
            if (activeScene != currentRoomLabel)
            {
                Dialog(currentRoom.Description);
            }
            else
            {
                currentRoom.UpdateDescription((Rooms.Find(room => room.Label == activeScene)).Description);
            }

            string userInput = Choice();
            string[] getVerbs = { "get", "take", "possess", "steal", "pickup" };
            string[] readVerbs = { "read", "look", "check" };
            string[] talkVerbs = { "talk", "speak", "chat" };

            List<UniqueAction> roomUAs = currentRoom.UniqueActions;
            AllRoom.UniqueActions.ForEach(ua => roomUAs.Add(ua));

            if (userInput.Contains(" ") && userInput.Split().Length <= 2)
            {
                string actionVerb = userInput.Split(" ")[0].ToLower();
                string actionItem = userInput.Split(" ")[1].ToLower();
                if (actionVerb == "use")
                {
                    if (Character.Inventory.Exists(inventoryItem => inventoryItem.Name.ToLower() == actionItem))
                    {
                        string uaPhrase = $"use {actionItem}";
                        EvalUniqueActions(roomUAs, uaPhrase, actionItem, actionVerb);
                    }
                    else
                    {
                        Dialog($"You don't possess {actionItem}.");
                    }
                }
                else if (getVerbs.Contains(actionVerb))
                {
                    string uaPhrase = $"get {actionItem.ToLower()}";
                    if (currentRoom.Items.Exists(item => item.Name.ToLower() == actionItem))
                    {
                        Item itemToAdd = Items.Find(item => item.Name.ToLower() == actionItem);
                        Character.Inventory.Add(itemToAdd);
                        Dialog($"You {actionVerb} the {actionItem}");
                        if (itemToAdd.NewRoomDescription != "")
                        {
                            (Rooms.Find(room => room.Label == activeScene)).UpdateDescription(itemToAdd.NewRoomDescription);
                        }
                    }
                    else if (currentRoom.UniqueActions.Exists(ua => ua.Action == uaPhrase))
                    {
                        EvalUniqueActions(roomUAs, uaPhrase, actionItem, actionVerb);
                    }
                    else
                    {
                        Dialog($"There is no {actionItem} that you can {actionVerb}.");
                    }
                }
                else if (actionVerb == "open")
                {
                    string uaPhrase = $"open {actionItem}";
                    EvalUniqueActions(roomUAs, uaPhrase, actionItem, actionVerb);
                }
                else if (readVerbs.Contains(actionVerb))
                {
                    string uaPhrase = $"read {actionItem}";
                    EvalUniqueActions(roomUAs, uaPhrase, actionItem, actionVerb);
                }
                else if (talkVerbs.Contains(actionVerb))
                {
                    string uaPhrase = $"talk {actionItem}";
                    EvalUniqueActions(roomUAs, uaPhrase, actionItem, actionVerb);
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
                            Move(currentRoom.Exits, "north", currentRoom.UniqueActions);
                            break;
                        }
                    case "s":
                        {
                            Move(currentRoom.Exits, "south", currentRoom.UniqueActions);
                            break;
                        }
                    case "e":
                        {
                            Move(currentRoom.Exits, "east", currentRoom.UniqueActions);
                            break;
                        }
                    case "w":
                        {
                            Move(currentRoom.Exits, "west", currentRoom.UniqueActions);
                            break;
                        }
                    case "d":
                        {
                            Dialog(currentRoom.Description);
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
                            foreach (string bodyPart in Character.BodyParts)
                            {
                                Dialog($"- {bodyPart}", ConsoleColor.Cyan);
                            }
                            break;
                        }
                    case "?":
                        {
                            Dialog("You can use the following commands:\n");
                            Dialog("- n/s/e/w: Move in the corresponding direction\n");
                            Dialog("- i: Check inventory.\n");
                            Dialog("- d: Display room description.\n");
                            Dialog("- q: Quit game\n");
                            Dialog("- Acceptable verbs include 'use', 'open', 'read', 'talk', 'get'; there are others too!\n");
                            break;
                        }
                    case "q":
                        {
                            if (Character.Flags.Contains("TreasureGet"))
                            {
                                Goal = true;
                            }
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
            currentRoomLabel = currentRoom.Label;
            if (Character.Flags.Contains("PlayerDead"))
            {
                Run = false;
            }
        }


        public static void EndGame()
        {
            if (Goal == true)
            {
                Console.Clear();
                Console.WriteLine($"Against all odds, the great hero {Character.playerName} has emerged from the dungeon victorious!");
                Console.WriteLine("With the treasure in hand, a mighty 'doot-doot' is sounded and echoes throughout the lands...");
                Console.WriteLine("COVID-19 has been eradicated! Humanity has been restored!");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            else
            {
                Console.Clear();
                Console.WriteLine($"{Character.playerName} leaves the dungeon without success. The world will never be rid of COVID-19 now...");
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

        public static void Move(string[] exits, string direction, List<UniqueAction> roomActions)
        {
            if (exits.Any(exit => exit.StartsWith(direction)))
            {
                string nextRoom = Array.Find(exits, room => room.StartsWith(direction));
                bool proceed = true;
                if (nextRoom.Split("_").Length == 3 && !Character.Flags.Contains(nextRoom.Split("_")[2]))
                {
                    proceed = false;
                    EvalUniqueActions(roomActions, direction);
                }

                if (proceed == true)
                {
                    Console.Clear();
                    activeScene = "Room" + nextRoom.Split("_")[1];
                }
            }
            else
            {
                Dialog("No way to travel that direction.");
            }
        }

        public static void RunUniqueAction(string roomText, string roomConsequence)
        {
            Dialog(roomText);
            string[] roomConsequences = roomConsequence.Split(";");
            foreach (string consequence in roomConsequences)
            {
                if (consequence.StartsWith("BodyDamage"))
                {
                    string targetBodyPart = "";
                    if (consequence == "BodyDamage")
                    {
                        Random rand = new Random();
                        int index = rand.Next(Character.BodyParts.Count());
                        targetBodyPart = Character.BodyParts[index];
                        Character.DamageBody(targetBodyPart);
                        Dialog($"You lose your {targetBodyPart}.");
                    }
                    else
                    {
                        switch (consequence.Substring(10, 2))
                        {
                            case "RF":
                                {
                                    targetBodyPart = "Right Foot";
                                    break;
                                }
                            case "LF":
                                {
                                    targetBodyPart = "Left Foot";
                                    break;
                                }
                            case "RH":
                                {
                                    targetBodyPart = "Right Hand";
                                    break;
                                }
                            case "LH":
                                {
                                    targetBodyPart = "Left Hand";
                                    break;
                                }
                        }
                        Character.DamageBody(targetBodyPart);
                        Dialog($"You lose your {targetBodyPart}.");
                    }

                    if (Character.BodyParts.Count() == 0)
                    {
                        Dialog($"You have run out of appendages, {Character.playerName}. You are now just a stumpy stump person with stumpy arms and legs stuck in this dungeon forever.", ConsoleColor.Red);
                        Console.ReadKey();
                        Run = false;
                    }
                }
                else if (consequence.StartsWith("Lose_"))
                {
                    string itemName = consequence.Split("_")[1];
                    Item itemToChange = Character.Inventory.Find(item => item.Name == itemName);
                    Character.Inventory.Remove(itemToChange);
                    Dialog($"You lose the {itemName}.");
                }
                else if (consequence.StartsWith("Gain_"))
                {
                    string itemName = consequence.Split("_")[1];
                    Item itemToChange = Items.Find(item => item.Name == itemName);
                    Character.Inventory.Add(itemToChange);
                    Dialog($"You gain the {itemName}.");
                    if (itemToChange.NewRoomDescription != "")
                    {
                        (Rooms.Find(room => room.Label == activeScene)).UpdateDescription(itemToChange.NewRoomDescription);
                    }
                }
                else if (consequence != "none")
                {
                    Character.SetFlags(consequence);
                }
            }
        }

        public static void EvalUniqueActions(List<UniqueAction> roomUniqueActions, string uaMatchPhrase, string uaActionItem = "thing", string uaActionVerb = "do")
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
                if (uaActionVerb == "use")
                {
                    Dialog($"You hear a voice somewhere says, \"{Character.playerName}! This is not the time to {uaActionVerb} the {uaActionItem}!");
                }
                else
                {
                    Dialog($"There is no {uaActionItem} to {uaActionVerb}.");
                }
            }
        }

    }
}
