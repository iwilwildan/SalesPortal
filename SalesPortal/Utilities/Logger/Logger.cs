using System.Data;
using System.Data.SqlClient;

namespace SalesPortal.Utilities.Logger
{
    public class Logger : ICustomLogger
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _logFilePath;

        public Logger(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SQLConnection");
            _logFilePath = _configuration.GetSection("FilePath").GetSection("LogFilePath").Value;
        }

        public void LogException(Exception ex, string additionalInfo = "")
        {
            string logMessage = $"[ERROR] {DateTime.Now}: {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}";

            if (!string.IsNullOrEmpty(additionalInfo))
            {
                logMessage += $"Additional Info: {additionalInfo}{Environment.NewLine}";
            }

            WriteToLogFile(logMessage);
            LogToDatabase("Error", logMessage);
        }

        private void LogToDatabase(string logLevel, string logMessage)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("LogError", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@LogLevel", logLevel);
                        command.Parameters.AddWithValue("@LogMessage", logMessage);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging to database: {ex.Message}");
            }
        }

        private void WriteToLogFile(string logMessage)
        {
            try
            {
                File.AppendAllText(_logFilePath, logMessage);
            }
            catch (Exception)
            {
                Console.WriteLine("Error writing to log file.");
            }
        }
    }

}
