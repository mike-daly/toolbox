// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace depend
{
    using System;
    using System.Collections.Generic;
    using System.Net.NetworkInformation;
    using System.Reflection;

    /// <summary>
    /// The program.
    /// </summary>
    class Depend
    {
        /// <summary>
        /// The already found.
        /// </summary>
        static Dictionary<string, int> alreadyFound = new Dictionary<string, int>();

        /// <summary>
        /// The indent level.
        /// </summary>
        static int indentLevel = 0;

        private static int maxDepthReached = 0;
    
        /// <summary>
        /// The max depth.
        /// </summary>
        private static int maxDepth = 2;

        /// <summary>
        /// The not loaded.
        /// </summary>
        static Dictionary<string, string> notLoaded = new Dictionary<string, string>();

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        static int Main(string[] args)
        {
            try
            {
                if (args.Length == 0 || args.Length > 2)
                {
                    WriteLine($"usage: Depend.exe AssemblyFileName [maxdepth]");
                    WriteLine(
                        $"This is a very stupid dependency walker that assumes all the file are either loadable via load assembly or in the current path.");
                    return 0;
                }

                string fileName = args[0]; // @"WhoisUploader.exe";

                if (args.Length == 2)
                {
                    if (!int.TryParse(args[1], out maxDepth) || maxDepth < 1 || maxDepth > 100)
                    {
                        WriteLine($"Error parsing MaxDepth : {args[1]}. Thie must be a positive integer value between 1 and 100 inclusive");
                        return 0;
                    }
                }

                alreadyFound.Clear();
                notLoaded.Clear();

                Assembly assembly = TryLoad(fileName);
                maxDepthReached = 0;

                RecurseGetReferencedAssemblies(assembly);
                Console.WriteLine($"Max Depth Reached : {maxDepthReached}");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error walking tree : {exception.Message}");
                return 0;
            }

            if (notLoaded.Count == 0)
            {
                return 1;
            }
            else
            {
                WriteLine(string.Empty);
                WriteLine("Assemblies which were not found or loaded : ");
                foreach (string name in notLoaded.Values)
                {
                    WriteLine($"{name}");
                }

                return -notLoaded.Count;
            }
        }

        /// <summary>
        /// The recurse get referenced assemblies.
        /// </summary>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        static void RecurseGetReferencedAssemblies(Assembly assembly)
        {
            if (maxDepthReached < indentLevel)
            {
                maxDepthReached = indentLevel;
            }

            if (assembly != null)
            {
                int count = alreadyFound.ContainsKey(assembly.FullName) ? alreadyFound[assembly.FullName] : 0;
                count++;
                alreadyFound[assembly.FullName] = count;

                if (count > 1 || indentLevel >= maxDepth)
                {
                    WriteLine($"{assembly.FullName} - Not Recursing");
                    WriteLine($"--{assembly.ImageRuntimeVersion} {assembly.Location}");
                }
                else 
                {
                    WriteLine($"{assembly.FullName}");
                    WriteLine($"--{assembly.ImageRuntimeVersion} {assembly.Location}");

                    if (assembly != null)
                    {
                        var refs = assembly.GetReferencedAssemblies();
                        foreach (var reference in refs)
                        {
                            Assembly childAssembly = null;
                            try
                            {
                                childAssembly = TryLoad(reference.FullName);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    childAssembly = TryLoad(reference.Name);
                                }
                                catch (Exception)
                                {
                                    // try with partial name
                                    WriteLine($" *** Could not load {reference.FullName} ***");
                                    notLoaded[reference.FullName] = reference.FullName;
                                }
                            }

                            indentLevel++;
                            RecurseGetReferencedAssemblies(childAssembly);
                            indentLevel--;
                        }
                    }
                }

                alreadyFound[assembly.FullName]--;
            }
            else
            {
                WriteLine("Null Assembly");
            }
        }

        /// <summary>
        /// The try load.
        /// </summary>
        /// <param name="assemblyName">
        /// The assembly name.
        /// </param>
        /// <returns>
        /// The <see cref="Assembly"/>.
        /// </returns>
        static Assembly TryLoad(string assemblyName)
        {
            Assembly assembly = null;
            try
            {
                // Try with the name.
                assembly = Assembly.ReflectionOnlyLoad(assemblyName);
            }
            catch (Exception exception)
            {
                try
                {
                    assembly = Assembly.ReflectionOnlyLoadFrom(assemblyName);
                }
                catch (Exception)
                {
                    try
                    {
                        assembly = Assembly.ReflectionOnlyLoadFrom(assemblyName + ".dll");
                    }
                    catch (Exception)
                    {
                        try
                        {
                            assembly = Assembly.ReflectionOnlyLoadFrom(assemblyName + ".exe");
                        }
                        catch (Exception)
                        {
                            try
                            {
                                assembly = Assembly.LoadWithPartialName(assemblyName);
                                if (assembly == null)
                                {
                                    throw;
                                }
                            }
                            catch { throw;}
                        }
                    }
                }
            }

            return assembly;
        }

        /// <summary>
        /// The write line.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        static void WriteLine(string text)
        {
            Console.WriteLine($"{string.Empty.PadLeft(indentLevel * 4)} {text}");
        }
    }
}