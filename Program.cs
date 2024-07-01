using coding_tracker.Repositories;
using coding_tracker.Services;
using coding_tracker.Controllers;
using System.Configuration;

namespace coding_tracker
{
    class Program
    {
        static string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
        
        static void Main(string[] args)
        {
            DatabaseManager databaseManager = new();

            databaseManager.CreateTable(connectionString);


            GenerateRecords.PopulateTableWithRandomData();


            GetUserInput getUserInput = new();

            getUserInput.MainMenu();
        }
    }
}