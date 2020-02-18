using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ListToLine
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();

            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.Equals("-h"))
                    {
                        DoHelp();
                        return;
                    }
                    else
                    {
                        sb.AppendFormat("{0} ", arg);
                    }
                }
            }


            int count = 0;
            int max = 20000;

            string s;
            do
            {
                s = Console.In.ReadLine();
                if (s == null)
                {
                    break;
                }
                count += s.Length;
                sb.AppendFormat("{0} ", s);


            } while (count < max);

            Console.WriteLine(sb.ToString());
        }

        private static void DoHelp()
        {
            Console.WriteLine(
@"
listtoline [p1] [p2] [...]
    [-h]    help (this)

ListToLine will read the lines provided in standard input, 
and concattenate these lines as a single line with blank
spaces between each line.

Any parameters (p1, p2, etc) passed to the command are
emmited first.

Example use: 
    dir /b *.cs | ListToLine gvim | cmd

This wil find all of the .cs files, and put all of the names on 
a single line, prepended with the string 'gvim' and then pass
the whole mess to the cmd processor for execution (or some other tool).");

        }
    }
}
