using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Management;

//using System.Reflection.Metadata.Ecma335;
//using System.Xml.Linq;


// DB Schema at the bottom

internal class ProgramGetDiskInfo
{

    //  "Availability", ""
    //  "BytesPerSector", ""
    //  "Capabilities", ""
    //  "CapabilityDescriptions", ""
    ////  "Caption", " Generic MassStorageClass USB Device"
    //  "CompressionMethod", ""
    //  "ConfigManagerErrorCode", " 0"
    //  "ConfigManagerUserConfig", " False"
    //  "CreationClassName", " Win32_DiskDrive"
    //  "DefaultBlockSize", ""
    //  "Description", " Disk drive"
    //  "DeviceID", " \\.\PHYSICALDRIVE4"
    //  "ErrorCleared", ""
    //  "ErrorDescription", ""
    //  "ErrorMethodology", ""
    ////  "FirmwareRevision", " 1539"
    //  "Index", " 4"
    //  "InstallDate", ""
    //  "InterfaceType", " USB"
    //  "LastErrorCode", ""
    //  "Manufacturer", " (Standard disk drives)"
    //  "MaxBlockSize", ""
    //  "MaxMediaSize", ""
    //  "MediaLoaded", " True"
    //  "MediaType", ""
    //  "MinBlockSize", ""
    ////  "Model", " Generic MassStorageClass USB Device"
    //  "Name", " \\.\PHYSICALDRIVE4"
    //  "NeedsCleaning", ""
    //  "NumberOfMediaSupported", ""
    //  "Partitions", " 0"
    ////  "PNPDeviceID", " USBSTOR\DISK&VEN_GENERIC&PROD_MASSSTORAGECLASS&REV_1539\000000001539&1"
    //  "PowerManagementCapabilities", ""
    //  "PowerManagementSupported", ""
    //  "SCSIBus", " 0"
    //  "SCSILogicalUnit", " 1"
    //  "SCSIPort", " 0"
    //  "SCSITargetId", " 0"
    //  "SectorsPerTrack", ""
    ////  "SerialNumber", " 000000001539"
    ////  "Signature", " 0"
    ////  "Size", ""
    //  "Status", " OK"
    //  "StatusInfo", ""
    //  "SystemCreationClassName", " Win32_ComputerSystem"
    //  "SystemName", " INISHNEE"
    //  "TotalCylinders", ""
    //  "TotalHeads", ""
    ////  "TotalSectors", ""
    //  "TotalTracks", ""
    //  "TracksPerCylinder", ""




    //string connectionString = "Server=your_server;Database=FileDatabase;User ID=your_user;Password=your_password;";
    //private static string connectionString = $"Server={Environment.MachineName}\\SQLEXPRESS;Database=FileDatabase;User ID={Environment.UserName};TrustServerCertificate=True";
    //private static string connectionString = $"Server={Environment.MachineName}\\SQLEXPRESS;Database=FileDatabase;User ID={Environment.UserName};Encrypt=false;Trusted_Connection=True;TrustServerCertificate=True";
    private static string connectionString = $"Server={Environment.MachineName}\\SQLEXPRESS;Database=FileDatabase;User ID={Environment.UserName};Encrypt=false;Trusted_Connection=True;";


    /// <summary>
    /// "everything we know about a disk" -- as persisted in the database
    /// </summary>
    public class MyDiskData
    {

        public string? Caption { get; set; }
        public string? FirmwareRevision { get; set; }
        public string? Model { get; set; }
        public string? PNPDeviceID { get; set; }
        public string? SerialNumber { get; set; }
        public string? Signature { get; set; }
        public string? Size { get; set; }
        public string? TotalSectors { get; set; }

        public DiskId diskId { get; set; }

        private string ConnectionString { get; set; }

        public MyDiskData(string connectionString)
        {
            this.ConnectionString = connectionString;
            //this.sqlConnection = new SqlConnection(connectionString);
        }

        /// <summary>
        /// save the disk data to the database table, returning the foreign key reference
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>

        public DiskId UpsertDiskData()
        {
            string query = @"INSERT INTO DiskInfo (
                Caption,
                FirmwareRevision,
                Model,
                PNPDeviceID,
                SerialNumber,
                Signature,
                Size,
                TotalSectors
                    ) VALUES (
                @Caption,
                @FirmwareRevision,
                @Model,
                @PNPDeviceID,
                @SerialNumber,
                @Signature,
                @Size,
                @TotalSectors )";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    //command.Parameters.AddWithValue("@FilePath", filePath);
                    //command.Parameters.AddWithValue("@FileHash", fileHash);

