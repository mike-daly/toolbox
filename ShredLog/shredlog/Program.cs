using System;
using System.Collections.Generic;
using System.IO;

namespace shredlog
{
    class Program
    {
        static Dictionary<string, TextWriter> outputList = new Dictionary<string, TextWriter>();

        static void Main(string[] args)
        {
            Program.outputList = new Dictionary<string, TextWriter>();

            string logPath = string.Empty;
            string targetedThread = string.Empty;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-help":
                        {
                            DoHelp(string.Empty);
                            break;
                        }
                    case "-thread":
                        {

                            if (i + 1 < args.Length)
                            {
                                targetedThread = args[++i].Replace(':', '_');
                            }
                            else
                            {
                                DoHelp("-thread requires a thread id parameter");
                            }
                            break;
                        }
                    default:
                        {
                            if (!string.IsNullOrWhiteSpace(logPath))
                            {
                                DoHelp($"only one log file name may be processed.  Already have {logPath}, now processing {args[i]}");
                                return;
                            }
                            logPath = args[i];
                            break;
                        }
                }
            }

            if (string.IsNullOrWhiteSpace(logPath))
            {
                DoHelp("missing name of log file to process");
                return;
            }

            TextReader textReader;

            try
            {
                Stream inStream = new FileStream(logPath, FileMode.Open, FileAccess.Read);
                textReader = new StreamReader(inStream);
            }
            catch (Exception ex)
            {
                DoHelp(string.Format($"Unable to open file {logPath}.  Exception:  {ex}"));
                return;
            }

            string header = textReader.ReadLine();
            if (!header.StartsWith("Build started"))
            {
                Console.WriteLine($"{logPath} does not appear to be a build log.  It does not start with 'Build started *date time*'");
                return;
            }

            //TODO: integration build log files start with ~23 characters of date/time on every line.
            //TODO: integration build log files don't start with "Build started...." but have a preamble.  

            int lineNumber = 2;     // line 1 is "Build started ....".
            const string emptyThreadToken = "      ";
            string currentThread = string.Empty;
            string line;
            TextWriter currentTextWriter = null;
            while ((line = textReader.ReadLine()) != null)
            {
                if (line.StartsWith("Build"))
                {
                    // indicates the end of all of the building.
                    break;
                }

                // determine which thread this line goes to.  Either the same as the last line, or a new one.
                // lines are in the form "    NNN>sssssss" where NNN is a thread id in form NNN or NNN:NN
                // if the line is attached to the same thread as the previous line, the "NNN>" is replaced with blanks
                string thread;
                int index = line.IndexOf('>');
                if (index > 0)
                {
                    thread = line.Substring(0, line.IndexOf('>'));
                }
                else
                {
                    thread = emptyThreadToken;
                }

                // handle the case where there is a '>' character, but it is not a thread id splitter
                if (thread.Length > 12)
                {
                    thread = emptyThreadToken;
                }

                // handle the case where the thread id is in the form NNN:NN, turn it into NNN_NN
                thread = thread.Replace(':', '_');

                // handle the case where there is something in the front of line, but it is not a thread id
                if (thread.Trim(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ', '_' }).Length > 0)
                {
                    Console.WriteLine($"{lineNumber} odd line({index}):  {line}");
                    thread = emptyThreadToken;
                }

                thread = thread.Trim();

                if (!thread.Equals(string.Empty) &&     // if it is blank, stay in the same thread (output buffer)
                    !currentThread.Equals(thread))         // if it is a different thread
                {
                    if (currentTextWriter != null)
                    {
                        currentTextWriter.Flush();
                    }
 
                    // if there is no targeted thread, or we are on the "target" then create.
                    if (string.IsNullOrWhiteSpace(targetedThread) ||
                        (!string.IsNullOrWhiteSpace(targetedThread) && thread.Equals(targetedThread)))
                    {
                        // upsert a new text writer (and new output file) if we have not seen this thread before.
                        if (!Program.outputList.ContainsKey(thread))
                        {
                            // todo remove existing one if any.
                            string filename = string.Format($"{logPath}.{thread}");

                            TextWriter writer = new StreamWriter(filename);
                            writer.WriteLine($"1:{header}");
                            Program.outputList.Add(thread, writer);
                        }
                        currentTextWriter = Program.outputList[thread];
                    }
                    else
                    {
                        // this will cause a crash if we have a bug elsewhere 
                        // selecting "current thread" and "targetThread" incorrectly.
                        currentTextWriter = null;
                    }
                    currentThread = thread;
                }

                if (string.IsNullOrWhiteSpace(targetedThread))
                {
                    currentTextWriter.WriteLine($"{lineNumber++}:{line}");
                }
                else
                {
                    if (currentThread.Equals(targetedThread))
                    {
                        currentTextWriter.WriteLine($"{lineNumber++}:{line}");
                    }
                    else
                    {
                        lineNumber++;
                    }
                }
            }

            // take all remianing contents of the file and put it in "summary" file
            if (!string.IsNullOrWhiteSpace(line) && line.StartsWith("Build"))
            {
                TextWriter summaryWriter = new StreamWriter(string.Format($"{logPath}.summary"));
                summaryWriter.WriteLine($"0:{header}");
                summaryWriter.WriteLine($"{lineNumber++}:{line}");
                while ((line = textReader.ReadLine()) != null)
                {
                    summaryWriter.WriteLine($"{lineNumber++}:{line}");
                }
                summaryWriter.Flush();
            }

            // clear all output buffers before we exit.
            foreach (TextWriter writer in Program.outputList.Values)
            {
                writer.Flush();
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.Write("press any key to exit");
                Console.ReadKey();
            }
        }

        public static void DoHelp(string errorMessage)
        {
            Console.WriteLine($@"
{errorMessage}

shredlog [-thread threadId] [-help] logfilename

This tool takes the log file (e.g. msbuild.log), and 
puts the output for each build thread in a different
file so you can read it.  The outpufiles are named
in the form logfilename.0, logfilename.1 for each
of the different build threads.

Each line in the output file is in the format:
    <sourcelinenumber>:<sourceline>

    -thread threadId    -- extract rows for this thread only
    -help               -- this");

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.Write("press any key");
                Console.ReadKey();
            }
        }
    }
}
