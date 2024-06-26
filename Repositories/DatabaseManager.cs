using Microsoft.Data.Sqlite;

namespace coding_tracker.Repositories
{
    internal class DatabaseManager
    {
        internal void CreateTable(string connectionString)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();

                    tableCmd.CommandText =
                        @"CREATE TABLE IF NOT EXISTS coding_session (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        StartTime TEXT,
                        EndTime TEXT,
                        Duration TEXT)";

                    tableCmd.ExecuteNonQuery();
                }
            }
        }
    }
}