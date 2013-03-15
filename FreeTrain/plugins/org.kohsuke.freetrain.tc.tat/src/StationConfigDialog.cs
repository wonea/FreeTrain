using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace freetrain.world.rail.tattc
{
	/// <summary>
	/// StationConfigDialog の概要の説明です。
	/// </summary>
	internal class StationConfigDialog : System.Windows.Forms.Form
	{
		internal StationConfigDialog( StationHandler _currentHandler ) {
			InitializeComponent();

			if( _currentHandler==null )
				_currentHandler = StationHandler.defaultHandler;

			if( _currentHandler is PassStationHandler )
				radioPass.Checked = true;
			if( _currentHandler is FixedDurationStationHandler) {
				FixedDurationStationHandler fdsh = (FixedDurationStationHandler)_currentHandler; 
				durationBox.Value = fdsh.duration.totalMinutes;
				checkTurn1.Checked = fdsh.turnAround;
				radioFixedDuration.Checked = true;
			}
			if( _currentHandler is OnceADayStationHandler ) {
				OnceADayStationHandler oash = (OnceADayStationHandler)_currentHandler;
				hourBox.Value = oash.minutes/60;
				minBox.Value = oash.minutes%60;
				checkTurn2.Checked = oash.turnAround;
				radioSimple.Checked = true;
			}
			if( _currentHandler is AdvancedStationHandler ) {
				radioAdvanced.Checked = true;
			}

			this.currentHandler = _currentHandler;
		}

		internal StationHandler currentHandler;

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private System.Windows.Forms.CheckBox checkTurn2;
		private System.Windows.Forms.CheckBox checkTurn1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown durationBox;
		private System.Windows.Forms.RadioButton radioFixedDuration;


		private System.Windows.Forms.RadioButton radioSimple;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton radioAdvanced;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonAdvanced;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.NumericUpDown hourBox;
		private System.Windows.Forms.NumericUpDown minBox;
		private System.Windows.Forms.RadioButton radioPass;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.radioFixedDuration = new System.Windows.Forms.RadioButton();
			this.radioSimple = new System.Windows.Forms.RadioButton();
			this.hourBox = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.minBox = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.radioAdvanced = new System.Windows.Forms.RadioButton();
			this.buttonAdvanced = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonOk = new System.Windows.Forms.Button();
			this.radioPass = new System.Windows.Forms.RadioButton();
			this.checkTurn2 = new System.Windows.Forms.CheckBox();
			this.checkTurn1 = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.durationBox = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.hourBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.minBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.durationBox)).BeginInit();
			this.SuspendLayout();
			// 
			// radioFixedDuration
			// 
			this.radioFixedDuration.Location = new System.Drawing.Point(8, 35);
			this.radioFixedDuration.Name = "radioFixedDuration";
			this.radioFixedDuration.Size = new System.Drawing.Size(128, 17);
			this.radioFixedDuration.TabIndex = 2;
			this.radioFixedDuration.Text = "Pla&nned halt";
			//! this.radioFixedDuration.Text = "一定時間停車(&N)";
			this.radioFixedDuration.CheckedChanged += new System.EventHandler(this.onRadioChanged);
			// 
			// radioSimple
			// 
			this.radioSimple.Location = new System.Drawing.Point(8, 113);
			this.radioSimple.Name = "radioSimple";
			this.radioSimple.Size = new System.Drawing.Size(128, 17);
			this.radioSimple.TabIndex = 3;
			this.radioSimple.Text = "&Scheduled departure";
			//! this.radioSimple.Text = "指定時刻発車(&S)";
			this.radioSimple.CheckedChanged += new System.EventHandler(this.onRadioChanged);
			// 
			// hourBox
			// 
			this.hourBox.Location = new System.Drawing.Point(32, 139);
			this.hourBox.Maximum = new System.Decimal(new int[] {
																	24,
																	0,
																	0,
																	0});
			this.hourBox.Name = "hourBox";
			this.hourBox.Size = new System.Drawing.Size(48, 20);
			this.hourBox.TabIndex = 4;
			this.hourBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.hourBox.ValueChanged += new System.EventHandler(this.onTimeChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(80, 139);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 17);
			this.label1.TabIndex = 5;
			this.label1.Text = "Hour";
			//! this.label1.Text = "時";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// minBox
			// 
			this.minBox.Increment = new System.Decimal(new int[] {
																	 10,
																	 0,
																	 0,
																	 0});
			this.minBox.Location = new System.Drawing.Point(117, 139);
			this.minBox.Maximum = new System.Decimal(new int[] {
																   60,
																   0,
																   0,
																   0});
			this.minBox.Name = "minBox";
			this.minBox.Size = new System.Drawing.Size(48, 20);
			this.minBox.TabIndex = 6;
			this.minBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.minBox.ValueChanged += new System.EventHandler(this.onTimeChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(171, 139);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(47, 17);
			this.label2.TabIndex = 7;
			this.label2.Text = "Minute";
			//! this.label2.Text = "分";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// radioAdvanced
			// 
			this.radioAdvanced.Location = new System.Drawing.Point(8, 191);
			this.radioAdvanced.Name = "radioAdvanced";
			this.radioAdvanced.Size = new System.Drawing.Size(24, 17);
			this.radioAdvanced.TabIndex = 8;
			this.radioAdvanced.CheckedChanged += new System.EventHandler(this.onRadioChanged);
			// 
			// buttonAdvanced
			// 
			this.buttonAdvanced.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonAdvanced.Location = new System.Drawing.Point(32, 191);
			this.buttonAdvanced.Name = "buttonAdvanced";
			this.buttonAdvanced.Size = new System.Drawing.Size(120, 26);
			this.buttonAdvanced.TabIndex = 9;
			this.buttonAdvanced.Text = "&Advanced Settings...";
			//! this.buttonAdvanced.Text = "高度な設定(&A)...";
			this.buttonAdvanced.Click += new System.EventHandler(this.buttonAdvanced_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Location = new System.Drawing.Point(218, -9);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(2, 304);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOk.Location = new System.Drawing.Point(226, 9);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(72, 26);
			this.buttonOk.TabIndex = 10;
			this.buttonOk.Text = "&OK";
			this.buttonOk.Click += new System.EventHandler(this.onOK);
			// 
			// radioPass
			// 
			this.radioPass.Location = new System.Drawing.Point(8, 9);
			this.radioPass.Name = "radioPass";
			this.radioPass.Size = new System.Drawing.Size(128, 17);
			this.radioPass.TabIndex = 1;
			this.radioPass.Text = "&Pass";
			//! this.radioPass.Text = "通過(&P)";
			this.radioPass.CheckedChanged += new System.EventHandler(this.onRadioChanged);
			// 
			// checkTurn2
			// 
			this.checkTurn2.Location = new System.Drawing.Point(32, 165);
			this.checkTurn2.Name = "checkTurn2";
			this.checkTurn2.Size = new System.Drawing.Size(144, 17);
			this.checkTurn2.TabIndex = 11;
			this.checkTurn2.Text = "&Turn back";
			//! this.checkTurn2.Text = "折り返す(&T)";
			// 
			// checkTurn1
			// 
			this.checkTurn1.Location = new System.Drawing.Point(32, 87);
			this.checkTurn1.Name = "checkTurn1";
			this.checkTurn1.Size = new System.Drawing.Size(144, 17);
			this.checkTurn1.TabIndex = 14;
			this.checkTurn1.Text = "&Turn back";
			//! this.checkTurn1.Text = "折り返す(&T)";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(80, 61);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 17);
			this.label3.TabIndex = 13;
			this.label3.Text = "Minutes";
			//! this.label3.Text = "分間";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// durationBox
			// 
			this.durationBox.Increment = new System.Decimal(new int[] {
																		  10,
																		  0,
																		  0,
																		  0});
			this.durationBox.Location = new System.Drawing.Point(32, 61);
			this.durationBox.Maximum = new System.Decimal(new int[] {
																		1215752191,
																		23,
																		0,
																		0});
			this.durationBox.Minimum = new System.Decimal(new int[] {
																		10,
																		0,
																		0,
																		0});
			this.durationBox.Name = "durationBox";
			this.durationBox.Size = new System.Drawing.Size(48, 20);
			this.durationBox.TabIndex = 12;
			this.durationBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.durationBox.Value = new System.Decimal(new int[] {
																	  10,
																	  0,
																	  0,
																	  0});
			// 
			// StationConfigDialog
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(310, 229);
			this.Controls.Add(this.checkTurn1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.durationBox);
			this.Controls.Add(this.checkTurn2);
			this.Controls.Add(this.radioPass);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonAdvanced);
			this.Controls.Add(this.radioAdvanced);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.minBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.hourBox);
			this.Controls.Add(this.radioSimple);
			this.Controls.Add(this.radioFixedDuration);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StationConfigDialog";
			this.ShowInTaskbar = false;
			this.Text = "Departure time settings";
			//! this.Text = "発車時刻の設定";
			((System.ComponentModel.ISupportInitialize)(this.hourBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.minBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.durationBox)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonAdvanced_Click(object sender, System.EventArgs e) {
			if(!(currentHandler is AdvancedStationHandler ))
				currentHandler = new AdvancedStationHandler();

			new StationAdvancedDialog( (AdvancedStationHandler)currentHandler ).ShowDialog(this);
		}

		private int getMinutes() {
			return ((int)hourBox.Value)*60 + ((int)minBox.Value);
		}

		private void onTimeChanged(object sender, System.EventArgs e) {
			if( (int)minBox.Value == 60 ) {
				minBox.Value = 0;
				hourBox.Value += 1;
			}
			if( (int)hourBox.Value == 24 ) {
				hourBox.Value = 0;
			}
		}

		private void onRadioChanged( object sender, System.EventArgs e ) {
			durationBox.Enabled = checkTurn1.Enabled = radioFixedDuration.Checked;
			
			hourBox.Enabled = minBox.Enabled = checkTurn2.Enabled = radioSimple.Checked;

			buttonAdvanced.Enabled = radioAdvanced.Checked;
		}

		private void onOK(object sender, System.EventArgs e) {
			if( radioFixedDuration.Checked )
				currentHandler = new FixedDurationStationHandler(TimeLength.fromMinutes((long)durationBox.Value),checkTurn1.Checked);
			if( radioPass.Checked )	
				currentHandler = new PassStationHandler();
			if( radioSimple.Checked )
				currentHandler = new OnceADayStationHandler(getMinutes(),checkTurn2.Checked);
			if( radioAdvanced.Checked ) {
				if(!(currentHandler is AdvancedStationHandler))
					currentHandler = new AdvancedStationHandler();
			}
		}
	}
}
