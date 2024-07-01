using Microsoft.Data.Sqlite;
using System.Configuration;
using coding_tracker.Models;
using coding_tracker.Repositories;

namespace coding_tracker.Services
{
    public static class GenerateRecords
    {
        static string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

        public static void GenerateCodingRecords()
        {
            Random random = new Random();
            
            var records = new List<CodingTracker>();

            while (records.Count < 100)
            {   
                DateTime randomDay = RandomDays(random);
                DateTime startTime = RandomTimeBetween06And00(random,randomDay);
                DateTime endTime = RandomTimeBetween06And00(random, randomDay);

                while (endTime.TimeOfDay <= startTime.TimeOfDay)
                {
                    endTime = RandomTimeBetween06And00(random, randomDay);
                }

                TimeSpan duration = endTime - startTime;

                records.Add(new CodingTracker
                {
                    Date = randomDay,
                    StartTime = startTime.TimeOfDay,
                    EndTime = endTime.TimeOfDay,
                    Duration = duration
                });
            }

            CodingRepository codingSession = new CodingRepository();

            foreach (var coding in records)
            {
                codingSession.Post(coding);
            }
        }

        public static void PopulateTableWithRandomData()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (var checkCmd = connection.CreateCommand())
                {
                    checkCmd.CommandText = $"SELECT COUNT(*) FROM coding_session";

                    long count = (long)checkCmd.ExecuteScalar();

                    if (count <= 10)
                    {
                        GenerateCodingRecords();
                    }
                }
            }
        }

        public static DateTime RandomDays(Random random)
        {
            DateTime start = new DateTime(2024, 01, 01);
            DateTime end = DateTime.Now;

            int range = (end - start).Days;

            return start.AddDays(random.Next(range + 1));
        }

        public static DateTime RandomTimeBetween06And00(Random random, DateTime date)
        {
            DateTime startTime = new DateTime(date.Year, date.Month, date.Day, 6, 0, 0);

            int minutes = 24*60 - 6*60;

            return startTime.AddMinutes(random.Next(minutes));
        }
    }
}