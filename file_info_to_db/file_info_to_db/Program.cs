using System;

namespace file_info_to_db
{
    class Program
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">arguments</param>
        static void Main(string[] args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
            Console.WriteLine("Hello World!");

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Debugger attached, press any key to exit session.");
                Console.ReadKey();
            }
        }

        static void DoHelp(String message)
        {
            if (message.Length >0)
            {
                Console.WriteLine(message);
            }

            Console.WriteLine(@"
")
        }

    }
}
