using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.controls;
using org.kohsuke.directdraw;
using freetrain.util;

namespace freetrain.framework
{
	/// <summary>
	/// AboutDialog の概要の説明です。
	/// (summary for AboutDialog)
	/// </summary>
	public class AboutDialog : System.Windows.Forms.Form
	{
		public static void show() {
			AboutDialog dlg = new AboutDialog();
			dlg.ShowDialog(MainWindow.mainWindow);
		}

		public AboutDialog() {
			InitializeComponent();

			browser.navigate("about:blank");
			browser.navigate(ResourceUtil.findSystemResource("about.html"));
		}

		protected override void OnLoad(System.EventArgs e) {
			using( WindowedDirectDraw dd = new WindowedDirectDraw(this) ) {
				this.size.Text = format(dd.availableVideoMemory)+"/"+format(dd.totalVideoMemory);
				this.displayMode.Text = dd.primarySurface.displayModeName;
				this.progressBar.Value = Math.Min( 10000,
					(int)(10000.0*dd.availableVideoMemory/dd.totalVideoMemory) );
			}
		}

		private string format( long value ) {
			value /= 1024;
			return value+"KB";
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label size;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label displayMode;
		private freetrain.controls.WebBrowser browser;
		private System.Windows.Forms.Panel panel1;
		private System.ComponentModel.Container components = null;
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AboutDialog));
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.label3 = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.size = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.displayMode = new System.Windows.Forms.Label();
			this.browser = new freetrain.controls.WebBrowser();
			this.panel1 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.browser)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.progressBar.Location = new System.Drawing.Point(8, 167);
			this.progressBar.Maximum = 10000;
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(320, 16);
			this.progressBar.TabIndex = 3;
			this.progressBar.Value = 30;
			// 
			// label3
			// 
			this.label3.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.label3.Location = new System.Drawing.Point(8, 151);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "Available VRAM:";
			//! this.label3.Text = "VRAM空き容量：";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// okButton
			// 
			this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.okButton.Location = new System.Drawing.Point(248, 191);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(80, 24);
			this.okButton.TabIndex = 5;
			this.okButton.Text = "&OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// size
			// 
			this.size.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.size.Location = new System.Drawing.Point(104, 151);
			this.size.Name = "size";
			this.size.Size = new System.Drawing.Size(152, 16);
			this.size.TabIndex = 6;
			this.size.Text = "100KB/64MB";
			this.size.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.label4.Location = new System.Drawing.Point(8, 135);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 16);
			this.label4.TabIndex = 7;
			this.label4.Text = "Display mode:";
			//! this.label4.Text = "画面モード：";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// displayMode
			// 
			this.displayMode.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.displayMode.Location = new System.Drawing.Point(104, 135);
			this.displayMode.Name = "displayMode";
			this.displayMode.Size = new System.Drawing.Size(144, 16);
			this.displayMode.TabIndex = 8;
			this.displayMode.Text = "---";
			this.displayMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// browser
			// 
			this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.browser.Enabled = true;
			this.browser.Size = new System.Drawing.Size(320, 192);
			this.browser.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.browser});
			this.panel1.Location = new System.Drawing.Point(8, 8);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(320, 120);
			this.panel1.TabIndex = 9;
			// 
			// AboutDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(338, 223);
			this.Controls.AddRange(
				new System.Windows.Forms.Control[] {
					this.panel1,
					this.displayMode,
					this.label4,
					this.size,
					this.okButton,
					this.label3,
					this.progressBar
				});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutDialog";
			this.ShowInTaskbar = false;
			this.Text = "About FreeTrain";
			//! this.Text = "FreeTrainについて";
			((System.ComponentModel.ISupportInitialize)(this.browser)).EndInit();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void okButton_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e) {
			UrlInvoker.openUrl( ((LinkLabel)sender).Text );
		}

	}
}
