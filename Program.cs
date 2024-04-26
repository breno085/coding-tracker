using System.Globalization;
using Microsoft.Data.Sqlite;
using coding_tracker.Models;
using System.Runtime.CompilerServices;
using Spectre.Console;
using Microsoft.VisualBasic;

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
                    Date TEXT,
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
            Console.WriteLine("Type 4 to Update Record");
            Console.WriteLine("--------------------------------\n");

            string command = Console.ReadLine();

            switch (command)
            {
                case "0":
                    Console.WriteLine("\nGoodbye\n\n");
                    closeApp = true;
                    Environment.Exit(0);
                    break;
                case "1":
                    GetAllRecords();
                    break;
                case "2":
                    Insert();
                    break;
                case "3":
                    Delete();
                    break;
                case "4":
                    Update();
                    break;
                default:
                    Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
                    break;
            }
            Console.WriteLine("\n\nPress any key to continue");
            Console.ReadLine();
        }
    }
    static void GetAllRecords()
    {
        Console.Clear();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText =
                $"SELECT * FROM coding_session";

            List<CodingTracker> tableData = new();

            SqliteDataReader reader = tableCmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    tableData.Add(
                    new CodingTracker
                    {
                        Id = reader.GetInt32(0),
                        Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yyyy", new CultureInfo("en-US")),
                        StartTime = TimeSpan.ParseExact(reader.GetString(2), "hh\\:mm", new CultureInfo("en-US")),
                        EndTime = TimeSpan.ParseExact(reader.GetString(3), "hh\\:mm", new CultureInfo("en-US")),
                        Duration = TimeSpan.ParseExact(reader.GetString(4), "hh\\:mm", new CultureInfo("en-US"))
                    });
                }
            }
            else
            {
                Console.WriteLine("No rows found");
            }

            connection.Close();

            SpectreTable(tableData);

        }
    }

    static void Insert()
    {
        Console.Clear();

        string startInput = StartTimeInput();
        string endInput = EndTimeInput();
        string codingDuration = CalculateDuration(startInput, endInput);
        CodingTracker date = new CodingTracker();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText =
                $"INSERT INTO coding_session(Date, StartTime, EndTime, Duration) VALUES ('{date.Date.ToString("dd-MM-yyyy")}','{startInput}', '{endInput}', '{codingDuration}')";

            tableCmd.ExecuteNonQuery();

            connection.Close();
        }
    }

    static void Delete()
    {
        GetAllRecords();

        int recordId = GetNumberInput("Type the Id of the record you want to delete, or type 0 to go back to the main menu.");

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText = $"DELETE FROM coding_session WHERE Id = {recordId}";

            int rowCount = tableCmd.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.WriteLine($"\nRecord with Id = {recordId} doesn't exist\n");
                Delete();
            }

            connection.Close();
        }

        Console.WriteLine($"\nRecord with Id = {recordId} was deleted.\n");

    }
    static void Update()
    {
        Console.Clear();
        GetAllRecords();

        int recordId = GetNumberInput("Type the Id of the record you want to update, or type 0 to go back to the main menu.");

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM coding_session WHERE Id = {recordId})";
            int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (checkQuery == 0)
            {
                Console.WriteLine($"\nRecord with Id = {recordId} doesn't exist.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadLine();

                connection.Close();
                Update();
            }

            string startInput = StartTimeInput();
            string endInput = EndTimeInput();
            string codingDuration = CalculateDuration(startInput, endInput);

            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText = $"UPDATE coding_session SET StartTime = '{startInput}', EndTime = '{endInput}', Duration = '{codingDuration}' WHERE Id = {recordId}";

            tableCmd.ExecuteNonQuery();

            connection.Close();
        }
    }

    static string StartTimeInput()
    {
        Console.WriteLine("\nPlease insert the start time of your coding session. Format: (hh:mm). Type 0 to return to main menu.");
        string startTimeInput = Console.ReadLine();

        if (startTimeInput == "0") GetUserInput();

        while (!TimeSpan.TryParseExact(startTimeInput, "hh\\:mm", CultureInfo.InvariantCulture, out _))
        {
            Console.WriteLine("Invalid time. Format: (hh:mm). Type 0 to return to main menu or try again: \n");
            startTimeInput = Console.ReadLine();
        }

        return startTimeInput;
    }

    static string EndTimeInput()
    {
        Console.WriteLine("\nPlease insert the end time of your coding session. Format: (hh:mm). Type 0 to return to main menu.");
        string endTimeInput = Console.ReadLine();

        if (endTimeInput == "0") GetUserInput();

        while (!TimeSpan.TryParseExact(endTimeInput, "hh\\:mm", CultureInfo.InvariantCulture, out _))
        {
            Console.WriteLine("Invalid time. Format: (hh:mm). Type 0 to return to main menu or try again: \n");
            endTimeInput = Console.ReadLine();
        }

        return endTimeInput;
    }
    
    static string CalculateDuration(string startTimeStr, string endTimeStr)
    {
        TimeSpan startTime = TimeSpan.Parse(startTimeStr);
        TimeSpan endTime = TimeSpan.Parse(endTimeStr);

        TimeSpan timeDifference = endTime - startTime;

        return timeDifference.ToString(@"hh\:mm");
    }

    static int GetNumberInput(string input)
    {
        Console.WriteLine(input);

        string numberInput = Console.ReadLine();

        if (numberInput == "0") 
            GetUserInput();

        while (!Int32.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0)
        {
            Console.WriteLine("\nInvalid number, try again.");
            numberInput = Console.ReadLine();
        }

        int finalInput = Convert.ToInt32(numberInput);

        return finalInput;
    }

    static void SpectreTable(List<CodingTracker> tableData)
    {
        var table = new Table();

        table.AddColumn("Id");
        table.AddColumn("Date");
        table.AddColumn("Start Time");
        table.AddColumn("End Time");
        table.AddColumn("Duration");

        foreach (var dw in tableData)
        {
            table.AddRow(
                dw.Id.ToString(),
                dw.Date.ToString("dd-MM-yyyy"),
                dw.StartTime.ToString("hh\\:mm"),
                dw.EndTime.ToString("hh\\:mm"),
                dw.Duration.ToString("hh\\:mm")
            );
        }
        // Render the table to the console
        AnsiConsole.Write(table);
    }
}