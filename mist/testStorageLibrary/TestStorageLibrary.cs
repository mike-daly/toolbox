
namespace testStorageLibrary
{
    using System;
    using Mist;

    class TestStorageLibrary
    {
        static internal MistStorage store;

        static void Main(string[] args)
        {
            TestStorageLibrary.store = new MistStorage("d:\\Mist", 1000);
            Guid blobName;

            /*
            WriteABlob("abcd", 1);
            DumpVersions("abcd");
            WriteABlob("abcd", 2);
            DumpVersions("abcd");
            WriteABlob("abcd", 2);
            DumpVersions("abcd");
            */


            for (int i = 0; i < 10; i++)
            {
                blobName = Guid.NewGuid();
                ListContents();
                WriteABlob(blobName, 1);

                if (i % 7 == 0)
                {
                    WriteABlob(blobName, 2);
                }

            }
            ListContents();
            blobName = Guid.NewGuid();
            WriteReadABlob(blobName, 1);
            WriteReadABlob(blobName, 2);
            ReadABlob(blobName, 1);
            Console.ReadLine();
        }

        static internal void WriteReadABlob(Guid blobBaseName, int version)
        {
            Console.WriteLine("++WWriteReadABlob()");
            WriteABlob(blobBaseName, version);
            ReadABlob(blobBaseName, version);
            Console.WriteLine();
        }


        static internal void ReadABlob(Guid blobBaseName, int version)
        {
            Console.WriteLine(string.Format(" +ReadABlob({0}, {1})", blobBaseName.ToString(), version));
            byte[] blobData = TestStorageLibrary.store.Read(blobBaseName.ToString(), version);
            for (int i = 0; i < blobData.Length; i++)
            {
                Console.Write(blobData[i].ToString() + " ");
            }
            Console.WriteLine();

        }

        static internal void WriteABlob(Guid blobBaseName, int version)
        {
            byte[] randomBytes = new byte[100];

            new Random().NextBytes(randomBytes);
            TestStorageLibrary.store.Write(blobBaseName, version, randomBytes);
        }

        static internal void ListContents()
        {
            Console.WriteLine("++ListContents++");
            string[] contents = TestStorageLibrary.store.ListContents();
            foreach (string s in contents)
            {
                Console.WriteLine("  " + s);
            }
        }

        static internal void DumpVersions(string blobBaseName)
        {
            Console.WriteLine(string.Format("++Versions({0})++", blobBaseName));
            int[] versions = TestStorageLibrary.store.ListBlobVersions(blobBaseName);
            foreach (int i in versions)
            {
                Console.WriteLine("  " + i.ToString());
            }
        }

    }
}
