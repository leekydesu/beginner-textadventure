using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CSharpGameExample
{
    public class Game
    {
        static string activeScene = "Room1";
        const ConsoleColor defaultColor = ConsoleColor.White;
        Item itemTaco = new Item("Taco", "Hard shelled taco with seasoned ground beef, shredded cheese, and cool iceberg lettuce.");
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

            string[] roomFiles = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "RoomData"));
            foreach (string roomFile in roomFiles)
            {
                string roomJson = File.ReadAllText(roomFile);
                Room newRoom = JsonConvert.DeserializeObject<Room>(roomJson);
                Rooms.Add(newRoom);
            }

            Menu();
            while (Game.activeScene == "Beginning")
            {
                Dialog("\nYou are at a fork in the road. There is a sign and a mailbox. Paths lead to the east and west.\n");
                Dialog("1) Read Sign\n2) Open mailbox\n3) Go east\n4) Go west", ConsoleColor.Green);
                string selection = Choice();
                switch (selection)
                {
                    case "1":
                        Dialog("Sign says \"Look out for mailboxes.\"");
                        break;
                    case "2":
                        bool handDamage = Character.DamageBody("Right Hand");
                        if (handDamage)
                        {
                            Dialog("You open the mailbox. The mailbox eats your hand! Oh no!");
                            Dialog("Your right hand has been eaten by the mailbox.");
                        }
                        else
                        {
                            Dialog("You prod at the mailbox with your bloody nub. You lament that you'll never open a mailbox again.");
                        }
                        break;
                    case "3":
                        Dialog("You go east. ... Welp...");
                        Game.activeScene = "EastOfBeginning";
                        break;
                    case "4":
                        Dialog("You go west. ... at least it isn't east?");
                        Game.activeScene = "WestOfBeginning";
                        break;
                    default:
                        Dialog("Don't break my game, you cheeky bastard.", ConsoleColor.Red);
                        break;
                }
            }
            Console.WriteLine($"Scene changed to {Game.activeScene}");
            while (Game.activeScene == "WestOfBeginning")
            {
                Dialog("\nYou arrive at the end of the trail going west. A T-Rex watches you expectingly. There is a taco in the street. Only one path leads to the east.\n");
                Dialog("1) Talk to T-Rex\n2) Take taco\n3) Aggressive T-pose \n4) Go east", ConsoleColor.Green);
                string selection = Choice();
                switch (selection)
                {
                    case "1":
                        bool footDamage = Character.DamageBody("Right Foot");
                        if (footDamage)
                        {
                            Dialog("The T-Rex says \"Finally, Doordash is so slow nowadays.\"");
                            Dialog("The T-Rex has eaten your right foot. Whoops!");
                        }
                        else
                        {
                            Dialog("The T-Rex says \"No no, I couldn't possibly eat another bite. Thank you, though.\"");
                        }
                        break;
                    case "2":
                        Dialog("You swipe that taco. Taco get!");

                        break;
                    case "3":
                        Dialog("You go east. ... Welp...");
                        Game.activeScene = "EastOfBeginning";
                        break;
                    case "4":
                        Dialog("You go west. ... at least it isn't east?");
                        Game.activeScene = "WestOfBeginning";
                        break;
                    default:
                        Dialog("Don't break my game, you cheeky bastard.", ConsoleColor.Red);
                        break;
                }
            }
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

        public static void Menu()
        {
            Console.Clear();

            string userInput = Choice();
            string[] getVerbs = { "get", "take", "possess", "steal", "pickup" };
            string[] readVerbs = { "read", "look", "check" };
            if (userInput.Contains(" "))
            {
                string actionVerb = userInput.Split(" ")[0];
                string actionItem = userInput.Split(" ")[1];
                if (actionVerb == "use")
                {
                    if (Character.inventory.Contains(actionItem))
                    {

                    }
                    else
                    {
                        Dialog($"You don't possess {actionItem}.");
                    }
                }
                else if (getVerbs.Contains(actionVerb))
                {
                    if (Character.inventory.Contains(actionItem))
                    {

                    }
                    else
                    {
                        Dialog($"There is no {actionItem} to {actionVerb}.");
                    }
                }
                else if (actionVerb == "open")
                {
                    if (Character.inventory.Contains(actionItem))
                    {

                    }
                    else
                    {
                        Dialog($"There is no {actionItem} to open.");
                    }
                }
                else if (readVerbs.Contains(actionVerb))
                {
                    if (Character.inventory.Contains(actionItem))
                    {

                    }
                    else
                    {
                        Dialog($"There is no {actionItem} read or check.");
                    }
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
                            break;
                        }
                    case "s":
                        {
                            break;
                        }
                    case "e":
                        {
                            break;
                        }
                    case "w":
                        {
                            break;
                        }
                    case "i":
                        {
                            Dialog("You rummage through your backpack and find the following:\n");
                            foreach (string possessedItem in Character.inventory)
                            {
                                Dialog($"- {possessedItem}", ConsoleColor.Cyan);
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
                Menu();
            }
            EndGame();
        }

    }
}
