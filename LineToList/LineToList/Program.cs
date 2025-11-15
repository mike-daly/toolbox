using System;
using System.Collections.Generic;
using System.Text;

namespace LineToList
{
    /// <summary>
    /// LineToList splits strings on the command line with delimiter char and
    /// turns the delimited string to a list of strings.  
    /// </summary>
    class Program
    {
        /// <summary>
        /// default program entry
        /// </summary>
        /// <param name="args">see DoHelp() for semantics</param>
        static void Main(string[] args)
        {
            string delimeter = " ";
            List<String> sb = new List<string>();

            StringBuilder filteredInput = new StringBuilder();

            //two pass.  First handle -h and -d DD
            if (args.Length > 0)
            {
                //foreach (string arg in args)
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Equals("-h"))
                    {
                        DoHelp(null);
                        return;
                    }
                    else if (args[i].Equals("-d"))
                    {
                        i++;
                        if (i < args.Length)
                        {
                            delimeter = args[i];
                        }
                        else
                        {
                            DoHelp("-d requires a delimiter argument");
                            return;
                        }
                    }
                    else
                    {
                        filteredInput.Append($" {args[i]}");
                    }
                }
            }

            // Console.WriteLine($"*{delimeter}*");
            // Console.WriteLine(filteredInput.ToString());

            // int tokenCount = 0;
            foreach (string s in filteredInput.ToString().
                Split(new string[] { delimeter }, StringSplitOptions.RemoveEmptyEntries))
            {
                //Console.WriteLine($"{tokenCount++}: {s}");
                Console.WriteLine($"{s}");
            }
        }


        /// <summary>
        /// Display help text, with optional error message.
        /// </summary>
        /// <param name="errorMessage">errorm message if any</param>
        private static void DoHelp(string errorMessage)
        {
            if (!String.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine();
                Console.WriteLine(errorMessage);
            }

            Console.WriteLine(
            @"
linetolist [-d 'd'] [s] 
    [-h]    help (this)

LineToList will split the string 's' on delimiter 'd'
with each substring emmitted to sdtout

The default delimiter is a single blank character.

Example: 
    linetolist -d ; %path%

will output each of the tokens in the path variable, one line
at a time.
");

        }
    }
}
