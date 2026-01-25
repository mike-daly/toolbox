using MySqlConnector;
using System;
using System.IO;
using System.Management;

namespace chkfile
{
    internal class StdOutWrite : ISaveResults
    {
        private readonly string connectionString;
        private MySqlConnection mySqlConnection;

        /// <summary>
        /// Construct new StdOutWrite
        /// </summary>
        /// <param name="connectionString"> "Server=localhost;Database=your_database;Integrated Security=true;"; /// </param>

        public StdOutWrite()
        {
        }


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
            string indent = new string(' ', indentLevel);
            Console.WriteLine(
$@"
{indent}p_DeviceID {p_DeviceID ?? (object)DBNull.Value}
{indent}p_PNPDeviceID {p_PNPDeviceID ?? (object)DBNull.Value}
{indent}p_Model {p_Model ?? (object)DBNull.Value}
{indent}p_InterfaceType {p_InterfaceType ?? (object)DBNull.Value}
{indent}p_MediaType {p_MediaType ?? (object)DBNull.Value}
{indent}p_Size {p_Size}
{indent}p_Partitions {p_Partitions}
{indent}p_SerialNumber {p_SerialNumber ?? (object)DBNull.Value}
{indent}p_Status {p_Status ?? (object)DBNull.Value} "
);

        }


        public void SaveAllDriveProperties(
            int indentLevel, ManagementObject disk)
        {
            string indent = new string(' ', indentLevel);
            foreach (PropertyData p in disk.Properties)
            {
                Console.WriteLine($"{indent}{p.Name}:    {p.Value}");
            }
        }
        */

        public void SavePhysicalDrive(
            int indentLevel, ManagementObject disk)
        {
            string indent = new string(' ', indentLevel);
            Console.WriteLine();
            Console.WriteLine($"{indent}=== Physical Drive ===");
            Console.WriteLine($"{indent}DeviceID: {disk["DeviceID"]}");
            Console.WriteLine($"{indent}PNPDeviceID: {disk["PNPDeviceID"]}");
            Console.WriteLine($"{indent}Model: {disk["Model"]}");
            Console.WriteLine($"{indent}InterfaceType: {disk["InterfaceType"]}");
            Console.WriteLine($"{indent}Media Type: {disk["MediaType"]}");
            Console.WriteLine($"{indent}Size: {Helpers.FormatBytes(disk["Size"])}");
            Console.WriteLine($"{indent}Partitions: {disk["Partitions"]}");
            Console.WriteLine($"{indent}Serial Number: {disk["SerialNumber"]}");
            Console.WriteLine($"{indent}Status: {disk["Status"]}");

            // Console.WriteLine($"Signature: {disk["Signature"]}");

            // Console.WriteLine($"PartitionStyle: {disk["PartitionStyle"]}");
            // Console.WriteLine($"**Disk Number**: {disk["PartitionStyle"]}");
        }

        public void SavePartition(
            int indentLevel, ManagementObject partition)
        {
            string indent = new string(' ', indentLevel);
            Console.WriteLine();
            Console.WriteLine($"{indent}=== Partition ===");
            Console.WriteLine($"{indent}Partition ID: {partition["DeviceID"]}");
            Console.WriteLine($"{indent}Type: {partition["Type"]}");
            Console.WriteLine($"{indent}Bootable: {partition["Bootable"]}");
            Console.WriteLine($"{indent}Primary Partition: {partition["PrimaryPartition"]}");
            Console.WriteLine($"{indent}Size: {Helpers.FormatBytes(partition["Size"])}");
            Console.WriteLine($"{indent}Starting Offset: {Helpers.FormatBytes(partition["StartingOffset"])}");
        }

        public void SaveLogicalDisk(
            int indentLevel, ManagementObject logicalDisk)
        {
            string indent = new string(' ', indentLevel);
            Console.WriteLine();
            Console.WriteLine($"{indent}=== logicalDisk ===");
            Console.WriteLine($"{indent}Drive Letter: {logicalDisk["DeviceID"]}");
            Console.WriteLine($"{indent}Drive Type: {Helpers.GetDriveType(logicalDisk["DriveType"])}");
            Console.WriteLine($"{indent}File System: {logicalDisk["FileSystem"]}");
            Console.WriteLine($"{indent}Free Space: {Helpers.FormatBytes(logicalDisk["FreeSpace"])}");
            Console.WriteLine($"{indent}Total Size: {Helpers.FormatBytes(logicalDisk["Size"])}");
            Console.WriteLine($"{indent}Volume Name: {logicalDisk["VolumeName"]}");
            Console.WriteLine($"{indent}Volume Serial Number: {logicalDisk["VolumeSerialNumber"]}");
        }

        public void SaveDriveInfo(
            int indentLevel, DriveInfo driveInfo)
        {
            if (driveInfo.IsReady)
            {
                string indent = new string(' ', indentLevel);
            Console.WriteLine();
            Console.WriteLine($"{indent}=== DriveInfo ===");
                Console.WriteLine($"{indent}Drive Format: {driveInfo.DriveFormat}");
                Console.WriteLine($"{indent}Drive Type (IO): {driveInfo.DriveType}");
                Console.WriteLine($"{indent}Root Directory: {driveInfo.RootDirectory}");
                Console.WriteLine($"{indent}Available Free Space: {Helpers.FormatBytes(driveInfo.AvailableFreeSpace)}");
                Console.WriteLine($"{indent}Total Free Space: {Helpers.FormatBytes(driveInfo.TotalFreeSpace)}");
                Console.WriteLine($"{indent}Total Size (IO): {Helpers.FormatBytes(driveInfo.TotalSize)}");
            }
            else
            {
                Console.WriteLine("Drive is not ready");
            }
        }

        /*
        void ISaveResults.UpsertFile(
            int indentLevel, string filename, string filePath, string checksum, DateTime created, DateTime lastModified)
        {
            throw new NotImplementedException();
        }
        */
    }
}
