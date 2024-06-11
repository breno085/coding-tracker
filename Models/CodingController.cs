using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace coding_tracker.Models
{
    internal class CodingController
    {
        static string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

        internal void Post(CodingTracker code)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();

                    tableCmd.CommandText =
                        $"INSERT INTO coding_session(Date, StartTime, EndTime, Duration) VALUES ('{code.Date.ToString("dd-MM-yyyy")}','{code.StartTime.ToString("hh\\:mm")}', '{code.EndTime.ToString("hh\\:mm")}', '{code.Duration.ToString("hh\\:mm")}')";

                    tableCmd.ExecuteNonQuery();
                }
            }
        }

        internal void Get()
        {
            List<CodingTracker> tableData = new();

            using (var connection = new SqliteConnection(connectionString))
            {
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();

                    tableCmd.CommandText = 
                    "SELECT * FROM coding_session ORDER BY " +
                    "SUBSTR(Date, 7, 4) || '-' || SUBSTR(Date, 4, 2) || '-' || SUBSTR(Date, 1, 2) ASC;";

                    using (var reader = tableCmd.ExecuteReader())
                    {
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
                    }
                }
            }
            SpectreTableRenderer.RenderTable(tableData);
        }

        internal void DeleteRecord(int recordId)
        {
            GetUserInput delete = new();

            using (var connection = new SqliteConnection(connectionString))
            {
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();

                    tableCmd.CommandText = $"DELETE FROM coding_session WHERE Id = {recordId}";

                    int rowCount = tableCmd.ExecuteNonQuery();

                    if (rowCount == 0)
                    {
                        Console.WriteLine($"\nRecord with Id = {recordId} doesn't exist\n");
                        delete.Delete();
                    }
                }
            }

            Console.WriteLine($"\nRecord with Id = {recordId} was deleted.\n");
        }

        internal void UpdateRecord(int recordId, string startInput, string endInput, string codingDuration, string date)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();

                    string updateCmd = $"UPDATE coding_session SET ";

                    if (!string.IsNullOrEmpty(date))
                        updateCmd += $"Date = '{date}', ";

                    if (!string.IsNullOrEmpty(startInput))
                        updateCmd += $"StartTime = '{startInput}', ";

                    if (!string.IsNullOrEmpty(endInput))
                        updateCmd += $"EndTime = '{endInput}', ";

                    if (!string.IsNullOrEmpty(codingDuration))
                        updateCmd += $"Duration = '{codingDuration}', ";

                    updateCmd = updateCmd.TrimEnd(',', ' ');

                    updateCmd += $" WHERE Id = {recordId}";

                    tableCmd.CommandText = updateCmd;

                    tableCmd.ExecuteNonQuery();
                }
            }

            if (string.IsNullOrEmpty(codingDuration))
            {
                string startTimeStr;
                string endTimeStr;
                string codeDuration;

                using (var connection = new SqliteConnection(connectionString))
                {
                    using (var tableCmd = connection.CreateCommand())
                    {
                        connection.Open();

                        tableCmd.CommandText =
                        $"SELECT StartTime, EndTime FROM coding_session WHERE Id = {recordId}";

                        using (var reader = tableCmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {

                                    startTimeStr = reader.GetString(0);
                                    endTimeStr = reader.GetString(1);

                                    codeDuration = GetUserInput.CalculateDuration(startTimeStr, endTimeStr);

                                    using (var updateCmd = connection.CreateCommand())
                                    {
                                        updateCmd.CommandText = $"UPDATE coding_session SET Duration = '{codeDuration}' WHERE Id = {recordId}";
                                        updateCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No rows found");
                            }
                        }
                    }
                }
            }
        }

        public bool RecordExists(int recordId)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                using (var checkCmd = connection.CreateCommand())
                {
                    connection.Open();

                    checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM coding_session WHERE Id = {recordId})";
                    int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                    return checkQuery != 0;
                }
            }
        }
    }
}