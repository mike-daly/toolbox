using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnNumbers
{
    class Program
    {
        static void Main(string[] args)
        {
            int rawWidth = Console.WindowWidth;
            if (rawWidth >= 100)
            {
                for (int i = 1; i * 100 < rawWidth; i++)
                {
                    Console.Write("{0,99}{1}", " ", i % 10);
                }
                Console.WriteLine();
            }
            if (rawWidth >= 10)
            {
                for (int i = 1; i * 10 < rawWidth; i++)
                {
                    Console.Write("{0,9}{1}", " ", i % 10);
                }
                Console.WriteLine();
            }
            for (int i = 1; i < rawWidth; i++)
            {
                Console.Write("{0}", i % 10);
            }
            Console.WriteLine();
        }
    }
}
