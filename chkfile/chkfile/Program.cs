using chkfile;
using System;
using System.IO;
using System.Management;

class DriveScanner
{
    static void Main()
    {
        int indentLevel = 0;
        ISaveResults SaveResults = new StdOutWrite();
        try
        {
            // Query physical drives
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
            {
                foreach (ManagementObject disk in searcher.Get())
                {
                    if (Convert.ToInt64(disk["Size"]) > 0)
                    {
                        SaveResults.SavePhysicalDrive(0, disk);
                    }

                    // Get associated partitions
                    string diskIndex = disk["Index"].ToString();
                    string partitionQuery = $"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{disk["DeviceID"]}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition";

                    using (ManagementObjectSearcher partitionSearcher = new ManagementObjectSearcher(partitionQuery))
                    {
                        foreach (ManagementObject partition in partitionSearcher.Get())
                        {
                            //Console.WriteLine("\n--- Partition ---");
                            SaveResults.SavePartition(4, partition);

                            // Get associated logical disks (volumes)
                            string logicalDiskQuery = $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass = Win32_LogicalDiskToPartition";

                            using (ManagementObjectSearcher logicalDiskSearcher = new ManagementObjectSearcher(logicalDiskQuery))
                            {
                                foreach (ManagementObject logicalDisk in logicalDiskSearcher.Get())
                                {
                                    //Console.WriteLine("\n---- Volume ----");
                                    SaveResults.SaveLogicalDisk(8, logicalDisk);

                                    // Get drive info using System.IO
                                    string driveLetter = logicalDisk["DeviceID"].ToString();
                                    try
                                    {
                                        DriveInfo driveInfo = new DriveInfo(driveLetter);
                                        SaveResults.SaveDriveInfo(12, driveInfo);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error accessing drive {driveLetter}: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

