using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Configuration;

namespace coding_tracker.Models
{
    internal class GetUserInput
    {
        static string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

        CodingController codingController = new();

        internal void MainMenu()
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

        private void GetAllRecords()
        {
            Console.Clear();

            codingController.Get();
        }

        private void Insert()
        {
            Console.Clear();

            string startInput = TimeInput("\nPlease insert the start time of your coding session. Format: (hh:mm). Type 0 to return to main menu.");
            string endInput = TimeInput("\nPlease insert the end time of your coding session. Format: (hh:mm). Type 0 to return to main menu.");
            string codingDuration = CalculateDuration(startInput, endInput);
            
            CodingTracker code = new CodingTracker();

            code.StartTime = TimeSpan.Parse(startInput);
            code.EndTime = TimeSpan.Parse(endInput);
            code.Duration = TimeSpan.Parse(codingDuration);

            codingController.Post(code);
        }

        public void Delete()
        {
            GetAllRecords();

            int recordId = GetNumberInput("Type the Id of the record you want to delete, or type 0 to go back to the main menu.");

            codingController.DeleteRecord(recordId);
        }

        private void Update()
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

                string startInput = TimeInput("\nPlease insert the start time of your coding session. Format: (hh:mm). Type 0 to return to main menu.");
                string endInput = TimeInput("\nPlease insert the end time of your coding session. Format: (hh:mm). Type 0 to return to main menu.");
                string codingDuration = CalculateDuration(startInput, endInput);

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"UPDATE coding_session SET StartTime = '{startInput}', EndTime = '{endInput}', Duration = '{codingDuration}' WHERE Id = {recordId}";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        static string TimeInput(string question)
        {
            Console.WriteLine(question);

            string timeInput = Console.ReadLine();

            GetUserInput mainMenu = new GetUserInput();

            if (timeInput == "0") mainMenu.MainMenu();

            while (!TimeSpan.TryParseExact(timeInput, "hh\\:mm", CultureInfo.InvariantCulture, out _))
            {
                Console.WriteLine("Invalid time. Format: (hh:mm). Type 0 to return to main menu or try again: \n");
                timeInput = Console.ReadLine();
            }

            return timeInput;
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

            GetUserInput mainMenu = new GetUserInput();

            while (!Int32.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0 || string.IsNullOrEmpty(numberInput))
            {
                Console.WriteLine("\nInvalid Id, try again. (Or type 0 to go back to the main menu)");
                numberInput = Console.ReadLine();
            }

            if (numberInput == "0") mainMenu.MainMenu();

            int finalInput = Convert.ToInt32(numberInput);

            return finalInput;
        }

    }
}