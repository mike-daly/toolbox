using System;

namespace chkfile
{
    public static class Helpers
    {
        public static string FormatBytes(object bytes)
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

        public static string GetDriveType(object driveType)
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
}