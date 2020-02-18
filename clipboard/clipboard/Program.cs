using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clipboard
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            DataObject o = (DataObject)Clipboard.GetDataObject();

            if (o != null)
            {
                string s = (string)o.GetData(DataFormats.Text);
                if (!string.IsNullOrEmpty(s)) Console.Write(s);
            }
            else 
                Console.WriteLine("no object from the clipboard");
            */
            if (Clipboard.ContainsText())
                Console.Write(Clipboard.GetText());
            else
                Console.WriteLine("no object from the clipboard");
        }
    }
}
