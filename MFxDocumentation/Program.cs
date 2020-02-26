// -------------------------------------------------------------------------
//  <copyright file="Program.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------- 
namespace MFxDocumentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Visio = Microsoft.Office.Interop.Visio;

    /// <summary>
    /// This program takes a dll or exe, finds all of the classes in it, and
    /// uses Visio to display the information as a tree.
    /// </summary>
    public class Program
    {
        /// <summary> BoxPichX is the initial spacing (inches) in the x direction of all objects.</summary>
        private const double BoxPitchX = 2.0;

        /// <summary> BoxX is the size (inches) in the x direction of all objects.</summary>
        private const double BoxX = 1.75;

        /// <summary> BoxPichY is the initial spacing (inches) in the y direction of all objects.</summary>
        private const double BoxPitchY = 0.5;

        /// <summary> BoxY is the size (inches) in the y direction of all objects.</summary>
        private const double BoxY = 0.375;

        /// <summary>Filter namespace for display.</summary>
        private static string searchNamespace = null;

        /// <summary>Name of the assembly to reflect and render.</summary>
        private static string assemblyName = null;

        /// <summary>Assembly to reflect and render.</summary>
        private static Assembly asm = null;

        /// <summary>Accumulates the rendered tree to support layout and grouping
        /// of just the tree.</summary>
        private static Visio.Selection selectionTree = null;

        /// <summary>
        /// Main entry point for the program
        /// </summary>
        /// <param name="args">First parameter is the object to search, second is
        /// the namespace to restrict the search (may be null).</param>
        public static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Usage();
            }

            Program.assemblyName = args[0];

            if (args.Length > 1)
            {
                Program.searchNamespace = args[1];
            }

            IEnumerable<Type> implementedTypesList = GetAllClasses(Program.assemblyName, Program.searchNamespace);

            Dictionary<string, string> namespaceColorMap = BuildColorMap(implementedTypesList);

            TypeTreeNode root = ConvertToTree(implementedTypesList);

            // TODO:  if (verbose):
            Console.WriteLine("node\tDepth\tDirectChildren\tTotalChildren\tfullName");
            ConsoleOutTree(root, string.Empty);
            Console.WriteLine(string.Format(
                "Class count:  {0}   Namespace count:  {1}",
                root.TotalChildren,
                namespaceColorMap.Keys.Count));

            BuildAndDisplayWithVisio(namespaceColorMap, root);
        }

        /// <summary>
        /// Given a list of types, turn them into a color map (Name=>rgb settings).
        /// </summary>
        /// <param name="typesList">List of Type objects to map.</param>
        /// <returns>Returns a dictionary of Name:Value pairs where Name is a type Name, and
        /// value is a RGB string for Visio consumption.</returns>
        private static Dictionary<string, string> BuildColorMap(IEnumerable<Type> typesList)
        {
            // Two pass, first get all of the names, sort them, and then assign colors.
            Dictionary<string, string> returnList = new Dictionary<string, string>();
            foreach (Type t in typesList.OrderBy(t => t.Namespace))
            {
                if (!String.IsNullOrEmpty(t.Namespace) && !returnList.ContainsKey(t.Namespace))
                {
                    returnList.Add(t.Namespace, "RGB(127, 127, 127)");
                }
            }

            // the color space size for each color is the cube root of the number of entries, rounded up.
            // this would be simplest, but cube goes 1, 8, 27 and a color table size of 10 will not get many
            // colors since it only uses part of n^3 (27).  So, we will just expand each of the three color 
            // channels until we have enough room:
            int neededColors = returnList.Count;

            int red = 1;
            int green = 1;
            int blue = 1;

            // factor the needed colors to find the three greatest common divisors "close to the cube root"
            red = (int)Math.Round(Math.Pow(neededColors, 1 / 3) + 0.5);
            green = (int)Math.Round(Math.Pow(neededColors, 1 / 2) + 0.5);
            blue = (int)Math.Round(((neededColors / red) / green) + 0.5) + 1;

            Console.WriteLine("RGB steps:  {0}, {1}, {2}", red, green, blue);

            string[] colors = new string[red * green * blue];

            // start with some signal in each RGB channel, and then add additional color
            // in steps of size ColorIncrement (available space / steps)
            const int ColorBase = 192;
            int redIncrement = (255 - ColorBase) / red;
            int greenIncrement = (255 - ColorBase) / green;
            int blueIncrement = (255 - ColorBase) / blue;

            for (int g = 0; g < green; g++)
            {
                for (int r = 0; r < red; r++)
                {
                    for (int b = 0; b < blue; b++)
                    {
                        colors[(r * green * blue) + (g * blue) + b] =
                            string.Format(
                            "RGB({0}, {1}, {2})",
                            ColorBase + (r * redIncrement),
                            ColorBase + (g * greenIncrement),
                            ColorBase + (b * blueIncrement));
                    }
                }
            }

            // now put the colors into the namespace table (copy the keys out of the dictionary, and then update)
            int i = 0;
            string[] keys = new string[returnList.Keys.Count];
            foreach (string key in returnList.Keys)
            {
                keys[i++] = key;
            }

            i = 0;
            foreach (string key in keys)
            {
                returnList[key] = colors[i++];
                Console.WriteLine(string.Format(
                    "Namespace {0} is color {1}",
                    key, 
                    returnList[key]));
            }

            return returnList;
        }

        /// <summary>
        /// Convert a list of types into a tree of TypeTreeNode objects.
        /// </summary>
        /// <param name="implementedTypesList">Flat list of Type objects.</param>
        /// <returns>Tree of objects describing the types.  The root object is above the last
        /// known type object.  This root object has no "Type".</returns>
        private static TypeTreeNode ConvertToTree(IEnumerable<Type> implementedTypesList)
        {
            TypeTreeNode root = new TypeTreeNode();
            root.Name = "root";
            root.Depth = 0;
            root.RawType = null;

            foreach (Type t in implementedTypesList.OrderBy(t => t.Namespace).ThenBy(t => t.Name))
            {
                Stack<Type> inProcess = new Stack<Type>();

                Type workingT = t;  // foreach iterator cannot be manipulated, need copy
                while (workingT != null)
                {
                    inProcess.Push(workingT);
                    workingT = workingT.BaseType;
                }

                // now take that stack and use it to populate the tree with
                // any missing elements from top down
                TypeTreeNode currentTreeNode = root;
                int depth = 0;
                while (inProcess.Count > 0)
                {
                    TypeTreeNode newNode = new TypeTreeNode();
                    newNode.RawType = inProcess.Pop();
                    newNode.Name = newNode.RawType.Name;

                    /* 
                    // for debug
                    if (newNode.RawType.ToString().Contains("MFxReference"))
                    {
                        Console.WriteLine("Found MFxReference" + newNode.ToString());
                    }
                    */

                    newNode.Depth = depth++;

                    currentTreeNode.TotalChildren++;

                    TypeTreeNode foundChildNode;

                    // if it isn't there, add it, then move to the new/found child
                    if (!currentTreeNode.Children.TryGetValue(newNode.Name, out foundChildNode))
                    {
                        currentTreeNode.Children.Add(newNode.Name, newNode);
                        currentTreeNode.DirectChildren++;
                        currentTreeNode = newNode;  // move to the new child
                    }
                    else
                    {
                        currentTreeNode = foundChildNode;
                    }
                }
            }

            return root;
        }

        /// <summary>
        /// Recursively dump the tree of TypeTreeNode data to the console.
        /// </summary>
        /// <param name="root">Root of the tree to dump (recursive).</param>
        /// <param name="indent">Indent string - - gets extended each recursion.</param>
        private static void ConsoleOutTree(TypeTreeNode root, string indent)
        {
            if (root == null)
            {
                return;
            }

            Console.WriteLine(string.Format("{0}{1}", indent, root.ToString()));

            foreach (TypeTreeNode c in root.Children.Values.OrderBy(t => t.RawType.Namespace).ThenBy(t => t.RawType.Name))
            {
                ConsoleOutTree(c, indent + "  ");
            }
        }

        /// <summary>
        /// This function uses the node information to place a copy of the templateShape 
        /// on the targetPage, in the right color, at the specified location.  
        /// The new shape is linked to the parent.  The Children of the node are then
        /// (recursively) placed on the page, linked to this new node.
        /// </summary>
        /// <param name="colorMap">Namespace color map to used for rendering.</param>
        /// <param name="targetPage">Visio page to place the objects.</param>
        /// <param name="node">Node object to place.</param>
        /// <param name="templateShape">Which shape to use to display.</param>
        /// <param name="parentShape">Parent visio node to attach child to.</param>
        /// <param name="baseLocationX">XLocation to start displaying subtree.</param>
        /// <param name="baseLocationY">YLocation to start displaying subtree.</param>
        /// <returns>Returns the next available Y coordinate to display an object.</returns>
        private static double PlaceOneShape(
            Dictionary<string, string> colorMap,
            Visio.Page targetPage, 
            TypeTreeNode node, 
            Visio.Master templateShape,
            Visio.Shape parentShape,
            double baseLocationX,
            double baseLocationY)
        {
            double movingYBase = baseLocationY;

            Visio.Shape newlyPlacedShape = targetPage.Drop(templateShape,  baseLocationX,  baseLocationY);

            // add the new object to the selection group for later "group" operation
            Program.selectionTree.Select(newlyPlacedShape, (short)Visio.VisSelectArgs.visSelect);

            if (parentShape != null)
            {
                Visio.Document stencilDocument = targetPage.Application.Documents.OpenEx(
                    "Blocks.vss", 
                    (short)Visio.VisOpenSaveArgs.visOpenDocked);
                Visio.Master connectorMaster = stencilDocument.Masters["Dynamic connector"];
                Visio.Shape connectorShape = targetPage.Drop(connectorMaster, 1.0, 1.0);
                Program.selectionTree.Select(connectorShape, (short)Visio.VisSelectArgs.visSelect);
                ConnectShapes(parentShape, newlyPlacedShape, connectorShape);
            }

            // set the templateShape height, width, text, and color
            newlyPlacedShape.get_CellsSRC(
               (short)Visio.VisSectionIndices.visSectionObject,
               (short)Visio.VisRowIndices.visRowXFormIn,
               (short)Visio.VisCellIndices.visXFormHeight).ResultIU = Program.BoxY;

            newlyPlacedShape.get_CellsSRC(
               (short)Visio.VisSectionIndices.visSectionObject,
               (short)Visio.VisRowIndices.visRowXFormIn,
               (short)Visio.VisCellIndices.visXFormWidth).ResultIU = Program.BoxX;

            string shortNamespace = String.Empty;
            if (!String.IsNullOrEmpty(node.RawType.Namespace))
            {
                string[] namespaceTokens = node.RawType.Namespace.Split(new char[] { '.' });
                shortNamespace = namespaceTokens[namespaceTokens.Length - 1];
            }

            newlyPlacedShape.Text = string.Format("{0} ({1})",  node.Name, shortNamespace);

            // if there is an entry in the color map, use it.
            if (!String.IsNullOrEmpty(node.RawType.Namespace) && colorMap.ContainsKey(node.RawType.Namespace))
            {
                newlyPlacedShape.get_CellsSRC(
                    (short)Visio.VisSectionIndices.visSectionObject,
                    (short)Visio.VisRowIndices.visRowFill,
                    (short)Visio.VisCellIndices.visFillForegnd).FormulaU =
                        colorMap[node.RawType.Namespace];
            }

            movingYBase += Program.BoxPitchY;

            foreach (TypeTreeNode t in node.Children.Values.OrderBy(t => t.RawType.Namespace).ThenBy(t => t.RawType.Name))
            {
                movingYBase = PlaceOneShape(
                    colorMap, 
                    targetPage, 
                    t, 
                    templateShape, 
                    newlyPlacedShape, 
                    baseLocationX + Program.BoxPitchX,              // x adds Depth
                    movingYBase);
            }

            return movingYBase;
        }

        /// <summary>
        /// This function connects two shapes using the provided connector.
        /// </summary>
        /// <param name="shape1">This is the "Parent" shape.</param>
        /// <param name="shape2">This is the "Child" shape.</param>
        /// <param name="connector">This is the connector shape.</param>
        private static void ConnectShapes(Visio.Shape shape1, Visio.Shape shape2, Visio.Shape connector)
        {
            // get the cell from the source side of the connector
            Visio.Cell beginXCell = connector.get_CellsSRC(
            (short)Visio.VisSectionIndices.visSectionObject,
            (short)Visio.VisRowIndices.visRowXForm1D,
            (short)Visio.VisCellIndices.vis1DBeginX);

            // glue the source side of the connector to the first shape
            beginXCell.GlueTo(shape1.get_CellsSRC(
            (short)Visio.VisSectionIndices.visSectionObject,
            (short)Visio.VisRowIndices.visRowXFormOut,
            (short)Visio.VisCellIndices.visXFormPinX));

            // get the cell from the destination side of the connector
            Visio.Cell endXCell = connector.get_CellsSRC(
            (short)Visio.VisSectionIndices.visSectionObject,
            (short)Visio.VisRowIndices.visRowXForm1D,
            (short)Visio.VisCellIndices.vis1DEndX);

            // glue the destination side of the connector to the second shape
            endXCell.GlueTo(shape2.get_CellsSRC(
            (short)Visio.VisSectionIndices.visSectionObject,
            (short)Visio.VisRowIndices.visRowXFormOut,
            (short)Visio.VisCellIndices.visXFormPinX));
        }

        /// <summary>
        /// Given a color map (Name:color), this function plots the information as 
        /// as a key on the page.
        /// </summary>
        /// <param name="colorMap">Colro information to display.</param>
        /// <param name="contentDocument">Document to display the info.</param>
        private static void DisplayNamespaceKey(
            Dictionary<string, string> colorMap,
            Visio.Document contentDocument)
        {
            const double KeyPitchX = 2.0;
            const double KeyX = 1.75;

            const double KeyPitchY = 1.0;
            const double KeyY = 0.75;

            const double KeyInitialX = 1.5;

            Visio.Document stencilDocument = contentDocument.Application.Documents.OpenEx(
                "Basic_U.vss", 
                (short)Visio.VisOpenSaveArgs.visOpenDocked);
            Visio.Master shape = stencilDocument.Masters["Rectangle"];

            Visio.Page targetPage = contentDocument.Pages[1];

            int i = 0;
            double dropX = KeyInitialX;
            double dropY = 1.0;

            // group all of the items in the key
            Program.selectionTree = contentDocument.DocumentSheet.Application.ActiveWindow.Selection;
            Program.selectionTree.DeselectAll();

            foreach (string colorName in colorMap.Keys)
            {
                Visio.Shape boxShape = targetPage.Drop(shape, dropX, dropY);
                Program.selectionTree.Select(boxShape, (short)Visio.VisSelectArgs.visSelect);

                // set the templateShape height, width, text, and color
                boxShape.get_CellsSRC(
                    (short)Visio.VisSectionIndices.visSectionObject,
                    (short)Visio.VisRowIndices.visRowXFormIn,
                    (short)Visio.VisCellIndices.visXFormHeight).ResultIU = KeyY;

                boxShape.get_CellsSRC(
                   (short)Visio.VisSectionIndices.visSectionObject,
                   (short)Visio.VisRowIndices.visRowXFormIn,
                   (short)Visio.VisCellIndices.visXFormWidth).ResultIU = KeyX;

                boxShape.get_CellsSRC(
                    (short)Visio.VisSectionIndices.visSectionObject,
                    (short)Visio.VisRowIndices.visRowFill,
                    (short)Visio.VisCellIndices.visFillForegnd).FormulaU =
                        colorMap[colorName];

                string[] namespaceTokens = colorName.Split(new char[] { '.' });
                string shortNamespace = namespaceTokens[namespaceTokens.Length - 1];
                boxShape.Text = string.Format("{0}\n({1})", shortNamespace, colorName);

                i++;
                dropX += KeyPitchX;

                // bit of a kluge, but it works.
                if (i % 4 == 0)
                {
                    dropX = KeyInitialX;
                    dropY += KeyPitchY;
                }
            }

            Program.selectionTree.Group(); 
            Program.selectionTree.DeselectAll();
        }

        /// <summary>
        /// This is the root of the recursive descent to display the tree on the Visio doc.
        /// The function uses PlaceOneShape() to start the recursive display.
        /// </summary>
        /// <param name="colorMap">Color map to use to render.</param>
        /// <param name="root">Root node to render.</param>
        private static void BuildAndDisplayWithVisio(
            Dictionary<string, string> colorMap,
            TypeTreeNode root)
        {
            Visio.Application app = null;
            Visio.Document contentDocument = null;

            try
            {
                app = new Visio.Application();
                app.Settings.EnableAutoConnect = false;
                ////app.Window.WindowState = (int)Visio.VisWindowStates.visWSRestored;
                app.Window.WindowState = (int)Visio.VisWindowStates.visWSMinimized;

                contentDocument = app.Documents.AddEx(
                    string.Empty, 
                    Visio.VisMeasurementSystem.visMSUS, 
                    (int)Visio.VisOpenSaveArgs.visAddDocked,
                    (int)0);

                contentDocument.PaperSize = Visio.VisPaperSizes.visPaperSizeE;
                contentDocument.PrintLandscape = true;

                /*
                Visio.Document stencilDocument = contentDocument.Application.Documents.OpenEx(
                    "Basic_U.vss", 
                    (short)Visio.VisOpenSaveArgs.visOpenDocked);
                Visio.Master shape = stencilDocument.Masters["Rectangle"];
                */

                Visio.Master shape = contentDocument.Application.Documents.OpenEx(
                    "Basic_U.vss", 
                    (short)Visio.VisOpenSaveArgs.visOpenDocked).Masters["Rectangle"];

                Visio.Page targetPage = contentDocument.Pages[1];
                targetPage.Name = "MFx Class Hirearchy";

                BuildHeadersAndFooters(colorMap, contentDocument);

                /*
                Program.selectionTree = app.ActiveWindow.Selection;
                Program.selectionTree.DeselectAll();
                */

                double movingYBase = 4.0;
                foreach (TypeTreeNode t in 
                    root.Children.Values.OrderBy(t => t.RawType.Namespace).ThenBy(t => t.RawType.Name))
                {
                    movingYBase = PlaceOneShape(
                        colorMap, 
                        targetPage, 
                        t, 
                        shape,
                        null,
                        2.0,
                        movingYBase);
                }

                // now turn the selection into a group, and run the layout code
                // to make the tree look "right"
                ////Program.selectionTree.Group(); 

                // resize, set the placement, and connector routing styles
                Visio.Cell layoutCell;

                layoutCell = targetPage.PageSheet.get_CellsSRC(
                    (short)Visio.VisSectionIndices.visSectionObject,
                    (short)Visio.VisRowIndices.visRowPageLayout,
                    (short)Visio.VisCellIndices.visPLOResizePage);

                layoutCell.FormulaU = "FALSE";  // don't add more pages to contain the tree, let the user edit
 
                layoutCell = targetPage.PageSheet.get_CellsSRC(
                    (short)Visio.VisSectionIndices.visSectionObject,
                    (short)Visio.VisRowIndices.visRowPageLayout,
                    (short)Visio.VisCellIndices.visPLOPlaceStyle);

                layoutCell.set_Result(
                    Visio.VisUnitCodes.visPageUnits,
                   (double)Visio.VisCellVals.visPLOPlaceCompactDownRight);

                layoutCell = targetPage.PageSheet.get_CellsSRC(
                    (short)Visio.VisSectionIndices.visSectionObject,
                    (short)Visio.VisRowIndices.visRowPageLayout,
                    (short)Visio.VisCellIndices.visPLORouteStyle);

                layoutCell.set_Result(
                    Visio.VisUnitCodes.visPageUnits,
                    (double)Visio.VisCellVals.visLORouteOrgChartNS);

                targetPage.Layout();
                ////Program.selectionTree.Layout();

                Program.selectionTree.Group(); 

                Console.WriteLine("Print and save Visio doc if you want, then enter to quit (Visio will close).");
                Console.ReadLine();
            }
            finally
            {
                if (contentDocument != null)
                {
                    contentDocument.Saved = true;   // not really, but we can lie so the close/quit works.
                    contentDocument.Close();
                }

                if (app != null)
                {
                    app.Quit();
                }
            }
        }

        /// <summary>
        /// Display the correct headers and footers for the current operation.
        /// </summary>
        /// <param name="colorMap">Color map to display.</param>
        /// <param name="contentDocument">Document to render the data on.</param>
        private static void BuildHeadersAndFooters(
            Dictionary<string, string> colorMap,
            Visio.Document contentDocument)
        {
            DisplayNamespaceKey(colorMap, contentDocument);
            contentDocument.FooterCenter = string.Format(
                "{0} - - rendered at:  {1}", Program.asm.ToString(), System.DateTime.Now.ToString());
            contentDocument.HeaderCenter = string.Format(
                "Class Library Hierarchy for {0}",
                Program.assemblyName);

            stdole.IFontDisp headerFont = (stdole.IFontDisp)new stdole.StdFont();
            headerFont.Name = "Arial";
            headerFont.Bold = false;
            headerFont.Size = 24;
            headerFont.Underline = true;
           
            contentDocument.HeaderFooterFont = headerFont;
        }

        /// <summary>
        /// Get all of the classes from the target assembly, filtered by nameSpace if needed.
        /// </summary>
        /// <param name="pathName">Assembly (exe or dll) to explore.</param>
        /// <param name="nameSpace">Namespace filter (may be null or empty).</param>
        /// <returns>Returns a list of types.</returns>
        private static IEnumerable<Type> GetAllClasses(string pathName, string nameSpace) 
        { 
            Program.asm = Assembly.LoadFrom(pathName);
            if (!String.IsNullOrWhiteSpace(nameSpace))
            {
                return Program.asm.GetTypes().Where(x => x.Namespace == nameSpace).Select(x => x);
            }
            else
            {
                return Program.asm.GetTypes();
            }
        }

        /// <summary> Display usage information and exit.</summary>
        private static void Usage()
        {
            Console.WriteLine("MFxClassLibraryChart.exe AssemblyName [Namespace]");
            Console.WriteLine("AssemblyName is the dll or exe to display.");
            Console.WriteLine("Namespace (optional) restricts the display to classes in a single Namespace.");
            Environment.Exit(1);
        }
    }
}