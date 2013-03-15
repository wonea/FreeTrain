using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.controls;
using freetrain.framework;
using freetrain.framework.plugin;

namespace freetrain.world.accounting
{
	/// <summary>
	/// Displays the balance sheet.
	/// </summary>
	public class BalanceSheetForm : Form
	{
		#region singleton instance
		public static void create() {
			if( theInstance==null ) {
				theInstance = new BalanceSheetForm();
				theInstance.Show();
			}
			theInstance.BringToFront();
		}

		private static Form theInstance = null;
		
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			theInstance = null;
		}
		#endregion


		private System.ComponentModel.Container components = null;
		private freetrain.controls.WebBrowser webBrowser;

		private BalanceSheetForm() {
//			this.MdiParent = MainWindow.mainWindow;
			InitializeComponent();

//            object flags = 0;
//            object targetFrame = String.Empty;
//            object postData = String.Empty;
//            object headers = String.Empty;
//            webBrowser.Navigate("about:hello", ref flags, ref targetFrame, ref postData, ref headers);

			webBrowser.navigate("about:blank");
			webBrowser.navigate(ResourceUtil.findSystemResource("balanceSheet.html"));
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BalanceSheetForm));
			this.webBrowser = new freetrain.controls.WebBrowser();
			((System.ComponentModel.ISupportInitialize)(this.webBrowser)).BeginInit();
			this.SuspendLayout();
			// 
			// webBrowser
			// 
			this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowser.Enabled = true;
			this.webBrowser.Size = new System.Drawing.Size(592, 206);
			this.webBrowser.TabIndex = 0;
			// 
			// BalanceSheetForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(592, 206);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.webBrowser});
			this.Name = "BalanceSheetForm";
			this.Text = "Balance Sheet";
			//! this.Text = "バランスシート";
			((System.ComponentModel.ISupportInitialize)(this.webBrowser)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
