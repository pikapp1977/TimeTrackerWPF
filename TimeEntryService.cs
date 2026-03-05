using Microsoft.Data.Sqlite;

namespace TimeTrackerWPF
{
    public class TimeEntryService
    {
        private readonly string dbPath;

        public TimeEntryService(string databasePath)
        {
            dbPath = databasePath;
        }

        public bool IsEntryBilled(int id)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            
            command.CommandText = "SELECT Billed FROM TimeEntries WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);
            var result = command.ExecuteScalar();
            
            return result != null && !Convert.IsDBNull(result) && Convert.ToInt32(result) == 1;
        }

        public bool IsEntryPaid(int id)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            
            command.CommandText = "SELECT Paid FROM TimeEntries WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);
            var result = command.ExecuteScalar();
            
            return result != null && !Convert.IsDBNull(result) && Convert.ToInt32(result) == 1;
        }

        public void ToggleBilled(int id)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            
            command.CommandText = "SELECT Billed FROM TimeEntries WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);
            var result = command.ExecuteScalar();
            int currentBilled = result != null && !Convert.IsDBNull(result) ? Convert.ToInt32(result) : 0;
            
            int newBilled = currentBilled == 1 ? 0 : 1;
            command.CommandText = "UPDATE TimeEntries SET Billed = $billed WHERE Id = $id";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("$billed", newBilled);
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        public PaidResult TogglePaid(int id)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            
            command.CommandText = "SELECT Billed, Paid FROM TimeEntries WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);
            using var reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                int billed = !reader.IsDBNull(0) ? reader.GetInt32(0) : 0;
                int paid = !reader.IsDBNull(1) ? reader.GetInt32(1) : 0;
                reader.Close();
                
                if (paid == 0 && billed == 0)
                {
                    return new PaidResult 
                    { 
                        Success = false, 
                        Message = "This entry must be billed before it can be marked as paid." 
                    };
                }
                
                int newPaid = paid == 1 ? 0 : 1;
                command.CommandText = "UPDATE TimeEntries SET Paid = $paid WHERE Id = $id";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("$paid", newPaid);
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
                
                return new PaidResult 
                { 
                    Success = true, 
                    IsPaid = newPaid == 1 
                };
            }
            
            return new PaidResult 
            { 
                Success = false, 
                Message = "Entry not found." 
            };
        }

        public void DeleteUnbilledEntries()
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM TimeEntries WHERE Billed = 0 OR Billed IS NULL";
            command.ExecuteNonQuery();
        }

        public int CountBilledEntries()
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM TimeEntries WHERE Billed = 1";
            var result = command.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int CountPaidEntries()
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM TimeEntries WHERE Paid = 1";
            var result = command.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int CountUnbilledEntries()
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM TimeEntries WHERE Billed = 0 OR Billed IS NULL";
            var result = command.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }

    public class PaidResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
    }
}
