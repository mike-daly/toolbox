using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace file_info_to_db
{
    class Program
    {
        /// <summary>
        /// output a csv header for the file descriptions
        /// </summary>
        static void OutputFileHeader()
        {
            Console.WriteLine("filename, <volumename>, fullpath, length, sha256");
        }

        static string GetHash(FileInfo fi)
        {
            if (fi.Length > 1000000)
            {
                return $"reallyBigFile:{fi.Length}";
            }

            FileStream fs;
            try
            {
                fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (System.IO.IOException ex)
            {
                return $"no hash:  {ex.Message}";
            }

            SHA256 hashEngine = SHA256.Create();
            byte[] hash = hashEngine.ComputeHash(fs);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Output everything we want to know about the file.
        /// </summary>
        /// <param name="fi">FileInfo to describe.</param>
        /// <remarks>output is a csv line.  replace with database output later</remarks>
        static void OutputFile(FileInfo fi)
        {


            Console.WriteLine($"{fi.Name}, {"<VOLNAME>"}, {fi.FullName}, {fi.Length}, {GetHash(fi)}");
        }

        static void TraverseAndProcess(String directoryPath /*delegate*/)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            TraverseAndProcess(directoryInfo);
        }

        static void TraverseAndProcess(DirectoryInfo directoryInfo /*delegate*/)
        {
            foreach (FileInfo fi in directoryInfo.GetFiles())
            {
                OutputFile(fi);
            }

            foreach (DirectoryInfo di in directoryInfo.GetDirectories())
            {
                TraverseAndProcess(di);
            }
        }

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">arguments</param>
        static void Main(string[] args)
        {
            List<String> filteredArgs = new List<String>();
            foreach (string s in args)
            {
                if (s.Equals("-D"))
                {
                    System.Diagnostics.Debugger.Break();
                }
                else
                {
                    filteredArgs.Add(s);
                }
            }

            String scanRoot = (filteredArgs.Count < 1) ? "." : filteredArgs[0];
            Console.WriteLine($"scanning {scanRoot}");

            OutputFileHeader();
            TraverseAndProcess(scanRoot);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Debugger attached, press any key to exit session.");
                Console.ReadKey();
            }
        }

        static void DoHelp(String message, String[] args)
        {
            if (message.Length > 0)
            {
                Console.WriteLine(message);
            }

            Console.WriteLine($"{args[0]} <path to scan>");
            Console.WriteLine($"-D      break into debugger");
        }

    }
}
