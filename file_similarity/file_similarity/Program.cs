using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace file_similarity
{
    class Program
    {
        static void Main(string[] args)
        {


            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.Write("press any key to exit");
                Console.ReadKey();
            }
        }

        private static void DoHelp(string s)
        {
            Console.WriteLine(@"
{0}

-help       this
-debugger   start the debugger
", s);

        }
    }
}
