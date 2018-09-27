using System;
using System.Collections.Generic;
using System.Threading;
using castle_grimtol.Interfaces;

namespace castle_grimtol.Models
{
  public class Game : IGame
  {
    public bool GameWon { get; set; } = false;
    public bool GameLost { get; set; } = false;
    public Room StartRoom { get; set; }
    public Room CurrentRoom { get; set; }
    public Player CurrentPlayer { get; set; }

    public void GetUserInput()
    {
      string command = "";
      List<string> options;
      do {
        Console.Write("> ");
        options = new List<string>(Console.ReadLine().ToLower().Split(" ")); // get input as list
        if (options.Count == 0) {
          continue;
        }
        command = options[0]; // first item is the command
        options.RemoveAt(0); // the rest are options
        string optionsString = String.Join(" ", options);

        switch (command) {
          case "go":
            string direction = optionsString;
            Go(direction);
            break;
          case "help":
            Help();
            break;
          case "inventory":
            Inventory();
            break;
          case "take":
            TakeItem(optionsString);
            break;
          case "look":
            Look();
            break;
          case "use":
            UseItem(optionsString);
            break;
          case "quit":
            Quit();
            break;
          default:
            Console.WriteLine("Stop speaking gibberish, that's not a command.");
            break;
        }
      } while (command != "quit" || GameWon);
      if (GameWon) {
        string input = "";
        do {
          Console.Write("Would you like to continue, yes or no? ");
          input = Console.ReadLine().ToLower();
        } while (input != "yes" && input != "no");
        if (input == "yes") {
          Reset();
          StartGame();
          return;
        } else {
          Quit();
        }
      }
    }

    public void Go(string direction)
    {
      List<string> directions = new List<string>(){"north", "east", "south", "west"};
      if (!directions.Contains(direction)) {
        // sorry, there aren't any waffle emojis
        Console.WriteLine("Never eat soggy 🥞 waffles.");
        return;
      }

      if (!CurrentRoom.Exits.ContainsKey(direction)) {
        Console.WriteLine("That's a wall 🤦");
        return;
      }

      if (CurrentRoom.Locked == direction) {
        Console.WriteLine("It's locked, 🤔 maybe there's a key nearby.");
        return;
      }

      CurrentRoom = CurrentRoom.Exits[direction];
      if (CurrentRoom == null) {
        GameWon = true;
        Console.WriteLine("Congrats you won the game 🎉! Now go write some code.");
        return;
      }
      Look();
    }

    public void Help()
    {
      Console.Write('\n');
      Console.WriteLine("Since you've apparently never played a game before, here ya go.");
      Console.WriteLine("Your commands:");
      Console.WriteLine("  go <cardinal direction>");
      Console.WriteLine("  take <item name>");
      Console.WriteLine("  use <item name>");
      Console.WriteLine("  inventory");
      Console.WriteLine("  look");
      Console.WriteLine("  quit");
    }

    public void Inventory()
    {
      if (CurrentPlayer.Inventory.Count == 0) {
        Console.WriteLine("Your pockets are empty.");
        return;
      }
      CurrentPlayer.Inventory.ForEach(item => Console.WriteLine($"{item.Name} - {item.Description}"));
    }

    public void Look()
    {
      Console.Write('\n');
      ConsoleColor background = Console.BackgroundColor;
      ConsoleColor foreground = Console.ForegroundColor;

      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      Console.WriteLine(CurrentRoom.Name);

      Console.BackgroundColor = background;
      Console.ForegroundColor = foreground;

      Console.WriteLine(CurrentRoom.Description);
    }

    public void Quit()
    {
      Console.WriteLine("Quitter.");
    }

    public void Reset()
    {
      GameWon = false;
    }

    public void Setup()
    {
      Console.Clear();

      Console.Write("What's your name adventurer? ");
      CurrentPlayer = new Player(Console.ReadLine());

      StartRoom = new Room(
        "Cell",
        $"This is the cell you woke up in {CurrentPlayer.PlayerName}, you don't know how you got here, but you know you need to get out. You notice a sharp knife-like shaped object in the corner. You also see what looks like a door on the East wall."
      );

      Room keyRoom = new Room(
        "Barred Room",
        "The room is dark, windows barred, but you notice a sparkling in the corner. You can't quite see it. You also notice two doors, one East, one South.."
      );
      keyRoom.Locked = "east";

      Room pitRoom = new Room(
        "T H E P I T",
        "It's even darker in this room. You hastily walk into the room looking for a source of light...\nYou fell into a pit."
      );
      pitRoom.KillsPlayer = true;

      Room warpRoom = new Room(
        "Canvas Area",
        "You enter the room, it seems very bright, very empty.. Suddenly the door closes behind you. There are two other doors, North and East. You can't see what's on the other side of the North door, it's completely dark in there. What will you do?"
      );
      warpRoom.Locked = "west";

      Room treasureRoom = new Room(
        "Treasure?",
        "You walk into the room. Light fades in. There's a box of treasure. It's so beatiful. Gold everywhere. To the East side you also notice a door with what seems like natural light escaping from it. Could it be the exit?"
      );

      StartRoom.AddRoom("east", keyRoom);
      keyRoom.AddRoom("south", pitRoom);
      keyRoom.AddRoom("east", warpRoom);
      warpRoom.AddRoom("north", treasureRoom);
      warpRoom.AddRoom("east", StartRoom, false);

      // null represents the room in which you win the game
      treasureRoom.AddRoom("west", null, false);
      CurrentRoom = StartRoom;

      Look();
    }

    public void StartGame()
    {
      Setup();
      GetUserInput();
    }

    public void TakeItem(string itemName)
    {
      
    }

    public void UseItem(string itemName)
    {
      int itemIndex = CurrentPlayer.IndexOfItemByName(itemName);
      if (itemIndex == -1) {
        Console.WriteLine($"Do you not know how to check your pockets? You don't even have a {itemName}.");
        return;
      }

      switch (itemName) {
        case "key":
          if (!CurrentRoom.Unlock()) {
            Console.WriteLine("What do you call someone who tries to unlock a door that doesn't have a lock?");
            Thread.Sleep(500);
            Console.WriteLine("Stupid, that's what you call them.");
          } else {
            Console.WriteLine("You unlocked the door!");
          }
          break;
        default:
          Console.WriteLine($"You used your {itemName}, it didn't do anything.");
          break;
      }
    }
  }
}