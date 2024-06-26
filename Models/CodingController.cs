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

        public static void FilterCodingRecords(string startingDate = "", string endingDate = "", string date = "year")
        {
            List<CodingTracker> tableData = new();

            string totalHoursCoding = "";
            string averageHoursCoding = "";

            using (var connection = new SqliteConnection(connectionString))
            {
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();

                    if (date == "days" || date == "weeks")
                    {
                        string startingDay = DateTime.ParseExact(startingDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        string endingDay = DateTime.ParseExact(endingDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        totalHoursCoding = CalculateTotalHours(startingDay, endingDay, date);
                        averageHoursCoding = CalculateAverageHoursPerPeriod(startingDay, endingDay, date);

                        tableCmd.CommandText = $@"
                            SELECT * 
                            FROM coding_session 
                            WHERE 
                                strftime('%Y-%m-%d', substr(Date, 7, 4) || '-' || substr(Date, 4, 2) || '-' || substr(Date, 1, 2)) 
                                BETWEEN '{startingDay}' AND '{endingDay}' 
                            ORDER BY 
                                strftime('%Y-%m-%d', substr(Date, 7, 4) || '-' || substr(Date, 4, 2) || '-' || substr(Date, 1, 2)) ASC";
                    }
                    else if (date == "months")
                    {
                        string startingMonth = ConvertToYearMonth(startingDate);
                        string endingMonth = ConvertToYearMonth(endingDate);
                        totalHoursCoding = CalculateTotalHours(startingMonth, endingMonth, "months");
                        averageHoursCoding = CalculateAverageHoursPerPeriod(startingMonth, endingMonth, "months");

                        tableCmd.CommandText = $@"
                            SELECT * 
                            FROM coding_session 
                            WHERE 
                                strftime('%Y-%m', substr(Date, 7, 4) || '-' || substr(Date, 4, 2) || '-' || substr(Date, 1, 2)) 
                                BETWEEN '{startingMonth}' AND '{endingMonth}' 
                            ORDER BY 
                                strftime('%Y-%m-%d', substr(Date, 7, 4) || '-' || substr(Date, 4, 2) || '-' || substr(Date, 1, 2)) ASC";
                    }
                    else if (date == "year")
                    {
                        totalHoursCoding = CalculateTotalHours(startingDate);
                        averageHoursCoding = CalculateAverageHoursPerPeriod(startingDate: startingDate);

                        tableCmd.CommandText = $@"
                        SELECT *
                        FROM coding_session
                        WHERE 
                            strftime('%Y', substr(Date, 7, 4) || '-' || substr(Date, 4, 2) || '-' || substr(Date, 1, 2)) = '{startingDate}';
                        ORDER BY 
                            Id ASC";
                    }
                    else
                    {
                        Console.WriteLine("Invalid date.");
                        return;
                    }

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
            Console.WriteLine(totalHoursCoding);
            Console.WriteLine(averageHoursCoding);
        }

        private static string ConvertToYearMonth(string date)
        {
            string[] dateYearMonth = date.Split('-');

            if (dateYearMonth.Length != 2)
            {
                throw new ArgumentException("Date format is invalid. Expected format is MM-yyyy.");
            }

            return dateYearMonth[1] + '-' + dateYearMonth[0];
        }

        public static string CalculateTotalHours(string startingDate = "", string endingDate = "", string date = "year")
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();

                    string query = "";

                    if (date == "days" || date == "weeks" || date == "months")
                    {
                        string dateFormat = date == "months" ? "%Y-%m" : "%Y-%m-%d";

                        query = $@"
                        SELECT SUM(CAST(SUBSTR(Duration, 1, 2) AS INTEGER) * 60 + CAST(SUBSTR(Duration, 4, 2) AS INTEGER)) 
                        FROM coding_session
                        WHERE 
                            strftime('{dateFormat}', substr(Date, 7, 4) || '-' || substr(Date, 4, 2) || '-' || substr(Date, 1, 2))
                        BETWEEN '{startingDate}' AND '{endingDate}'";
                    }
                    else if (date == "year")
                    {
                        query = $@"
                        SELECT SUM(CAST(SUBSTR(Duration, 1, 2) AS INTEGER) * 60 + CAST(SUBSTR(Duration, 4, 2) AS INTEGER)) 
                        FROM coding_session
                        WHERE 
                            strftime('%Y', substr(Date, 7, 4) || '-' || substr(Date, 4, 2) || '-' || substr(Date, 1, 2))
                        = '{startingDate}'";
                    }
                    else
                    {
                        Console.WriteLine("Invalid date.");
                        return "0";
                    }
                    tableCmd.CommandText = query;
                    var result = tableCmd.ExecuteScalar();

                    int totalMinutes = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    return $"Total Time Coding: {totalMinutes / 60} hours and {totalMinutes % 60} minutes.";
                }
            }
        }
        public static string CalculateAverageHoursPerPeriod(string startingDate = "", string endingDate = "", string date = "year")
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();

                    string query = "";
                    double averageHours = 0.0;

                    if (date == "days" || date == "weeks" || date == "months")
                    {
                        string dateFormat = date == "months" ? "%Y-%m" : "%Y-%m-%d";

                        DateTime startDate, endDate;
                        if (startingDate.Length == 7 && endingDate.Length == 7)
                        {
                            startDate = DateTime.ParseExact(startingDate + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            endDate = DateTime.ParseExact(endingDate + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
                        }
                        else
                        {
                            startDate = DateTime.ParseExact(startingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            endDate = DateTime.ParseExact(endingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }

                        query = $@"
                            SELECT SUM(CAST(SUBSTR(Duration, 1, 2) AS INTEGER) * 60 + CAST(SUBSTR(Duration, 4, 2) AS INTEGER)) 
                            FROM coding_session
                            WHERE 
                                strftime('{dateFormat}', substr(Date, 7, 4) || '-' || substr(Date, 4, 2) || '-' || substr(Date, 1, 2))
                            BETWEEN '{startingDate}' AND '{endingDate}'";

                        tableCmd.CommandText = query;
                        var result = tableCmd.ExecuteScalar();
                        int totalMinutes = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                        double totalHours = totalMinutes / 60.0;

                        double totalDays = (endDate - startDate).TotalDays + 1;

                        if (date == "weeks")
                        {
                            double totalWeeks = totalDays / 7.0;
                            averageHours = totalHours / totalWeeks;
                        }
                        else if (date == "days")
                        {
                            averageHours = totalHours / totalDays;
                        }
                        else if (date == "months")
                        {
                            double totalMonths = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month + 1;
                            averageHours = totalHours / totalMonths;
                        }
                    }
                    else if (date == "year")
                    {
                        query = $@"
                            SELECT SUM(CAST(SUBSTR(Duration, 1, 2) AS INTEGER) * 60 + CAST(SUBSTR(Duration, 4, 2) AS INTEGER)) 
                            FROM coding_session
                            WHERE 
                                strftime('%Y', substr(Date, 7, 4) || '-' || substr(Date, 4, 2) || '-' || substr(Date, 1, 2))
                            = '{startingDate}'";

                        tableCmd.CommandText = query;
                        var result = tableCmd.ExecuteScalar();
                        int totalMinutes = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                        double totalHours = totalMinutes / 60.0;

                        double totalDaysInYear = 365.0;
                        averageHours = totalHours / totalDaysInYear;
                    }
                    else
                    {
                        Console.WriteLine("Invalid date.");
                        return "0";
                    }

                    string period = date == "days" ? "day" : date == "weeks" ? "week" : date == "months" ? "month" : "year";
                    return $"Average Coding Time per {period}: {averageHours:F2} hours.\n";
                }
            }
        }
    }
}