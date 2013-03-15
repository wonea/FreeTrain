using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace freetrain.tools.vcr
{
	/// <summary>
	/// ConfigDialog の概要の説明です。
	/// </summary>
	public class ConfigDialog : System.Windows.Forms.Form
	{
		/// <summary>
		/// Fill in this class upon the successful completeion of the dialog.
		/// </summary>
		private readonly VCROptions options;

		public ConfigDialog( VCROptions _options )
		{
			this.options = _options;

			InitializeComponent();
			fps.ValueChanged += new EventHandler(updateHelp);
			period.ValueChanged += new EventHandler(updateHelp);

			CompressorMoniker[] cm = DirectShowUtil.EnumCompressors();
			compressors.DataSource = cm;

			if( options.compressor!=null ) {
				string curName = options.compressor.name;
				for( int i=0; i<cm.Length; i++ ) {
					if( cm[i].name==curName )
						compressors.SelectedIndex = i;
				}
			}
			fps.Value = options.fps;
			period.Value = options.period;

		}


		#region Windows Form Designer generated code
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox compressors;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown fps;
		private System.Windows.Forms.NumericUpDown period;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label helpPanel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.ComponentModel.Container components = null;

		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.compressors = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.fps = new System.Windows.Forms.NumericUpDown();
			this.period = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.helpPanel = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.fps)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.period)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Compression Method";
			//! this.label1.Text = "圧縮方式(&C)：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// compressors
			// 
			this.compressors.DisplayMember = "name";
			this.compressors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.compressors.Location = new System.Drawing.Point(88, 8);
			this.compressors.Name = "compressors";
			this.compressors.Size = new System.Drawing.Size(248, 20);
			this.compressors.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 24);
			this.label2.TabIndex = 2;
			this.label2.Text = "&Exposure Interval";
			//! this.label2.Text = "露出間隔(&E)：";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(208, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 24);
			this.label3.TabIndex = 4;
			this.label3.Text = "One frame a minute";
			//! this.label3.Text = "分に一コマ";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 24);
			this.label4.TabIndex = 5;
			this.label4.Text = "&Framerate:";
			//! this.label4.Text = "ﾌﾚｰﾑﾚｰﾄ(&F)：";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// fps
			// 
			this.fps.Location = new System.Drawing.Point(88, 56);
			this.fps.Name = "fps";
			this.fps.Size = new System.Drawing.Size(112, 19);
			this.fps.TabIndex = 6;
			this.fps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// period
			// 
			this.period.Increment = new System.Decimal(new int[] {
																	 30,
																	 0,
																	 0,
																	 0});
			this.period.Location = new System.Drawing.Point(88, 32);
			this.period.Maximum = new System.Decimal(new int[] {
																   10000000,
																   0,
																   0,
																   0});
			this.period.Name = "period";
			this.period.Size = new System.Drawing.Size(112, 19);
			this.period.TabIndex = 7;
			this.period.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.period.Value = new System.Decimal(new int[] {
																 120,
																 0,
																 0,
																 0});
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(208, 56);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(72, 24);
			this.label5.TabIndex = 8;
			this.label5.Text = "fps";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// helpPanel
			// 
			this.helpPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.helpPanel.Location = new System.Drawing.Point(88, 88);
			this.helpPanel.Name = "helpPanel";
			this.helpPanel.Size = new System.Drawing.Size(248, 32);
			this.helpPanel.TabIndex = 10;
			this.helpPanel.Text = "For each second of the video, 6 game hour passes";
			//! this.helpPanel.Text = "ビデオ１秒の間にゲーム時間は6時間進みます";
			// 
			// btnOK
			// 
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(168, 128);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(80, 24);
			this.btnOK.TabIndex = 11;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(256, 128);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 24);
			this.btnCancel.TabIndex = 12;
			this.btnCancel.Text = "&Cancel";
			//! this.btnCancel.Text = "ｷｬﾝｾﾙ(&C)";
			// 
			// ConfigDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(346, 158);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnCancel,
																		  this.btnOK,
																		  this.helpPanel,
																		  this.label5,
																		  this.period,
																		  this.fps,
																		  this.label4,
																		  this.label3,
																		  this.label2,
																		  this.compressors,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigDialog";
			this.ShowInTaskbar = false;
			this.Text = "Recording Settings";
			//! this.Text = "録画の設定";
			((System.ComponentModel.ISupportInitialize)(this.fps)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.period)).EndInit();
			this.ResumeLayout(false);

		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}
		#endregion

		private void btnOK_Click(object sender, EventArgs e) {
			options.compressor = (CompressorMoniker)compressors.SelectedItem;
			options.fps = (int)fps.Value;
			options.period = (int)period.Value;

			options.save();
			Close();
		}

		private void updateHelp(object sender, EventArgs e) {
			long GMperVS = (long)(period.Value/fps.Value);	// Game minutes per video second
			string[] units = new string[]{"mins","hrs","days","mons","years"};
			//! string[] units = new string[]{"分","時間","日","月","年"};
			int[]  sizes = new int[]     {60,24,30,12};

			long v=GMperVS;
			int idx=0;
			bool exact = (period.Value%fps.Value)==0;

			while( idx<sizes.Length && v>2*sizes[idx] ) {
				if( (v%sizes[idx])!=0 )		exact=false;
				v /= sizes[idx];
				idx++;
			}
			
			helpPanel.Text = string.Format(
				"For each second of the video, {0}{1}{2} of game time passes",
				//! "ビデオ１秒の間にゲーム時間は{0}{1}{2}進みます",
				exact?"":"ca",
				//! exact?"":"約",
				v,
				units[idx] );
		}
	}
}
