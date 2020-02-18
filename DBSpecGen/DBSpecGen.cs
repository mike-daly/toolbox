using System;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Resources;
using System.Collections.Specialized;
using System.Globalization;


namespace DBSpecGen
{
    public class DatabaseSpecGenerator
    {
        #region fields
        private int    m_PieChartRadius = 125;
        private int    m_PieChartWidth  = 800;
        private int    m_PieChartHeight = 600;
        private int    m_timeout = 60;
        private int    m_MaxLabelLength = 12;
        private int    m_ItemsPerRow = 6;

        private bool   m_bIsVML = false;
        private bool   m_bQuiet = false;
        private bool   m_ParseXmlComments = false;
        private bool   m_bShowAllExtendedProperties = false;
        private bool   m_bHideDBNameOnPageTitles = false;
        private bool   m_bUseMBAsSizeUnit = false;
        private bool   m_GenerateXmlOnly = false;
        private bool   m_bDrawPieCharts = true;
 
        private string m_ConfigPath = "";
        private string m_WorkingDirectory = "";
        private string m_OutputDirectory = "";
        private string m_ChmFilePath = "";
        private string m_ChmName = "";
        private string m_DataModelXml = "";

        private StringCollection m_ConnectionStrings = new StringCollection();
        private StringCollection m_ServerNames = new StringCollection();
        private StringCollection m_DatabaseNames = new StringCollection();
        private StringCollection m_DatabaseSchemaPaths = new StringCollection();
        private StringCollection m_InternalDatabaseSchemaPaths = new StringCollection();
        private StringCollection m_ExternalObjectPaths = new StringCollection();
        private StringCollection m_OutputDirectories = new StringCollection();
        private StringCollection m_PieChartImageMaps = new StringCollection();
        private StringCollection m_FakeServerNames = new StringCollection();
        private StringCollection m_FakeDatabaseNames = new StringCollection();

        private DBSpecGen m_Gui = null;
        
        private XmlDocument m_xmlExcludedObjects = new XmlDocument();
        #endregion

        #region constructors
        public DatabaseSpecGenerator()
        {
        }
        #endregion
       
        #region properties
        public string DataModelXml
        {
            get
            {
                return m_DataModelXml;
            }
            set
            {
                m_DataModelXml = value;
            }
        }
        public int Timeout
        {
            get
            {
                return m_timeout;
            }
            set
            {
                m_timeout = value;
            }
        }
        public bool BeQuiet
        {
            get
            {
                return m_bQuiet;
            }
            set
            {
                m_bQuiet = value;
            }
        }
        public int MaxLabelLength
        {
            get
            {
                return m_MaxLabelLength;
            }
            set
            {
                m_MaxLabelLength = value;
            }
        }
        public int ItemsPerRow
        {
            get
            {
                return m_ItemsPerRow;
            }
            set
            {
                m_ItemsPerRow = value;
            }
        }
        public string ChmName
        {
            get
            {
                return m_ChmName;
            }
            set
            {
                m_ChmName = value;
            }
        }
        public string ChmFilePath
        {
            get
            {
                return m_ChmFilePath;
            }
        }
        
        public bool ParseXmlComments
        {
            get
            {
                return m_ParseXmlComments;
            }
            set
            {
                m_ParseXmlComments = value;
            }
        }
        public bool UseVml
        {
            get
            {
                return m_bIsVML;
            }
            set
            {
                m_bIsVML = value;
            }
        }

        public bool ShowAllExtendedProperties
        {
            get
            {
                return m_bShowAllExtendedProperties;
            }
            set
            {
                m_bShowAllExtendedProperties = value;
            }
        }

        public bool HideDBNameOnPageTitles
        {
            get
            {
                return m_bHideDBNameOnPageTitles;
            }
            set
            {
                m_bHideDBNameOnPageTitles = value;
            }
        }

        public bool UseMBAsSizeUnit
        {
            get
            {
                return m_bUseMBAsSizeUnit;
            }
            set
            {
                m_bUseMBAsSizeUnit = value;
            }
        }

        public bool GeneratePieCharts
        {
            get
            {
                return m_bDrawPieCharts;
            }
            set
            {
                m_bDrawPieCharts = value;
            }
        }

        public bool GenerateXmlOnly
        {
            get
            {
                return m_GenerateXmlOnly;
            }
            set
            {
                m_GenerateXmlOnly = value;
            }
        }

        public string ConfigPath
        {
            get 
            {
                return m_ConfigPath; 
            }
            set
            {
                m_ConfigPath = value;
            }
        }

        public string WorkingDirectory
        {
            get 
            {
                return m_WorkingDirectory; 
            }
            set
            {
                m_WorkingDirectory = value;
            }
        }

        public string OutputDirectory
        {
            get
            {
                return m_OutputDirectory;
            }
            set
            {
                m_OutputDirectory = value;
            }
        }
       
        public DBSpecGen Gui
        {
            get
            {
                return m_Gui;
            }
            set
            {
                m_Gui = value;
            }
        }
        #endregion

        #region private/internal methods

        private XmlDocument parseXmlComments(XmlDocument xml)
        {
            XmlDocument xmlOut = new System.Xml.XmlDocument();
            XmlDocument xmlTemp = new System.Xml.XmlDocument();
            XmlDocumentFragment frag;
            XmlNodeList codeBlocks;
            XmlNodeList codeComments;
            string owner = "";
            string comments = "";
            xmlOut.LoadXml(xml.OuterXml);
            frag = xmlOut.CreateDocumentFragment();

            // for each code block, get the xml comments, put them in a codeComments element
            codeBlocks = xmlOut.SelectNodes("/database/procedure/code | /database/table/code");
            XmlElement elemError = xmlOut.CreateElement("error");
            for(int i = 0; i < codeBlocks.Count; ++i)
            {
                owner = codeBlocks[i].ParentNode.SelectSingleNode("@name").Value;
                comments = codeBlocks[i].InnerText;

                // this really sucks but it works i guess...

                // first get rid of the ---, then the xml-breaking chars...
                comments = comments.Replace("---","");
                comments = comments.Replace("--","");
                comments = comments.Replace("&","&amp;");
                comments = comments.Replace("<","&lt;");
                comments = comments.Replace("]]>","]]&gt;");
                comments = comments.Replace("<![CDATA[","&lt;![CDATA[");

                // now replace our tags...
				comments = comments.Replace("&lt;logic","<logic");  
				comments = comments.Replace("&lt;/logic","</logic");
                comments = comments.Replace("&lt;scope","<scope");  
                comments = comments.Replace("&lt;/scope","</scope");
                comments = comments.Replace("&lt;summary","<summary");  
                comments = comments.Replace("&lt;/summary","</summary");
                comments = comments.Replace("&lt;parameters","<parameters");  
                comments = comments.Replace("&lt;/parameters","</parameters");
                comments = comments.Replace("&lt;attributes","<attributes");  
                comments = comments.Replace("&lt;/attributes","</attributes");
				comments = comments.Replace("&lt;param","<param");  
				comments = comments.Replace("&lt;/param","</param");
                comments = comments.Replace("&lt;return","<return");  
                comments = comments.Replace("&lt;/return","</return");
                comments = comments.Replace("&lt;recordset","<recordset");  
                comments = comments.Replace("&lt;/recordset","</recordset");
                comments = comments.Replace("&lt;column","<column");  
                comments = comments.Replace("&lt;/column","</column");
                comments = comments.Replace("&lt;historylog","<historylog");  
                comments = comments.Replace("&lt;/historylog","</historylog");
                comments = comments.Replace("&lt;log","<log");  
                comments = comments.Replace("&lt;/log","</log");
                comments = comments.Replace("&lt;sample","<sample");  
                comments = comments.Replace("&lt;/sample","</sample");
                comments = comments.Replace("&lt;description","<description");  
                comments = comments.Replace("&lt;/description","</description");
                comments = comments.Replace("&lt;code","<code");  
                comments = comments.Replace("&lt;/code","</code");

                // would be nice to come up with a better solution to the crap above...

                comments = "<codeComments name='"+owner+"'>" + comments + "</codeComments>";
                try
                {
                    xmlTemp.LoadXml(comments);
                }
                catch(Exception error)
                {
                    elemError.InnerText = "An error occured when parsing the xml code comments for [" + owner + "].  The xml did not load into an XmlDocument: " + error.Message;
                    codeBlocks[i].ParentNode.AppendChild(elemError);
                    continue;
                }
                
                comments = "<codeComments name='"+owner+"'>";
                codeComments = xmlTemp.SelectNodes(
                    "/codeComments/scope | /codeComments/logic | " +
                    "/codeComments/summary | " +
                    "/codeComments/parameters | " +
                    "/codeComments/returns | " +
                    "/codeComments/historylog | " +
                    "/codeComments/samples"
                    );
                for(int j=0; j<codeComments.Count; ++j)
                {
                    comments += codeComments[j].OuterXml;
                }
                comments += "</codeComments>";

                if(codeComments.Count > 0)
                {
                    frag.InnerXml = comments;
                    codeBlocks[i].ParentNode.AppendChild(frag);
                }
            }
            return xmlOut;
        }

