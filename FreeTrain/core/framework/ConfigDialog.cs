using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.world;
using org.kohsuke.directdraw;

namespace freetrain.framework
{
	/// <summary>
	/// ConfigDialog の概要の説明です。
	/// </summary>
	public class ConfigDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioMsgBox;
		private System.Windows.Forms.RadioButton radioStatus;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TrackBar msgStatusLength;
		private System.Windows.Forms.CheckBox drawStationNames;
		private System.Windows.Forms.CheckBox showBoundingBox;
		private System.Windows.Forms.CheckBox hideTrees;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ComboBox comboSurfaceAlloc;

		private readonly GlobalOptions opts;
		
		public ConfigDialog() : this(Core.options) {}

		public ConfigDialog( GlobalOptions opts ) {
			this.opts = opts;
			InitializeComponent();

			radioMsgBox.Checked = opts.showErrorMessageBox;
			radioStatus.Checked = !opts.showErrorMessageBox;
			msgStatusLength.Value = opts.messageDisplayTime;
			drawStationNames.Checked = opts.drawStationNames;
			showBoundingBox.Checked = opts.drawBoundingBox;
			hideTrees.Checked = opts.hideTrees;
			comboSurfaceAlloc.SelectedIndex = (int)opts.SurfaceAlloc;
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		private System.ComponentModel.IContainer components;

		#region Windows Form Designer generated code

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.msgStatusLength = new System.Windows.Forms.TrackBar();
			this.radioStatus = new System.Windows.Forms.RadioButton();
			this.radioMsgBox = new System.Windows.Forms.RadioButton();
			this.drawStationNames = new System.Windows.Forms.CheckBox();
			this.showBoundingBox = new System.Windows.Forms.CheckBox();
			this.hideTrees = new System.Windows.Forms.CheckBox();
			this.comboSurfaceAlloc = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.msgStatusLength)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(232, 296);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(80, 24);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "&OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(320, 296);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(80, 24);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "&Cancel";
			//! this.buttonCancel.Text = "ｷｬﾝｾﾙ(&C)";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.msgStatusLength);
			this.groupBox1.Controls.Add(this.radioStatus);
			this.groupBox1.Controls.Add(this.radioMsgBox);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(392, 80);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Display error messages";
			//! this.groupBox1.Text = "エラーメッセージの表示";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(160, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Display time:";
			//! this.label1.Text = "表示時間：";
			// 
			// msgStatusLength
			// 
			this.msgStatusLength.Location = new System.Drawing.Point(160, 32);
			this.msgStatusLength.Minimum = 1;
			this.msgStatusLength.Name = "msgStatusLength";
			this.msgStatusLength.Size = new System.Drawing.Size(224, 42);
			this.msgStatusLength.TabIndex = 2;
			this.msgStatusLength.Value = 1;
			// 
			// radioStatus
			// 
			this.radioStatus.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioStatus.Location = new System.Drawing.Point(16, 48);
			this.radioStatus.Name = "radioStatus";
			this.radioStatus.Size = new System.Drawing.Size(144, 16);
			this.radioStatus.TabIndex = 1;
			this.radioStatus.Text = "Display in status bar";
			//! this.radioStatus.Text = "ステータスバーに表示";
			this.radioStatus.CheckedChanged += new System.EventHandler(this.onRadioMsgStyle);
			// 
			// radioMsgBox
			// 
			this.radioMsgBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioMsgBox.Location = new System.Drawing.Point(16, 24);
			this.radioMsgBox.Name = "radioMsgBox";
			this.radioMsgBox.Size = new System.Drawing.Size(144, 16);
			this.radioMsgBox.TabIndex = 0;
			this.radioMsgBox.Text = "Display message box";
			//! this.radioMsgBox.Text = "メッセージボックスを表示";
			this.radioMsgBox.CheckedChanged += new System.EventHandler(this.onRadioMsgStyle);
			// 
			// drawStationNames
			// 
			this.drawStationNames.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.drawStationNames.Location = new System.Drawing.Point(24, 104);
			this.drawStationNames.Name = "drawStationNames";
			this.drawStationNames.Size = new System.Drawing.Size(168, 16);
			this.drawStationNames.TabIndex = 3;
			this.drawStationNames.Text = "Display station names";
			//! this.drawStationNames.Text = "駅の名前を画面に表示";
			// 
			// showBoundingBox
			// 
			this.showBoundingBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.showBoundingBox.Location = new System.Drawing.Point(24, 128);
			this.showBoundingBox.Name = "showBoundingBox";
			this.showBoundingBox.Size = new System.Drawing.Size(168, 16);
			this.showBoundingBox.TabIndex = 4;
			this.showBoundingBox.Text = "Display drawing range (debug)";
			//! this.showBoundingBox.Text = "描画範囲を表示(デバッグ)";
			// 
			// hideTrees
			// 
			this.hideTrees.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.hideTrees.Location = new System.Drawing.Point(24, 152);
			this.hideTrees.Name = "hideTrees";
			this.hideTrees.Size = new System.Drawing.Size(168, 16);
			this.hideTrees.TabIndex = 4;
			this.hideTrees.Text = "Do not display trees";
			//! this.hideTrees.Text = "樹木の描画を省略";
			// 
			// comboSurfaceAlloc
			// 
			this.comboSurfaceAlloc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSurfaceAlloc.Items.AddRange(new object[] {
																   "Automatically",
																   "Limit to VRAM",
																   "Limit to system RAM"});
																   //! "自動的に判断する",
																   //! "ビデオメモリに限定する",
																   //! "システムメモリに限定する"});
			this.comboSurfaceAlloc.Location = new System.Drawing.Point(184, 182);
			this.comboSurfaceAlloc.Name = "comboSurfaceAlloc";
			this.comboSurfaceAlloc.Size = new System.Drawing.Size(200, 20);
			this.comboSurfaceAlloc.TabIndex = 5;
			this.toolTip.SetToolTip(this.comboSurfaceAlloc, "Change if drawing is slow and causing errors.");
			//! this.toolTip.SetToolTip(this.comboSurfaceAlloc, "描画が遅かったりエラーがでるようなら変更してください。");
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 184);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(160, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "Cache offscreen surfaces:";
			//! this.label2.Text = "オフスクリーンサーフェスの確保：";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip.SetToolTip(this.label2, "Change if drawing is slow and causing errors.");
			//! this.toolTip.SetToolTip(this.label2, "描画が遅かったりエラーがでるようなら変更してください。");
			// 
			// ConfigDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(408, 325);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboSurfaceAlloc);
			this.Controls.Add(this.showBoundingBox);
			this.Controls.Add(this.drawStationNames);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.hideTrees);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "FreeTrain settings";
			//! this.Text = "FreeTrainの設定";
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.msgStatusLength)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void onRadioMsgStyle(object sender, EventArgs e) {
			msgStatusLength.Enabled = radioStatus.Checked;
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			opts.showErrorMessageBox = radioMsgBox.Checked;
			opts.messageDisplayTime = msgStatusLength.Value;
			opts.drawStationNames = drawStationNames.Checked;
			opts.drawBoundingBox = showBoundingBox.Checked;
			opts.hideTrees = hideTrees.Checked;
			opts.SurfaceAlloc = (DDSurfaceAllocation)comboSurfaceAlloc.SelectedIndex;
			opts.save();
			Close();
		}


	}
}
