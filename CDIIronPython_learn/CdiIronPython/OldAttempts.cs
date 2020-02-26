using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;

namespace host_ironpython
{
    public class Application
    {
        public string Name { get { return "MyApp"; } }
    }

    public class SourceData
    {
        public string GetNextData { get { return DateTime.UtcNow.ToLongTimeString(); } }
    }
    public class DestinationData
    {
        public string PutData { set; get; }
    }

    public class DoWorkClass
    {
        public int DoWork()
        {
            return new Random().Next();
        }
    }

    class OldAttempts
    {

        /*
        struct s1
        {
            public string f1;
            public int f2;
        }
        */

        static void try1(string[] args)
        {
            string code =
    @"
outputdata.PutData = inputdata.GetNextData; 
print inputdata.GetNextData;
print worker.DoWork();

def function1(val):
    print (val)

def function2(val1, val2):
    print (val1, val2)
";

            ScriptEngine engine = IronPython.Hosting.Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();

            scope.SetVariable("application", new Application());
            scope.SetVariable("inputdata", new SourceData());

            DestinationData dd = new DestinationData();
            scope.SetVariable("outputdata", dd);

            scope.SetVariable("worker", new DoWorkClass());

            //ScriptSource source = engine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
            dynamic source = engine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);

            CompiledCode compiled = source.Compile();

            ScriptCodeParseResult codeProps = source.GetCodeProperties();

            Console.WriteLine("codeProps:  {0}", codeProps);

            source.Execute(scope);
            Console.WriteLine("got it -- {0}", dd.PutData);
            Console.WriteLine("-----------");

            source.Function1("function1");
            source.Function2("function2", "parameter2");

            Console.ReadKey();
        }

        static void try2(string[] args)
        {
            ScriptRuntime ironPythonRuntime = Python.CreateRuntime();
            ScriptEngine engine = IronPython.Hosting.Python.CreateEngine();
            ScriptScope engineModule = Python.CreateModule(engine, "testmodule", "try2.py");

            try
            {
                dynamic loadedPython = ironPythonRuntime.UseFile("try2.py");
                loadedPython.function0();

                loadedPython.ifn0();
                Console.WriteLine(loadedPython.ifn0.__doc__);

                Console.WriteLine(loadedPython.class1.__doc__);
                ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }
    }
}
