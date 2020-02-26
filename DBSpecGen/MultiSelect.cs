using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;	
using System.Globalization;


namespace DBSpecGen
{
	/// <summary>
	/// Summary description for MultiSelect.
	/// </summary>
	public class MultiSelect : System.Windows.Forms.Form
	{
        internal System.Windows.Forms.ListView listViewLeft;
        internal System.Windows.Forms.ListView listViewRight;
        private ListViewColumnSorter lvColumnSorter;

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;      
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;

        

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MultiSelect()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            lvColumnSorter = new ListViewColumnSorter();
            this.listViewLeft.ListViewItemSorter = lvColumnSorter;
            this.listViewRight.ListViewItemSorter = lvColumnSorter;
		}

        public void SetTitles(string leftTitle, string rightTitle)
        {
            this.label1.Text = leftTitle;
            this.label3.Text = rightTitle;
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.listViewLeft = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.button4 = new System.Windows.Forms.Button();
            this.listViewRight = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.button1.Location = new System.Drawing.Point(336, 165);
            this.button1.Name = "button1";
            this.button1.TabIndex = 2;
            this.button1.Text = "add >>";
            this.button1.Click += new System.EventHandler(this.moveItemLeftToRight);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.button2.Location = new System.Drawing.Point(336, 197);
            this.button2.Name = "button2";
            this.button2.TabIndex = 3;
            this.button2.Text = "<< remove";
            this.button2.Click += new System.EventHandler(this.moveItemRightToLeft);
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.button3.Location = new System.Drawing.Point(336, 269);
            this.button3.Name = "button3";
            this.button3.TabIndex = 5;
            this.button3.Text = "OK";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "Available database objects";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(424, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(184, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "Database objects to document";
            // 
            // listViewLeft
            // 
            this.listViewLeft.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left);
            this.listViewLeft.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                           this.columnHeader1,
                                                                                           this.columnHeader2,
                                                                                           this.columnHeader3,
                                                                                           this.columnHeader4});
            this.listViewLeft.FullRowSelect = true;
            this.listViewLeft.GridLines = true;
            this.listViewLeft.Location = new System.Drawing.Point(8, 24);
            this.listViewLeft.Name = "listViewLeft";
            this.listViewLeft.Size = new System.Drawing.Size(312, 472);
            this.listViewLeft.TabIndex = 10;
            this.listViewLeft.View = System.Windows.Forms.View.Details;
            this.listViewLeft.DoubleClick += new System.EventHandler(this.listViewLeft_DoubleClick);
            this.listViewLeft.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "server";
            this.columnHeader1.Width = 45;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "db";
            this.columnHeader2.Width = 45;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "type";
            this.columnHeader3.Width = 35;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "name";
            this.columnHeader4.Width = 183;
            // 
            // button4
            // 
            this.button4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.button4.Location = new System.Drawing.Point(336, 301);
            this.button4.Name = "button4";
            this.button4.TabIndex = 12;
            this.button4.Text = "Cancel";
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // listViewRight
            // 
            this.listViewRight.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left);
            this.listViewRight.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                            this.columnHeader5,
                                                                                            this.columnHeader6,
                                                                                            this.columnHeader7,
                                                                                            this.columnHeader8});
            this.listViewRight.FullRowSelect = true;
            this.listViewRight.GridLines = true;
            this.listViewRight.Location = new System.Drawing.Point(424, 24);
            this.listViewRight.Name = "listViewRight";
            this.listViewRight.Size = new System.Drawing.Size(312, 472);
            this.listViewRight.TabIndex = 13;
            this.listViewRight.View = System.Windows.Forms.View.Details;
            this.listViewRight.DoubleClick += new System.EventHandler(this.listViewRight_DoubleClick);
            this.listViewRight.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "server";
            this.columnHeader5.Width = 45;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "db";
            this.columnHeader6.Width = 45;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "type";
            this.columnHeader7.Width = 35;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "name";
            this.columnHeader8.Width = 183;
            // 
            // MultiSelect
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(744, 501);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.listViewRight,
                                                                          this.button4,
                                                                          this.listViewLeft,
                                                                          this.label3,
                                                                          this.label1,
                                                                          this.button3,
                                                                          this.button2,
                                                                          this.button1});
            this.MinimumSize = new System.Drawing.Size(640, 408);
            this.Name = "MultiSelect";
            this.ShowInTaskbar = false;
            this.Text = "MultiSelect";
            this.ResumeLayout(false);

        }
		#endregion

        private void button3_Click(object sender, System.EventArgs e)
        {
            this.DialogResult=DialogResult.OK;
            this.Close();
        }

        private void button4_Click(object sender, System.EventArgs e)
        {
            this.DialogResult=DialogResult.Cancel;
            this.Close();
        }

        private void moveItemLeftToRight(object sender, System.EventArgs e)
        {
            MoveListViewItems(listViewLeft, listViewRight);
        }

        private void moveItemRightToLeft(object sender, System.EventArgs e)
        {
            MoveListViewItems(listViewRight, listViewLeft);
        }

        private void MoveListViewItems(ListView lvFrom, ListView lvTo)
        {
            if (lvFrom.SelectedItems.Count > 0)
            {
                int index = 0;
                while (lvFrom.SelectedItems.Count > 0)
                {
                    ListViewItem lvi = (ListViewItem)lvFrom.SelectedItems[0];
                    index = lvFrom.SelectedItems[0].Index;
                    lvFrom.Items.Remove(lvFrom.SelectedItems[0]);
                    lvTo.Items.Add(lvi);  
                }

                // unselect all items at the destination
                for (int i = 0; i < lvTo.Items.Count; ++i)
                {
                    lvTo.Items[i].Selected = false;
                    lvTo.Items[i].Focused = false;
                }

                // select the next item
                if (lvFrom.Items.Count > index)
                {
                    lvFrom.Items[index].Selected = true;
                    lvFrom.Items[index].Focused = true;
                }
            }
        }

        private void listViewLeft_DoubleClick(object sender, System.EventArgs e)
        {
            moveItemLeftToRight(sender, e);
        }

        private void listViewRight_DoubleClick(object sender, System.EventArgs e)
        {
            moveItemRightToLeft(sender, e);
        }

        // for sorting by column
        private void listView_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            ListView myListView = (ListView)sender;
            
            if (e.Column == lvColumnSorter.SortColumn)
            {
                if (lvColumnSorter.Order == SortOrder.Ascending)
                {
                    lvColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                lvColumnSorter.SortColumn = e.Column;
                lvColumnSorter.Order = SortOrder.Ascending;
            }
            myListView.Sort();
        }
	}

    // for sorting by columns
    public class ListViewColumnSorter : IComparer
    {
        private int columnToSort;
        private SortOrder sortOrder;
        private CaseInsensitiveComparer comparer;

        public ListViewColumnSorter()
        {
            columnToSort = 0;
            sortOrder = SortOrder.Ascending;
            comparer = new CaseInsensitiveComparer(CultureInfo.CurrentCulture);
        }

        public int Compare(object x, object y)
        {
            ListViewItem lviLeft, lviRight;
            lviLeft = (ListViewItem)x;
            lviRight = (ListViewItem)y;

            int compareResult = comparer.Compare(lviLeft.SubItems[columnToSort].Text,lviRight.SubItems[columnToSort].Text);
            
            if (sortOrder == SortOrder.Ascending)
            {
                return compareResult;
            }
            else if (sortOrder == SortOrder.Descending)
            {
                return (-compareResult);
            }
            else
            {
                return 0;
            }
        }
        public int SortColumn
        {
            set
            {
                columnToSort = value;
            }
            get
            {
                return columnToSort;
            }
        }
        public SortOrder Order
        {
            set
            {
                sortOrder = value;
            }
            get
            {
                return sortOrder;
            }
        }
    }
}
