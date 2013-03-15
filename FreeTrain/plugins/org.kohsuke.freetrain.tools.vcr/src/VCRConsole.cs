using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using freetrain.framework;
using System.Runtime.InteropServices;

namespace freetrain.tools.vcr
{
	/// <summary>
	/// Video Recorder Console.
	/// 
	/// One cannot change the setting while in the recording state.
	/// </summary>
	public class VCRConsole : System.Windows.Forms.Form
	{
		/// <summary>
		/// Configuration
		/// </summary>
		private readonly VCROptions options = new VCROptions().load();

		/// <summary>
		/// Capture rect.
		/// </summary>
		private Rectangle rect;

		/// <summary>
		/// Recorder object, if we are in the recording/pausing state.
		/// </summary>
		private Recorder recorder;

		/// <summary>
		/// If in the region select mode, this variable
		/// holds a reference to the controller
		/// </summary>
		private VCRController controller;


		private readonly Bitmap imageIcons;

		public VCRConsole() {
			try {
				InitializeComponent();
				Stream s =System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(
					typeof(VCRConsole),"resource.icons.bmp");
				
				imageIcons=new Bitmap(s);
				imageIcons.MakeTransparent(imageIcons.GetPixel(0,0));
				imageList.Images.AddStrip(imageIcons);

				btnRecord.ImageList = imageList;
				btnRecord.ImageIndex = 0;
				btnPause.ImageList = imageList;
				btnPause.ImageIndex = 1;
				btnStop.ImageList = imageList;
				btnStop.ImageIndex = 2;

				btnStop.Checked = true;
			} catch( Exception e ) {
				Debug.WriteLine(e);
				throw e;
			}
		}

