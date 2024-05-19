using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GameLauncher
{
    class Program
    {
        static Dictionary<string, string> games = new Dictionary<string, string>();
        static string saveFilePath = "games.txt";

        static void Main(string[] args)
        {
            LoadGames();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Game Launcher Console");
                Console.WriteLine("1. List Games");
                Console.WriteLine("2. Add Game");
                Console.WriteLine("3. Remove Game");
                Console.WriteLine("4. Launch Game");
                Console.WriteLine("5. Search Game");
                Console.WriteLine("6. Update Game Path");
                Console.WriteLine("7. Save & Exit");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ListGames();
                        break;
                    case "2":
                        AddGame();
                        break;
                    case "3":
                        RemoveGame();
                        break;
                    case "4":
                        LaunchGame();
                        break;
                    case "5":
                        SearchGame();
                        break;
                    case "6":
                        UpdateGamePath();
                        break;
                    case "7":
                        SaveGames();
                        return;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void ListGames()
        {
            Console.Clear();
            Console.WriteLine("List of Games:");

            if (games.Count == 0)
            {
                Console.WriteLine("No games added.");
            }
            else
            {
                foreach (var game in games)
                {
                    Console.WriteLine($"{game.Key} - {game.Value}");
                }
            }
        }

        static void AddGame()
        {
            Console.Clear();
            Console.Write("Enter the name of the game: ");
            string name = Console.ReadLine();

            Console.Write("Enter the path to the game executable: ");
            string path = Console.ReadLine();

            if (File.Exists(path))
            {
                if (!games.ContainsKey(name))
                {
                    games.Add(name, path);
                    Console.WriteLine("Game added successfully.");
                }
                else
                {
                    Console.WriteLine("A game with that name already exists.");
                }
            }
            else
            {
                Console.WriteLine("The path to the game executable is invalid.");
            }
        }

        static void RemoveGame()
        {
            Console.Clear();
            Console.Write("Enter the name of the game to remove: ");
            string name = Console.ReadLine();

            if (games.ContainsKey(name))
            {
                games.Remove(name);
                Console.WriteLine("Game removed successfully.");
            }
            else
            {
                Console.WriteLine("Game not found.");
            }
        }

        static void LaunchGame()
        {
            Console.Clear();
            Console.Write("Enter the name of the game to launch: ");
            string name = Console.ReadLine();

            if (games.ContainsKey(name))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = games[name],
                        UseShellExecute = true
                    });
                    Console.WriteLine("Game launched successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to launch game: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Game not found.");
            }
        }

        static void SearchGame()
        {
            Console.Clear();
            Console.Write("Enter the name of the game to search: ");
            string name = Console.ReadLine();

            if (games.ContainsKey(name))
            {
                Console.WriteLine($"{name} - {games[name]}");
            }
            else
            {
                Console.WriteLine("Game not found.");
            }
        }

        static void UpdateGamePath()
        {
            Console.Clear();
            Console.Write("Enter the name of the game to update: ");
            string name = Console.ReadLine();

            if (games.ContainsKey(name))
            {
                Console.Write("Enter the new path to the game executable: ");
                string newPath = Console.ReadLine();

                if (File.Exists(newPath))
                {
                    games[name] = newPath;
                    Console.WriteLine("Game path updated successfully.");
                }
                else
                {
                    Console.WriteLine("The new path to the game executable is invalid.");
                }
            }
            else
            {
                Console.WriteLine("Game not found.");
            }
        }

        static void SaveGames()
        {
            using (StreamWriter writer = new StreamWriter(saveFilePath))
            {
                foreach (var game in games)
                {
                    writer.WriteLine($"{game.Key}|{game.Value}");
                }
            }
            Console.WriteLine("Games saved successfully.");
        }

        static void LoadGames()
        {
            if (File.Exists(saveFilePath))
            {
                using (StreamReader reader = new StreamReader(saveFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split('|');
                        if (parts.Length == 2)
                        {
                            games[parts[0]] = parts[1];
                        }
                    }
                }
                Console.WriteLine("Games loaded successfully.");
            }
        }
    }
}
