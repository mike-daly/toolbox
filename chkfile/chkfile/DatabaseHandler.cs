using MySqlConnector;
using System;
using System.Data;
using System.IO;
using System.Management;

namespace chkfile
{
    internal class DatabaseHandler : ISaveResults
    {
        private readonly string connectionString;
        private MySqlConnection mySqlConnection;

        /// <summary>
        /// Construct new DatabaseHandler
        /// </summary>
        /// <param name="connectionString"> "Server=localhost;Database=your_database;Integrated Security=true;"; /// </param>

        public DatabaseHandler(int indentLevel, string connectionString)
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


        /*
        public void UpsertDisk(
            int indentLevel,
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
        */

        /*
        public void SaveAllDriveProperties(int indentLevel, ManagementObject disk)
        {
            foreach (PropertyData p in disk.Properties)
            {
                Console.WriteLine($"{p.Name}:    {p.Value}");
            }
        }
        */

        public void SavePhysicalDrive(int indentLevel, ManagementObject disk)
        {
            Console.WriteLine($"DeviceID: {disk["DeviceID"]}");
            Console.WriteLine($"PNPDeviceID: {disk["PNPDeviceID"]}");
            Console.WriteLine($"Model: {disk["Model"]}");
            Console.WriteLine($"InterfaceType: {disk["InterfaceType"]}");
            Console.WriteLine($"Media Type: {disk["MediaType"]}");
            Console.WriteLine($"Size: {Helpers.FormatBytes(disk["Size"])}");
            Console.WriteLine($"Partitions: {disk["Partitions"]}");
            Console.WriteLine($"Serial Number: {disk["SerialNumber"]}");
            Console.WriteLine($"Status: {disk["Status"]}");

            // Console.WriteLine($"Signature: {disk["Signature"]}");

            // Console.WriteLine($"PartitionStyle: {disk["PartitionStyle"]}");
            // Console.WriteLine($"**Disk Number**: {disk["PartitionStyle"]}");
        }

        public void SavePartition(int indentLevel, ManagementObject partition)
        {
            Console.WriteLine($"Partition ID: {partition["DeviceID"]}");
            Console.WriteLine($"Type: {partition["Type"]}");
            Console.WriteLine($"Bootable: {partition["Bootable"]}");
            Console.WriteLine($"Primary Partition: {partition["PrimaryPartition"]}");
            Console.WriteLine($"Size: {Helpers.FormatBytes(partition["Size"])}");
            Console.WriteLine($"Starting Offset: {Helpers.FormatBytes(partition["StartingOffset"])}");
        }

        public void SaveLogicalDisk(int indentLevel, ManagementObject logicalDisk)
        {
            Console.WriteLine($"Drive Letter: {logicalDisk["DeviceID"]}");
            Console.WriteLine($"Drive Type: {Helpers.GetDriveType(logicalDisk["DriveType"])}");
            Console.WriteLine($"File System: {logicalDisk["FileSystem"]}");
            Console.WriteLine($"Free Space: {Helpers.FormatBytes(logicalDisk["FreeSpace"])}");
            Console.WriteLine($"Total Size: {Helpers.FormatBytes(logicalDisk["Size"])}");
            Console.WriteLine($"Volume Name: {logicalDisk["VolumeName"]}");
            Console.WriteLine($"Volume Serial Number: {logicalDisk["VolumeSerialNumber"]}");
        }

        public void SaveDriveInfo(int indentLevel, DriveInfo driveInfo)
        {
            if (driveInfo.IsReady)
            {
                Console.WriteLine($"Drive Format: {driveInfo.DriveFormat}");
                Console.WriteLine($"Drive Type (IO): {driveInfo.DriveType}");
                Console.WriteLine($"Root Directory: {driveInfo.RootDirectory}");
                Console.WriteLine($"Available Free Space: {Helpers.FormatBytes(driveInfo.AvailableFreeSpace)}");
                Console.WriteLine($"Total Free Space: {Helpers.FormatBytes(driveInfo.TotalFreeSpace)}");
                Console.WriteLine($"Total Size (IO): {Helpers.FormatBytes(driveInfo.TotalSize)}");
            }
            else
            {
                Console.WriteLine("Drive is not ready");
            }
        }

        /*
        void ISaveResults.UpsertFile(int indentLevel, string filename, string filePath, string checksum, DateTime created, DateTime lastModified)
        {
            throw new NotImplementedException();
        }
        */
    }
}
