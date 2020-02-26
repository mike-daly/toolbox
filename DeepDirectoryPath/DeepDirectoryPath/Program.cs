using System.IO;
using System;

namespace DeepDirectoryPath
{
    class Program
    {
        static void Main(string[] args)
        {
            int maxPathLength = 128;
            if (args.Length > 0)
            {
                int.TryParse(args[0], out maxPathLength);
            }
            Console.WriteLine($"traversing, looking for paths longer than {maxPathLength}");
            DirectoryInfo stem = new DirectoryInfo(".\\.");
            TraverseDeep(stem, maxPathLength);
        }

        static void TraverseDeep(DirectoryInfo start, int depth)
        {
            foreach (FileInfo file in start.GetFiles())
            {
                if (file.FullName.Length > depth)
                {
                    Console.Write($"FILE TOO DEEP ({file.FullName.Length:D3}):");
                }
                else
                {
                    Console.Write("              ");
                }

                Console.WriteLine($"{file.FullName}");
            }

            /*
            if (start.FullName.Length > depth)
            {
                Console.Write("DIR TOO DEEP ({start.FullName.Length:D3}):");
            }
            else
            {
                Console.Write("             ");
            }
            Console.WriteLine($"{start.FullName}");
            */

            foreach (DirectoryInfo child in start.EnumerateDirectories())
            {
                TraverseDeep(child, depth);
            }
        }

        private static void DoHelp(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine(message);
            }
            Console.WriteLine(@"
DeepDirectoryPath [maxDirectoryPathLength]

maxDirectoryPathLength  report all directory paths longer than this value (128 default)
");
        }
    }
}