        static internal bool GetXmlFromSql(System.Data.SqlClient.SqlCommand com, System.Xml.XmlDocument xml)
        {
            try
            {
                System.Xml.XmlReader xr=com.ExecuteXmlReader();
                xr.Read();
                if(!xr.EOF)
                {
                    xml.Load(xr);
                }
                xr.Close();
            } 
            catch(System.NullReferenceException e)
            {
                // have to do this because the stupid ExecuteXmlReader method 
                // throws an exception internally if nothing is returned by the query.
                // would be much better to just return null so i could check for it,
                // but it doesn't.  It's not really an exceptional case that a query
                // returns zero records. 
                throw new System.Exception("No data was returned by the sql query.", e);
            }
            return true;
        }

        private XmlDocument FixUpXml(XmlDocument xmlIn)
        {
            if(xmlIn==null)
            {
                throw new System.ArgumentNullException("xmlIn", "The xmlIn paramater may not be null.");
            }

            if (xmlIn.SelectSingleNode("/database/@name") == null)
            {
                throw new System.Exception("The xml passed to FixUpXml() did not have a name attribute on the root database node.");
            }

            XmlDocument xmlOut = new System.Xml.XmlDocument();
            xmlOut.LoadXml("<database/>");
            
            // add attributes to database node...
            System.Xml.XmlNodeList nodes = xmlIn.SelectNodes("/database/@*");
            for(int i = 0; i < nodes.Count; ++i)
            {
                xmlOut.DocumentElement.SetAttribute(nodes[i].LocalName, "", nodes[i].Value);   
            }

            // first attach all the nodes that won't change...
            nodes = xmlIn.SelectNodes(
                @"	/database/userDefinedType | 
					/database/table | 
                    /database/file |
                    /database/user |
					/database/procedure | 
					/database/dependency |
                    /database/permission |
                    /database/serverProperty |
					/database/extendedProperty");
            for(int i=0; i<nodes.Count; ++i)
            {
                xmlOut.DocumentElement.AppendChild(xmlOut.ImportNode(nodes[i],true));
            }

            // now paste together the xml. first the procedures...
            string objName = "";
            string xtype = "";
            string code = "";
            System.Xml.XmlNodeList codeNodes;
            nodes = xmlIn.SelectNodes("/database/procedure");
            for(int i=0; i<nodes.Count; ++i)
            {
                code = "";
                objName = nodes[i].SelectSingleNode("@name").Value;
                xtype = nodes[i].SelectSingleNode("@xtype").Value; 
                codeNodes = xmlIn.SelectNodes("/database/code[@name='"+objName+"' and @type='"+xtype+"']");
                for(int j=0; j<codeNodes.Count; ++j)
                {
                    code += codeNodes[j].InnerText;
                }
                XmlElement codeElem = xmlOut.CreateElement("code");
                codeElem.InnerText = code;
                if(xmlOut.SelectSingleNode("/database/procedure[@name='"+objName+"' and @xtype='"+xtype+"']") != null)
                {
                    xmlOut.SelectSingleNode("/database/procedure[@name='"+objName+"' and @xtype='"+xtype+"']").AppendChild(codeElem);
                }
            }

            // now the check constraints...
            nodes = xmlIn.SelectNodes("/database/table/constraint");
            for(int i=0; i<nodes.Count; ++i)
            {
                code = "";
                objName = nodes[i].SelectSingleNode("@name").Value;
                codeNodes = xmlIn.SelectNodes("/database/code[@name='"+objName+"' and @type='C']");
                for(int j=0; j<codeNodes.Count; ++j)
                {
                    code += codeNodes[j].InnerText;
                }
                XmlElement codeElem = xmlOut.CreateElement("code");
                codeElem.InnerText = code;
                if(xmlOut.SelectSingleNode("/database/table/constraint[@name='"+objName+"' and @hasCheck='1']") != null)
                {
                    xmlOut.SelectSingleNode("/database/table/constraint[@name='"+objName+"' and @hasCheck='1']").AppendChild(codeElem);
                }
            }

            // now the triggers...
            nodes = xmlIn.SelectNodes("/database/table/trigger");
            for(int i=0; i<nodes.Count; ++i)
            {
                code = "";
                objName = nodes[i].SelectSingleNode("@name").Value;
                codeNodes = xmlIn.SelectNodes("/database/code[@name='"+objName+"' and @type='TR']");
                for(int j=0; j<codeNodes.Count; ++j)
                {
                    code += codeNodes[j].InnerText;
                }
                XmlElement codeElem = xmlOut.CreateElement("code");
                codeElem.InnerText = code;
                if(xmlOut.SelectSingleNode("/database/table/trigger[@name='"+objName+"']") != null)
                {
                    xmlOut.SelectSingleNode("/database/table/trigger[@name='"+objName+"']").AppendChild(codeElem);
                }
            }

            // now the views...
            nodes = xmlIn.SelectNodes("/database/table[@xtype='V']");
            for(int i=0; i<nodes.Count; ++i)
            {
                code = "";
                objName = nodes[i].SelectSingleNode("@name").Value;
                codeNodes = xmlIn.SelectNodes("/database/code[@name='"+objName+"' and (@type='V')]");
                for(int j=0; j<codeNodes.Count; ++j)
                {
                    code += codeNodes[j].InnerText;
                }
                XmlElement codeElem = xmlOut.CreateElement("code");
                codeElem.InnerText = code;
                if(xmlOut.SelectSingleNode("/database/table[@name='"+objName+"' and (@xtype='V')]") != null)
                {
                    xmlOut.SelectSingleNode("/database/table[@name='"+objName+"' and (@xtype='V')]").AppendChild(codeElem);
                }
            }
            return xmlOut;
        }
        
        private string DrawCodeSizePieChart(
            XPathDocument xpathdoc, 
            string XPath,
            int max, 
            string saveToPath, 
            string title, 
            string xtype,
            string htmContainingDirectory)
        {
            string imageMap = "<map name='"+title.Replace(" ","").Replace(".","")+"'>";
            XPathNavigator nav = xpathdoc.CreateNavigator();
            XPathExpression expr = nav.Compile(XPath);
            expr.AddSort("string-length(code)", XmlSortOrder.Descending, XmlCaseOrder.None, "", XmlDataType.Number);
            XPathNodeIterator iterator = nav.Select(expr);
            int count = 0;
            LabelAndValue[] nameAndSize = new LabelAndValue[max];
            int total = 0;
            while (iterator.MoveNext())
            {
                XPathNavigator nav2 = iterator.Current.Clone();
                nav2.MoveToAttribute("name","");
                string name = nav2.Value;
                nav2 = iterator.Current.Clone();
                int size = System.Convert.ToInt32(nav2.Evaluate("string-length(code)"), CultureInfo.CurrentCulture);
                if (count < max)
                {
                    LabelAndValue ns = new LabelAndValue();
                    ns.Name = name;
                    ns.Size = size;
                    nameAndSize[count] = ns;
                }
                total += size;
                count++;
            }
            if (count > 0 && total > 0)
            {
                string imageMapGuts = "";
                PieChart pc = new PieChart();
                Bitmap bm = pc.Draw(
                    Color.White, 
                    m_PieChartWidth, 
                    m_PieChartHeight, 
                    m_PieChartRadius, 
                    nameAndSize, 
                    total, 
                    title, 
                    xtype,
                    htmContainingDirectory,
                    out imageMapGuts);
                bm.Save(saveToPath, ImageFormat.Gif);
                imageMap += imageMapGuts;
            }
            imageMap += "</map>";
            return imageMap;
        }

        private string DrawPieChart(
            XPathDocument xpathdoc, 
            string XPath, 
            string attr, 
            int max, 
            string saveToPath, 
            string title, 
            string xtype,
            string htmContainingDirectory)
        {
            string imageMap = "<map name='"+title.Replace(" ","").Replace(".","")+"'>";
            XPathNavigator nav = xpathdoc.CreateNavigator();
            XPathExpression expr = nav.Compile(XPath);
            expr.AddSort("@" + attr, XmlSortOrder.Descending, XmlCaseOrder.None, "", XmlDataType.Number);
            XPathNodeIterator iterator = nav.Select(expr);
            int count = 0;
            int total = 0;
            
            LabelAndValue[] nameAndSize = new LabelAndValue[max];
			try
			{
				while (iterator.MoveNext())
				{
					XPathNavigator nav2 = iterator.Current.Clone();
					nav2.MoveToAttribute("name","");
					string name = nav2.Value;
					nav2 = iterator.Current.Clone();
					nav2.MoveToAttribute(attr,"");
					int size = System.Convert.ToInt32(nav2.Value, CultureInfo.CurrentCulture);
					if (count < max)
					{
						LabelAndValue ns = new LabelAndValue();
						ns.Name = name;
						ns.Size = size;
						nameAndSize[count] = ns;
					}
					total += size;
					count++;
				}
				if (count > 0)
				{
					string imageMapGuts = "";
					PieChart pc = new PieChart();
					Bitmap bm = pc.Draw(
						Color.White, 
						m_PieChartWidth, 
						m_PieChartHeight, 
						m_PieChartRadius, 
						nameAndSize, 
						total, 
						title, 
						xtype,
						htmContainingDirectory,
						out imageMapGuts);
					bm.Save(saveToPath, ImageFormat.Gif);
					imageMap += imageMapGuts;
				}
			}
			catch
			{
				// do nothing.
			}

            imageMap += "</map>";
            return imageMap;
        }
        
        private XmlDocument GetConfigInfo()
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml("<configInfo/>");

            // put the dependencies for our custom objects in the xml...
            if (m_ConfigPath.Length > 0)
            {
                XmlDocument xmlConfig = new XmlDocument();
                xmlConfig.Load(m_ConfigPath);
                XmlNode configNode = xml.ImportNode(xmlConfig.SelectSingleNode("/DBSpecGen"), true);
                xml.DocumentElement.AppendChild(configNode);
            }	

            // paste in any dependencies from our custom objects that we may have...
            for (int k = 0; k < m_ExternalObjectPaths.Count; ++k)
            {
                XmlDocument xmlCustom = new XmlDocument();
                xmlCustom.Load(m_ExternalObjectPaths[k]);
                XmlNodeList nodes = xmlCustom.SelectNodes("/customObjects/*");
                for (int m = 0; m < nodes.Count; ++m)
                {
                    XmlNode dep = xml.ImportNode(nodes[m], true);
                    xml.DocumentElement.AppendChild(dep);
                }
            }
            
			
            // add the exclusion stuff from the ui to the xml...
            if (m_xmlExcludedObjects != null &&
                m_xmlExcludedObjects.SelectSingleNode("/DBSpecGen") != null)
            {
                XmlNode configNode = xml.ImportNode(m_xmlExcludedObjects.SelectSingleNode("/DBSpecGen"), true);
                xml.DocumentElement.AppendChild(configNode);
            }
            return xml;
        }

