using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;

namespace host_ironpython
{
    public class MockEHEvent
    {
        public DateTime dateTime;
        public DateTime now { get { return DateTime.UtcNow; } }
        public string message;

        public MockEHEvent()
        {
            this.dateTime = DateTime.UtcNow;
            this.message = Guid.NewGuid().ToString();
        }
    }

    public class WatermarkData
    {
        public WatermarkData()
        {
        }

        public string endpoint { get { return "wm_endpoint"; } }

        public string offset { get { return new Random().Next(0, 100).ToString(); } }

        public override string ToString()
        {
            return string.Format("{0} -- {1}", this.endpoint, this, offset);
        }
    }

    public class WatermarkProvider
    {
        public static void UpdateWatermark(WatermarkData data)
        {
            Console.WriteLine("WatermarkProvider::UpdateWatermark({0} -- {1})", data.endpoint, data.offset);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //try1(args);
            //try2(args);
            //try3(args);
            testDelegate(args);
            //Console.ReadKey();
            try5(args);
            Console.ReadKey();
        }

        public delegate void WatermarkDelegate();
        public delegate void WatermarkDelegateWithVal(string s);
        public static void testDelegate(string[] args)
        {
            for (int i = 0; i < 5; i++)
            {
                workerFunction( delegate() { Console.WriteLine("delegate {0}", i); } );
                workerFunction(delegate (string s) { Console.WriteLine("w val:  {0}, {1}", s, i); } );
            }
        }

        static WatermarkDelegate previousWM;
        public static void workerFunction(WatermarkDelegateWithVal d)
        {
            Console.Write("with val:  ");
            d("the val");
        }
        public static void workerFunction(WatermarkDelegate d)
        {
            if (previousWM != null)
            {
                previousWM();
            }
            else
            {
                Console.WriteLine("no previous WM to invoke");
            }
            previousWM = d;
        }

        public static void try5(string[] args)
        {
            ScriptEngine pyEngine = Python.CreateEngine();
            dynamic pyScope = pyEngine.CreateScope();

            ScriptSource source = pyEngine.CreateScriptSourceFromFile("CdiClient.py");
            source.Compile().Execute(pyScope);
            ObjectOperations ops = pyEngine.CreateOperations(pyScope);

            // create member objects for worker object:  watermark provider, tracer, logger
            WatermarkProvider watermarkProvider = new WatermarkProvider();
            pyScope.SetVariable("watermarkProvider", watermarkProvider);

            dynamic cdiWorker = pyEngine.Execute("CdiClient()", pyScope);   // create an instance of the worker obj

            // TODO -- check that all of the right entrypoints are available.

            Console.WriteLine("static function doc: {0}", pyScope.SampleStaticFunction.__doc__);
            Console.WriteLine("indirect class doc: {0}", pyScope.CdiClient.__doc__);
            Console.WriteLine("direct class doc: {0}", cdiWorker.__doc__);
            Console.WriteLine("class function doc: {0}", cdiWorker.ProcessOneEvent.__doc__);

            //CheckForEntryPoint();
            Console.WriteLine("NoOp:  {0}", ops.ContainsMember(cdiWorker, "NoOp"));
            Console.WriteLine("FooBar:  {0}", ops.ContainsMember(cdiWorker, "FooBar"));

            IList<string> memberNames = ops.GetMemberNames(cdiWorker);
            foreach (string s in memberNames)
            {
                Console.WriteLine("    names:  {0}", s);
            }

            IList<string> signatures = ops.GetCallSignatures(cdiWorker);
            foreach (string s in signatures)
            {
                Console.WriteLine("    sig:  {0}", s);
            }


            dynamic result;
            result = ops.InvokeMember(cdiWorker, "NoOp", new object[] { "hi mom" });
            result = ops.InvokeMember(cdiWorker, "NoOp", new object[] { "hi dad" });
            result = ops.InvokeMember(cdiWorker, "NoOp", new object[] { "watch this" });

            for (int i = 0; i < 10; i++)
            {
                MockEHEvent ehEvent = new MockEHEvent();
                WatermarkData wmData = new WatermarkData();
                System.Threading.Thread.Sleep(args.Length > 0 ? int.Parse(args[0]) : 100);
                result = ops.InvokeMember(cdiWorker, "ProcessOneEvent", new object[] { ehEvent, wmData });
            }

            WatermarkDelegate del = delegate () { Console.WriteLine("delegate"); };
            result = ops.InvokeMember(cdiWorker, "ProcessDelegate", new object[] { del });

            WatermarkDelegateWithVal delVal = delegate (string s) { Console.WriteLine("delegate {0}", s); };
            result = ops.InvokeMember(cdiWorker, "ProcessDelegateWithVal", new object[] { delVal });

        }
    }
}