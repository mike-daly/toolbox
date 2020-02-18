using System;
using System.Xml;
using System.Xml.Xsl;
using System.Globalization;


namespace HelpGen
{
	/// <summary>
	/// Summary description for HelpGenerator.
	/// </summary>
	public class HelpGenerator
	{
		public HelpGenerator()
		{
			this.xmlHelpMe = new XmlDocument();
			this.xslPages = new XslTransform();
			this.xslTree = new XslTransform();
			this.isInitialized = false;
		}

		// this is the big xml file that gets 
		// transformed into html documentation
		private XmlDocument xmlHelpMe;

		// the xsl that operates on xmlHelpMe to 
		// create the html tree navigation pane
		private XslTransform xslTree;

		// the xsl that creates all the documentation 
		// pages from xmlHelpMe
		private XslTransform xslPages;
		private bool isInitialized = false;

		// path to xmlHelpMe
		private string _PathHelpMe = "";

		// where to put the output.
		private string _PathOut = "";

		// path to xslTree
		private string _PathTreeXsl = "";

		// path to xslPages
		private string _PathPagesXsl = "";

		// if false, output is .chm, otherwise it's html.
        private bool _IsHtmlOutput = false;


		public bool IsHtmlOutput
		{
			get 
			{
				return _IsHtmlOutput; 
			}
			set 
			{
				_IsHtmlOutput = value; 
			}
		}

		public string PathHelpMe
		{
			get
			{
				return _PathHelpMe;
			}
			set
			{
				_PathHelpMe = value;
			}
		}

		public string PathPagesXsl
		{
			get
			{
				return _PathPagesXsl;
			}
			set
			{
				_PathPagesXsl = value;
			}
		}

		public string PathTreeXsl
		{
			get
			{
				return _PathTreeXsl;
			}
			set
			{
				_PathTreeXsl = value;
			}
		}

		public string PathOut
		{
			get
			{
				return _PathOut;
			}
			set
			{
				_PathOut = value;
			}
		}
		

		private bool init()
		{
			this.xmlHelpMe.Load(this.PathHelpMe);
			this.xslTree.Load(this.PathTreeXsl);
			this.xslPages.Load(this.PathPagesXsl);
			this.isInitialized = true;
			return true;
		}

		public bool Generate()
		{
			if(!this.isInitialized)
			{
				if(!System.IO.File.Exists(PathHelpMe))
				{
					throw new Exception("Did not find the input xml file.  passed was: " + PathHelpMe);
				}
				if(!System.IO.File.Exists(PathTreeXsl))
				{
                    throw new Exception("Did not find the tree xsl file.  passed was: " + PathTreeXsl);
				}
				if(!System.IO.File.Exists(PathPagesXsl))
				{
					throw new Exception("Did not find the pages xsl file.  passed was: " + PathPagesXsl);
				}
				init();
			}
			
			
			System.IO.StringWriter stringWriter = new System.IO.StringWriter(CultureInfo.CurrentCulture);
			System.Xml.XmlDocument xmlPages = new System.Xml.XmlDocument();
			System.Xml.XmlNodeList pageNodes;
			
			string fileName = "";
			string title="";
			
			System.Xml.Xsl.XsltArgumentList args = new XsltArgumentList();
            if(this.IsHtmlOutput)
            {
                // this makes the tree...
#if (DOT_NET_VERSION_10)
                xslTree.Transform(this.PathHelpMe, this.PathOut + "\\tree.htm");
#else
                xslTree.Transform(this.PathHelpMe, this.PathOut + "\\tree.htm", null);
#endif

                args.AddParam("ShowSyncTree","","true"); 
            }
            else
            {
                args.AddParam("ShowSyncTree","","false");
            }

            // this makes the html pages in one big xml file...
#if (DOT_NET_VERSION_10)
            xslPages.Transform(this.xmlHelpMe, args, stringWriter);
#else
            xslPages.Transform(this.xmlHelpMe, args, stringWriter, null);
#endif
            
			// save a page for each page in the pages.xml...
			xmlPages.LoadXml(stringWriter.ToString());
			pageNodes = xmlPages.SelectNodes("/root/page");

			// for the index (only used if IsHtmlOutput is true)
			string innerText = "";
			System.Xml.XmlDocument xmlIndex = new System.Xml.XmlDocument();
			System.Xml.XmlElement elem;
			System.Xml.XmlCDataSection cdata;
			xmlIndex.LoadXml("<root/>");
			for(int i=0;i<pageNodes.Count;i++)
			{
				// make and save an htm for each page...
				fileName = pageNodes[i].SelectSingleNode("@name").Value;
				title = pageNodes[i].SelectSingleNode("@title").Value;
				System.IO.StreamWriter streamWriter = System.IO.File.CreateText(this.PathOut + "\\" + fileName);
				streamWriter.Write("<html xmlns:v=\"urn:schemas-microsoft-com:vml\">" + pageNodes[i].SelectSingleNode("html").InnerXml + "</html>");
				streamWriter.Close();

                if(this.IsHtmlOutput)
                {
                    // have to do this replace cuz we don't want 
                    // their ]]> (if one is present) to break our cdata section...
                    innerText = pageNodes[i].InnerText.Replace("]]>","]]"); 
                    elem = xmlIndex.CreateElement("page");
                    cdata = xmlIndex.CreateCDataSection(innerText);
                    elem.SetAttribute("title",title);
                    elem.SetAttribute("name",fileName);
                    elem.AppendChild(cdata);

                    // add this page to the index...
                    xmlIndex.DocumentElement.AppendChild(elem);
                }
            }	
            if(this.IsHtmlOutput)
            {
                xmlIndex.Save(this.PathOut + "\\indexText.xml");
            }
            
            return true;
		}
	}
}
