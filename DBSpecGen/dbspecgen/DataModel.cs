using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace DBSpecGen
{
	/// <summary>
	/// Summary description for DataModel.
	/// </summary>
	public class DataModel : System.Windows.Forms.Form
	{
        internal System.Windows.Forms.CheckBox checkBoxAllowOverlap;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label1; 
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.TextBox textBoxModelName;
        internal System.Windows.Forms.TextBox textBoxIconsPerRow;
        internal System.Windows.Forms.TextBox textBoxMaxLabelLength;
        internal System.Windows.Forms.TextBox textBoxHorizontalSpacer;
        internal System.Windows.Forms.TextBox textBoxVerticalSpacer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.ComponentModel.IContainer components;

		public DataModel()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxModelName = new System.Windows.Forms.TextBox();
            this.checkBoxAllowOverlap = new System.Windows.Forms.CheckBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.textBoxIconsPerRow = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxMaxLabelLength = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxHorizontalSpacer = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxVerticalSpacer = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Model name";
            // 
            // textBoxModelName
            // 
            this.textBoxModelName.Location = new System.Drawing.Point(8, 24);
            this.textBoxModelName.Name = "textBoxModelName";
            this.textBoxModelName.Size = new System.Drawing.Size(296, 20);
            this.textBoxModelName.TabIndex = 1;
            this.textBoxModelName.Text = "my model";
            // 
            // checkBoxAllowOverlap
            // 
            this.checkBoxAllowOverlap.Location = new System.Drawing.Point(8, 168);
            this.checkBoxAllowOverlap.Name = "checkBoxAllowOverlap";
            this.checkBoxAllowOverlap.TabIndex = 2;
            this.checkBoxAllowOverlap.Text = "Allow overlap";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // textBoxIconsPerRow
            // 
            this.textBoxIconsPerRow.Location = new System.Drawing.Point(8, 72);
            this.textBoxIconsPerRow.Name = "textBoxIconsPerRow";
            this.textBoxIconsPerRow.Size = new System.Drawing.Size(136, 20);
            this.textBoxIconsPerRow.TabIndex = 4;
            this.textBoxIconsPerRow.Text = "8";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Icons per row";
            // 
            // textBoxMaxLabelLength
            // 
            this.textBoxMaxLabelLength.Location = new System.Drawing.Point(168, 72);
            this.textBoxMaxLabelLength.Name = "textBoxMaxLabelLength";
            this.textBoxMaxLabelLength.Size = new System.Drawing.Size(136, 20);
            this.textBoxMaxLabelLength.TabIndex = 6;
            this.textBoxMaxLabelLength.Text = "12";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(168, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Max label length";
            // 
            // textBoxHorizontalSpacer
            // 
            this.textBoxHorizontalSpacer.Location = new System.Drawing.Point(8, 128);
            this.textBoxHorizontalSpacer.Name = "textBoxHorizontalSpacer";
            this.textBoxHorizontalSpacer.Size = new System.Drawing.Size(136, 20);
            this.textBoxHorizontalSpacer.TabIndex = 8;
            this.textBoxHorizontalSpacer.Text = "75";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Horizontal spacer";
            // 
            // textBoxVerticalSpacer
            // 
            this.textBoxVerticalSpacer.Location = new System.Drawing.Point(168, 128);
            this.textBoxVerticalSpacer.Name = "textBoxVerticalSpacer";
            this.textBoxVerticalSpacer.Size = new System.Drawing.Size(136, 20);
            this.textBoxVerticalSpacer.TabIndex = 10;
            this.textBoxVerticalSpacer.Text = "75";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(168, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 16);
            this.label5.TabIndex = 9;
            this.label5.Text = "Vertical spacer";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(144, 168);
            this.button1.Name = "button1";
            this.button1.TabIndex = 11;
            this.button1.Text = "cancel";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(232, 168);
            this.button2.Name = "button2";
            this.button2.TabIndex = 12;
            this.button2.Text = "OK";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // DataModel
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(312, 197);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.button2,
                                                                          this.button1,
                                                                          this.textBoxVerticalSpacer,
                                                                          this.label5,
                                                                          this.textBoxHorizontalSpacer,
                                                                          this.label4,
                                                                          this.textBoxMaxLabelLength,
                                                                          this.label3,
                                                                          this.textBoxIconsPerRow,
                                                                          this.label2,
                                                                          this.checkBoxAllowOverlap,
                                                                          this.textBoxModelName,
                                                                          this.label1});
            this.MaximumSize = new System.Drawing.Size(320, 224);
            this.MinimumSize = new System.Drawing.Size(320, 224);
            this.Name = "DataModel";
            this.Text = "Data model properties";
            this.ResumeLayout(false);

        }
		#endregion

        private void button2_Click(object sender, System.EventArgs e)
        {
            // verify text box values...
            if (this.textBoxModelName.Text.Length == 0)
            {
                MessageBox.Show("A data model must have a name");
                return;
            }

            try
            {
                Int32.Parse(this.textBoxMaxLabelLength.Text);
                Int32.Parse(this.textBoxIconsPerRow.Text);
                Int32.Parse(this.textBoxVerticalSpacer.Text);
                Int32.Parse(this.textBoxHorizontalSpacer.Text);
            }
            catch
            {
                MessageBox.Show("All values besides the model name must be positive integers");
            }
            
            this.DialogResult=DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.DialogResult=DialogResult.Cancel;
            this.Close();
        }
	}
}
