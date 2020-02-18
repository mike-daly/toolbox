using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace find_nonascii
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string filename in args)
            {
                int linenumber = 0;
                try
                {
                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        using (TextReader tx = new StreamReader(fs))
                        {
                            string line = tx.ReadLine();
                            while (!string.IsNullOrWhiteSpace(line))
                            {
                                string newline = Regex.Replace(line, @"[^\u0001-\u007f]", string.Empty);
                                if (0!=newline.CompareTo(line))
                                {
                                    Console.WriteLine("{0}:{1} {2}", filename, linenumber, line);
                                }
                                linenumber++;
                                line = tx.ReadLine();
                            } 
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failure processing file {0}  Exception:  {1}  Continuing.", filename, ex.Message);
                }
            }
        }
    }
}