		#region Windows Form Designer generated code
		private System.Windows.Forms.Label helpPanel;
		private System.Windows.Forms.Button btnConfig;
		private System.Windows.Forms.RadioButton btnRecord;
		private System.Windows.Forms.RadioButton btnPause;
		private System.Windows.Forms.RadioButton btnStop;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.CheckBox btnSetRect;

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.btnConfig = new System.Windows.Forms.Button();
			this.btnRecord = new System.Windows.Forms.RadioButton();
			this.btnPause = new System.Windows.Forms.RadioButton();
			this.btnStop = new System.Windows.Forms.RadioButton();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.btnSetRect = new System.Windows.Forms.CheckBox();
			this.helpPanel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnConfig
			// 
			this.btnConfig.Font = new System.Drawing.Font("Webdings", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.btnConfig.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnConfig.Location = new System.Drawing.Point(136, 0);
			this.btnConfig.Name = "btnConfig";
			this.btnConfig.Size = new System.Drawing.Size(32, 32);
			this.btnConfig.TabIndex = 3;
			this.btnConfig.Text = "@";
			this.btnConfig.Click += new System.EventHandler(this.config);
			// 
			// btnRecord
			// 
			this.btnRecord.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnRecord.Name = "btnRecord";
			this.btnRecord.Size = new System.Drawing.Size(32, 32);
			this.btnRecord.TabIndex = 4;
			this.btnRecord.CheckedChanged += new System.EventHandler(this.stateChanged);
			// 
			// btnPause
			// 
			this.btnPause.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnPause.Location = new System.Drawing.Point(32, 0);
			this.btnPause.Name = "btnPause";
			this.btnPause.Size = new System.Drawing.Size(32, 32);
			this.btnPause.TabIndex = 5;
			this.btnPause.CheckedChanged += new System.EventHandler(this.stateChanged);
			// 
			// btnStop
			// 
			this.btnStop.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnStop.Location = new System.Drawing.Point(64, 0);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(32, 32);
			this.btnStop.TabIndex = 6;
			this.btnStop.CheckedChanged += new System.EventHandler(this.stateChanged);
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList.ImageSize = new System.Drawing.Size(24, 24);
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// btnSetRect
			// 
			this.btnSetRect.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnSetRect.Font = new System.Drawing.Font("Webdings", 18F);
			this.btnSetRect.Location = new System.Drawing.Point(104, 0);
			this.btnSetRect.Name = "btnSetRect";
			this.btnSetRect.Size = new System.Drawing.Size(32, 32);
			this.btnSetRect.TabIndex = 9;
			this.btnSetRect.Text = "c";
			this.btnSetRect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btnSetRect.CheckedChanged += new System.EventHandler(this.setRect);
			// 
			// helpPanel
			// 
			this.helpPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.helpPanel.Location = new System.Drawing.Point(4, 36);
			this.helpPanel.Name = "helpPanel";
			this.helpPanel.Size = new System.Drawing.Size(160, 24);
			this.helpPanel.TabIndex = 10;
			// 
			// VCRConsole
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(170, 64);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.helpPanel,
																		  this.btnSetRect,
																		  this.btnStop,
																		  this.btnPause,
																		  this.btnRecord,
																		  this.btnConfig});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "VCRConsole";
            this.Text = "Video Recorder";
            //! this.Text = "ビデオレコーダ";
			this.ResumeLayout(false);

		}

		#endregion

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			imageIcons.Dispose();
			if(recorder!=null)
				recorder.Dispose();
			base.Dispose( disposing );
		}

		private void stateChanged(object sender, System.EventArgs e) {
			if( btnRecord.Checked ) {
				if( recorder==null )
					recorder = new Recorder( rect, options );
				recorder.start();
			}
			if( btnPause.Checked ) {
				Debug.Assert(recorder!=null);
				recorder.pause();
			}
			if( btnStop.Checked ) {
				if( recorder!=null ) {
					recorder.stop();
					recorder.Dispose();
					recorder = null;
				}
			}
			update();
		}


		/// <summary>
		/// Updates the states of buttons and the help panel.
		/// </summary>
		private void update() {
			Recorder.State s;
			if( recorder!=null )	s = recorder.currentState;
			else					s = Recorder.State.Stopping;
			
			btnPause.Enabled   = (s!=Recorder.State.Stopping && controller==null);
			btnSetRect.Enabled = (s==Recorder.State.Stopping && controller==null);
			btnConfig.Enabled  = (s==Recorder.State.Stopping && controller==null);

			btnSetRect.Enabled = (s==Recorder.State.Stopping);
			btnConfig.Enabled  = (s==Recorder.State.Stopping);

			// update the help panel
			string str;
			if( controller!=null ) {
				str = "Left click to begin rectangle setting. Right click or use the recording range button to finish"+
					"Click the border to change size";
				//! str = "左クリックで矩形設定開始。右クリックまたは録画範囲設定ボタンで設定完了。"+
				//! 	"境界をクリックしてサイズ変更";
			} else
			if( btnStop.Checked ) {
				str = "In order from the left: record, pause, stop, recording range, settings";
				//! str = "左から順に「録画」「中断」「停止」「録画範囲指定」「設定」";
			} else
			if( btnPause.Checked ) {
				str = "Recording is paused";
				//! str = "録画は中断中です";
			} else {
				str = "Recording...";
				//! str = "録画中です";
			}

			helpPanel.Text = str;
		}

		private void config(object sender, System.EventArgs e) {
			using(ConfigDialog cfg=new ConfigDialog(options)) {
				cfg.ShowDialog(this);
			}
		}
		private void setRect(object sender, System.EventArgs e) {
			if( btnSetRect.Checked ) {
				controller = new VCRController(rect);
				controller.OnClosed += new EventHandler(onControllerClosed);
				MainWindow.mainWindow.attachController(controller);
			} else {
				if( MainWindow.mainWindow.currentController==controller )
					MainWindow.mainWindow.detachController();
					// this will close the controller
			}
			update();
		}

		private void onControllerClosed(object sender, EventArgs e) {
			Debug.WriteLine("rect selector closed");
			Debug.Assert(controller!=null);
			this.rect = controller.rect;
			controller = null;
			if( btnSetRect.Checked )
				btnSetRect.Checked = false;
		}


		// interop
		private class InterOp1
		{
			[DllImport("DirectShow.VideoRecorder.dll",EntryPoint="DllRegisterServer")]
			public static extern void regsvr();
		}
		private class InterOp2
		{
			[DllImport("BitmapWriter.TypeLib.dll",EntryPoint="DllRegisterServer")]
			public static extern void regsvr();
		}

		static VCRConsole() {
			try {
				InterOp1.regsvr();
			} catch( Exception ) {
				Debug.WriteLine("unable to register VideoRecorder.dll");
			}
			try {
				InterOp2.regsvr();
			} catch( Exception ) {
				Debug.WriteLine("unable to register BitmapWriter.TypeLib.dll");
			}
		}
	}
}
