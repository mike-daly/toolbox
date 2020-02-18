namespace Mist
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Mist.Interface;
    using MistClient;

    class Program
    {
        private static string baseUri;
        private static IMistServer mistServer = null;

        /// <summary>
        /// root of the world.
        /// </summary>
        /// <param name="args">commandline</param>
        static void Main(string[] args)
        {
            ParseArgs(args);

            TestCreateListDelete();
        }

        /// <summary>
        /// simple test....
        /// </summary>
        private static void TestCreateListDelete()
        {
            try
            {
                ListBlobs();
                CreateAndSet();
                CreateAndSet();
                CreateAndSet();
                ListBlobs();
                DeleteRandomBlob();
                ListBlobs();
                ReadBlob();
                ReadBlobFail();
                ListBlobs();
                TestStringInOut();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught:");
                Console.WriteLine(ex);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Write a string, and get it back.
        /// </summary>
        private static void TestStringInOut()
        {
            Console.WriteLine("++TestStringInOut()++");
            Guid theId = Guid.NewGuid();

            Blob blob = new Blob();

            string message = "Hi Mom!";
            UTF8Encoding encoder = new UTF8Encoding();
            blob.data = encoder.GetBytes(message);
            blob.checksum = blob.ComputeChecksum();
            blob.id = theId;

            Blob createdBlob = Program.mistServer.Create(blob);

            Blob r = Program.mistServer.Get(theId.ToString());

            Console.WriteLine(String.Format("  got bytes *{0}* {1}", encoder.GetString(r.data), theId.ToString()));

        }

        /// <summary>
        /// create and put some data into a new blob.
        /// </summary>
        private static void CreateAndSet()
        {
            Console.WriteLine("++CreateAndSet()++");

            Random rnd = new Random();

            Blob blob = new Blob();
            blob.data = new byte[300];
            rnd.NextBytes(blob.data);
            blob.checksum = blob.ComputeChecksum();
            blob.id = Guid.NewGuid();

            Blob createdBlob = Program.mistServer.Create(blob);

            rnd.NextBytes(createdBlob.data);

            Program.mistServer.Update(createdBlob.id.ToString(), createdBlob);
        }

        /// <summary>
        /// Get and display the list of blobs in the service.
        /// </summary>
        private static void ListBlobs()
        {
            Console.WriteLine("++ListBlobs()++");

            List<Blob> blobs = Program.mistServer.GetCollection();

            if (blobs.Count == 0)
            {
                Console.WriteLine("  Empty");
            }

            foreach (Blob b in blobs)
            {
                Console.WriteLine("  " + b.ToString());
            }
            Console.WriteLine();
        }

        ///
        // Read some random blob.
        ///
        private static void ReadBlob()
        {
            Console.WriteLine("++ReadBlob()++");
            List<Blob> blobs = Program.mistServer.GetCollection();

            if (blobs.Count > 0)
            {
                int target = new Random().Next(blobs.Count - 1);
                string blobUri = Program.baseUri + '/' + blobs[target].id;
                Console.WriteLine(string.Format("  Get({0})", blobUri));

                Blob r = Program.mistServer.Get(blobs[target].id.ToString());
                Console.WriteLine("  " + r.ToString());
            }
            else
            {
                Console.WriteLine("  no blobs returned.");
            }
        }

        ///
        // Read some random blob.
        ///
        private static void ReadBlobFail()
        {
            Console.WriteLine("++ReadBlobFail()++");
            List<Blob> blobs = Program.mistServer.GetCollection();

            if (blobs.Count > 0)
            {
                int target = new Random().Next(blobs.Count - 1);
                string blobUri = Program.baseUri + '/' + blobs[target].id + "xx";
                try
                {
                    Blob r = Program.mistServer.Get(blobUri);
                    Console.WriteLine("  " + ((r == null) ? "NULL" : r.ToString()));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("  ReadBlobFail() caught Exception: {0}", ex.Message));
                }
            }
            else
            {
                Console.WriteLine("   no blobs returned.");
            }
        }

        /// <summary>
        /// list the blobs, delete one.
        /// </summary>
        private static void DeleteRandomBlob()
        {
            Console.WriteLine("++DeleteRandomBlob()++");
            List<Blob> blobs = Program.mistServer.GetCollection();
            if (blobs.Count < 1)
            {
                return;
            }

            int target = new Random().Next(blobs.Count - 1);

            Program.mistServer.Delete(blobs[target].id.ToString());

        }

        #region infrastructure and help text
        /// <summary>
        /// parse the command line, set data in global variables.
        /// </summary>
        /// <param name="args">Commandline args.</param>
        private static void ParseArgs(string[] args)
        {
            if (args.Length != 1)
            {
                DoHelp();
                Environment.Exit(1);
            }

            Program.baseUri = "http://" + args[0] + "/Mist";
            Program.mistServer = new MistHelper(Program.baseUri);
        }

        /// <summary>
        /// Display usage info.
        /// </summary>
        private static void DoHelp()
        {
            Console.WriteLine("MistClient server:port");
        }
        #endregion
    }
}
