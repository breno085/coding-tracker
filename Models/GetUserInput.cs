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
                Console.WriteLine("Type 6 to filter your records per period (days, weeks, months, years)");
                Console.WriteLine("Type 7 to create study goals");

                Console.WriteLine("--------------------------------\n");

                string command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye\n");
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
                    case "6":
                        FilterRecords();
                        break;
                    case "7":
                        StudyGoalMenu();
                        break;
                    default:
                        Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
                        break;
                }
                Console.WriteLine("\nPress any key to continue");
                Console.ReadLine();
            }
        }

        public void StudyGoalMenu()
        {
            bool exit = false;
            object[] studyGoalUserInput = null;
            string showGoalProgress = "";

            while (!exit)
            {
                Console.WriteLine("1 - Insert new goal");
                Console.WriteLine("2 - Insert daily hours");
                Console.WriteLine("3 - Show goal progress");
                Console.WriteLine("4 - Go back to the main menu");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        //Need to store studyGoalUserInput in a table, currently it only stores in memory
                        //Also everytime the user insert new goal, it should overrite the existing one (warn the user)
                        studyGoalUserInput = StudyGoalUserInput();
                        break;
                    case "2":
                        if (studyGoalUserInput == null)
                        {
                            Console.WriteLine("You need to insert new goal first before daily hours");
                            Console.WriteLine("Press any key to continue.\n");
                            Console.ReadLine();
                            studyGoalUserInput = StudyGoalUserInput();
                        }

                        showGoalProgress = InsertDailyHours(studyGoalUserInput);
                        break;
                    case "3":
                        if (showGoalProgress != "" && studyGoalUserInput != null)
                            Console.WriteLine(showGoalProgress);
                        else if (studyGoalUserInput == null)
                            Console.WriteLine("You need to insert a new goal first");
                        else if (showGoalProgress == "")
                            Console.WriteLine("You need to insert your daily hours first");
                        else
                            Console.WriteLine("\nGoal and daily hours is empty. Please create a new coding goal:\n");

                        StudyGoalMenu();
                        break;
                    case "4":
                        exit = true;
                        MainMenu();
                        break;
                    default:
                        Console.WriteLine("Type a valid option.");
                        break;
                }
            }
        }

        public string InsertDailyHours(object[] studyGoalUserInput)
        {
            Insert();

            double totalHours = (double)studyGoalUserInput[0];
            double hoursPerDay = (double)studyGoalUserInput[1];
            string startDate = Convert.ToString(studyGoalUserInput[2]);
            int daysPerWeek = (int)studyGoalUserInput[3];

            return CodingController.StudyGoalsData(totalHours, hoursPerDay, startDate, daysPerWeek);
        }
        public object[] StudyGoalUserInput()
        {
            string answer;
            bool successConv = false;
            double totalHours, hoursPerDay;
            int daysPerWeek;

            object[] studyGoalUserInput;

            do
            {
                Console.WriteLine("Insert the total hours:");
                answer = Console.ReadLine();
                successConv = double.TryParse(answer, out totalHours) && totalHours > 0;

                if (!successConv)
                    Console.WriteLine("Type a valid answer.");
            } while (!successConv);

            do
            {
                Console.WriteLine("Insert the amount of hours you will code per day (12 hours limit):");
                answer = Console.ReadLine();
                successConv = double.TryParse(answer, out hoursPerDay) && hoursPerDay <= 12 && hoursPerDay > 0;

                if (!successConv)
                    Console.WriteLine("Type a valid answer within the limit of 12 hours.");
            } while (!successConv);

            do
            {
                Console.WriteLine("how many days a week you would like to study?");
                answer = Console.ReadLine();
                successConv = int.TryParse(answer, out daysPerWeek) && daysPerWeek > 0 && daysPerWeek <= 7;

                if (!successConv)
                    Console.WriteLine("Type a valid answer.");
            } while (!successConv);

            int daysToReachGoal = (int)Math.Ceiling(totalHours / hoursPerDay);

            int totalDaysToReachGoal = (int)Math.Ceiling((double)(daysToReachGoal * 7 / daysPerWeek));

            DateTime startDate = DateTime.Now;

            Console.WriteLine($"Start Date {startDate:dd-MM-yyyy}");
            Console.WriteLine($"Your estimate closing date is {DateTime.Now.AddDays(totalDaysToReachGoal):dd-MM-yyyy}, studying {hoursPerDay:F2} hours per day and {daysPerWeek} days per week.\n");

            Console.WriteLine("Press any key to continue.\n");
            Console.ReadLine();

            studyGoalUserInput = new object[] { totalHours, hoursPerDay, startDate.ToString("yyyy-MM-dd"), daysPerWeek };

            return studyGoalUserInput;
        }

        public void FilterRecords()
        {
            bool filtering = true;

            do
            {
                Console.WriteLine("Type to filter:");
                Console.WriteLine("1 - Days");
                Console.WriteLine("2 - Weeks");
                Console.WriteLine("3 - Months");
                Console.WriteLine("4 - Year");
                Console.WriteLine("or type 0 to go back to the main menu");

                string op = Console.ReadLine();

                switch (op)
                {
                    case "1":
                        string startingDay = DateInput("Please insert the starting day. Format: (dd-MM-yyyy). Type 0 to return to main menu.");
                        string endingDay = DateInput("Please insert the ending day. Format: (dd-MM-yyyy). Type 0 to return to main menu.");
                        CodingController.FilterCodingRecords(startingDay, endingDay, "days");
                        break;
                    case "2":
                        string startingWeekDay = DateInput("Please insert the starting week day. Format: (dd-MM-yyyy). Type 0 to return to main menu.");
                        string endingWeekDay = DateInput("Please insert the ending week day. Format: (dd-MM-yyyy). Type 0 to return to main menu.");
                        CodingController.FilterCodingRecords(startingWeekDay, endingWeekDay, "weeks");
                        break;
                    case "3":
                        string startingMonth = MonthInput("Please insert the starting month and year. Format: (MM-yyyy). Type 0 to return to main menu.");
                        string endingMonth = MonthInput("Please insert the ending month and year. Format: (MM-yyyy). Type 0 to return to main menu.");
                        CodingController.FilterCodingRecords(startingMonth, endingMonth, "months");
                        break;
                    case "4":
                        string startingYear = "";

                        do
                        {
                            Console.WriteLine("Insert the year.");
                            startingYear = Console.ReadLine();
                        } while (!int.TryParse(startingYear, out _));

                        CodingController.FilterCodingRecords(startingDate: startingYear);
                        break;
                    case "0":
                        filtering = false;
                        break;
                    default:
                        Console.WriteLine("Invalid command. Type a valid command from the menu.");
                        break;
                }

            } while (filtering);
        }

        public string MonthInput(string date)
        {
            Console.WriteLine(date);
            string monthInput = Console.ReadLine();

            if (monthInput == "0") MainMenu();

            bool validDate = false;
            string[] monthValidation = monthInput.Split('-');

            while (validDate == false)
            {
                if (monthValidation.Length == 2)
                {
                    if (int.TryParse(monthValidation[0], out _) && int.TryParse(monthValidation[1], out _))
                    {
                        while (monthValidation[0].Length != 2 && monthValidation[1].Length != 4)
                        {
                            monthValidation = MonthValidationFormat();
                        }
                        validDate = true;
                    }
                    else
                    {
                        validDate = false;

                        monthValidation = MonthValidationFormat();
                    }
                }
                else
                {
                    validDate = false;

                    monthValidation = MonthValidationFormat();
                }
            }

            return String.Join("-", monthValidation);
        }

        public string[] MonthValidationFormat()
        {
            Console.WriteLine("Type a valid date. Format: (MM-yyyy). Type 0 to return to main menu.");
            string monthInput = Console.ReadLine();

            if (monthInput == "0") MainMenu();

            return monthInput.Split('-');
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

                if (timeInput == "0") mainMenu.MainMenu();
            }

            return timeInput;
        }

        public string DateInput(string dateinput = "\nPlease insert date of your coding session you want to update. Format: (dd-MM-yyyy). Type 0 to return to main menu.")
        {
            Console.WriteLine(dateinput);
            string dateInput = Console.ReadLine();

            if (dateInput == "0") MainMenu();

            while (!DateTime.TryParseExact(dateInput, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                Console.WriteLine("Invalid time. Format: (dd-MM-yyyy). Type 0 to return to main menu or try again: \n");
                dateInput = Console.ReadLine();

                if (dateInput == "0") MainMenu();
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