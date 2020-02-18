namespace IronPythonTest2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.lstEntries = new System.Windows.Forms.ListBox();
            this.btnSetupPython = new System.Windows.Forms.Button();
            this.btnTest1 = new System.Windows.Forms.Button();
            this.btnTest2 = new System.Windows.Forms.Button();
            this.btnTest3 = new System.Windows.Forms.Button();
            this.btnTest4 = new System.Windows.Forms.Button();
            this.btnTest5 = new System.Windows.Forms.Button();
            this.btnTest6 = new System.Windows.Forms.Button();
            this.btnTest7 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // lstEntries
            // 
            this.lstEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstEntries.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstEntries.FormattingEnabled = true;
            this.lstEntries.ItemHeight = 16;
            this.lstEntries.Location = new System.Drawing.Point(18, 19);
            this.lstEntries.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstEntries.Name = "lstEntries";
            this.lstEntries.Size = new System.Drawing.Size(641, 292);
            this.lstEntries.TabIndex = 0;
            // 
            // btnSetupPython
            // 
            this.btnSetupPython.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSetupPython.Location = new System.Drawing.Point(18, 337);
            this.btnSetupPython.Name = "btnSetupPython";
            this.btnSetupPython.Size = new System.Drawing.Size(75, 61);
            this.btnSetupPython.TabIndex = 1;
            this.btnSetupPython.Text = "Setup Python";
            this.btnSetupPython.UseVisualStyleBackColor = true;
            this.btnSetupPython.Click += new System.EventHandler(this.btnSetupPython_Click);
            // 
            // btnTest1
            // 
            this.btnTest1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTest1.Location = new System.Drawing.Point(99, 337);
            this.btnTest1.Name = "btnTest1";
            this.btnTest1.Size = new System.Drawing.Size(75, 61);
            this.btnTest1.TabIndex = 2;
            this.btnTest1.Text = "Test \r\n1";
            this.btnTest1.UseVisualStyleBackColor = true;
            this.btnTest1.Click += new System.EventHandler(this.btnTest1_Click);
            // 
            // btnTest2
            // 
            this.btnTest2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTest2.Location = new System.Drawing.Point(180, 337);
            this.btnTest2.Name = "btnTest2";
            this.btnTest2.Size = new System.Drawing.Size(75, 61);
            this.btnTest2.TabIndex = 3;
            this.btnTest2.Text = "Test \r\n2";
            this.btnTest2.UseVisualStyleBackColor = true;
            this.btnTest2.Click += new System.EventHandler(this.btnTest2_Click);
            // 
            // btnTest3
            // 
            this.btnTest3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTest3.Location = new System.Drawing.Point(261, 337);
            this.btnTest3.Name = "btnTest3";
            this.btnTest3.Size = new System.Drawing.Size(75, 61);
            this.btnTest3.TabIndex = 4;
            this.btnTest3.Text = "Test \r\n3";
            this.btnTest3.UseVisualStyleBackColor = true;
            this.btnTest3.Click += new System.EventHandler(this.btnTest3_Click);
            // 
            // btnTest4
            // 
            this.btnTest4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTest4.Location = new System.Drawing.Point(342, 337);
            this.btnTest4.Name = "btnTest4";
            this.btnTest4.Size = new System.Drawing.Size(75, 61);
            this.btnTest4.TabIndex = 5;
            this.btnTest4.Text = "Test \r\n4";
            this.btnTest4.UseVisualStyleBackColor = true;
            this.btnTest4.Click += new System.EventHandler(this.btnTest4_Click);
            // 
            // btnTest5
            // 
            this.btnTest5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTest5.Location = new System.Drawing.Point(423, 337);
            this.btnTest5.Name = "btnTest5";
            this.btnTest5.Size = new System.Drawing.Size(75, 61);
            this.btnTest5.TabIndex = 6;
            this.btnTest5.Text = "Test \r\n5";
            this.btnTest5.UseVisualStyleBackColor = true;
            this.btnTest5.Click += new System.EventHandler(this.btnTest5_Click);
            // 
            // btnTest6
            // 
            this.btnTest6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTest6.Location = new System.Drawing.Point(504, 337);
            this.btnTest6.Name = "btnTest6";
            this.btnTest6.Size = new System.Drawing.Size(75, 61);
            this.btnTest6.TabIndex = 7;
            this.btnTest6.Text = "Test \r\n6";
            this.btnTest6.UseVisualStyleBackColor = true;
            this.btnTest6.Click += new System.EventHandler(this.btnTest6_Click);
            // 
            // btnTest7
            // 
            this.btnTest7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTest7.Location = new System.Drawing.Point(585, 337);
            this.btnTest7.Name = "btnTest7";
            this.btnTest7.Size = new System.Drawing.Size(75, 61);
            this.btnTest7.TabIndex = 8;
            this.btnTest7.Text = "Test \r\n7";
            this.btnTest7.UseVisualStyleBackColor = true;
            this.btnTest7.Click += new System.EventHandler(this.btnTest7_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 407);
            this.Controls.Add(this.btnTest7);
            this.Controls.Add(this.btnTest6);
            this.Controls.Add(this.btnTest5);
            this.Controls.Add(this.btnTest4);
            this.Controls.Add(this.btnTest3);
            this.Controls.Add(this.btnTest2);
            this.Controls.Add(this.btnTest1);
            this.Controls.Add(this.btnSetupPython);
            this.Controls.Add(this.lstEntries);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "IronPython Demonstration";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer tmrUpdate;
        private System.Windows.Forms.ListBox lstEntries;
        private System.Windows.Forms.Button btnSetupPython;
        private System.Windows.Forms.Button btnTest1;
        private System.Windows.Forms.Button btnTest2;
        private System.Windows.Forms.Button btnTest3;
        private System.Windows.Forms.Button btnTest4;
        private System.Windows.Forms.Button btnTest5;
        private System.Windows.Forms.Button btnTest6;
        private System.Windows.Forms.Button btnTest7;
    }
}