        private bool CopyFiles(string sourcePath, string destPath)
        {
            string [] files = 
            {
                "\\blank.htm", 
                "\\help.css",
                "\\help.js"
            };


            string source = "";
            string dest = "";
            for(int i=0; i<files.Length; ++i)
            {
                source = sourcePath + files[i];
                dest = destPath + files[i];
                System.IO.File.Copy(source, dest, true);
            }
            return true;
        }
        
        private void WriteHhcFile(string name)
        {
            FileStream fs = new FileStream(m_OutputDirectory + "\\" + name + ".hhc", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter w = new StreamWriter(fs);
            w.Write("<HTML><HEAD><!-- Sitemap 1.0 --></HEAD><BODY>\r\n");
            w.Write("<OBJECT type=\"text/site properties\"><param name=\"ImageType\" value=\"Folder\"/></OBJECT><UL>\r\n");

            XmlDocument xmlConfigInfo = GetConfigInfo();

            for(int i=0; i < m_OutputDirectories.Count; ++i)
            {
                string dbName = m_DatabaseNames[i]; 
                string serverName = m_ServerNames[i]; 
                XmlDocument xml = new XmlDocument();
                xml.Load(m_OutputDirectories[i] + "\\" + dbName + ".xml");

                XmlNodeList nodesToAdd = xmlConfigInfo.SelectNodes("/configInfo/*");
                for (int count = 0; count < nodesToAdd.Count; ++count)
                {
                    XmlNode configNode = xml.ImportNode(nodesToAdd[count], true);
                    xml.DocumentElement.AppendChild(configNode);
                }

                w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"" + (this.HideDBNameOnPageTitles ? "" : serverName + ".") + dbName + "\"/><param name=\"Local\" value=\"" + m_OutputDirectories[i] + "\\default.htm\"/></OBJECT><UL>\r\n");
                

                // add tables
                bool bWritten = false;
                XmlNodeList nodes = xml.SelectNodes("/database/table[@xtype='U']/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/table[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        if(!bWritten)w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"Tables\"/><param name=\"Local\" value=\"" + m_OutputDirectories[i] + "\\cntTables.htm\"/></OBJECT><UL>\r\n");
                        bWritten = true;
                        w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\""+nodes[j].Value+"\"></param>");
                        w.Write("<param name=\"Local\" value=\""+m_OutputDirectories[i] + "\\tbl_"+nodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                    }
                }
                if(bWritten)w.Write("</UL>\r\n");
				
				
                // add views
                bWritten = false;
                nodes = xml.SelectNodes("/database/table[@xtype='V']/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/view[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        if(!bWritten)w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"Views\"/><param name=\"Local\" value=\"" + m_OutputDirectories[i] + "\\cntViews.htm\"/></OBJECT><UL>\r\n");
                        bWritten = true;
                        w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\""+nodes[j].Value+"\"></param>");
                        w.Write("<param name=\"Local\" value=\""+m_OutputDirectories[i] + "\\vw_"+nodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                    }
                }
                if(bWritten)w.Write("</UL>\r\n");
				
                // add sprocs
                bWritten = false;
                nodes = xml.SelectNodes("/database/procedure[@xtype='P']/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/sproc[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        if(!bWritten)w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"Stored Procedures\"/><param name=\"Local\" value=\"" + m_OutputDirectories[i] + "\\cntSprocs.htm\"/></OBJECT><UL>\r\n");
                        bWritten = true;
                        w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\""+nodes[j].Value+"\"></param>");
                        w.Write("<param name=\"Local\" value=\""+m_OutputDirectories[i] + "\\sp_"+nodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                    }
                }
                if(bWritten)w.Write("</UL>\r\n");

                // add udfs
                bWritten = false;
                nodes = xml.SelectNodes("/database/procedure[@xtype='FN' or @xtype='IF' or @xtype='TF']/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/udf[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        if(!bWritten)w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"User Defined Functions\"/><param name=\"Local\" value=\"" + m_OutputDirectories[i] + "\\cntUDFs.htm\"/></OBJECT><UL>\r\n");
                        bWritten = true;
                        w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\""+nodes[j].Value+"\"></param>");
                        w.Write("<param name=\"Local\" value=\""+m_OutputDirectories[i] + "\\udf_"+nodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                    }
                }
                if(bWritten)w.Write("</UL>\r\n");

                // add udts
                bWritten = false;
                nodes = xml.SelectNodes("/database/userDefinedType/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/udt[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        if(!bWritten)w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"User Defined Types\"/><param name=\"Local\" value=\"" + m_OutputDirectories[i] + "\\cntUDTs.htm\"/></OBJECT><UL>\r\n");
                        bWritten = true;
                        w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\""+nodes[j].Value+"\"></param>");
                        w.Write("<param name=\"Local\" value=\""+m_OutputDirectories[i] + "\\udt_"+nodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                    }
                }
                if(bWritten)w.Write("</UL>\r\n");

				// add "all columns" page
				nodes = xml.SelectNodes("/database/table/@name");
				for(int j=0; j<nodes.Count; ++j)
				{
					// check if this object is excluded...
					XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/*[@name='"+nodes[j].Value+"']");
					if(node == null)
					{
						w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"All table columns\"/><param name=\"Local\" value=\"" + m_OutputDirectories[i] + "\\allTableColumns.htm\"/></OBJECT>\r\n");
						break;
					}
				}


				// add "all procedure params" page
				nodes = xml.SelectNodes("/database/procedure/@name");
				for(int j=0; j<nodes.Count; ++j)
				{
					// check if this object is excluded...
					XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/*[@name='"+nodes[j].Value+"']");
					if(node == null)
					{
						w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"All procedure parameters\"/><param name=\"Local\" value=\"" + m_OutputDirectories[i] + "\\allProdecureParameters.htm\"/></OBJECT>\r\n");
						break;
					}
				}

                w.Write("</UL>");
            }

            if (xmlConfigInfo.SelectNodes("/configInfo/*").Count > 0)
            {
                // write out contents for data models.
                XmlNodeList nodes = xmlConfigInfo.SelectNodes("/configInfo/DBSpecGen/models/model/@name");
                bool bWritten = false;
                for(int j=0; j<nodes.Count; ++j)
                {
                    if(!bWritten) w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"Data Models\"/><param name=\"Local\" value=\"\"/></OBJECT><UL>\r\n");
                    bWritten = true;
                    w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\""+nodes[j].Value+"\"></param>");
                    w.Write("<param name=\"Local\" value=\"" + m_OutputDirectory + "\\models\\model_"+nodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                }
                if(bWritten)w.Write("</UL>\r\n");

                // write out contents for external objects...
                if (m_ExternalObjectPaths.Count > 0)
                {
                    bool bAnyCustomNodes = xmlConfigInfo.SelectNodes("/configInfo/object").Count > 0 ? true : false;
                    if (bAnyCustomNodes)
                    {
                        w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"External Objects\"/><param name=\"Local\" value=\"\"/></OBJECT><UL>\r\n");
                        nodes = xmlConfigInfo.SelectNodes("/configInfo/DBSpecGen/definitions/object/@xtype");
                        for(int k = 0; k < nodes.Count; ++k)
                        {
                            string objType = nodes[k].Value;
                            string objPlural = xmlConfigInfo.SelectSingleNode("/configInfo/DBSpecGen/definitions/object[@xtype='"+objType+"']/@plural").Value;
                            XmlNodeList customNodes = xmlConfigInfo.SelectNodes("/configInfo/object[@xtype='"+objType+"']/@name");
                            bWritten = false;
                            for(int j=0; j<customNodes.Count; ++j)
                            {

                                if(!bWritten)w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\""+objPlural+"\"/><param name=\"Local\" value=\"ExternalObjects\\cnt"+objType.Replace(" ","").Replace(".","")+".htm\"/></OBJECT><UL>\r\n");
                                bWritten = true;
                                w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\""+customNodes[j].Value+"\"></param>");
                                w.Write("<param name=\"Local\" value=\"" + m_OutputDirectory + "\\ExternalObjects\\"+objType.Replace(" ","").Replace(".","")+"_"+customNodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                            }
                            if(bWritten)w.Write("</UL>\r\n");
                        }
                        w.Write("</UL>\r\n");
                    }
                }

                nodes = xmlConfigInfo.SelectNodes("/configInfo/DBSpecGen/customContents/item");
                for(int q = 0; q < nodes.Count; ++q)
                {
                    WriteCustomContentNode(nodes[q], w);
                }
            }
			
            
            w.Write("</UL></BODY></HTML>");
            w.Flush();
            w.Close(); 
        }

        private void WriteCustomContentNode(XmlNode node, StreamWriter w)
        {
            string name = node.Attributes.GetNamedItem("name").Value;
            string href = node.Attributes.GetNamedItem("href").Value;
            w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\""+name+"\"/><param name=\"Local\" value=\""+href+"\"/></OBJECT><UL>\r\n");
            XmlNodeList nodes = node.SelectNodes("item");
            for(int q = 0; q < nodes.Count; ++q)
            {
                WriteCustomContentNode(nodes[q], w);
            }
            w.Write("</UL>\r\n");
        }

        private void WriteHhkFile(string name)
        {
            FileStream fs = new FileStream(m_OutputDirectory + "\\" + name + ".hhk", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter w = new StreamWriter(fs);
            w.Write("<HTML><HEAD><!-- Sitemap 1.0 --></HEAD><BODY><UL>\r\n");

            XmlDocument xmlConfigInfo = GetConfigInfo();

            if (xmlConfigInfo.SelectNodes("/configInfo/*").Count > 0)
            {
                // write out contents for data models.
                XmlNodeList nodes = xmlConfigInfo.SelectNodes("/configInfo/DBSpecGen/models/model/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\""+nodes[j].Value+"\"></param>");
                    w.Write("<param name=\"Local\" value=\"" + m_OutputDirectory + "\\models\\model_"+nodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");                
                }


                // write out contents for external objects...
                if (m_ExternalObjectPaths.Count > 0)
                {
                    bool bAnyCustomNodes = xmlConfigInfo.SelectNodes("/configInfo/object").Count > 0 ? true : false;
                    if (bAnyCustomNodes)
                    {
                        nodes = xmlConfigInfo.SelectNodes("/configInfo/DBSpecGen/definitions/object/@xtype");
                        for(int k = 0; k < nodes.Count; ++k)
                        {
                            string objType = nodes[k].Value;
                            string objPlural = xmlConfigInfo.SelectSingleNode("/configInfo/DBSpecGen/definitions/object[@xtype='"+objType+"']/@plural").Value;
                            XmlNodeList customNodes = xmlConfigInfo.SelectNodes("/configInfo/object[@xtype='"+objType+"']/@name");
                            for(int j=0; j<customNodes.Count; ++j)
                            {
                                w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\""+customNodes[j].Value+"\"></param>");
                                w.Write("<param name=\"Local\" value=\"" + m_OutputDirectory + "\\ExternalObjects\\"+objType.Replace(" ","").Replace(".","")+"_"+customNodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                            }
                        }
                    }
                }
            }

            for(int i = 0; i < m_OutputDirectories.Count; ++i)
            {
                string dbName = m_DatabaseNames[i]; // m_OutputDirectories[i].Substring(m_OutputDirectories[i].LastIndexOf('.') + 1);
                string serverName = m_ServerNames[i]; //m_OutputDirectories[i].Substring(m_OutputDirectories[i].LastIndexOf('\\') + 1, m_OutputDirectories[i].LastIndexOf('.') - m_OutputDirectories[i].LastIndexOf('\\') - 1);
				
                XmlDocument xml = new XmlDocument();
                xml.Load(m_OutputDirectories[i] + "\\" + dbName + ".xml");

                XmlNodeList nodesToAdd = xmlConfigInfo.SelectNodes("/configInfo/*");
                for (int count = 0; count < nodesToAdd.Count; ++count)
                {
                    XmlNode configNode = xml.ImportNode(nodesToAdd[count], true);
                    xml.DocumentElement.AppendChild(configNode);
                }

                // add tables
                XmlNodeList nodes = xml.SelectNodes("/database/table[@xtype='U']/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/table[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"" + nodes[j].Value + "\"></param>");
                        w.Write("<param name=\"Local\" value=\""+m_OutputDirectories[i] + "\\tbl_"+nodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                    }
                }
				
                // add views
                nodes = xml.SelectNodes("/database/table[@xtype='V']/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/view[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"" + nodes[j].Value + "\"></param>");
                        w.Write("<param name=\"Local\" value=\""+m_OutputDirectories[i] + "\\vw_"+nodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                    }
                }

                // add sprocs
                nodes = xml.SelectNodes("/database/procedure[@xtype='P']/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/sproc[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"" + nodes[j].Value + "\"></param>");
                        w.Write("<param name=\"Local\" value=\""+m_OutputDirectories[i] + "\\sp_"+nodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                    }
                }

                // add udfs
                nodes = xml.SelectNodes("/database/procedure[@xtype='FN' or @xtype='IF' or @xtype='TF']/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/udf[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"" + nodes[j].Value + "\"></param>");
                        w.Write("<param name=\"Local\" value=\""+m_OutputDirectories[i] + "\\udf_"+nodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                    }
                }

                // add udts
                nodes = xml.SelectNodes("/database/userDefinedType/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/udt[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        w.Write("<LI><OBJECT type=\"text/sitemap\"><param name=\"Name\" value=\"" + nodes[j].Value + "\"></param>");
                        w.Write("<param name=\"Local\" value=\""+m_OutputDirectories[i] + "\\udt_"+nodes[j].Value.Replace(" ","").Replace(".","")+".htm\"></param></OBJECT>\r\n");
                    }
                }
            }
            w.Write("</UL></BODY></HTML>");
            w.Flush();
            w.Close();
        }

        private void WriteHhpFile(string name, string defaultTopic)
        {
            FileStream fs = new FileStream(m_OutputDirectory + "\\" + name + ".hhp", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter w = new StreamWriter(fs);
            w.Write("[OPTIONS]\r\n");
            w.Write("Binary TOC=Yes\r\n");
            w.Write("Compatibility=1.1 or later\r\n");
            w.Write("Compiled file=" + name + ".chm\r\n");
            w.Write("Contents file=" + name + ".hhc\r\n");
            w.Write("Default Window=" + name + "\r\n");
            if(defaultTopic.Length > 0) w.Write("Default topic=" + defaultTopic + "\r\n");
            w.Write("Display compile progress=Yes\r\n");
            w.Write("Full-text search=Yes\r\n");
            w.Write("Index file=" + name + ".hhk\r\n");
            w.Write("Language=0x409 English (United States)\r\n");
            w.Write("Title=" + name + "\r\n");
            w.Write("Error log file=" + name + ".log\r\n");
            w.Write("\r\n");
            w.Write("[WINDOWS]\r\n");
            w.Write(name + "=\"" + name + "\",");
            w.Write("\"" + name + ".hhc\",");
            w.Write("\"" + name + ".hhk\",");
            w.Write( (defaultTopic.Length > 0 ? "\"" + defaultTopic + "\"" : "") + ",,,,,,0x21420,,0x380e,,,,,,,,0\r\n");
            w.Write("\r\n");
            w.Write("[FILES]\r\n");
            w.Write("blank.htm\r\n");

            XmlDocument xmlConfigInfo = GetConfigInfo();

            for(int i = 0; i < m_OutputDirectories.Count; ++i)
            {
                string dbName = m_DatabaseNames[i]; // m_OutputDirectories[i].Substring(m_OutputDirectories[i].LastIndexOf('.') + 1);
                string serverName = m_ServerNames[i]; // m_OutputDirectories[i].Substring(m_OutputDirectories[i].LastIndexOf('\\') + 1, m_OutputDirectories[i].LastIndexOf('.') - m_OutputDirectories[i].LastIndexOf('\\') - 1);
                XmlDocument xml = new XmlDocument();
                string p = m_OutputDirectories[i] + "\\" + dbName + ".xml";
                xml.Load(p);

                XmlNodeList nodesToAdd = xmlConfigInfo.SelectNodes("/configInfo/*");
                for (int count = 0; count < nodesToAdd.Count; ++count)
                {
                    XmlNode configNode = xml.ImportNode(nodesToAdd[count], true);
                    xml.DocumentElement.AppendChild(configNode);
                }

                // page for the db overview...
                w.Write(m_OutputDirectories[i] + "\\default.htm\r\n");
				w.Write(m_OutputDirectories[i] + "\\allTableColumns.htm\r\n");
				w.Write(m_OutputDirectories[i] + "\\allProdecureParameters.htm\r\n");
				
                // add tables
                bool bWritten = false;
                XmlNodeList nodes = xml.SelectNodes("/database/table[@xtype='U']/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/table[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        if(!bWritten) w.Write(m_OutputDirectories[i] + "\\cntTables.htm\r\n");
                        bWritten = true;
                        w.Write(m_OutputDirectories[i] + "\\tbl_" + nodes[j].Value.Replace(" ","").Replace(".","") + ".htm\r\n");
                    }
                }

                // add views
                bWritten = false;
                nodes = xml.SelectNodes("/database/table[@xtype='V']/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/view[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        if(!bWritten) w.Write(m_OutputDirectories[i] + "\\cntViews.htm\r\n");
                        bWritten = true;
                        w.Write(m_OutputDirectories[i] + "\\vw_" + nodes[j].Value.Replace(" ","").Replace(".","") + ".htm\r\n");
                    }
                }

                // add sprocs
                bWritten = false;
                nodes = xml.SelectNodes("/database/procedure[@xtype='P']/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/sproc[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        if(!bWritten) w.Write(m_OutputDirectories[i] + "\\cntSprocs.htm\r\n");
                        bWritten = true;
                        w.Write(m_OutputDirectories[i] + "\\sp_" + nodes[j].Value.Replace(" ","").Replace(".","") + ".htm\r\n");
                    }
                }

                // add udfs
                bWritten = false;
                nodes = xml.SelectNodes("/database/procedure[@xtype='FN' or @xtype='IF' or @xtype='TF']/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/udf[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        if(!bWritten) w.Write(m_OutputDirectories[i] + "\\cntUDFs.htm\r\n");
                        bWritten = true;
                        w.Write(m_OutputDirectories[i] + "\\udf_" + nodes[j].Value.Replace(" ","").Replace(".","") + ".htm\r\n");
                    }
                }

                // add udts
                bWritten = false;
                nodes = xml.SelectNodes("/database/userDefinedType/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    // check if this object is excluded...
                    XmlNode node = xml.SelectSingleNode("/database/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+dbName+"']/udt[@name='"+nodes[j].Value+"']");
                    if(node == null)
                    {
                        if(!bWritten) w.Write(m_OutputDirectories[i] + "\\cntUDTs.htm\r\n");
                        bWritten = true;
                        w.Write(m_OutputDirectories[i] + "\\udt_" + nodes[j].Value.Replace(" ","").Replace(".","") + ".htm\r\n");
                    }
                }
            }

            if (xmlConfigInfo.SelectNodes("/configInfo/*").Count > 0)
            {
                // write out contents for data models.
                XmlNodeList nodes = xmlConfigInfo.SelectNodes("/configInfo/DBSpecGen/models/model/@name");
                for(int j=0; j<nodes.Count; ++j)
                {
                    w.Write(m_OutputDirectory + "\\models\\model_"+nodes[j].Value.Replace(" ","").Replace(".","") + ".htm\r\n");
                }

                // write out contents for external objects...
                if (m_ExternalObjectPaths.Count > 0)
                {
                    bool bAnyCustomNodes = xmlConfigInfo.SelectNodes("/configInfo/object").Count > 0 ? true : false;
                    if (bAnyCustomNodes)
                    {
                        nodes = xmlConfigInfo.SelectNodes("/configInfo/DBSpecGen/definitions/object/@xtype");
                        for(int k = 0; k < nodes.Count; ++k)
                        {
                            string objType = nodes[k].Value;
                            XmlNodeList customNodes = xmlConfigInfo.SelectNodes("/configInfo/object[@xtype='"+objType+"']/@name");
                            for(int j=0; j<customNodes.Count; ++j)
                            {
                                w.Write(m_OutputDirectory + "\\ExternalObjects\\"+objType.Replace(" ","").Replace(".","")+"_"+customNodes[j].Value.Replace(" ","").Replace(".","")+".htm\r\n");
                            }
                        }
                    }
                }
            }

            w.Write("\r\n");
            w.Write("[INFOTYPES]\r\n");
            w.Flush();
            w.Close();
        }
 
        private XmlDocument GenerateXmlFromSql(string connectionString)
        {
            if (connectionString.Length == 0)
            {
                throw new System.ArgumentException("You must pass the connectionString argument to call GenerateXmlFromSql()");
            }

            bool cancel = false;
            DBSpecGen.ShowProgress("Executing SQL query to\r\n" +  connectionString, 10, false, m_Gui, out cancel);
            if (cancel) 
            {
                DBSpecGen.ShowProgress("Operation canceled", 0, true, m_Gui, out cancel);
                return null;
            }

            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            System.Data.SqlClient.SqlConnection conn = null;
            System.Data.SqlClient.SqlCommand com = null;
            string sql = "";

			System.Resources.ResourceManager rm = new ResourceManager("DBSpecGen.strings",typeof(DBSpecGen).Assembly);
        
            try
            {
                conn=new System.Data.SqlClient.SqlConnection(connectionString);

				// first check the compat level on this db...
				sql = rm.GetString("getCompatLevel", CultureInfo.CurrentCulture);
				com = new System.Data.SqlClient.SqlCommand(sql, conn);
				com.CommandTimeout = m_timeout;
				conn.Open();
				cancel = GetXmlFromSql(com, xml);

				if (xml.SelectSingleNode("/database/@compatlevel") == null)
				{
					throw new Exception("Failed to determine the compatibility level of the database.");
				}
				
				int compatLevel = System.Convert.ToInt32(xml.SelectSingleNode("/database/@compatlevel").Value, CultureInfo.CurrentCulture);
				
                sql = rm.GetString("ginormousSqlQuery", CultureInfo.CurrentCulture);

				if (compatLevel >= 90)
				{
                    // uncomment any code that is ok to run on 
                    // compatibility level 8.0 and above.
					sql = sql.Replace("--YUKON_ONLY", "");
				}
				else if (compatLevel < 80)
				{
                    // this is not legal in CREATE TABLE syntax on SQL Server 6.5 
                    sql = sql.Replace("COLLATE Latin1_General_CI_AS", "");
				}

				//DBSpecGen.ShowProgress(sql, 80, false, m_Gui, out cancel);

				com = new System.Data.SqlClient.SqlCommand(sql, conn);
				com.CommandTimeout = m_timeout;
				cancel = GetXmlFromSql(com, xml);
            }
            catch(System.Exception e)
            {
                if(conn != null && conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                }
                throw e;
            }

            conn.Close();
            if(!cancel)
            {
                throw new System.Exception("Failed to get data from SQL server.  DatabaseSpecGenerator.GetXml() returned false.");
            }

            // need to fix up the xml before returning it - paste code chunks from 
            // procedures, triggers, and check constraints together as necessary.
            xml = FixUpXml(xml);

            // parse sql code for xml comments, rip them out
            if(m_ParseXmlComments)
            {
                xml = parseXmlComments( xml);
            }
            
            return xml;
        }

        private void DrawPieCharts(string databaseSchemaXmlFilePath, string outputPath)
        {
            if (databaseSchemaXmlFilePath == null || databaseSchemaXmlFilePath.Length == 0)
            {
                throw new ArgumentException("The databaseSchemaXmlFilePath parameter cannot be null or an empty string.", "databaseSchemaXmlFilePath");
            }

            if (outputPath == null || outputPath.Length == 0)
            {
                throw new ArgumentException("The outputPath parameter cannot be null or an empty string.", "outputPath");
            }

            XPathDocument xpathdoc = new XPathDocument(databaseSchemaXmlFilePath);

            int maxWedges = 100;
            string maps = DrawPieChart(xpathdoc, "/database/table[@xtype='U']", "datasizeKB", maxWedges, outputPath + "\\tablesizes.gif", "Largest tables by size on disk", "tbl_", outputPath);
            maps += DrawPieChart(xpathdoc, "/database/table[@xtype='U']", "indexsizeKB", maxWedges, outputPath + "\\indexsizes.gif", "Largest indexes by size on disk", "tbl_", outputPath);
            maps += DrawPieChart(xpathdoc, "/database/table[@xtype='U']", "rowcount", maxWedges, outputPath + "\\tablerows.gif", "Largest tables by row count", "tbl_", outputPath);
            maps += DrawCodeSizePieChart(xpathdoc, "/database/procedure[@xtype='P']", maxWedges, outputPath + "\\sproccomplexity.gif", "Largest stored procedures by code length", "sp_", outputPath);
            maps += DrawCodeSizePieChart(xpathdoc, "/database/procedure[@xtype='FN' or @xtype='TF' or @xtype='IF']", maxWedges, outputPath + "\\udfcomplexity.gif", "Largest user defined functions by code length", "udf_", outputPath);
            maps += DrawCodeSizePieChart(xpathdoc, "/database/table[@xtype='V']", maxWedges, outputPath + "\\viewcomplexity.gif", "Largest views by code length", "vw_", outputPath);
            m_PieChartImageMaps.Add(maps);

            // add the pie chart image maps to the appropriate files...
            if (System.IO.File.Exists(outputPath + "\\cntTables.htm"))
            {
                AppendImageMapToHtmlFile(outputPath + "\\cntTables.htm", maps);
            }
            if (System.IO.File.Exists(outputPath + "\\cntViews.htm"))
            {
                AppendImageMapToHtmlFile(outputPath + "\\cntViews.htm", maps);
            }
            if (System.IO.File.Exists(outputPath + "\\cntSprocs.htm"))
            {
                AppendImageMapToHtmlFile(outputPath + "\\cntSprocs.htm", maps);
            }
            if (System.IO.File.Exists(outputPath + "\\cntUDFs.htm"))
            {
                AppendImageMapToHtmlFile(outputPath + "\\cntUDFs.htm", maps);
            }
        }

        private void AppendImageMapToHtmlFile(string htmlFilePath, string imageMap)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(htmlFilePath);
            XmlDocumentFragment frag = xml.CreateDocumentFragment();
            frag.InnerXml = imageMap;
            xml.SelectSingleNode("/html/body").AppendChild(frag);
            xml.Save(htmlFilePath);
        }

        private void GenerateHelpGenInput(int specIndex)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(m_InternalDatabaseSchemaPaths[specIndex]);

            CopyFiles(m_WorkingDirectory + "\\misc", this.m_OutputDirectories[specIndex]);

			//bool cancel = false;
			//DBSpecGen.ShowProgress("about to call xslHelpInput.Load() with " + m_WorkingDirectory + "\\misc\\helpgeninput.xsl", 0, false, this.m_Gui, out cancel);

            // need to make the input to the helpgenner
            XslTransform xslHelpInput = new XslTransform();    
			xslHelpInput.Load(m_WorkingDirectory + "\\misc\\helpgeninput.xsl");

			//DBSpecGen.ShowProgress("finshed calling xslHelpInput.Load()", 0, false, this.m_Gui, out cancel);

            // make the help gen input xml...
            System.IO.StringWriter sw = new System.IO.StringWriter(CultureInfo.CurrentCulture);
            string helpGenInputXmlPath = this.m_OutputDirectories[specIndex] + "\\" + "HelpGenInput.xml";
		
            System.Xml.Xsl.XsltArgumentList args = new XsltArgumentList();
            args.AddParam("IsVML","", m_bIsVML ? "1" : "0"); 
            args.AddParam("ShowAllExtendedProperties","", m_bShowAllExtendedProperties ? "1" : "0"); 
            args.AddParam("ShowDbNameOnPageTitles", "", !m_bHideDBNameOnPageTitles ? "1" : "0");
            args.AddParam("MaxLabelLength","", m_MaxLabelLength.ToString(CultureInfo.CurrentCulture));
            args.AddParam("ItemsPerRow","", m_ItemsPerRow.ToString(CultureInfo.CurrentCulture));
            args.AddParam("UseMBAsSizeUnit", "", m_bUseMBAsSizeUnit ? "1" : "0");
            args.AddParam("DrawPieCharts", "", m_bDrawPieCharts ? "1" : "0");

            XmlDocument xmlConfigInfo = GetConfigInfo();
            XmlNodeList nodesToAdd = xmlConfigInfo.SelectNodes("/configInfo/*");
            for (int i = 0; i < nodesToAdd.Count; ++i)
            {
                XmlNode configNode = xml.ImportNode(nodesToAdd[i], true);
                xml.DocumentElement.AppendChild(configNode);
            }

            // this makes the html pages in one big xml file...
#if DOT_NET_VERSION_10
            xslHelpInput.Transform(xml, args, sw);
#else
            xslHelpInput.Transform(xml, args, sw, null);
#endif
            System.IO.StreamWriter streamWriter = System.IO.File.CreateText(this.m_OutputDirectories[specIndex] + "\\helpgeninput.xml");
            streamWriter.Write(sw.ToString());
            streamWriter.Close();
        }

        private void DrawDataModels()
        {
            System.IO.Directory.CreateDirectory(m_OutputDirectory + "\\models");
            XmlDocument xml = new XmlDocument();
            XmlDocument xmlConfig = new XmlDocument();
            System.IO.File.Copy(m_WorkingDirectory + "\\misc\\help.css", m_OutputDirectory + "\\models\\help.css", true);
            
            if (m_ConfigPath.Length > 0)
            {
                xmlConfig.Load(m_ConfigPath);
                if (m_DataModelXml.Length > 0)
                {
                    XmlDocumentFragment frag = xmlConfig.CreateDocumentFragment();
                    frag.InnerXml = "<models>" + m_DataModelXml + "</models>";
                    xmlConfig.DocumentElement.AppendChild(frag);
                }

            }
            else if (m_DataModelXml.Length > 0)
            {
                xmlConfig.LoadXml("<DBSpecGen><models>" + m_DataModelXml + "</models></DBSpecGen>");
            }
            else 
            {
                return;
            }
    
            // some default values for each data model diagram...
            int horizontalSpace = 75;
            int verticalSpace = 75;
            int iconsPerRow = 8;
            int maxLabelLength = 12;
            bool allowOverlap = false;
            int seed = 1;

            XmlNodeList modelNodes = xmlConfig.SelectNodes("/DBSpecGen/models/model");
            for(int i=0; i<modelNodes.Count; ++i)
            {  
                XmlNode n = modelNodes[i].SelectSingleNode("@seed");
                seed = (n == null) ? 1 : System.Convert.ToInt32(n.Value, CultureInfo.CurrentCulture);

                n = modelNodes[i].SelectSingleNode("@allowOverlap");
                int ii = (n == null) ? 0 : System.Convert.ToInt32(n.Value, CultureInfo.CurrentCulture);
                allowOverlap = (ii == 1) ? true : false;
                
                n = modelNodes[i].SelectSingleNode("@horizontalSpace");
                horizontalSpace = (n == null) ? 75 : System.Convert.ToInt32(n.Value, CultureInfo.CurrentCulture);

                n = modelNodes[i].SelectSingleNode("@verticalSpace");
                verticalSpace = (n == null) ? 75 : System.Convert.ToInt32(n.Value, CultureInfo.CurrentCulture);

                n = modelNodes[i].SelectSingleNode("@iconsPerRow");
                iconsPerRow = (n == null) ? 8 : System.Convert.ToInt32(n.Value, CultureInfo.CurrentCulture);

                n = modelNodes[i].SelectSingleNode("@maxLabelLength");
                maxLabelLength = (n == null) ? 12 : System.Convert.ToInt32(n.Value, CultureInfo.CurrentCulture);
               
                string lastLoaded = "";
                string modelName = modelNodes[i].SelectSingleNode("@name").Value;
                bool bVerbose = !m_bQuiet;
                MinimalCrossing.ConnectedGraph graph = new MinimalCrossing.ConnectedGraph(0, 0, horizontalSpace, verticalSpace, iconsPerRow, bVerbose, allowOverlap, seed);
                XmlNodeList iconNodes = modelNodes[i].SelectNodes("server/database/*");
                for(int j = 0; j < iconNodes.Count; ++j)
                {
                    string serverName   = iconNodes[j].SelectSingleNode("../../@name").Value;
                    string databaseName = iconNodes[j].SelectSingleNode("../@name").Value;
                    string objName      = iconNodes[j].SelectSingleNode("@name").Value;
                    string objType      = iconNodes[j].Name;
                    string objColor		= "";
                    string objPrefix	= "";
                    string objxclause   = "";
                    string objxclause2  = "";
                    string objNodeName  = "";
                    bool isExternalObject = false;
                    switch(objType) 
                    {	
                        case "table":
                            objNodeName = "table";
                            objColor = "blue";
                            objPrefix = "tbl_";
                            objxclause  = " @xtype='U' ";
                            objxclause2 = " @refXtype='U' ";
                            break;
                        case "view":
                            objNodeName = "table";
                            objColor = "green";
                            objPrefix = "vw_";
                            objxclause  = " @xtype='V' ";
                            objxclause2 = " @refXtype='V' ";
                            break;
                        case "sproc":
                            objNodeName = "procedure";
                            objColor = "red";
                            objPrefix = "sp_";
                            objxclause  = " @xtype='P' ";
                            objxclause2 = " @refXtype='P' ";
                            break;
                        case "udf":
                            objNodeName = "procedure";
                            objColor = "black";
                            objPrefix = "udf_";
                            objxclause = " @xtype='FN' or @xtype='IF' or @xtype='TF' ";
                            objxclause2 = " @refXtype='FN' or @refXtype='IF' or @refXtype='TF' ";
                            break;
                        default:
							objxclause = " 666=666 ";
							objxclause2 = " 666=666 ";
                            objNodeName = "object";
                            isExternalObject = true;
                            break;
                    }

                    // see if we need to load the xml for this database...
                    if(lastLoaded != serverName + "." + databaseName)
                    {
                        lastLoaded = serverName + "." + databaseName;

                        // paste the config stuff into the xml for the current db...
                        xml.Load(m_OutputDirectory + "\\" + lastLoaded + "\\" + databaseName + ".xml");
                        XmlNode configNode = xml.ImportNode(xmlConfig.SelectSingleNode("/DBSpecGen"), true);
                        xml.DocumentElement.AppendChild(configNode);

                        // paste in any dependencies from our custom objects that we may have...
                        for (int k = 0; k < m_ExternalObjectPaths.Count; ++k)
                        {
                            if (!System.IO.File.Exists(m_ExternalObjectPaths[k]))
                            {
                                continue;
                            }
                            XmlDocument xmlCustom = new XmlDocument();
                            xmlCustom.Load(m_ExternalObjectPaths[k]);
                            XmlNodeList nodes = xmlCustom.SelectNodes("/customObjects/dependency | /customObjects/object");
                            for (int m = 0; m < nodes.Count; ++m)
                            {
                                XmlNode dep = xml.ImportNode(nodes[m], true);
                                xml.DocumentElement.AppendChild(dep);
                            }
                        }
                    }
			
                    // make sure this object is actually present...
                    if (null != xml.SelectSingleNode("/database/" + objNodeName + "[" + objxclause+" and @name='" + objName + "']"))
                    {
                        // now get the dependencies of this object, 
                        XmlNodeList dependencies = null;
                        if(isExternalObject)
                        {
                            dependencies = xml.SelectNodes("/database/dependency[@dependsOnObj='"+objName+"']/@objName");
                        }
                        else
                        {
                            dependencies = xml.SelectNodes("/database/dependency[@dependsOnObj='"+objName+"' and "+objxclause2+"]/@objName");
                        }

                        // get the tables linked to via pk/fk links
                        XmlNodeList foreignKeys = null;
                        if(!isExternalObject)
                        {
                            foreignKeys = xml.SelectNodes("/database/table[@name='"+objName+"' and "+objxclause+"]/constraint[@isForeignKey=1]/@refTable");
                        }

                        // put each one that it depends on into an array...
                        int foreignKeyCount = (foreignKeys != null) ? foreignKeys.Count : 0;
                        int dependencyCount = (dependencies != null) ? dependencies.Count : 0;
                        string[] linkTo = new string[dependencyCount + foreignKeyCount];
                        for(int k=0; k < dependencyCount; ++k)
                        {
                            linkTo[k] = dependencies[k].Value;
                        }
                        for(int k=0; k < foreignKeyCount; ++k)
                        {
                            linkTo[k + dependencyCount] = foreignKeys[k].Value;
                        }

                        int numNonNulls = 0;
                        for(int q=0; q<linkTo.Length; ++q)
                        {
                            XmlNode node = xmlConfig.SelectSingleNode("/DBSpecGen/models/model[@name='"+modelName+"']/server/database/*[@name='"+linkTo[q]+"']");
                            if (node == null)
                            {
                                linkTo[q] = null;
                                continue;
                            }
                            ++numNonNulls;
                        }

                        string[] to = new string[numNonNulls];
                        int qq = 0;
                        for(int q=0; q<linkTo.Length; ++q)
                        {
                            if(linkTo[q] == null) continue;
                            to[qq] = linkTo[q];
                            ++qq;
                        }

                        if(to.Length == 0) 
                        {
                            to = null;
                        }
					
                        string url = "../" + lastLoaded + "/" + objPrefix + objName.Replace(" ","").Replace(".","") + ".htm";
					
                        // put the objects the current object links to in it's title.
                        string title = objName;
                        if(to != null) 
                        {
                            bool bTo = false;
                            Array.Sort(to);
                            for(int k = 0; k < to.Length; ++k)
                            {
                                if (to[k] == null) continue;
                                if (!bTo) 
                                {
                                    title += " to ";
                                    bTo = true;
                                }
                                title += "\r\n  " + to[k];
                                if (k < to.Length - 1) title += ", ";
                            }
                        }

                        // need to get the objColor if it's a custom object...
                        if(isExternalObject)
                        {
                            if (xml.SelectSingleNode("/database/object[@name='"+objName+"']/@xtype") == null)
                            {
                                throw new Exception("The xtype attribute was not found for the external object '" + objName + "'.");
                            }

                            string xtype = xml.SelectSingleNode("/database/object[@name='"+objName+"']/@xtype").Value;
                            url = "../ExternalObjects/" + xtype + "_" + objName.Replace(" ","").Replace(".","") + ".htm";
                            objColor = xml.SelectSingleNode("/database/DBSpecGen/definitions/object[@xtype='"+xtype+"']/@color") != null ? 
                                xml.SelectSingleNode("/database/DBSpecGen/definitions/object[@xtype='"+xtype+"']/@color").Value : "blask";
                        }

                        MinimalCrossing.Icon icon = new MinimalCrossing.Icon(
                            objName,
                            title,
                            url,
                            objColor,
                            20,
                            500,
                            500,
                            maxLabelLength,
                            to);
                        graph.AddIcon(icon);
                    }
                }

                graph.PlaceIcons(this.Gui);

                string s = "<html xmlns:v='urn:schemas-microsoft-com:vml'><head><link rel='stylesheet' href='help.css'/><title>" + modelName + "</title></head>";
                s+="<body id='bodyID' class='dtBODY'><div id='nsbanner'><div id='bannerrow1'>";
                s+="<table class='bannerparthead' cellspacing='0' id='Table1'>";
                s+="<tr id='hdr'><td class='runninghead'>Database Reference</td>";
                s+="<td class='product'/></tr></table></div><div id='TitleRow'><h1 class='dtH1'>" + modelName + "</h1></div></div>";
                s+=graph.GenerateHtmlString() + "</body></html>";
                System.IO.StreamWriter sw = System.IO.File.CreateText(m_OutputDirectory + "\\models\\model_" + modelName.Replace(" ","").Replace(".","") + ".htm");
                sw.Write(s);
                sw.Close();
            }
        }
        private void DrawExternalObjects()
        {
            // put all external objects in a big blob...
            XmlDocument xml = new XmlDocument();
            XmlDocument xmlConfig = new XmlDocument();
            xml.LoadXml("<database/>");
            xmlConfig.Load(m_ConfigPath);
            XmlNode configNode = xml.ImportNode(xmlConfig.SelectSingleNode("/DBSpecGen"), true);
            xml.DocumentElement.AppendChild(configNode);


            // paste in any dependencies from our custom objects that we may have...
            for (int k = 0; k < m_ExternalObjectPaths.Count; ++k)
            {
                if (!System.IO.File.Exists(m_ExternalObjectPaths[k]))
                {
                    continue;
                }
                XmlDocument xmlCustom = new XmlDocument();
                xmlCustom.Load(m_ExternalObjectPaths[k]);
                XmlNodeList nodes = xmlCustom.SelectNodes("/customObjects/dependency | /customObjects/object");
                for (int m = 0; m < nodes.Count; ++m)
                {
                    XmlNode dep = xml.ImportNode(nodes[m], true);
                    xml.DocumentElement.AppendChild(dep);
                }
            }
            

            // need to make the input to the helpgenner
            XslTransform xslHelpGenInput = new XslTransform();
            xslHelpGenInput.Load(m_WorkingDirectory + "\\misc\\HelpGenInput.xsl");

            // make the help gen input xml...
            System.IO.StringWriter sw = new System.IO.StringWriter(CultureInfo.CurrentCulture);

            System.Xml.Xsl.XsltArgumentList args = new XsltArgumentList();
            if(m_bIsVML)
            {
                args.AddParam("IsVML","","1"); 
            }
            else
            {
                args.AddParam("IsVML","","0"); 
            }
				
            // this makes the html pages in one big xml file...
            string helpGenInputFile = m_OutputDirectory + "\\ExternalObjects.xml";
            
#if DOT_NET_VERSION_10
            xslHelpGenInput.Transform(xml, args, sw);
#else
            xslHelpGenInput.Transform(xml, args, sw, null);
#endif
            System.IO.StreamWriter streamWriter = System.IO.File.CreateText(helpGenInputFile);
            streamWriter.Write(sw.ToString());
            streamWriter.Close();

            // need a place to put the external objects...
            System.IO.Directory.CreateDirectory(m_OutputDirectory + "\\ExternalObjects");
			
            // convert this using help generator...
            HelpGen.HelpGenerator hg = new HelpGen.HelpGenerator();
            hg.PathHelpMe = helpGenInputFile;
            hg.PathOut = m_OutputDirectory + "\\ExternalObjects";
            hg.PathPagesXsl = m_WorkingDirectory + "\\misc\\fancypages.xsl";
            hg.PathTreeXsl = m_WorkingDirectory + "\\misc\\fancytree.xsl";
            hg.IsHtmlOutput = false;
            hg.Generate();

            System.IO.File.Delete(helpGenInputFile);

            // copy some files we need...
            CopyFiles(m_WorkingDirectory + "\\misc", m_OutputDirectory + "\\ExternalObjects");
        }
        private void GenerateChmFile()
        {
            string chmName = "";
            string defaultTopic = "blank.htm";
            if(m_OutputDirectories.Count == 1)
            {
                chmName = m_ChmName.Length == 0 ? m_OutputDirectories[0].Substring(m_OutputDirectories[0].LastIndexOf('.') + 1) : m_ChmName;
            }
            else
            {
                chmName = m_ChmName.Length == 0 ? "dbspec" : m_ChmName;
            }

            defaultTopic = m_OutputDirectories[0] + "\\default.htm";
            if(!System.IO.File.Exists(defaultTopic))
            {
                defaultTopic = "blank.htm";
            }

            System.IO.File.Copy(m_WorkingDirectory + "\\misc\\blank.htm", m_OutputDirectory + "\\blank.htm", true);
				
            WriteHhpFile(chmName, defaultTopic);
            WriteHhkFile(chmName);
            WriteHhcFile(chmName);

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = m_WorkingDirectory + "\\misc\\hhc.exe";
            p.StartInfo.Arguments = "\"" + chmName + ".hhp\"";
            p.StartInfo.WorkingDirectory = m_OutputDirectory;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            this.m_ChmFilePath = m_OutputDirectory + "\\" + chmName + ".chm";
            if (!System.IO.File.Exists(this.m_ChmFilePath))
            {
                this.m_ChmFilePath = "";
            }
        }

        private bool GenerateHtmlFiles(int specIndex)
        {
            bool cancel = false;
            
            // transform into the helpgeninput.xml file...
            DBSpecGen.ShowProgress("Transforming xml for " + m_DatabaseNames[specIndex] + " into html documentation (stage 1)...", 40, false, m_Gui, out cancel);
            if (cancel) 
            {
                DBSpecGen.ShowProgress("Operation canceled", 0, true, m_Gui, out cancel);
                return false;
            }
            GenerateHelpGenInput(specIndex);

            // turn the helpgeninput.xml file into html...
            DBSpecGen.ShowProgress("Transforming xml for " +  m_DatabaseNames[specIndex] + " into html documentation (stage 2)...", 50, false, m_Gui, out cancel);
            if (cancel) 
            {
                DBSpecGen.ShowProgress("Operation canceled", 0, true, m_Gui, out cancel);
                return false;
            }
            HelpGen.HelpGenerator hg = new HelpGen.HelpGenerator();
            hg.PathPagesXsl = m_WorkingDirectory + "\\misc\\fancypages.xsl";
            hg.PathTreeXsl  = m_WorkingDirectory + "\\misc\\fancytree.xsl";
            hg.IsHtmlOutput = false;
            hg.PathHelpMe = m_OutputDirectories[specIndex] + "\\helpgeninput.xml";
            hg.PathOut = m_OutputDirectories[specIndex];
            hg.Generate();

            // delete helpgeninput.xml, we don't need it anymore. 
            System.IO.File.Delete(hg.PathHelpMe);

            DBSpecGen.ShowProgress("Finished generating htm files for " + m_DatabaseNames[specIndex], 80, false, m_Gui, out cancel);
            if (cancel) 
            {
                DBSpecGen.ShowProgress("Operation canceled", 0, true, m_Gui, out cancel);
                return false;
            }

            if (m_bDrawPieCharts)
            {
                // draw some piecharts...
                DBSpecGen.ShowProgress("Drawing pie charts...", 25, false, m_Gui, out cancel);
                if (cancel) 
                {
                    DBSpecGen.ShowProgress("Operation canceled", 0, true, m_Gui, out cancel);
                    return false;
                }
                DrawPieCharts(m_InternalDatabaseSchemaPaths[specIndex], m_OutputDirectories[specIndex]);
            }

            return true;
        }


        #endregion

        #region public methods

        public void AddConnectionString(string connection)
        {
            if (connection == null || connection.Length == 0)
            {
                throw new ArgumentException("The connection parameter must be a string with length greater than 0.","connection");
            }
            m_ConnectionStrings.Add(connection);
        }

        public void AddDatabaseSchemaPath(string databaseSchemaPath)
        {
            if (databaseSchemaPath == null || databaseSchemaPath.Length == 0)
            {
                throw new ArgumentException("The databaseSchemaPath parameter must be a string with length greater than 0.", "databaseSchemaPath");
            }
            m_DatabaseSchemaPaths.Add(databaseSchemaPath);
        }

        public void AddFakeDatabaseName(string fakeDatabaseName)
        {
            if (fakeDatabaseName == null || fakeDatabaseName.Length == 0)
            {
                throw new ArgumentException("The fakeDatabaseName parameter must be a string with length greater than 0.", "fakeDatabaseName");
            }
            m_FakeDatabaseNames.Add(fakeDatabaseName);
        }

        public void AddFakeServerName(string fakeServerName)
        {
            if (fakeServerName == null || fakeServerName.Length == 0)
            {
                throw new ArgumentException("The fakeServerName parameter must be a string with length greater than 0.", "fakeServerName");
            }
            m_FakeServerNames.Add(fakeServerName);
        }

        public void AddExternalObjectPath(string path)
        {
            if (path == null || path.Length == 0)
            {
                throw new ArgumentException("The path parameter must be a string with length greater than 0.", "path");
            }
            m_ExternalObjectPaths.Add(path);
        }

        public void SetExcludedObjectsXml(string excluded)
        {
            m_xmlExcludedObjects.LoadXml(excluded);
        }
        
        public void GenerateSpec()
        {
            if (!(m_DatabaseSchemaPaths.Count > 0 || m_ConnectionStrings.Count > 0))
            {
                throw new Exception("You must set either the ConnectionStrings property or the DatabaseSchemaPaths property before calling GenerateSpec");
            }

            bool cancel = false;
            XmlDocument xml = new XmlDocument();

            // 
            // are we generating specs from xml that we generated previously?
            //
			if (m_DatabaseSchemaPaths.Count > 0)
			{
				for(int i = 0; i < m_DatabaseSchemaPaths.Count; ++i)
				{
					xml.Load(m_DatabaseSchemaPaths[i]);

					m_DatabaseNames.Add(xml.SelectSingleNode("/database/@name").Value);
					m_ServerNames.Add(xml.SelectSingleNode("/database/@server").Value);
					m_OutputDirectories.Add(m_OutputDirectory + "\\" + m_ServerNames[i] + "." + m_DatabaseNames[i]);
					m_InternalDatabaseSchemaPaths.Add(m_OutputDirectories[i] + "\\" + m_DatabaseNames[i] + ".xml");

					System.IO.Directory.CreateDirectory(m_OutputDirectories[i]);
					xml.Save(m_InternalDatabaseSchemaPaths[i]);
					GenerateHtmlFiles(i);
				}
			}

			// 
			// are we generating specs from sql queries?
			//
			else if (m_ConnectionStrings.Count > 0)
			{
				for(int i = 0; i < m_ConnectionStrings.Count; ++i)
				{
					xml = this.GenerateXmlFromSql(m_ConnectionStrings[i]);

					// do we have want to replace the names with fake database and server names?
					if (m_FakeServerNames != null && 
						m_FakeServerNames.Count == m_ConnectionStrings.Count &&
						m_FakeServerNames[i].Length > 0)
					{
						xml.DocumentElement.SetAttribute("server", m_FakeServerNames[i]);
					}

					if (m_FakeDatabaseNames != null && 
						m_FakeDatabaseNames.Count == m_ConnectionStrings.Count &&
						m_FakeDatabaseNames[i].Length > 0)
					{
						xml.DocumentElement.SetAttribute("name", m_FakeDatabaseNames[i]);
					}

					m_DatabaseNames.Add(xml.SelectSingleNode("/database/@name").Value);
					m_ServerNames.Add(xml.SelectSingleNode("/database/@server").Value);
					m_OutputDirectories.Add(m_OutputDirectory + "\\" + m_ServerNames[i] + "." + m_DatabaseNames[i]);
					m_InternalDatabaseSchemaPaths.Add(m_OutputDirectories[i] + "\\" + m_DatabaseNames[i] + ".xml");

					System.IO.Directory.CreateDirectory(m_OutputDirectories[i]);
					xml.Save(m_InternalDatabaseSchemaPaths[i]);
                    
					if (m_GenerateXmlOnly)
					{
						continue;
					}

					GenerateHtmlFiles(i);
				}
			}

			if (m_GenerateXmlOnly)
			{
				DBSpecGen.ShowProgress("Finished.  Your output is here: " + m_OutputDirectory, 100, true, m_Gui, out cancel);
				return;
			}

			// generate html for external objects...
			if (m_ExternalObjectPaths.Count > 0)
			{
				DBSpecGen.ShowProgress("Generating html for external objects...", 75, false, m_Gui, out cancel);
				if (cancel) 
				{
					DBSpecGen.ShowProgress("Operation canceled", 0, true, m_Gui, out cancel);
					return;
				}
				DrawExternalObjects();
			}

			// generate html for data models...
			if (m_DataModelXml.Length > 0 || m_ConfigPath.Length > 0)
			{
				DBSpecGen.ShowProgress("Generating html for data models...", 85, false, m_Gui, out cancel);
				if (cancel) 
				{
					DBSpecGen.ShowProgress("Operation canceled", 0, true, m_Gui, out cancel);
					return;
				}
				DrawDataModels();
			}

			// now compile the chm...
			DBSpecGen.ShowProgress("Compiling chm file...", 90, false, m_Gui, out cancel);
			if (cancel) 
			{
				DBSpecGen.ShowProgress("Operation canceled", 0, true, m_Gui, out cancel);
				return;
			}
			GenerateChmFile();

			DBSpecGen.ShowProgress("Finished.  Your output is here:\r\n" + m_ChmFilePath, 100, true, m_Gui, out cancel);
        }
        
        #endregion
    }

}
