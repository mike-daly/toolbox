using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using System.Runtime.InteropServices;	// to get DllImport() and SYSTEMTIME


namespace GodClockControl
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>


	[ StructLayout( LayoutKind.Sequential )]
	public class SystemTime 
	{
		public ushort year; 
		public ushort month;
		public ushort dayOfWeek;
		public ushort day;
		public ushort hour;
		public ushort minute;
		public ushort second;
		public ushort milliseconds; 
	}

	public class GodClockWrapper 
	{
		[DllImport("godclockclient.dll")]
		public static extern void GetVSystemTime( [In,Out] SystemTime st );
	}

	// Declare IMediaControl as a COM interface which 
	// derives from the IDispatch interface. 
	[Guid("3EDF9FAA-FE8B-43C0-A462-1930299E98DA"),
	InterfaceType(ComInterfaceType.InterfaceIsIUnknown)] 
	public interface IGCStub // cannot list any base interfaces here 
	{ 
		//
		void SetClock(
			[In] long lLastSetHigh, [In] long lLastSetLow, 
			[In] long lAdvanceHigh, [In] long lAdvanceLow,
			[In] double  fSlewRate );

		void GetVClock(
			[Out]	long plVNowHigh, [Out]	long plVNowLow );
		

	};

	public class ClockControl_t 
	{
		//[DllImport("godclockclient.dll")]
		//public static extern void GetVSystemTime( [In,Out] SystemTime st );
		[ComImport, Guid("A76F4742-E169-410F-BD25-E562FC00628D")] 
			public class GodClockStub
		{ 
		}

		public GodClockStub	cGodClock;
		public IGCStub		pIGodClock;
		public TimeSpan		tsDelta;
		public Double		fSlewRate = 1.0;
		public DateTime		dtLastSet;
		//public TimeSpan		tsAccumulatedSlew;	// every tick, add iSlewRate-1 to this

		public ClockControl_t()
		{
			this.tsDelta = new TimeSpan(0);
			//this.tsAccumulatedSlew = new TimeSpan(0);
			this.dtLastSet = new DateTime(0);

			this.cGodClock = new GodClockStub();
			this.pIGodClock = (IGCStub) this.cGodClock;
		}
	}

	public class Form1 : System.Windows.Forms.Form
	{
		public GodClockControl.ClockControl_t ClockControl;

		private System.Windows.Forms.TabControl tabControl_SetTime;
		private System.Windows.Forms.TabPage tabPage_Relative;
		private System.Windows.Forms.TabPage tabPage_Accelerated;
		private System.Windows.Forms.NumericUpDown relative_hour;
		private System.Windows.Forms.NumericUpDown relative_minute;
		private System.Windows.Forms.NumericUpDown relative_second;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button relative_settime;
		private System.Windows.Forms.TextBox Display_SystemTimeNow;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox display_VSystemTimeNow;
		private System.Windows.Forms.TextBox display_ClockSlewRate;
		private System.Windows.Forms.NumericUpDown SlewRateInput;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button accelerated_setslew;
		private System.Windows.Forms.Timer DisplayUpdateTimer;
		private System.Windows.Forms.NumericUpDown relative_days;
		private System.Windows.Forms.Button ZeroRelative;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox displayCurrentOffset;
		private System.Windows.Forms.TextBox displayAccumulatedSlew;
		private System.ComponentModel.IContainer components;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.ClockControl = new ClockControl_t();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.tabControl_SetTime = new System.Windows.Forms.TabControl();
			this.tabPage_Relative = new System.Windows.Forms.TabPage();
			this.relative_settime = new System.Windows.Forms.Button();
			this.relative_hour = new System.Windows.Forms.NumericUpDown();
			this.relative_minute = new System.Windows.Forms.NumericUpDown();
			this.relative_second = new System.Windows.Forms.NumericUpDown();
			this.relative_days = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.ZeroRelative = new System.Windows.Forms.Button();
			this.tabPage_Accelerated = new System.Windows.Forms.TabPage();
			this.accelerated_setslew = new System.Windows.Forms.Button();
			this.SlewRateInput = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.Display_SystemTimeNow = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.display_VSystemTimeNow = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.display_ClockSlewRate = new System.Windows.Forms.TextBox();
			this.DisplayUpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.displayCurrentOffset = new System.Windows.Forms.TextBox();
			this.displayAccumulatedSlew = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.tabControl_SetTime.SuspendLayout();
			this.tabPage_Relative.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.relative_hour)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.relative_minute)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.relative_second)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.relative_days)).BeginInit();
			this.tabPage_Accelerated.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.SlewRateInput)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl_SetTime
			// 
			this.tabControl_SetTime.Controls.AddRange(new System.Windows.Forms.Control[] {
																							 this.tabPage_Relative,
																							 this.tabPage_Accelerated});
			this.tabControl_SetTime.Location = new System.Drawing.Point(8, 8);
			this.tabControl_SetTime.Name = "tabControl_SetTime";
			this.tabControl_SetTime.SelectedIndex = 0;
			this.tabControl_SetTime.Size = new System.Drawing.Size(712, 256);
			this.tabControl_SetTime.TabIndex = 0;
			// 
			// tabPage_Relative
			// 
			this.tabPage_Relative.Controls.AddRange(new System.Windows.Forms.Control[] {
																						   this.relative_settime,
																						   this.relative_hour,
																						   this.relative_minute,
																						   this.relative_second,
																						   this.relative_days,
																						   this.label2,
																						   this.label3,
																						   this.ZeroRelative});
			this.tabPage_Relative.Location = new System.Drawing.Point(4, 22);
			this.tabPage_Relative.Name = "tabPage_Relative";
			this.tabPage_Relative.Size = new System.Drawing.Size(704, 230);
			this.tabPage_Relative.TabIndex = 1;
			this.tabPage_Relative.Text = "RelativeTime";
			// 
			// relative_settime
			// 
			this.relative_settime.Location = new System.Drawing.Point(424, 136);
			this.relative_settime.Name = "relative_settime";
			this.relative_settime.Size = new System.Drawing.Size(96, 23);
			this.relative_settime.TabIndex = 2;
			this.relative_settime.Text = "SetTime";
			this.relative_settime.Click += new System.EventHandler(this.relative_settime_Click);
			// 
			// relative_hour
			// 
			this.relative_hour.Location = new System.Drawing.Point(240, 136);
			this.relative_hour.Maximum = new System.Decimal(new int[] {
																		  23,
																		  0,
																		  0,
																		  0});
			this.relative_hour.Name = "relative_hour";
			this.relative_hour.Size = new System.Drawing.Size(40, 20);
			this.relative_hour.TabIndex = 0;
			// 
			// relative_minute
			// 
			this.relative_minute.Location = new System.Drawing.Point(280, 136);
			this.relative_minute.Maximum = new System.Decimal(new int[] {
																			59,
																			0,
																			0,
																			0});
			this.relative_minute.Name = "relative_minute";
			this.relative_minute.Size = new System.Drawing.Size(40, 20);
			this.relative_minute.TabIndex = 0;
			// 
			// relative_second
			// 
			this.relative_second.Location = new System.Drawing.Point(320, 136);
			this.relative_second.Maximum = new System.Decimal(new int[] {
																			59,
																			0,
																			0,
																			0});
			this.relative_second.Name = "relative_second";
			this.relative_second.Size = new System.Drawing.Size(40, 20);
			this.relative_second.TabIndex = 0;
			// 
			// relative_days
			// 
			this.relative_days.Location = new System.Drawing.Point(192, 136);
			this.relative_days.Maximum = new System.Decimal(new int[] {
																		  3650,
																		  0,
																		  0,
																		  0});
			this.relative_days.Name = "relative_days";
			this.relative_days.Size = new System.Drawing.Size(40, 20);
			this.relative_days.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(80, 136);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Day:HH:MM:SS";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(464, 56);
			this.label3.TabIndex = 1;
			this.label3.Text = "Set the desired interval to add to the current time, and press button to set the " +
				"relative time.  You cannot set time backwards.  The \"Minimum Val.\" button will s" +
				"et the interval to the minimum legal value.";
			// 
			// ZeroRelative
			// 
			this.ZeroRelative.Location = new System.Drawing.Point(424, 176);
			this.ZeroRelative.Name = "ZeroRelative";
			this.ZeroRelative.Size = new System.Drawing.Size(96, 23);
			this.ZeroRelative.TabIndex = 2;
			this.ZeroRelative.Text = "Minimum Val.";
			this.ZeroRelative.Click += new System.EventHandler(this.ZeroRelative_Click);
			// 
			// tabPage_Accelerated
			// 
			this.tabPage_Accelerated.Controls.AddRange(new System.Windows.Forms.Control[] {
																							  this.accelerated_setslew,
																							  this.SlewRateInput,
																							  this.label7,
																							  this.label8});
			this.tabPage_Accelerated.Location = new System.Drawing.Point(4, 22);
			this.tabPage_Accelerated.Name = "tabPage_Accelerated";
			this.tabPage_Accelerated.Size = new System.Drawing.Size(704, 230);
			this.tabPage_Accelerated.TabIndex = 2;
			this.tabPage_Accelerated.Text = "AcceleratedTime";
			// 
			// accelerated_setslew
			// 
			this.accelerated_setslew.Location = new System.Drawing.Point(56, 120);
			this.accelerated_setslew.Name = "accelerated_setslew";
			this.accelerated_setslew.TabIndex = 6;
			this.accelerated_setslew.Text = "Set Slew";
			this.accelerated_setslew.Click += new System.EventHandler(this.accelerated_setslew_Click);
			// 
			// SlewRateInput
			// 
			this.SlewRateInput.DecimalPlaces = 2;
			this.SlewRateInput.Location = new System.Drawing.Point(160, 88);
			this.SlewRateInput.Maximum = new System.Decimal(new int[] {
																		  1000,
																		  0,
																		  0,
																		  0});
			this.SlewRateInput.Name = "SlewRateInput";
			this.SlewRateInput.Size = new System.Drawing.Size(80, 20);
			this.SlewRateInput.TabIndex = 4;
			this.SlewRateInput.Value = new System.Decimal(new int[] {
																		1,
																		0,
																		0,
																		0});
			this.SlewRateInput.ValueChanged += new System.EventHandler(this.SlewRateInput_ValueChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(48, 88);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(112, 16);
			this.label7.TabIndex = 5;
			this.label7.Text = "Accelerate Clock ";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(256, 88);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(112, 16);
			this.label8.TabIndex = 5;
			this.label8.Text = "times normal.";
			// 
			// Display_SystemTimeNow
			// 
			this.Display_SystemTimeNow.CausesValidation = false;
			this.Display_SystemTimeNow.Location = new System.Drawing.Point(208, 288);
			this.Display_SystemTimeNow.Name = "Display_SystemTimeNow";
			this.Display_SystemTimeNow.ReadOnly = true;
			this.Display_SystemTimeNow.Size = new System.Drawing.Size(160, 20);
			this.Display_SystemTimeNow.TabIndex = 1;
			this.Display_SystemTimeNow.TabStop = false;
			this.Display_SystemTimeNow.Tag = "";
			this.Display_SystemTimeNow.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(96, 288);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(104, 23);
			this.label4.TabIndex = 2;
			this.label4.Text = "SystemTime() now:";
			// 
			// display_VSystemTimeNow
			// 
			this.display_VSystemTimeNow.CausesValidation = false;
			this.display_VSystemTimeNow.Location = new System.Drawing.Point(208, 328);
			this.display_VSystemTimeNow.Name = "display_VSystemTimeNow";
			this.display_VSystemTimeNow.ReadOnly = true;
			this.display_VSystemTimeNow.Size = new System.Drawing.Size(160, 20);
			this.display_VSystemTimeNow.TabIndex = 1;
			this.display_VSystemTimeNow.TabStop = false;
			this.display_VSystemTimeNow.Tag = "";
			this.display_VSystemTimeNow.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(56, 328);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(144, 23);
			this.label5.TabIndex = 2;
			this.label5.Text = "VirtualSystemTime() now:";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(416, 376);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(104, 16);
			this.label6.TabIndex = 2;
			this.label6.Text = "Clock Slew rate:";
			// 
			// display_ClockSlewRate
			// 
			this.display_ClockSlewRate.CausesValidation = false;
			this.display_ClockSlewRate.Location = new System.Drawing.Point(528, 376);
			this.display_ClockSlewRate.Name = "display_ClockSlewRate";
			this.display_ClockSlewRate.ReadOnly = true;
			this.display_ClockSlewRate.Size = new System.Drawing.Size(160, 20);
			this.display_ClockSlewRate.TabIndex = 1;
			this.display_ClockSlewRate.TabStop = false;
			this.display_ClockSlewRate.Tag = "";
			this.display_ClockSlewRate.Text = "";
			// 
			// DisplayUpdateTimer
			// 
			this.DisplayUpdateTimer.Enabled = true;
			this.DisplayUpdateTimer.Interval = 1000;
			this.DisplayUpdateTimer.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(416, 288);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 23);
			this.label1.TabIndex = 2;
			this.label1.Text = "Current Offset:";
			// 
			// displayCurrentOffset
			// 
			this.displayCurrentOffset.CausesValidation = false;
			this.displayCurrentOffset.Location = new System.Drawing.Point(528, 288);
			this.displayCurrentOffset.Name = "displayCurrentOffset";
			this.displayCurrentOffset.ReadOnly = true;
			this.displayCurrentOffset.Size = new System.Drawing.Size(160, 20);
			this.displayCurrentOffset.TabIndex = 1;
			this.displayCurrentOffset.TabStop = false;
			this.displayCurrentOffset.Tag = "";
			this.displayCurrentOffset.Text = "";
			// 
			// displayAccumulatedSlew
			// 
			this.displayAccumulatedSlew.CausesValidation = false;
			this.displayAccumulatedSlew.Location = new System.Drawing.Point(528, 328);
			this.displayAccumulatedSlew.Name = "displayAccumulatedSlew";
			this.displayAccumulatedSlew.ReadOnly = true;
			this.displayAccumulatedSlew.Size = new System.Drawing.Size(160, 20);
			this.displayAccumulatedSlew.TabIndex = 1;
			this.displayAccumulatedSlew.TabStop = false;
			this.displayAccumulatedSlew.Tag = "";
			this.displayAccumulatedSlew.Text = "";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(416, 328);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(104, 23);
			this.label9.TabIndex = 2;
			this.label9.Text = "Accumulated Slew:";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(736, 421);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label4,
																		  this.Display_SystemTimeNow,
																		  this.tabControl_SetTime,
																		  this.display_VSystemTimeNow,
																		  this.label5,
																		  this.label6,
																		  this.display_ClockSlewRate,
																		  this.label1,
																		  this.displayCurrentOffset,
																		  this.displayAccumulatedSlew,
																		  this.label9});
			this.Name = "Form1";
			this.Text = "Form1";
			this.tabControl_SetTime.ResumeLayout(false);
			this.tabPage_Relative.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.relative_hour)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.relative_minute)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.relative_second)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.relative_days)).EndInit();
			this.tabPage_Accelerated.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.SlewRateInput)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			// update the status display on the bottom
			// SystemTime
			this.Display_SystemTimeNow.Text = DateTime.UtcNow.ToString();
			
			// VSystemTime
			SystemTime stVNow;
			stVNow = new SystemTime();
			GodClockWrapper.GetVSystemTime( stVNow );

			DateTime dtVNow;
			dtVNow = new DateTime(stVNow.year, stVNow.month, stVNow.day, stVNow.hour, stVNow.minute, stVNow.second, stVNow.milliseconds);
			this.display_VSystemTimeNow.Text = dtVNow.ToString();
			
			// Slew
			this.display_ClockSlewRate.Text = ClockControl.fSlewRate.ToString();
			// Adjustment
			this.displayCurrentOffset.Text = ClockControl.tsDelta.ToString();

		}
		private void update_remote()
		{
			long ftLastSetHigh = 0;
			long ftLastSetLow = 0;
			long ftAdvanceHigh = 0;
			long ftAdvanceLow = 0;

			ClockControl.pIGodClock.SetClock(ftLastSetHigh, ftLastSetLow, 
											ftAdvanceHigh, ftAdvanceLow, 
											ClockControl.fSlewRate );
		}

		private void accelerated_setslew_Click(object sender, System.EventArgs e)
		{
			ClockControl.fSlewRate = (Double)this.SlewRateInput.Value;
			this.display_ClockSlewRate.Text = ClockControl.fSlewRate.ToString();

			update_remote();
		}

		private void relative_settime_Click(object sender, System.EventArgs e)
		{
			ClockControl.tsDelta = new System.TimeSpan( 
				Math.Max(ClockControl.tsDelta.Days, (int)this.relative_days.Value), 
				Math.Max(ClockControl.tsDelta.Hours, (int)this.relative_hour.Value), 
				Math.Max(ClockControl.tsDelta.Minutes, (int)this.relative_minute.Value), 
				Math.Max(ClockControl.tsDelta.Seconds, (int)this.relative_second.Value));

			ZeroRelative_Click(sender, e);

			update_remote();
        }

		private void ZeroRelative_Click(object sender, System.EventArgs e)
		{
			this.relative_days.Value = ClockControl.tsDelta.Days;
			this.relative_hour.Value = ClockControl.tsDelta.Hours;
			this.relative_minute.Value = ClockControl.tsDelta.Minutes;
			this.relative_second.Value = ClockControl.tsDelta.Seconds;
		}

		private void SlewRateInput_ValueChanged(object sender, System.EventArgs e)
		{
		}

	}
}
