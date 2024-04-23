using System.Globalization;
using Microsoft.Data.Sqlite;

namespace CodingTracker;

class Program
{
    static string connectionString = @"Data Source=coding-Tracker.db";
    
    static void Main(string[] args)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS coding_session (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StartTime TEXT,
                    EndTime TEXT,
                    Duration TEXT
                )";

            tableCmd.ExecuteNonQuery();

            connection.Close();
        }

        GetUserInput();
    }
    static void GetUserInput()
    {
        Console.Clear();
        bool closeApp = false;

        while (!closeApp)
        {
            Console.WriteLine("\n\nMAIN MENU");
            Console.WriteLine("\nWhat would you like to do?");
            Console.WriteLine("\nType 0 to Close Application");
            Console.WriteLine("Type 1 to View All Records");
            Console.WriteLine("Type 2 to Insert Record");
            Console.WriteLine("Type 3 to Delete Record");
            Console.WriteLine("Type to Update Record");
            Console.WriteLine("--------------------------------\n");

            string command = Console.ReadLine();

            switch (command)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye\n\n");
                        closeApp = true;
                        Environment.Exit(0);
                        break;
                    // case "1":
                    //     GetAllRecords();
                    //     break;
                    case "2":
                        Insert();
                        break;
                    // case "3":
                    //     Delete();
                    //     break;
                    // case "4":
                    //     Update();
                    //     break;
                    default:
                        Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
                        break;
                }
                Console.WriteLine("\n\n\nPress any key to continue");
                Console.ReadLine();
        }
    }

    static void Insert()
    {
        Console.Clear();

        string startInput = StartTimeInput();
        string endInput = EndTimeInput();
        string codingDuration = CalculateDuration(startInput, endInput);

        using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = 
                    $"INSERT INTO coding_session(StartTime, EndTime, Duration) VALUES ('{startInput}', '{endInput}', '{codingDuration}')";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }


    }

    static string StartTimeInput()
    {
        Console.WriteLine("\nPlease insert the start time of your coding session. Format: (hh:mm). Type 0 to return to main menu.");
        string startTimeInput = Console.ReadLine();

        if (startTimeInput == "0") GetUserInput();

        return startTimeInput;
    }
    
    static string EndTimeInput()
    {
        Console.WriteLine("\nPlease insert the end time of your coding session. Format: (hh:mm). Type 0 to return to main menu.");
        string endTimeInput = Console.ReadLine();

        if (endTimeInput == "0") GetUserInput();

        return endTimeInput;
    }

    static string CalculateDuration(string startTimeStr, string endTimeStr)
    {
        DateTime startTime = DateTime.Parse(startTimeStr);
        DateTime endTime = DateTime.Parse(endTimeStr);

        TimeSpan timeDifference = endTime - startTime;

        return timeDifference.ToString(@"hh\:mm");
    }
}