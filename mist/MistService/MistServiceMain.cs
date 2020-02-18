
namespace Mist
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Web;

    class Program
    {
        static bool runAsCommandline = false;


        static MistClass mistService;
        static string baseUri;
        static string baseFilePath;
        static Mist.MistStorage mistStorage;
        static Dictionary<string, WebServiceHost> hosts;

        static void Main(string[] args)
        {
            Program.hosts = new Dictionary<string, WebServiceHost>();

            ParseArgs(args);

            RegisterEndpoints();

            if (Program.runAsCommandline)
            {
                Console.WriteLine(string.Format("running at port {0}", Program.baseUri));
                Console.WriteLine(string.Format("storing data at {0}", Program.baseFilePath));
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("service mode not implemented");
                Console.ReadLine();
            }
        }

        private static void RegisterEndpoints()
        {

            /*
            ServiceHost sh;
            ServiceEndpoint se;
            sh = new ServiceHost(typeof(MistClass));
            se = sh.AddServiceEndpoint(typeof(Mist.Interface.IMistServer), new WebHttpBinding(), Program.baseUri);
            se.Behaviors.Add(new WebHttpBehavior());

            //TODO real service, healthbeat, etc.
            //  http://localhost:5566/Mist/HelloWorld/himom

            sh = new ServiceHost(typeof(HelloWorld));
            se =  sh.AddServiceEndpoint(typeof(Mist.HelloWorld), new WebHttpBinding(), Program.baseUri);
            se.Behaviors.Add(new WebHttpBehavior());

            sh.Open();
            */

            WebServiceHost wsh;

            wsh = new WebServiceHost(typeof(Mist.HelloWorld), new Uri[] { new System.Uri(Program.baseUri + "HelloWorld") });
            wsh.Open();
            Program.hosts.Add("HelloWorld", wsh);

            wsh = new WebServiceHost(typeof(Mist.Healthbeat), new Uri[] { new System.Uri(Program.baseUri + "Healthbeat") });
            wsh.Open();
            Program.hosts.Add("HelloWorld", wsh);

            wsh = new WebServiceHost(Program.mistService, new Uri[] { new System.Uri(Program.baseUri + "Mist") });
            wsh.Open();
            Program.hosts.Add("Mist", wsh);

            Console.WriteLine(string.Format("{0} endpoint registered", Program.baseUri));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private static void ParseArgs(string[] args)
        {
            Program.runAsCommandline = true;

            if (args.Length != 4)
            {
                DoHelp();
                Environment.Exit(1);
            }

            int i = 0;
            while (i < args.Length)
            {
                switch (args[i++].ToLower())
                {
                    case "-p":
                        {
                            Program.baseUri = string.Format("http://localhost:{0}/", args[i++]);
                            break;
                        }
                    case "-d":
                        {
                            Program.baseFilePath = args[i++];
                            Program.mistStorage = new MistStorage(baseFilePath, 10);
                            break;
                        }
                    default:
                        {
                            DoHelp();
                            Environment.Exit(1);
                            break;
                        }
                }
                Program.mistService = new MistClass(Program.mistStorage);
            }
        }

        /// <summary>
        /// Display usage info.
        /// </summary>
        private static void DoHelp()
        {
            Console.WriteLine("mistService -p port -d diskpath");
            Console.WriteLine("  -p is the port to listen on.");
            Console.WriteLine("  -d is the disk location for data.");
        }
    }
}