using System;
using System.Windows;

namespace echoclip
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (System.Windows.Clipboard.ContainsText())
            {
                string s = System.Windows.Clipboard.GetText();
                Console.WriteLine(s);
            }
            else
            {
                Console.Error.WriteLine("no text in clipboard");
            }
        }
    }
}
