using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;

class DriveScanner
{
    static void Main()
    {
        try
        {
            // Query physical drives
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
            {
                foreach (ManagementObject disk in searcher.Get())
                {
                    Console.WriteLine("=== Physical Drive ===");
                    //DisplayAllDriveProperties(disk);
                    DisplayDriveProperties(disk);

                    /*
                     * 
                    // Get associated partitions
                    string diskIndex = disk["Index"].ToString();
                    string partitionQuery = $"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{disk["DeviceID"]}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition";
                    
                    using (ManagementObjectSearcher partitionSearcher = new ManagementObjectSearcher(partitionQuery))
                    {
                        foreach (ManagementObject partition in partitionSearcher.Get())
                        {
                            Console.WriteLine("\n--- Partition ---");
                            DisplayPartitionProperties(partition);
                            
                            // Get associated logical disks (volumes)
                            string logicalDiskQuery = $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass = Win32_LogicalDiskToPartition";
                            
                            using (ManagementObjectSearcher logicalDiskSearcher = new ManagementObjectSearcher(logicalDiskQuery))
                            {
                                foreach (ManagementObject logicalDisk in logicalDiskSearcher.Get())
                                {
                                    Console.WriteLine("\n---- Volume ----");
                                    DisplayVolumeProperties(logicalDisk);
                                    
                                    // Get drive info using System.IO
                                    string driveLetter = logicalDisk["DeviceID"].ToString();
                                    try
                                    {
                                        DriveInfo driveInfo = new DriveInfo(driveLetter);
                                        DisplayDriveInfoProperties(driveInfo);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error accessing drive {driveLetter}: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                    *
                    */
                    Console.WriteLine("\n=================");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void DisplayAllDriveProperties(ManagementObject disk)
    {
        foreach (PropertyData p in disk.Properties)
        {
            Console.WriteLine($"{p.Name}:    {p.Value}");
        }
    }

    static void DisplayDriveProperties(ManagementObject disk)
    {
        Console.WriteLine($"DeviceID: {disk["DeviceID"]}");
        Console.WriteLine($"PNPDeviceID: {disk["PNPDeviceID"]}");
        Console.WriteLine($"Model: {disk["Model"]}");
        Console.WriteLine($"InterfaceType: {disk["InterfaceType"]}");
        Console.WriteLine($"Media Type: {disk["MediaType"]}");
        Console.WriteLine($"Size: {FormatBytes(disk["Size"])}");
        Console.WriteLine($"Partitions: {disk["Partitions"]}");
        Console.WriteLine($"Serial Number: {disk["SerialNumber"]}");
        Console.WriteLine($"Status: {disk["Status"]}");

        // Console.WriteLine($"Signature: {disk["Signature"]}");

        // Console.WriteLine($"PartitionStyle: {disk["PartitionStyle"]}");
        // Console.WriteLine($"**Disk Number**: {disk["PartitionStyle"]}");
    }

    static void DisplayPartitionProperties(ManagementObject partition)
    {
        Console.WriteLine($"Partition ID: {partition["DeviceID"]}");
        Console.WriteLine($"Type: {partition["Type"]}");
        Console.WriteLine($"Bootable: {partition["Bootable"]}");
        Console.WriteLine($"Primary Partition: {partition["PrimaryPartition"]}");
        Console.WriteLine($"Size: {FormatBytes(partition["Size"])}");
        Console.WriteLine($"Starting Offset: {FormatBytes(partition["StartingOffset"])}");
    }

    static void DisplayVolumeProperties(ManagementObject logicalDisk)
    {
        Console.WriteLine($"Drive Letter: {logicalDisk["DeviceID"]}");
        Console.WriteLine($"Drive Type: {GetDriveType(logicalDisk["DriveType"])}");
        Console.WriteLine($"File System: {logicalDisk["FileSystem"]}");
        Console.WriteLine($"Free Space: {FormatBytes(logicalDisk["FreeSpace"])}");
        Console.WriteLine($"Total Size: {FormatBytes(logicalDisk["Size"])}");
        Console.WriteLine($"Volume Name: {logicalDisk["VolumeName"]}");
        Console.WriteLine($"Volume Serial Number: {logicalDisk["VolumeSerialNumber"]}");
    }

    static void DisplayDriveInfoProperties(DriveInfo driveInfo)
    {
        if (driveInfo.IsReady)
        {
            Console.WriteLine($"Drive Format: {driveInfo.DriveFormat}");
            Console.WriteLine($"Drive Type (IO): {driveInfo.DriveType}");
            Console.WriteLine($"Root Directory: {driveInfo.RootDirectory}");
            Console.WriteLine($"Available Free Space: {FormatBytes(driveInfo.AvailableFreeSpace)}");
            Console.WriteLine($"Total Free Space: {FormatBytes(driveInfo.TotalFreeSpace)}");
            Console.WriteLine($"Total Size (IO): {FormatBytes(driveInfo.TotalSize)}");
        }
        else
        {
            Console.WriteLine("Drive is not ready");
        }
    }

    static string FormatBytes(object bytes)
    {
        try
        {
            long byteCount = Convert.ToInt64(bytes);
            string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            int index = 0;
            double size = byteCount;

            while (size >= 1024 && index < suffixes.Length - 1)
            {
                size /= 1024;
                index++;
            }

            return $"{size:0.##} {suffixes[index]}";
        }
        catch
        {
            return "N/A";
        }
    }

    static string GetDriveType(object driveType)
    {
        try
        {
            switch (Convert.ToInt32(driveType))
            {
                case 2: return "Removable";
                case 3: return "Fixed";
                case 4: return "Network";
                case 5: return "CD-ROM";
                case 6: return "RAM Disk";
                default: return "Unknown";
            }
        }
        catch
        {
            return "N/A";
        }
    }

}

