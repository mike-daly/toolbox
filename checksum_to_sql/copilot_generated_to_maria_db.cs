using System;
using System.Management;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
//using MySql.Data.MySqlClient;
using MySqlConnector;

namespace checksum_to_sql
{
    class Program
    {
        static void Main()
        {
            string[] drives = Environment.GetLogicalDrives();
            Dictionary<string, List<string>> fileHashes = new Dictionary<string, List<string>>();

            foreach (string drive in drives)
            {
                TraverseDirectory(drive, fileHashes);
            }

            foreach (var hash in fileHashes)
            {
                if (hash.Value.Count > 1)
                {
                    Console.WriteLine($"Duplicate files with hash {hash.Key}:");
                    foreach (var file in hash.Value)
                    {
                        Console.WriteLine(file);
                    }
                }
            }
        }

        static void TraverseDirectory(string directory, Dictionary<string, List<string>> fileHashes)
        {
            try
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    string hash = ComputeFileHash(file);
                    if (!fileHashes.ContainsKey(hash))
                    {
                        fileHashes[hash] = new List<string>();
                    }
                    fileHashes[hash].Add(file);
                }

                foreach (string subDirectory in Directory.GetDirectories(directory))
                {
                    TraverseDirectory(subDirectory, fileHashes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing {directory}: {ex.Message}");
            }
        }

        static string ComputeFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }




    class ProgramGetDiskInfo
    {
        static void Main()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            foreach (ManagementObject disk in searcher.Get())
            {
                Console.WriteLine("Model: " + disk["Model"]);
                Console.WriteLine("Serial Number: " + disk["SerialNumber"]);
                Console.WriteLine("Interface Type: " + disk["InterfaceType"]);
                Console.WriteLine("Media Type: " + disk["MediaType"]);
                Console.WriteLine();
            }
        }
    }



    class ProgramIntoDatabase
    {
        static void Main()
        {
            string connectionString = "Server=your_server;Database=FileDatabase;User ID=your_user;Password=your_password;";

            string[] drives = Environment.GetLogicalDrives();
            Dictionary<string, List<string>> fileHashes = new Dictionary<string, List<string>>();

            foreach (string drive in drives)
            {
                TraverseDirectory(drive, fileHashes);
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                foreach (var hash in fileHashes)
                {
                    foreach (var file in hash.Value)
                    {
                        InsertFileInfo(connection, file, hash.Key);
                    }
                }
            }
        }

#if false
        static void TraverseDirectory(string directory, Dictionary<string, List<string>> fileHashes)
        {
            try
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    string hash = ComputeFileHash(file);
                    if (!fileHashes.ContainsKey(hash))
                    {
                        fileHashes[hash] = new List<string>();
                    }
                    fileHashes[hash].Add(file);
                }

                foreach (string subDirectory in Directory.GetDirectories(directory))
                {
                    TraverseDirectory(subDirectory, fileHashes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing {directory}: {ex.Message}");
            }
        }


    static string ComputeFileHash(string filePath)
    {
        using (var sha256 = SHA256.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = sha256.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
#endif

        static void InsertFileInfo(MySqlConnection connection, string filePath, string fileHash)
        {
            string query = "INSERT INTO FileInfo (FilePath, FileHash) VALUES (@FilePath, @FileHash)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FilePath", filePath);
                command.Parameters.AddWithValue("@FileHash", fileHash);
                command.ExecuteNonQuery();
            }
        }
    }
}


#if false

dotnet add package MySql.Data

//# in MariaDB:

CREATE DATABASE FileDatabase;       
USE FileDatabase;

CREATE TABLE FileInfo (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FilePath VARCHAR(255),
    FileHash VARCHAR(64)
);


#endif
