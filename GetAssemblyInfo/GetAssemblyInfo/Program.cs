using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GetAssemblyInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            string filepath = args[0];
            Assembly a = Assembly.ReflectionOnlyLoadFrom(filepath);
            Console.WriteLine("ImageRuntimeVersion:  {0}", a.ImageRuntimeVersion);
            IEnumerable<Module> modules = a.Modules;
            int i = 0;
            foreach (Module mod in modules)
            {
                Console.WriteLine("Modules[{0}].Assembly.FullName:  {1}", i++, mod.Assembly.FullName);
            }

            IEnumerator en = a.Evidence.GetAssemblyEnumerator();
            int c = 0;
            while (en.MoveNext())
                Console.WriteLine("Assembly Evidence[{0}]:  {1}", c++, en.Current); 

            en = a.Evidence.GetHostEnumerator();
            c = 0;
            while (en.MoveNext())
                Console.WriteLine("Host Evidence[{0}]:  {1}", c++, en.Current); 
        }
    }
}
