
namespace Mist
{
    using System;
    using System.IO;

    public class MistStorage
    {
        public string storageRoot
        {
            get;
            private set;
        }

        public int maxStorageInGB
        {
            get;
            private set;
        }

        public MistStorage(string storageRoot, int maxStorageInGB)
        {
            this.storageRoot = storageRoot;
            this.maxStorageInGB = maxStorageInGB;

            if (!Directory.Exists(this.storageRoot))
            {
                Directory.CreateDirectory(this.storageRoot);
            }

            // todo:  validate there is enough space left (available to user + consumed in this directory)
        }

        public int[] ListBlobVersions(string blobName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(this.storageRoot);
            FileSystemInfo[] fileInfo = directoryInfo.GetFileSystemInfos(blobName + "*");
            int[] returnVersions = new int[fileInfo.Length];

            string[] splitstrings = new string[] { string.Concat(blobName, "_") };
            int i = 0;
            foreach (FileSystemInfo info in fileInfo)
            {
                returnVersions[i++] = Convert.ToInt32(info.Name.Split(
                    splitstrings,
                    StringSplitOptions.RemoveEmptyEntries)[0]);
            }
            return returnVersions;
        }

        public void Write(Guid blobName, int version, byte[] buffer)
        {
            string blobNamestring = blobName.ToString();
            string targetFilename = this.storageRoot + '\\' + blobNamestring + '_' + version.ToString();
            if (File.Exists(targetFilename))
            {
                Console.WriteLine(string.Format("File {0} with version {1} exists.", blobNamestring, version));
                // throw new ArgumentException(string.Format("File {0} with version {1} exists.", blobName, version));
            }

            using (FileStream fs = File.Create(targetFilename))
            {
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
            return;
        }

        public byte[] Read(string blobName, int version)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(this.storageRoot);
            FileSystemInfo[] fileInfo = directoryInfo.GetFileSystemInfos(blobName + "*");

            if (version == 0) // get the latest version of the blob available
            {
                int[] versions = this.ListBlobVersions(blobName);

                foreach (int v in versions)
                {
                    version = (version > v) ? version : v;
                }
            }

            string[] concat = new string[] {
                this.storageRoot, 
                "\\", 
                blobName, 
                "_",
                version.ToString()
            };

            string fileName = string.Concat(concat);
            byte[] returnBytes;

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                returnBytes = new byte[fs.Length];
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(returnBytes, 0, (int)fs.Length);
            }
            return returnBytes;
        }

        public string[] ListContents()
        {
            string[] returnBlobNames;

            DirectoryInfo directoryInfo = new DirectoryInfo(this.storageRoot);
            FileSystemInfo[] fileInfo = directoryInfo.GetFileSystemInfos();
            returnBlobNames = new string[fileInfo.Length];
            int i = 0;
            foreach (FileSystemInfo info in fileInfo)
            {
                returnBlobNames[i++] = info.Name;
            }
            return returnBlobNames;
        }
    }
}
