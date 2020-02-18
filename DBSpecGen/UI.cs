using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Resources;
using System.Globalization;

// /w ..\.. /c server=(local);database=pubs;trusted_connection=yes:server=(local);database=northwind;trusted_connection=yes /d ..\..\BVT\SPRING\PubsNorthwind.xml
// /w ..\.. /c server=(local);database=pubs;trusted_connection=yes 
// /w ..\.. /c server=(local);database=pubs;trusted_connection=yes /e ..\..\bvt\spring\externalObjects.xml /d ..\..\BVT\SPRING\extObjConfig.xml
// /w ..\.. /m 0 /n Multiple /x 1 /d ..\..\BVT\SPRING\multipleconfig.xml /c server=(local);database=CentralAccounts8080;trusted_connection=yes:server=(local);database=SalesLead;trusted_connection=yes:server=(local);database=CentralAccounts;trusted_connection=yes

// junk to make FxCop happy...
[assembly: AssemblyTitle("Database Spec Generator")]
[assembly: AssemblyDescription("DBSpecGen is a tool for generating documentation for any SQL Server 2000 or Yukon database.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyProduct("Database Spec Generator")]
[assembly: AssemblyCopyright("© Microsoft Corporation. All rights reserved.")]
[assembly: AssemblyTrademark("© Microsoft Corporation. All rights reserved.")]
[assembly: AssemblyCulture("")]		
[assembly:AssemblyVersionAttribute("1.7.6")]
[assembly:CLSCompliant(true)]
[assembly:ComVisible(false)]
[assembly: AssemblyDelaySign(false)]
[assembly:AssemblyKeyFileAttribute("dbspecgen.snk")]
//[assembly:SecurityPermission(SecurityAction.RequestMinimum, Execution=true)]


namespace DBSpecGen
{



	public class DBSpecGen : System.Windows.Forms.Form
	{
        #region private fields...
        private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox checkBoxXMLComments;
        private System.Windows.Forms.CheckBox checkBoxVML;
		private System.Windows.Forms.Button buttonBrowseForOutputDir;
        private System.Windows.Forms.Button buttonBrowseForConfigFile;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonModels;
        private System.Windows.Forms.Button buttonObjectsToDocument;
        private System.Windows.Forms.Button buttonViewChm;
		private System.Windows.Forms.TextBox textBoxChmName;
        private System.Windows.Forms.TextBox textBoxConfigPath;
        private System.Windows.Forms.TextBox textBoxOutputDir;
        private System.Windows.Forms.TextBox textBoxConnectionString;
        private System.Windows.Forms.TextBox textBoxTimeout;
		private System.Windows.Forms.Label labelChmName;
		private System.Windows.Forms.Label labelConfigPath;
		private System.Windows.Forms.Label labelTimeout;
		private System.Windows.Forms.Label labelOptions;
        private System.Windows.Forms.Label labelSaveOutputPath;
        private System.Windows.Forms.Label labelConnectionString;
        private System.Windows.Forms.Label labelProgress; 
        private System.Windows.Forms.Label labelItemsPerRow;
        private System.Windows.Forms.Label labelMaxLabelLength;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox textBoxItemsPerRow;
        private System.Windows.Forms.TextBox textBoxMaxLabelLength;
        private System.Windows.Forms.RichTextBox richTextBoxProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        

        static private int      m_timeout = 60;
        static private int      m_ItemsPerRow = 6;
        static private int      m_MaxLabelLength = 12;

        static private bool     m_bParseXmlComments = false;
        static private bool     m_bQuiet = false;
        static private bool		m_bUI = false;
        static private bool		m_bIsVML = true;
        static private bool		m_bCreateXmlOnly = false;
        static private bool     m_bDiagramsOnly = false;
        static private bool		m_bShowAllExtendedProperties = false;
        static private bool     m_bShowDbNameOnPageTitles = true;
        static private bool     m_bObjectsChosen = false;
        static private bool     m_bUseMBAsSizeUnit = false;
        static private bool     m_bDrawPieCharts = true;

		static private string   m_workingDir = System.IO.Directory.GetCurrentDirectory();
		static private string   m_outputDir = m_workingDir + "\\output";
        static private string   m_ConfigPath = "";
        static private string   m_version = "Database Spec Generator v.1.7.6";
        static private string	m_ChmName = "";
		static private string   m_PathToChm = "";
        static private string   m_DataModels = "";
	
        static private string[] m_rgConnString = {""};
		static private string[] m_rgXmlFromSqlPaths = {""};
		static private string[] m_rgExternalObjectPaths = {""};
		static private string[] m_rgFakeServerNames = {""};
		static private string[] m_rgFakeDatabaseNames = {""};
        

        static private DatabaseObjectList m_IncludeExcludeObjects = new DatabaseObjectList();
	
        #endregion
        
        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.labelConnectionString = new System.Windows.Forms.Label();
			this.textBoxConnectionString = new System.Windows.Forms.TextBox();
			this.buttonGo = new System.Windows.Forms.Button();
			this.textBoxTimeout = new System.Windows.Forms.TextBox();
			this.labelTimeout = new System.Windows.Forms.Label();
			this.checkBoxXMLComments = new System.Windows.Forms.CheckBox();
			this.textBoxOutputDir = new System.Windows.Forms.TextBox();
			this.labelSaveOutputPath = new System.Windows.Forms.Label();
			this.buttonBrowseForOutputDir = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.textBoxChmName = new System.Windows.Forms.TextBox();
			this.labelChmName = new System.Windows.Forms.Label();
			this.checkBoxVML = new System.Windows.Forms.CheckBox();
			this.textBoxConfigPath = new System.Windows.Forms.TextBox();
			this.buttonBrowseForConfigFile = new System.Windows.Forms.Button();
			this.labelConfigPath = new System.Windows.Forms.Label();
			this.labelOptions = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.richTextBoxProgress = new System.Windows.Forms.RichTextBox();
			this.labelProgress = new System.Windows.Forms.Label();
			this.labelItemsPerRow = new System.Windows.Forms.Label();
			this.textBoxItemsPerRow = new System.Windows.Forms.TextBox();
			this.labelMaxLabelLength = new System.Windows.Forms.Label();
			this.textBoxMaxLabelLength = new System.Windows.Forms.TextBox();
			this.buttonObjectsToDocument = new System.Windows.Forms.Button();
			this.buttonViewChm = new System.Windows.Forms.Button();
			this.buttonModels = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelConnectionString
			// 
			this.labelConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelConnectionString.Location = new System.Drawing.Point(8, 8);
			this.labelConnectionString.Name = "labelConnectionString";
			this.labelConnectionString.Size = new System.Drawing.Size(512, 16);
			this.labelConnectionString.TabIndex = 0;
			this.labelConnectionString.Text = "Enter the connection string(s) to your database(s) below (colon-separated list):";
			// 
			// textBoxConnectionString
			// 
			this.textBoxConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxConnectionString.Location = new System.Drawing.Point(8, 24);
			this.textBoxConnectionString.Name = "textBoxConnectionString";
			this.textBoxConnectionString.Size = new System.Drawing.Size(512, 20);
			this.textBoxConnectionString.TabIndex = 1;
			this.textBoxConnectionString.Text = "server = (local) ; database = pubs ; Trusted_Connection = yes ;";
			// 
			// buttonGo
			// 
			this.buttonGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonGo.Location = new System.Drawing.Point(440, 248);
			this.buttonGo.Name = "buttonGo";
			this.buttonGo.Size = new System.Drawing.Size(80, 23);
			this.buttonGo.TabIndex = 2;
			this.buttonGo.Text = "Go!";
			this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
			// 
			// textBoxTimeout
			// 
			this.textBoxTimeout.Location = new System.Drawing.Point(128, 208);
			this.textBoxTimeout.Name = "textBoxTimeout";
			this.textBoxTimeout.Size = new System.Drawing.Size(84, 20);
			this.textBoxTimeout.TabIndex = 8;
			this.textBoxTimeout.Text = "60";
			// 
			// labelTimeout
			// 
			this.labelTimeout.Location = new System.Drawing.Point(128, 192);
			this.labelTimeout.Name = "labelTimeout";
			this.labelTimeout.Size = new System.Drawing.Size(100, 16);
			this.labelTimeout.TabIndex = 9;
			this.labelTimeout.Text = "timeout (seconds)";
			// 
			// checkBoxXMLComments
			// 
			this.checkBoxXMLComments.Location = new System.Drawing.Point(16, 192);
			this.checkBoxXMLComments.Name = "checkBoxXMLComments";
			this.checkBoxXMLComments.Size = new System.Drawing.Size(104, 16);
			this.checkBoxXMLComments.TabIndex = 10;
			this.checkBoxXMLComments.Text = "XML comments";
			// 
			// textBoxOutputDir
			// 
			this.textBoxOutputDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxOutputDir.Location = new System.Drawing.Point(40, 72);
			this.textBoxOutputDir.Name = "textBoxOutputDir";
			this.textBoxOutputDir.Size = new System.Drawing.Size(480, 20);
			this.textBoxOutputDir.TabIndex = 11;
			this.textBoxOutputDir.Text = "";
			// 
			// labelSaveOutputPath
			// 
			this.labelSaveOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelSaveOutputPath.Location = new System.Drawing.Point(8, 56);
			this.labelSaveOutputPath.Name = "labelSaveOutputPath";
			this.labelSaveOutputPath.Size = new System.Drawing.Size(512, 16);
			this.labelSaveOutputPath.TabIndex = 12;
			this.labelSaveOutputPath.Text = "Enter path to save output:";
			// 
			// buttonBrowseForOutputDir
			// 
			this.buttonBrowseForOutputDir.Location = new System.Drawing.Point(8, 72);
			this.buttonBrowseForOutputDir.Name = "buttonBrowseForOutputDir";
			this.buttonBrowseForOutputDir.Size = new System.Drawing.Size(24, 23);
			this.buttonBrowseForOutputDir.TabIndex = 13;
			this.buttonBrowseForOutputDir.Text = "...";
			this.buttonBrowseForOutputDir.Click += new System.EventHandler(this.buttonBrowseForOutputDir_Click);
			// 
			// buttonHelp
			// 
			this.buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonHelp.Location = new System.Drawing.Point(440, 168);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(80, 23);
			this.buttonHelp.TabIndex = 14;
			this.buttonHelp.Text = "Help";
			this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
			// 
			// textBoxChmName
			// 
			this.textBoxChmName.Location = new System.Drawing.Point(128, 168);
			this.textBoxChmName.Name = "textBoxChmName";
			this.textBoxChmName.Size = new System.Drawing.Size(84, 20);
			this.textBoxChmName.TabIndex = 17;
			this.textBoxChmName.Text = "";
			// 
			// labelChmName
			// 
			this.labelChmName.Location = new System.Drawing.Point(128, 152);
			this.labelChmName.Name = "labelChmName";
			this.labelChmName.Size = new System.Drawing.Size(80, 16);
			this.labelChmName.TabIndex = 18;
			this.labelChmName.Text = ".chm file name";
			// 
			// checkBoxVML
			// 
			this.checkBoxVML.Location = new System.Drawing.Point(16, 176);
			this.checkBoxVML.Name = "checkBoxVML";
			this.checkBoxVML.Size = new System.Drawing.Size(88, 16);
			this.checkBoxVML.TabIndex = 19;
			this.checkBoxVML.Text = "VML graphs";
			this.checkBoxVML.CheckedChanged += new System.EventHandler(this.checkBoxVML_CheckedChanged);
			// 
			// textBoxConfigPath
			// 
			this.textBoxConfigPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxConfigPath.Location = new System.Drawing.Point(40, 120);
			this.textBoxConfigPath.Name = "textBoxConfigPath";
			this.textBoxConfigPath.Size = new System.Drawing.Size(480, 20);
			this.textBoxConfigPath.TabIndex = 20;
			this.textBoxConfigPath.Text = "";
			// 
			// buttonBrowseForConfigFile
			// 
			this.buttonBrowseForConfigFile.Location = new System.Drawing.Point(8, 120);
			this.buttonBrowseForConfigFile.Name = "buttonBrowseForConfigFile";
			this.buttonBrowseForConfigFile.Size = new System.Drawing.Size(24, 23);
			this.buttonBrowseForConfigFile.TabIndex = 21;
			this.buttonBrowseForConfigFile.Text = "...";
			this.buttonBrowseForConfigFile.Click += new System.EventHandler(this.buttonBrowseForConfigFile_Click);
			// 
			// labelConfigPath
			// 
			this.labelConfigPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelConfigPath.Location = new System.Drawing.Point(8, 104);
			this.labelConfigPath.Name = "labelConfigPath";
			this.labelConfigPath.Size = new System.Drawing.Size(512, 16);
			this.labelConfigPath.TabIndex = 22;
			this.labelConfigPath.Text = "Enter path to config xml file (optional):";
			// 
			// labelOptions
			// 
			this.labelOptions.Location = new System.Drawing.Point(8, 160);
			this.labelOptions.Name = "labelOptions";
			this.labelOptions.Size = new System.Drawing.Size(80, 16);
			this.labelOptions.TabIndex = 24;
			this.labelOptions.Text = "Other options:";
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(8, 424);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(512, 16);
			this.progressBar.TabIndex = 26;
			// 
			// richTextBoxProgress
			// 
			this.richTextBoxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBoxProgress.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.richTextBoxProgress.Location = new System.Drawing.Point(8, 280);
			this.richTextBoxProgress.Name = "richTextBoxProgress";
			this.richTextBoxProgress.Size = new System.Drawing.Size(512, 136);
			this.richTextBoxProgress.TabIndex = 27;
			this.richTextBoxProgress.Text = "";
			// 
			// labelProgress
			// 
			this.labelProgress.Location = new System.Drawing.Point(8, 240);
			this.labelProgress.Name = "labelProgress";
			this.labelProgress.Size = new System.Drawing.Size(100, 16);
			this.labelProgress.TabIndex = 28;
			this.labelProgress.Text = "Progress:";
			// 
			// labelItemsPerRow
			// 
			this.labelItemsPerRow.Location = new System.Drawing.Point(232, 152);
			this.labelItemsPerRow.Name = "labelItemsPerRow";
			this.labelItemsPerRow.Size = new System.Drawing.Size(80, 16);
			this.labelItemsPerRow.TabIndex = 32;
			this.labelItemsPerRow.Text = "icons/row";
			// 
			// textBoxItemsPerRow
			// 
			this.textBoxItemsPerRow.Location = new System.Drawing.Point(232, 168);
			this.textBoxItemsPerRow.Name = "textBoxItemsPerRow";
			this.textBoxItemsPerRow.Size = new System.Drawing.Size(84, 20);
			this.textBoxItemsPerRow.TabIndex = 31;
			this.textBoxItemsPerRow.Text = "6";
			// 
			// labelMaxLabelLength
			// 
			this.labelMaxLabelLength.Location = new System.Drawing.Point(232, 192);
			this.labelMaxLabelLength.Name = "labelMaxLabelLength";
			this.labelMaxLabelLength.Size = new System.Drawing.Size(100, 16);
			this.labelMaxLabelLength.TabIndex = 30;
			this.labelMaxLabelLength.Text = "max label length";
			// 
			// textBoxMaxLabelLength
			// 
			this.textBoxMaxLabelLength.Location = new System.Drawing.Point(232, 208);
			this.textBoxMaxLabelLength.Name = "textBoxMaxLabelLength";
			this.textBoxMaxLabelLength.Size = new System.Drawing.Size(84, 20);
			this.textBoxMaxLabelLength.TabIndex = 29;
			this.textBoxMaxLabelLength.Text = "12";
			// 
			// buttonObjectsToDocument
			// 
			this.buttonObjectsToDocument.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonObjectsToDocument.Location = new System.Drawing.Point(336, 168);
			this.buttonObjectsToDocument.Name = "buttonObjectsToDocument";
			this.buttonObjectsToDocument.Size = new System.Drawing.Size(80, 23);
			this.buttonObjectsToDocument.TabIndex = 33;
			this.buttonObjectsToDocument.Text = "Objects...";
			this.buttonObjectsToDocument.Click += new System.EventHandler(this.buttonObjectsToDocument_Click);
			// 
			// buttonViewChm
			// 
			this.buttonViewChm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonViewChm.Location = new System.Drawing.Point(440, 208);
			this.buttonViewChm.Name = "buttonViewChm";
			this.buttonViewChm.Size = new System.Drawing.Size(80, 24);
			this.buttonViewChm.TabIndex = 34;
			this.buttonViewChm.Text = "View .chm";
			this.buttonViewChm.Click += new System.EventHandler(this.buttonViewChm_Click);
			// 
			// buttonModels
			// 
			this.buttonModels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonModels.Location = new System.Drawing.Point(336, 208);
			this.buttonModels.Name = "buttonModels";
			this.buttonModels.Size = new System.Drawing.Size(80, 23);
			this.buttonModels.TabIndex = 35;
			this.buttonModels.Text = "Models...";
			this.buttonModels.Click += new System.EventHandler(this.buttonModels_Click);
			this.buttonModels.Hide();
			this.buttonModels.Enabled = false;
			// 
			// DBSpecGen
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(528, 445);
			this.Controls.Add(this.buttonModels);
			this.Controls.Add(this.buttonViewChm);
			this.Controls.Add(this.buttonObjectsToDocument);
			this.Controls.Add(this.labelItemsPerRow);
			this.Controls.Add(this.textBoxItemsPerRow);
			this.Controls.Add(this.labelMaxLabelLength);
			this.Controls.Add(this.textBoxMaxLabelLength);
			this.Controls.Add(this.labelProgress);
			this.Controls.Add(this.richTextBoxProgress);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.labelOptions);
			this.Controls.Add(this.labelConfigPath);
			this.Controls.Add(this.buttonBrowseForConfigFile);
			this.Controls.Add(this.textBoxConfigPath);
			this.Controls.Add(this.checkBoxVML);
			this.Controls.Add(this.labelChmName);
			this.Controls.Add(this.textBoxChmName);
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.buttonBrowseForOutputDir);
			this.Controls.Add(this.labelSaveOutputPath);
			this.Controls.Add(this.textBoxOutputDir);
			this.Controls.Add(this.checkBoxXMLComments);
			this.Controls.Add(this.labelTimeout);
			this.Controls.Add(this.textBoxTimeout);
			this.Controls.Add(this.buttonGo);
			this.Controls.Add(this.textBoxConnectionString);
			this.Controls.Add(this.labelConnectionString);
			this.MinimumSize = new System.Drawing.Size(536, 472);
			this.Name = "DBSpecGen";
			this.ResumeLayout(false);

		}
        #endregion

        #region Main entry point, etc...

        [STAThread]
        static void Main(string[] args) 
        {
			// for sean
			/*
			Console.WriteLine();
			Console.WriteLine("Printing out all command line args...");
			for (int i = 0; i < args.Length; ++i)
			{
				Console.WriteLine("\t" + args[i]);
			}
			Console.WriteLine();
			*/

            if(args.Length == 0)
            {
                // launch the ui
                m_bUI = true;
                Application.Run(new DBSpecGen());
            }
            else if(ParseCommandLine(args))
            {
                // launch the command line app
                m_bUI = false;
                GenerateSpec(null);  
            }
            else
            {
                // unable to interpret command line flags.
                WriteCommandLineHelpToConsole();
            }
        }

        public DBSpecGen()
        {
            InitializeComponent();
            textBoxOutputDir.Text = m_workingDir + "\\output";
            this.checkBoxVML.Checked = true;
            this.Text = m_version;
            m_workingDir = System.IO.Directory.GetCurrentDirectory();
            this.buttonViewChm.Enabled = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region functions related to the Gui...
        private WorkingState m_state = WorkingState.Idle;
        delegate void ShowProgressDelegate(string message, int progress, bool finished, DBSpecGen ui, out bool cancel);
        internal static void ShowProgress(string message, int progress, bool finished, DBSpecGen ui, out bool cancel) 
        {
            cancel = false;
            if (ui != null)
            {
                // Make sure we're on the right thread
                if( ui.richTextBoxProgress.InvokeRequired == false ) 
                {
                    ui.richTextBoxProgress.Text += message + "\r\n";
                    ui.progressBar.Maximum = 100;
                    if (progress > 0) ui.progressBar.Value = progress;

                    // Check for Cancel
                    cancel = (ui.m_state == WorkingState.Canceled);

                    // Check for completion
                    if (cancel || finished)
                    {
                        ui.m_state = WorkingState.Idle;
                        ui.buttonGo.Text = "Go!";
                        ui.buttonGo.Enabled = true;
                    }
                }
                else 
                {
                    ShowProgressDelegate  showProgress = new ShowProgressDelegate(ShowProgress);
                    object inoutCancel = false; // Avoid boxing and losing our return value

                    // Show progress synchronously (so we can check for cancel)
                    ui.Invoke(showProgress, new object[] { message, progress, finished, ui, inoutCancel});
                    cancel = (bool)inoutCancel;
                }
            }
            else if (!m_bQuiet)
            {
                Console.WriteLine(message);
            }
        }

        delegate bool GenerateSpecDelegate(DBSpecGen ui);
        private void buttonGo_Click(object sender, System.EventArgs e)
        {
            switch( m_state )
            {
                case WorkingState.Idle:
                {
                    this.richTextBoxProgress.Text = "";

                    int timeout = m_timeout;
                    try
                    {
                        timeout = System.Convert.ToInt32(this.textBoxTimeout.Text, CultureInfo.CurrentCulture);
                    }
                    catch
                    {
                        this.richTextBoxProgress.Text = "The timeout must be a positive integer.";
                        return;
                    }
                    if(timeout < 1)
                    {
                        this.richTextBoxProgress.Text = "The timeout must be a positive integer.";
                        return;
                    }

                    int maxLabelLength = m_MaxLabelLength;
                    try
                    {
                        maxLabelLength = System.Convert.ToInt32(this.textBoxMaxLabelLength.Text, CultureInfo.CurrentCulture);
                    }
                    catch
                    {
                        this.richTextBoxProgress.Text = "Maximum label length must be a positive integer.";
                        return;
                    }
                    if(maxLabelLength < 1)
                    {
                        this.richTextBoxProgress.Text = "Maximum label length must be a positive integer.";
                        return;
                    }

                    int itemsPerRow = m_ItemsPerRow;
                    try
                    {
                        itemsPerRow = System.Convert.ToInt32(this.textBoxItemsPerRow.Text, CultureInfo.CurrentCulture);
                    }
                    catch
                    {
                        this.richTextBoxProgress.Text = "Icons per row must be a positive integer greater than 1.";
                        return;
                    }
                    if(itemsPerRow < 2)
                    {
                        this.richTextBoxProgress.Text = "Icons per row must be a positive integer greater than 1.";
                        return;
                    }


                    m_rgConnString = this.textBoxConnectionString.Text.Split(':');
                    m_timeout = timeout;
                    m_outputDir = this.textBoxOutputDir.Text;
                    m_ChmName = this.textBoxChmName.Text;
                    m_bParseXmlComments = this.checkBoxXMLComments.Checked;
                    m_bIsVML = this.checkBoxVML.Checked;
                    m_ConfigPath = this.textBoxConfigPath.Text;
                    m_ItemsPerRow = itemsPerRow;
                    m_MaxLabelLength = maxLabelLength;

                    // Allow canceling
                    m_state = WorkingState.Working;
                    buttonGo.Text = "Cancel";

                    // Asynch delegate method
                    GenerateSpecDelegate genSpec = new GenerateSpecDelegate(GenerateSpec);

                    try
                    {
                        genSpec.BeginInvoke(this, null, null);
                    }
                    catch(Exception error)
                    {
                        this.richTextBoxProgress.Text += "\r\n\r\nAn error occured:";
                        this.richTextBoxProgress.Text += "\t" + error.Message;
                    }
                    
                    break;
                }
                    
                case WorkingState.Working:
                {
                    // Cancel a running calculation
                    m_state = WorkingState.Canceled;
                    buttonGo.Enabled = false;
                    break;
                }
            }
        }

        private void buttonBrowseForOutputDir_Click(object sender, System.EventArgs e)
        {
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {	
                string path = openFileDialog.FileName;
                path = path.Substring(0, path.LastIndexOf('\\'));
                this.textBoxOutputDir.Text = path;
            }
        }

        private void buttonViewChm_Click(object sender, System.EventArgs e)
        {
            if (m_PathToChm.Length > 0 && System.IO.File.Exists(m_PathToChm))
            {
                // launch the html help...
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = m_PathToChm;
                p.StartInfo.UseShellExecute = true;
                p.Start();
            }
            else
            {
                MessageBox.Show("The .chm file was not found. " + m_PathToChm, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonHelp_Click(object sender, System.EventArgs e)
        {
            if (System.IO.File.Exists(m_workingDir + "\\dbspecgen.htm"))
            {
                // launch the html help...
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = m_workingDir + "\\dbspecgen.htm";
                p.StartInfo.UseShellExecute = true;
                p.Start();
            }

            else
            {
                MessageBox.Show(
                    @"1.  Set the connection string to point the the database that you want to generate a spec for.  To do more than one database, enter multiple connection strings, separated by semicolons.  If you provide multiple connection strings, a single chm with documentation about all your databases will be created.  For large databases, you may need to increase the timeout.

2.  If you have commented your stored procedures and user defined functions with xml comments, check the 'XML comments' box and your comments will be parsed and put in the documentation.  See examples of how to use xml comments in the misc\samples directory.

3.  If you want to generate a dependency and foreign key graphs (rendered using VML) for each database object that has dependencies or foreign keys, make sure the 'vml graphs' box is checked.

4.  You can specify the name for the output file in the box provided.  If you want foo.chm, just enter foo.  Otherwise a default name will be used.

5.  If you want to generate data model diagrams or exclude certain database objects from your documentation, specify a config xml file in the space provided.  See pubsconfig.xml for an example of how to write a config file.  It's in the dbspecgen\misc\samples directory.

6.  A folder called 'SERVERNAME.MyDB' will be created in the path you specify, containing a file called MyDB.xml and several other htm files.  Open MyDB.chm to view your docs.  By default, the folder will appear in the same folder as DBSpecGen.exe, where MyDB is the name of your database.", "Instructions");
            }
        }

        private void buttonBrowseForConfigFile_Click(object sender, System.EventArgs e)
        {
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {	
                string path = openFileDialog.FileName;
                this.textBoxConfigPath.Text = path;
            }
        }

        private void checkBoxVML_CheckedChanged(object sender, System.EventArgs e)
        {
            this.textBoxItemsPerRow.Enabled = this.checkBoxVML.Checked;
            this.textBoxMaxLabelLength.Enabled = this.checkBoxVML.Checked;
        }

        private System.Xml.XmlDocument getAllSqlObjects()
        {
            string sql = "";
            bool cancel = false;
            bool bRet = false;
            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            string xmlRet = "<root>";
            try
            {
                System.Resources.ResourceManager rm = new ResourceManager("DBSpecGen.strings",typeof(DBSpecGen).Assembly);
                sql = rm.GetString("allObjects", CultureInfo.CurrentCulture);
            }
            catch
            {
                DBSpecGen.ShowProgress("Failed to get sql query from assembly resources.", 0, true, this, out cancel);
                return null;
            }

            string[] rgConnString = this.textBoxConnectionString.Text.Split(':');
            
            for(int i = 0; i < rgConnString.Length; ++i)
            {

                System.Data.SqlClient.SqlConnection conn = null;
                System.Data.SqlClient.SqlCommand com = null;

                try
                {
                    conn=new System.Data.SqlClient.SqlConnection(rgConnString[i]);
                    com=new System.Data.SqlClient.SqlCommand(sql,conn);
                    com.CommandTimeout = m_timeout;
                    conn.Open();
                    bRet = DatabaseSpecGenerator.GetXmlFromSql(com, xml); 
                }
                catch(System.Exception error)
                {
                    if(conn != null && conn.State != System.Data.ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    ShowProgress("SQL query failed. Error returned by SQL Server was:", 0, true, this, out cancel);
                    ShowProgress(error.Message, 0, true, this, out cancel);
                    return null;
                }

                conn.Close();
                xmlRet += xml.OuterXml;
            }
            xmlRet += "</root>";
            xml.LoadXml(xmlRet);
            return xml;
        }

        private bool AddItemsToListView(MultiSelect ms, bool populateFresh)
        {
            System.Xml.XmlDocument xml = getAllSqlObjects();
            if (null == xml)
            {
                return false;
            }

            if (!m_bObjectsChosen || populateFresh)
            {
                System.Xml.XmlNodeList databaseNodes = xml.SelectNodes("/root/database");
                for(int i = 0; i < databaseNodes.Count; ++i)
                {
                    string server = databaseNodes[i].SelectSingleNode("@server").Value;
                    string database = databaseNodes[i].SelectSingleNode("@name").Value;
                    System.Xml.XmlNodeList objectNodes = databaseNodes[i].SelectNodes("object");
                    for (int j = 0; j < objectNodes.Count; ++j)
                    {
                        string xtype = objectNodes[j].SelectSingleNode("@xtype").Value;
                        string name = objectNodes[j].SelectSingleNode("@name").Value;

                        // add the item to the left view
                        string[] s = {server, database, xtype, name};
                        ListViewItem lvi = new ListViewItem(s);
                        ms.listViewLeft.Items.Add(lvi);
                    }
                }
            }
            else
            {
                // we need to populate the listviews based on what they chose last time.
                // the stuff they excluded is in m_xmlExcludedObjects.
                XmlNodeList excludeNodes = m_IncludeExcludeObjects.Excluded.SelectNodes("/DBSpecGen/exclude/server/database/*");
                for (int i=0; i < excludeNodes.Count; ++i)
                {
                    string serverName = excludeNodes[i].SelectSingleNode("../../@name").Value;
                    string databaseName = excludeNodes[i].SelectSingleNode("../@name").Value;
                    string xtype = excludeNodes[i].SelectSingleNode("@xtype").Value;
                    string objectName = excludeNodes[i].SelectSingleNode("@name").Value;
                    string xpath = "/root/database[@name='"+databaseName+"' and @server='"+serverName+"']/object[@xtype='"+xtype+"' and @name='"+objectName+"']";
                    if (null != xml.SelectSingleNode(xpath))
                    {
                        // add this object back to the exclude list...
                        string[] s = {serverName, databaseName, xtype, objectName};
                        ListViewItem lvi = new ListViewItem(s);
                        ms.listViewLeft.Items.Add(lvi);
                    }
                }

                // add the stuff that is included to the right hand listview
                XmlNodeList includeNodes = m_IncludeExcludeObjects.Included.SelectNodes("/DBSpecGen/include/server/database/*");
                for (int i=0; i < includeNodes.Count; ++i)
                {
                    string serverName = includeNodes[i].SelectSingleNode("../../@name").Value;
                    string databaseName = includeNodes[i].SelectSingleNode("../@name").Value;
                    string xtype = includeNodes[i].SelectSingleNode("@xtype").Value;
                    string objectName = includeNodes[i].SelectSingleNode("@name").Value;
                    string xpath = "/root/database[@name='"+databaseName+"' and @server='"+serverName+"']/object[@xtype='"+xtype+"' and @name='"+objectName+"']";
                    if (null != xml.SelectSingleNode(xpath))
                    {
                        // add this object back to the include list...
                        string[] s = {serverName, databaseName, xtype, objectName};
                        ListViewItem lvi = new ListViewItem(s);
                        ms.listViewRight.Items.Add(lvi);
                    }
                }

                // add any objects that are not in either list to the "excluded" (left hand) listview...
                XmlNodeList allObjects = xml.SelectNodes("/root/database/object");
                for (int i = 0; i < allObjects.Count; ++i)
                {
                    string serverName = allObjects[i].SelectSingleNode("../@server").Value;
                    string databaseName = allObjects[i].SelectSingleNode("../@name").Value;
                    string xtype = allObjects[i].SelectSingleNode("@xtype").Value;
                    string objectName = allObjects[i].SelectSingleNode("@name").Value;
                    string includeXPath = "/DBSpecGen/include/server[@name='"+serverName+"']/database[@name='"+databaseName+"']/*[@xtype='"+xtype+"' and @name='"+objectName+"']";
                    string excludeXPath = "/DBSpecGen/exclude/server[@name='"+serverName+"']/database[@name='"+databaseName+"']/*[@xtype='"+xtype+"' and @name='"+objectName+"']";
                    if (null == m_IncludeExcludeObjects.Excluded.SelectSingleNode(excludeXPath) &&
                        null == m_IncludeExcludeObjects.Included.SelectSingleNode(includeXPath))
                    {
                        // add this object back to the exclude list...
                        string[] s = {serverName, databaseName, xtype, objectName};
                        ListViewItem lvi = new ListViewItem(s);
                        ms.listViewLeft.Items.Add(lvi);
                    }
                }
            }
            return true;
        }

        private void buttonObjectsToDocument_Click(object sender, System.EventArgs e)
        {
            MultiSelect ms = new MultiSelect();   
            ms.Left=300;
            ms.Top=300;
            ms.FormBorderStyle=FormBorderStyle.FixedToolWindow;
            ms.Text = "Select which objects to document";

            // add items to listview
            if (!AddItemsToListView(ms, false))
            {
                return;
            }

            if (ms.ShowDialog(this) == DialogResult.OK)
            {
                m_bObjectsChosen = true;

                // save off the excluded objects...
                string xmlstring = "<DBSpecGen><exclude>";
                for (int i = 0; i < ms.listViewLeft.Items.Count; ++i)
                {
                    ListViewItem lvi = ms.listViewLeft.Items[i];
                    xmlstring += "<server name='"+lvi.SubItems[0].Text+"'>";
                    xmlstring += "<database name='"+lvi.SubItems[1].Text+"'>";
                    switch (lvi.SubItems[2].Text)
                    {
                        case "U":
                            xmlstring += "<table xtype='" + lvi.SubItems[2].Text + "' name='"+lvi.SubItems[3].Text+"'/>";
                            break;
                        case "V":
                            xmlstring += "<view xtype='" + lvi.SubItems[2].Text + "' name='"+lvi.SubItems[3].Text+"'/>";
                            break;
                        case "P":
                            xmlstring += "<sproc xtype='" + lvi.SubItems[2].Text + "' name='"+lvi.SubItems[3].Text+"'/>";
                            break;
                        case "FN":
                        case "TF":
                        case "IF":
                            xmlstring += "<udf xtype='" + lvi.SubItems[2].Text + "' name='"+lvi.SubItems[3].Text+"'/>";
                            break;
                    }
                    xmlstring += "</database>";
                    xmlstring += "</server>";
                }
                xmlstring += "</exclude></DBSpecGen>";
                m_IncludeExcludeObjects.Excluded.LoadXml(xmlstring);

                // save off the included objects...
                xmlstring = "<DBSpecGen><include>";
                for (int i = 0; i < ms.listViewRight.Items.Count; ++i)
                {
                    ListViewItem lvi = ms.listViewRight.Items[i];
                    xmlstring += "<server name='"+lvi.SubItems[0].Text+"'>";
                    xmlstring += "<database name='"+lvi.SubItems[1].Text+"'>";
                    switch (lvi.SubItems[2].Text)
                    {
                        case "U":
                            xmlstring += "<table xtype='" + lvi.SubItems[2].Text + "' name='"+lvi.SubItems[3].Text+"'/>";
                            break;
                        case "V":
                            xmlstring += "<view xtype='" + lvi.SubItems[2].Text + "' name='"+lvi.SubItems[3].Text+"'/>";
                            break;
                        case "P":
                            xmlstring += "<sproc xtype='" + lvi.SubItems[2].Text + "' name='"+lvi.SubItems[3].Text+"'/>";
                            break;
                        case "FN":
                        case "TF":
                        case "IF":
                            xmlstring += "<udf xtype='" + lvi.SubItems[2].Text + "' name='"+lvi.SubItems[3].Text+"'/>";
                            break;
                    }
                    xmlstring += "</database>";
                    xmlstring += "</server>";
                }
                xmlstring += "</include></DBSpecGen>";
                m_IncludeExcludeObjects.Included.LoadXml(xmlstring);
            }
            ms.Dispose();
        }

        #endregion
        
        #region GenerateSpec functions...

		static private bool GenerateSpec(DBSpecGen ui)
		{
            bool cancel = false;
            
            if (ui != null)
            {
                ui.buttonViewChm.Enabled = false;
                m_PathToChm = "";
            }

            if (!m_bQuiet && !m_bUI) 
            {
                WriteCommandLineArgsToConsole();
            }

			System.IO.Directory.CreateDirectory(m_outputDir);

            DatabaseSpecGenerator dbgen = new DatabaseSpecGenerator();
            dbgen.Gui = ui;
            dbgen.UseVml = m_bIsVML;
            dbgen.BeQuiet = m_bQuiet;
            dbgen.ChmName = m_ChmName;
            dbgen.Timeout = m_timeout;
            dbgen.ConfigPath = m_ConfigPath;
            dbgen.ItemsPerRow = m_ItemsPerRow;
            dbgen.OutputDirectory = m_outputDir;
            dbgen.WorkingDirectory = m_workingDir;
            dbgen.MaxLabelLength = m_MaxLabelLength;
            dbgen.GenerateXmlOnly = m_bCreateXmlOnly;
            dbgen.ParseXmlComments = m_bParseXmlComments;
            dbgen.HideDBNameOnPageTitles = !m_bShowDbNameOnPageTitles;
            dbgen.GeneratePieCharts = m_bDrawPieCharts;

            if (m_DataModels.Length > 0)
            {
                dbgen.DataModelXml = m_DataModels;
            }
            
            if (m_IncludeExcludeObjects.Excluded.OuterXml.Length > 0)
            {
                dbgen.SetExcludedObjectsXml(m_IncludeExcludeObjects.Excluded.OuterXml);
            }

            // add all the paths to custom object files...
            if (m_rgExternalObjectPaths != null && m_rgExternalObjectPaths[0].Length > 0)
            {
                for (int i = 0; i < m_rgExternalObjectPaths.Length; ++i)
                {
                    if (m_rgExternalObjectPaths[i].Length > 0)
                    {
                        dbgen.AddExternalObjectPath(m_rgExternalObjectPaths[i]);
                    }
                }          
            }

            if (m_rgFakeServerNames != null && m_rgFakeServerNames[0].Length > 0)
            {
                for (int i = 0; i < m_rgFakeServerNames.Length; ++i)
                {
                    if (m_rgFakeServerNames[i].Length > 0)
                    {
                        dbgen.AddFakeServerName(m_rgFakeServerNames[i]);
                    }
                }          
            }

            if (m_rgFakeDatabaseNames != null && m_rgFakeDatabaseNames[0].Length > 0)
            {
                for (int i = 0; i < m_rgFakeDatabaseNames.Length; ++i)
                {
                    if (m_rgFakeDatabaseNames[i].Length > 0)
                    {
                        dbgen.AddFakeDatabaseName(m_rgFakeDatabaseNames[i]);
                    }
                }          
            }

            

			// see if we already have the xml, 
			// and are just generating html from them...
			if (m_rgXmlFromSqlPaths != null && m_rgXmlFromSqlPaths[0].Length > 0)
			{
                for (int i = 0; i < m_rgXmlFromSqlPaths.Length; ++i)
                {
                    if (m_rgXmlFromSqlPaths[i].Length > 0)
                    {
                        dbgen.AddDatabaseSchemaPath(m_rgXmlFromSqlPaths[i]);
                    }
                }          
			}

			// otherwise generate a spec for each of the 
			// connection strings we have...
			else if (m_rgConnString != null && m_rgConnString[0].Length > 0)
			{
                for (int i = 0; i < m_rgConnString.Length; ++i)
                {
                    if (m_rgConnString[i].Length > 0)
                    {
                        dbgen.AddConnectionString(m_rgConnString[i]);
                    }
                }   
			}
			else 
			{
                ShowProgress("Error:  no xml paths or connection strings for GenerateSpec to party with.", 0, true, ui, out cancel);
				return false;
			}

            try
            {
                dbgen.GenerateSpec();
            }
            catch(Exception e)
            {
                //ShowProgress("Error: " + e.Message, 0, true, ui, out cancel);
				ShowProgress("Error: " + e.Message + "\r\n" + e.StackTrace + "\r\n" + e.Source, 0, true, ui, out cancel);
                return false;
            }

            if (m_bCreateXmlOnly || dbgen.ChmFilePath.Length > 0)
            {
                if (ui != null)
                {
                    ui.buttonViewChm.Enabled = true;
                    m_PathToChm = dbgen.ChmFilePath;
                }
                return true;
            }
            else
            {
                ShowProgress("An error occured, somehow the .chm was not created", 0, true, ui, out cancel);
                return false;
            }		
		}
        
        #endregion

        #region Command line app functions...
		static private bool ParseCommandLine(string[] args)
		{
			for(int i=0;i<args.Length;++i)
			{
                bool bError = false;
				switch(args[i])
				{
					case "/a":
					case "-a":
						i++;
						if(i >= args.Length) return false;
						m_bCreateXmlOnly = (args[i]=="1") ? true : false;
						break;

					case "/b":
					case "-b":
						i++;
						if(i >= args.Length) return false;
						string paths = args[i];
						m_rgXmlFromSqlPaths = paths.Split(',');
						break;
					
					case "/c":
					case "-c":
						i++;
						if(i >= args.Length) return false;
						string arg = args[i];
						m_rgConnString = arg.Split(':');
						break;

					case "/d":
					case "-d":
						i++;
						if(i >= args.Length) return false;
						m_ConfigPath = args[i];
						break;

					case "/dd":
					case "-dd":
						i++;
						if(i >= args.Length) return false;
						m_bDiagramsOnly = (args[i]=="1") ? true : false;
						break;

					case "/e":
					case "-e":
						i++;
						if(i >= args.Length) return false;
						string objpaths = args[i];
						m_rgExternalObjectPaths = objpaths.Split(',');
						break;

					case "/f":
					case "-f":
						i++;
						if(i >= args.Length) return false;
						string fakeServers = args[i];
						m_rgFakeServerNames = fakeServers.Split(',');
						break;

					case "/g":
					case "-g":
						i++;
						if(i >= args.Length) return false;
						string fakeDatabases = args[i];
						m_rgFakeDatabaseNames = fakeDatabases.Split(',');
						break;

					case "/i":
					case "-i":
						i++;
						if(i >= args.Length) return false;
						m_bShowAllExtendedProperties = (args[i]=="1") ? true : false;
						break;

                    case "/j":
                    case "-j":
                        i++;
                        if(i >= args.Length) return false;
                        try
                        {
                            m_MaxLabelLength = System.Convert.ToInt32(args[i], CultureInfo.CurrentCulture);
                        }
                        catch
                        {
                            bError = true;
                        }
                        if(bError || m_MaxLabelLength < 1)
                        {
                            Console.WriteLine("Error: Maximum label length must be a positive integer.");
                            Console.WriteLine();
                            return false;
                        }
                        break;

                    case "/k":
                    case "-k":                        
                        i++;
                        if(i >= args.Length) return false;
                        try
                        {
                            m_ItemsPerRow = System.Convert.ToInt32(args[i], CultureInfo.CurrentCulture);
                        }
                        catch
                        {
                            bError = true;
                        }
                        if(bError || m_ItemsPerRow < 1)
                        {
                            Console.WriteLine("Error: Items per row must be a positive integer greater than one.");
                            Console.WriteLine();
                            return false;
                        }
                        break;

                    case "/m":
                    case "-m":
                        i++;
                        if(i >= args.Length) return false;
                        m_bShowDbNameOnPageTitles = (args[i]=="0") ? false : true;
                        break;
					
					case "/n":
					case "-n":
						i++;
						if(i >= args.Length) return false;
						m_ChmName = args[i];
						break;
					
					case "/o":
					case "-o":
						i++;
						if(i >= args.Length) return false;
						m_outputDir = args[i];
						break;

                    case "/p":
                    case "-p":
                        i++;
                        if(i >= args.Length) return false;
                        m_bUseMBAsSizeUnit = (args[i]=="1") ? true : false;
                        break;

					case "/q":
					case "-q":
						i++;
						if(i >= args.Length) return false;
						m_bQuiet = (args[i]=="1") ? true : false;
						break;

                    case "/r":
                    case "-r":
                        i++;
                        if(i >= args.Length) return false;
                        m_bDrawPieCharts = (args[i]=="0") ? false : true;
                        break;

					case "/t":
					case "-t":
						i++;
						if(i >= args.Length) return false;
						try
						{
							m_timeout = System.Convert.ToInt32(args[i], CultureInfo.CurrentCulture);
						}
						catch
						{
                            bError = true;
						}
                        if (bError || m_timeout < 1)
                        {
                            Console.WriteLine("Error: value passed in /t flag must be a positive integer.");
                            Console.WriteLine();
                            return false;
                        }
						break;

					case "/v":
					case "-v":
						i++;
						if(i >= args.Length) return false;
						m_bIsVML = (args[i]=="0") ? false : true;
						break;					

					case "/w":
					case "-w":
						i++;
						if(i >= args.Length) return false;
						m_workingDir = args[i];
						break;

					case "/x":
					case "-x":
						i++;
						if(i >= args.Length) return false;
						m_bParseXmlComments = (args[i]=="1") ? true : false;
						break;

					case "/?":
					case "-?":
						return false;

					default:
						break;
				}
			}

			// if either of these is defined, then we need 
			// one fake name for each connection string.
			if((m_rgFakeServerNames[0].Length > 0 || m_rgFakeDatabaseNames[0].Length > 0) &&
				(m_rgFakeServerNames.Length != m_rgFakeDatabaseNames.Length ||
				m_rgFakeServerNames.Length != m_rgConnString.Length ||
				m_rgFakeDatabaseNames.Length != m_rgConnString.Length))
			{
				Console.WriteLine("If you are using fake server and database names, then you must");
				Console.WriteLine("provide one of each for each connection string.");
				return false;
			}

			if((m_rgConnString[0].Length == 0 && m_rgXmlFromSqlPaths[0].Length == 0) ||
			   (m_rgConnString[0].Length > 0 && m_rgXmlFromSqlPaths[0].Length > 0))
			{
				Console.WriteLine("You must provide either a path to an xml file or a connection string, but not both.");
				return false;
			}
			m_outputDir = System.IO.Path.GetFullPath(m_outputDir);
			m_workingDir = System.IO.Path.GetFullPath(m_workingDir);
			return true;
		}

        static private void WriteCommandLineHelpToConsole()
        {
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine(m_version);
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("flags:");
            Console.WriteLine();
            Console.WriteLine("[/a 0|1]               1 = generate xml only, no documentation. default is 0.");
            Console.WriteLine("                       that is, the default is to generate human readable,");
            Console.WriteLine("                       nicely formatted documentation.");
            Console.WriteLine();
            Console.WriteLine("[/b pathsToXmlFiles]   contains a comma-separated list of paths to xml files");
            Console.WriteLine("                       generated previously by dbspecgen.  Use this to");
            Console.WriteLine("                       generate docs for xml files that you generated");
            Console.WriteLine("                       previously using the '/a 1' flag.");
            Console.WriteLine();
            Console.WriteLine("[/c connectionStrings] contains a colon-sepatated list of connection strings");
            Console.WriteLine("                       to SQL Server 2000 or Yukon databases.");
            Console.WriteLine();
            Console.WriteLine("[/d config file path]  path to a xml config file, used for specifying");
            Console.WriteLine("                       data model diagrams, an exclusion list, etc.");
            Console.WriteLine();
            Console.WriteLine("[/e external objects]  contains a comma-separated list of paths to xml files ");
            Console.WriteLine("                       describing what external objects you want to include in");
            Console.WriteLine("                       the documentation.");
            Console.WriteLine();
            Console.WriteLine("[/f fake server names] Contains a comma-separated list of fake server names to");
            Console.WriteLine("                       be used in the documentation instead of the server names");
            Console.WriteLine("                       specified in the connection strings passed in /c.  Only");
            Console.WriteLine("                       used if /c is passed.");
            Console.WriteLine();
            Console.WriteLine("[/g fake db names]     Contains a comma-separated list of fake database names");
            Console.WriteLine("                       to be used in the documentation instead of the server");
            Console.WriteLine("                       names specified in the connection strings passed in /c.");
            Console.WriteLine("                       Only used if /c is passed.");
            Console.WriteLine();
            Console.WriteLine("[/i 0|1]               Specifies whether to include extended properties of type");
            Console.WriteLine("                       'MS_Description' in the table of extended properties for");
            Console.WriteLine("                       each table and view.  1 means include them, and is the");
            Console.WriteLine("                       default.");
            Console.WriteLine();
            Console.WriteLine("[/j max label length]  Specifies the maximum number of chars to display for"); 
            Console.WriteLine("                       item labels in VML graphs.  After this number or chars,");
            Console.WriteLine("                       the name will be cut off and ellipsis appended.  Default");
            Console.WriteLine("                       is 12 chars.");
            Console.WriteLine();
            Console.WriteLine("[/k items per row]     Specifies the number of items to display per row in VML");
            Console.WriteLine("                       reference and dependency graphs.  Default is 6.");
            Console.WriteLine();
            Console.WriteLine("[/m 0|1]               Specifies whether include the database name on page");
            Console.WriteLine("                       titles.  Default is 1.");
            Console.WriteLine();
            Console.WriteLine("[/n chm file name]     a name for your chm file.  default is");
            Console.WriteLine("                       the name of the database, or 'dbspec' if");
            Console.WriteLine("                       multiple values are passed in the /c or /b flags.");
            Console.WriteLine();
            Console.WriteLine("[/o outputDir]         default is current directory");
            Console.WriteLine();
            Console.WriteLine("[/p 0|1]               Specifies whether to use mb as units for table");
            Console.WriteLine("                       and index sizes.  Default is 0 (use kb).");
            Console.WriteLine();
            Console.WriteLine("[/q 0|1]               1 = quiet mode.  0 is default.");
            Console.WriteLine();
            Console.WriteLine("[/r 0|1]               1 = draw pie charts, 0 = don't.  Default is 1.");
            Console.WriteLine();
            Console.WriteLine("[/t timeout]           used to specify timout in seconds. 60 is default.");
            Console.WriteLine();
            Console.WriteLine("[/v 0|1]               1 = draw vml dependency and pk-fk relationship graphs.");
            Console.WriteLine("                       0 = don't draw graphs.  default is 1.");
            Console.WriteLine();
            Console.WriteLine("[/w workingDir]        path to dbspecgen.exe folder.");
            Console.WriteLine("                       default is current directory.");
            Console.WriteLine("                       needed so the app can find the misc directory.");
            Console.WriteLine();
            Console.WriteLine("[/x 0|1]               1 = parse xml comments. 0 is default.");
            Console.WriteLine();
            Console.WriteLine("[/?]                   show this help page.");
            Console.WriteLine();
            Console.WriteLine("Note that all flags are optional, except that you must pass");
            Console.WriteLine("either the /c or /b flag, but not both.  You will get an");
            Console.WriteLine("error if you try to pass both, or if you pass niether.");
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("example:");
            Console.WriteLine();
            Console.WriteLine("dbspecgen.exe /c \"server=myserver; database=pubs; user id=myuser; pwd=mypass\"");
            Console.WriteLine("              /o \"c:\\my db specs\"");
            Console.WriteLine("              /w \"c:\\tools\\dbspecgen\"");
            Console.WriteLine("              /h fancy ");
            Console.WriteLine("              /x 1 ");
            Console.WriteLine("              /t 120 ");
            Console.WriteLine("              /q 1");
            Console.WriteLine();
            Console.WriteLine("In this example, the output would be placed in c:\\my db specs\\MYSERVER.pubs,");
            Console.WriteLine("dbspecgen is located in c:\\tools\\dbspecgen, using html output,");
            Console.WriteLine("parsing of xml comments is turned on, timeout is set at 120 sec,");
            Console.WriteLine("and quiet mode is turned on.");
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("another example:");
            Console.WriteLine();
            Console.WriteLine("dbspecgen.exe /c \"server=myserver; database=mydb; user id=myuser; pwd=mypass: server=(local); database=pubs; Trusted_Connection=yes;\""); 
            Console.WriteLine("              /n MyDocs");
            Console.WriteLine();
            Console.WriteLine("In this example, documentation will be generated from two");
            Console.WriteLine("databases and placed into a single MyDocs.chm file.");
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("yet another example:");
            Console.WriteLine();
            Console.WriteLine("dbspecgen.exe /b \"c:\\my db files\\pubs.xml,c:\\my db files\\northwind.xml\""); 
            Console.WriteLine("              /o \"c:\\my db files\\output\"");
            Console.WriteLine("              /h chm");
            Console.WriteLine("              /n NorthwindAndPubs");
            Console.WriteLine();
            Console.WriteLine("In this example, pubs.xml and northwind.xml will be parsed"); 
            Console.WriteLine("and a chm named NorthwindAndPubs.chm will be produced and placed in");
            Console.WriteLine("c:\\my db files\\output");
            Console.WriteLine("-------------------------------------------------------------------------------");
        }

		static void WriteCommandLineArgsToConsole()
		{
			Console.WriteLine();
			Console.WriteLine(m_version);
			Console.WriteLine();
			
            Console.Write("connection strings: ");
			for(int i=0; i<m_rgConnString.Length; ++i)
			{
				if(i==0) Console.WriteLine(m_rgConnString[i]);		
				else Console.WriteLine("                    " + m_rgConnString[i]);
			}

			Console.Write("database xml files: ");
			for(int i=0; i<m_rgXmlFromSqlPaths.Length; ++i)
			{
				if(i==0) Console.WriteLine(m_rgXmlFromSqlPaths[i]);		
				else Console.WriteLine("                    " + m_rgXmlFromSqlPaths[i]);
			}

            Console.Write("ext. object files:  ");
            for(int i=0; i<m_rgExternalObjectPaths.Length; ++i)
            {
                if(i==0) Console.WriteLine(m_rgExternalObjectPaths[i]);		
                else Console.WriteLine("                    " + m_rgExternalObjectPaths[i]);
            }

			Console.WriteLine("output directory:   " + m_outputDir);
			Console.WriteLine("timeout:            " + m_timeout);
			Console.WriteLine("parse xml comments: " + m_bParseXmlComments);
			Console.WriteLine("generate xml only:  " + m_bCreateXmlOnly);
			Console.WriteLine("working directory:  " + m_workingDir);
			Console.WriteLine("chm name:           " + m_ChmName);
			Console.WriteLine("config path:        " + m_ConfigPath);  
            Console.WriteLine("all extended props: " + m_bShowAllExtendedProperties);   
            Console.WriteLine("draw VML graphs:    " + m_bIsVML);   
            Console.WriteLine("show db name:       " + m_bShowDbNameOnPageTitles);   
            Console.WriteLine("items/row:          " + m_ItemsPerRow);   
            Console.WriteLine("max label length:   " + m_MaxLabelLength);  
            Console.WriteLine("use mb units:       " + m_bUseMBAsSizeUnit); 
            Console.WriteLine("draw pie charts:    " + m_bDrawPieCharts);

			Console.WriteLine();
		}
        #endregion

        private void buttonModels_Click(object sender, System.EventArgs e)
        {
            DataModel dm = new DataModel();
            dm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            dm.Text = "Data model properties";
            dm.Left=300;
            dm.Top=300;
            if (dm.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            
            int maxLabelLength = Int32.Parse(dm.textBoxMaxLabelLength.Text);
            int iconsPerRow    = Int32.Parse(dm.textBoxIconsPerRow.Text);
            int verticalSpacer = Int32.Parse(dm.textBoxVerticalSpacer.Text);
            int horizontalSpacer = Int32.Parse(dm.textBoxHorizontalSpacer.Text);
            string modelName = dm.textBoxModelName.Text;
            string allowOverLap = dm.checkBoxAllowOverlap.Checked ? "1" : "0";
            dm.Dispose();
           
            MultiSelect ms = new MultiSelect();   
            ms.Left=300;
            ms.Top=300;
            ms.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            ms.Text = "Select objects for the model: '"+modelName+"'";
            ms.SetTitles("Available objects", "Objects in model");

            // add items to listview
            if (!AddItemsToListView(ms, true))
            {
                return;
            }

            if (ms.ShowDialog(this) == DialogResult.OK)
            {
                string xmlstring = "<model name='"+modelName+"' allowOverLap='"+allowOverLap+"' maxLabelLength='"+maxLabelLength+"' verticalSpace='"+verticalSpacer+"' iconsPerRow='"+iconsPerRow+"' horizontalSpace='"+horizontalSpacer+"'>";
                for (int i = 0; i < ms.listViewRight.Items.Count; ++i)
                {
                    ListViewItem lvi = ms.listViewRight.Items[i];
                    xmlstring += "<server name='"+lvi.SubItems[0].Text+"'>";
                    xmlstring += "<database name='"+lvi.SubItems[1].Text+"'>";
                    switch (lvi.SubItems[2].Text)
                    {
                        case "U":
                            xmlstring += "<table xtype='" + lvi.SubItems[2].Text + "' name='"+lvi.SubItems[3].Text+"'/>";
                            break;
                        case "V":
                            xmlstring += "<view xtype='" + lvi.SubItems[2].Text + "' name='"+lvi.SubItems[3].Text+"'/>";
                            break;
                        case "P":
                            xmlstring += "<sproc xtype='" + lvi.SubItems[2].Text + "' name='"+lvi.SubItems[3].Text+"'/>";
                            break;
                        case "FN":
                        case "TF":
                        case "IF":
                            xmlstring += "<udf xtype='" + lvi.SubItems[2].Text + "' name='"+lvi.SubItems[3].Text+"'/>";
                            break;
                    }
                    xmlstring += "</database>";
                    xmlstring += "</server>";
                }
                xmlstring += "</model>";
                m_DataModels += xmlstring;
            }
            ms.Dispose();
        }
    }
	public class DatabaseObjectList
	{
		private XmlDocument m_xmlExcludedObjects = new XmlDocument();
		private XmlDocument m_xmlIncludedObjects = new XmlDocument();
        
		public DatabaseObjectList(){}
       
		public XmlDocument Included
		{
			get
			{
				return m_xmlIncludedObjects;
			}
			set
			{
				m_xmlIncludedObjects = value;   
			}
		}

		public XmlDocument Excluded
		{
			get
			{
				return m_xmlExcludedObjects;
			}
			set
			{
				m_xmlExcludedObjects = value;   
			}
		}
	}

	public class LabelAndValue 
	{
		private string _Name;
		private int _Size;
		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}
		public int Size
		{
			get
			{
				return _Size;
			}
			set
			{
				_Size = value;
			}
		}
	}
   
	public class DataModelList
	{
		private string _name = "UNKNOWN";
		private DatabaseObjectList _dataModelList = new DatabaseObjectList();
		public DataModelList(){}
		public DataModelList(string name){ _name = name; }
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		public DatabaseObjectList List
		{
			get
			{
				return _dataModelList;
			}
			set 
			{
				_dataModelList = value;
			}
		}
	}

	public enum WorkingState
	{
		Idle = 0,
		Working = 1,
		Canceled = 2
	}

}