                    command.Parameters.AddWithValue("@Caption", this.Caption);
                    command.Parameters.AddWithValue("@FirmwareRevision", this.FirmwareRevision);
                    command.Parameters.AddWithValue("@Model", this.Model);
                    command.Parameters.AddWithValue("@PNPDeviceID", this.PNPDeviceID);
                    command.Parameters.AddWithValue("@SerialNumber", this.SerialNumber);
                    command.Parameters.AddWithValue("@Signature", this.Signature);
                    command.Parameters.AddWithValue("@Size", this.Size);
                    command.Parameters.AddWithValue("@TotalSectors", this.TotalSectors);

                    command.ExecuteNonQuery();
                }
            }

            return new DiskId("");
        }

    }

    /// <summary>
    /// foreign key to reference a row in the database disk information table
    /// </summary>
    internal class DiskId
    {
        public string Name { get; set; }
        public DiskId(string name) => this.Name = name;
    }

    private static void Main(string[] args)
    {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

        foreach (ManagementObject disk in searcher.Get())
        {
            foreach (PropertyData prop in disk.Properties)
            {
                Console.WriteLine($"{prop.Name} :: {prop.Value}");
            }
            Console.WriteLine();

            MyDiskData diskData = new MyDiskData(ProgramGetDiskInfo.connectionString);

            diskData.Caption = Convert.ToString(disk["Caption"]);
            diskData.FirmwareRevision = Convert.ToString(disk["FirmwareRevision"]);
            diskData.Model = Convert.ToString(disk["Model"]);
            diskData.PNPDeviceID = Convert.ToString(disk["PNPDeviceID"]);
            diskData.SerialNumber = Convert.ToString(disk["SerialNumber"]);
            diskData.Signature = Convert.ToString(disk["Signature"]);
            diskData.Size = Convert.ToString(disk["Size"]);
            diskData.TotalSectors = Convert.ToString(disk["TotalSectors"]);


            DiskId returnedDiskId = diskData.UpsertDiskData();
            Console.WriteLine($"Upsert of new disk data -- FK:  {returnedDiskId.Name}");

        }
    }
}

#if _ExposeSchema_
// never true

USE [master]
GO

/****** Object:  Database [FileDatabase]    Script Date: 1/1/2025 8:09:29 PM ******/
CREATE DATABASE [FileDatabase]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'FileDatabase', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\FileDatabase.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'FileDatabase_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\FileDatabase_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FileDatabase].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [FileDatabase] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [FileDatabase] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [FileDatabase] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [FileDatabase] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [FileDatabase] SET ARITHABORT OFF 
GO

ALTER DATABASE [FileDatabase] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [FileDatabase] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [FileDatabase] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [FileDatabase] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [FileDatabase] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [FileDatabase] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [FileDatabase] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [FileDatabase] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [FileDatabase] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [FileDatabase] SET  DISABLE_BROKER 
GO

ALTER DATABASE [FileDatabase] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [FileDatabase] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [FileDatabase] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [FileDatabase] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [FileDatabase] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [FileDatabase] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [FileDatabase] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [FileDatabase] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [FileDatabase] SET  MULTI_USER 
GO

ALTER DATABASE [FileDatabase] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [FileDatabase] SET DB_CHAINING OFF 
GO

ALTER DATABASE [FileDatabase] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [FileDatabase] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [FileDatabase] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [FileDatabase] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [FileDatabase] SET QUERY_STORE = ON
GO

ALTER DATABASE [FileDatabase] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO

ALTER DATABASE [FileDatabase] SET  READ_WRITE 
GO



USE [FileDatabase]
GO

/****** Object:  Table [dbo].[DiskInfo]    Script Date: 1/1/2025 8:08:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DiskInfo](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Caption] [nvarchar](255) NULL,
    [FirmwareRevision] [nvarchar](255) NULL,
    [Model] [nvarchar](255) NULL,
    [PNPDeviceID] [nvarchar](255) NULL,
    [SerialNumber] [nvarchar](255) NULL,
    [Signature] [nvarchar](255) NULL,       
    [Size] [nvarchar](255) NULL,            // !!! should be int type
    [TotalSectors] [nvarchar](255) NULL,            // !!! should be int type
 CONSTRAINT [PK_DiskInfo] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


#endif
