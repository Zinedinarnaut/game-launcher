using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace GameLauncher
{
    class Program
    {
        static Dictionary<string, string> games = new Dictionary<string, string>();
        static string connectionString = "Data Source=games.db;Version=3;";

        static void Main(string[] args)
        {
            InitializeDatabase();
            LoadGames();
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("====================================");
                Console.WriteLine("        Game Launcher Console       ");
                Console.WriteLine("====================================");
                Console.ResetColor();
                Console.WriteLine();
                PrintMenu();

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
                        PrintError("Invalid option, please try again.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void PrintMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1. List Games");
            Console.WriteLine("2. Add Game");
            Console.WriteLine("3. Remove Game");
            Console.WriteLine("4. Launch Game");
            Console.WriteLine("5. Search Game");
            Console.WriteLine("6. Update Game Path");
            Console.WriteLine("7. Save & Exit");
            Console.ResetColor();
        }

        static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void InitializeDatabase()
        {
            if (!File.Exists("games.db"))
            {
                SQLiteConnection.CreateFile("games.db");
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = "CREATE TABLE IF NOT EXISTS games (Name TEXT PRIMARY KEY, Path TEXT)";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        static void ListGames()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("List of Games:");
            Console.ResetColor();

            if (games.Count == 0)
            {
                PrintError("No games added.");
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
                    PrintSuccess("Game added successfully.");
                }
                else
                {
                    PrintError("A game with that name already exists.");
                }
            }
            else
            {
                PrintError("The path to the game executable is invalid.");
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
                PrintSuccess("Game removed successfully.");
            }
            else
            {
                PrintError("Game not found.");
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
                    PrintSuccess("Game launched successfully.");
                }
                catch (Exception ex)
                {
                    PrintError($"Failed to launch game: {ex.Message}");
                }
            }
            else
            {
                PrintError("Game not found.");
            }
        }

        static void SearchGame()
        {
            Console.Clear();
            Console.Write("Enter the name of the game to search: ");
            string name = Console.ReadLine();

            if (games.ContainsKey(name))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{name} - {games[name]}");
                Console.ResetColor();
            }
            else
            {
                PrintError("Game not found.");
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
                    PrintSuccess("Game path updated successfully.");
                }
                else
                {
                    PrintError("The new path to the game executable is invalid.");
                }
            }
            else
            {
                PrintError("Game not found.");
            }
        }

        static void SaveGames()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    // Clear existing data
                    using (var clearCommand = new SQLiteCommand("DELETE FROM games", connection, transaction))
                    {
                        clearCommand.ExecuteNonQuery();
                    }

                    // Insert current games
                    foreach (var game in games)
                    {
                        using (var insertCommand = new SQLiteCommand("INSERT INTO games (Name, Path) VALUES (@Name, @Path)", connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@Name", game.Key);
                            insertCommand.Parameters.AddWithValue("@Path", game.Value);
                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
            PrintSuccess("Games saved successfully.");
        }

        static void LoadGames()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT Name, Path FROM games";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["Name"].ToString();
                            string path = reader["Path"].ToString();
                            games[name] = path;
                        }
                    }
                }
            }
            PrintSuccess("Games loaded successfully.");
        }
    }
}