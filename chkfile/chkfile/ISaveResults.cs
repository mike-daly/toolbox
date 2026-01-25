using System;
using System.IO;
using System.Management;

namespace chkfile
{
    internal interface ISaveResults
    {
        /*
        void UpsertDisk(
            int indentLevel,
                   string p_DeviceID,
                   string p_PNPDeviceID,
                   string p_Model,
                   string p_InterfaceType,
                   string p_MediaType,
                   int p_Size,
                   int p_Partitions,
                   string p_SerialNumber,
                   string p_Status);

        void UpsertFile(
            int indentLevel,
            // something diskhandle
            string filename,
            string filePath,
            string checksum,    // or something
            DateTime created,   // filetime?
            DateTime lastModified   // filetime?
            );

        void SaveAllDriveProperties(int indentLevel, ManagementObject disk);
        */

        void SavePhysicalDrive(int indentLevel, ManagementObject disk);

        void SavePartition(int indentLevel, ManagementObject partition);

        void SaveLogicalDisk(int indentLevel, ManagementObject logicalDisk);

        void SaveDriveInfo(int indentLevel, DriveInfo driveInfo);

    }
}