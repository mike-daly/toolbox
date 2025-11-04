using MySqlConnector;
using System;
using System.Data;

namespace chkfile
{
    internal class DatabaseHandler
    {
        private readonly string connectionString;
        private MySqlConnection mySqlConnection;

        /// <summary>
        /// Construct new DatabaseHandler
        /// </summary>
        /// <param name="connectionString"> "Server=localhost;Database=your_database;Integrated Security=true;"; /// </param>

        public DatabaseHandler(string connectionString)
        {
            this.connectionString = connectionString;

            try
            {
                // Create connection using Windows Identity
                {
                    this.mySqlConnection = new MySqlConnection(this.connectionString);
                    this.mySqlConnection.Open();
                    Console.WriteLine("Successfully connected to MariaDB using Windows Identity!");
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }

        }

        ~DatabaseHandler()
        {
            this.mySqlConnection.Close();
        }

        /*
        static void Connect(string[] args)
        {
            // Connection string using Integrated Security

            try
            {
                // Create connection using Windows Identity
                {

                    // Example: Execute a simple query
                    using (var command = new MySqlCommand("SELECT CURRENT_USER();", connection))
                    {
                        string user = command.ExecuteScalar()?.ToString();
                        Console.WriteLine($"Connected as: {user}");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }
        }
        */


        public void UpsertDisk(
            string p_DeviceID,
            string p_PNPDeviceID,
            string p_Model,
            string p_InterfaceType,
            string p_MediaType,
            int p_Size,
            int p_Partitions,
            string p_SerialNumber,
            string p_Status)
        {
            using (var command = new MySqlCommand("UpsertDisk", this.mySqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("p_DeviceID", p_DeviceID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("p_PNPDeviceID", p_PNPDeviceID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("p_Model", p_Model ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("p_InterfaceType", p_InterfaceType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("p_MediaType", p_MediaType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("p_Size", p_Size);
                command.Parameters.AddWithValue("p_Partitions", p_Partitions);
                command.Parameters.AddWithValue("p_SerialNumber", p_SerialNumber ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("p_Status", p_Status ?? (object)DBNull.Value);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    // Handle or log the exception as needed
                    throw new Exception("Error executing UpsertDisk procedure", ex);
                }
            }
        }
    }
}
