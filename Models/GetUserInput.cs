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
                Console.WriteLine("Type 5 to track your coding time with a stopwatch");
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
                        UpdateProcess();
                        break;
                    case "5":
                        StopWatch();
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

        private void Update(string option)
        {
            Console.Clear();

            GetAllRecords();

            CodingController codingController = new CodingController();

            int recordId;

            do
            {
                recordId = GetNumberInput("Type the Id of the record you want to update, or type 0 to go back to the main menu.");
            } while (!codingController.RecordExists(recordId));

            string startInput = "";
            string endInput = "";
            string codingDuration = "";
            string date = "";

            // CodingTracker code = new();

            // code.StartTime = TimeSpan.Parse(TimeInput("\nPlease insert the start time of your coding session. Format: (hh:mm). Type 0 to return to main menu."));
            if (option == "d")
                date = DateInput();

            if (option == "s" || option == "b")
                startInput = TimeInput("\nPlease insert the start time of your coding session. Format: (hh:mm). Type 0 to return to main menu.");

            if (option == "e" || option == "b")
                endInput = TimeInput("\nPlease insert the end time of your coding session. Format: (hh:mm). Type 0 to return to main menu.");

            if (option == "b")
                codingDuration = CalculateDuration(startInput, endInput);

            codingController.UpdateRecord(recordId, startInput, endInput, codingDuration, date);
        }

        private void UpdateProcess()
        {
            bool updating = true;

            while (updating)
            {
                Console.WriteLine("\nWhat propertie(s) do you want to update ?\n");
                Console.WriteLine("Type 'd' for date");
                Console.WriteLine($"Type 's' for start time");
                Console.WriteLine($"Type 'e' for end time");
                Console.WriteLine($"Type 'b' for both start and end times");
                Console.WriteLine($"Type '0' to go back to main menu\n");

                string updateInput = Console.ReadLine();

                switch (updateInput)
                {
                    case "d":
                        Update("d");
                        break;

                    case "s":
                        Update("s");
                        break;

                    case "e":
                        Update("e");
                        break;

                    case "b":
                        Update("b");
                        break;

                    case "0":
                        MainMenu();
                        updating = false;
                        break;

                    default:
                        Console.WriteLine("Invalid command, type a valid command from the menu");
                        break;
                }
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

                if (timeInput == "0")  mainMenu.MainMenu();
            }

            return timeInput;
        }

        public string DateInput()
        {
            Console.WriteLine("\nPlease insert date of your coding session you want to update. Format: (dd-MM-yyyy). Type 0 to return to main menu.");
            string dateInput = Console.ReadLine();

            if (dateInput == "0") MainMenu();

            while (!DateTime.TryParseExact(dateInput, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                Console.WriteLine("Invalid time. Format: (dd-MM-yyyy). Type 0 to return to main menu or try again: \n");
                dateInput = Console.ReadLine();

                if (dateInput == "0")  MainMenu();
            }

            return dateInput;
        }
            public static string CalculateDuration(string startTimeStr, string endTimeStr)
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

            public void StopWatch()
            {
                StopWatch watch = new StopWatch();

                watch.Run();
            }

        }
    }