using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.contributions.land;
using freetrain.contributions.common;
using freetrain.contributions.sound;
using freetrain.views;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.terrain;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.util;
using org.kohsuke.directdraw;

namespace freetrain.controllers.land
{
	/// <summary>
	/// Controller that allows the user to
	/// place/remove lands.
	/// </summary>
	public class BulldozeController : ControllerHostForm
	{
		#region Singleton instance management
		/// <summary>
		/// Creates a new controller window, or active the existing one.
		/// </summary>
		public static void create() {
			if(theInstance==null)
				theInstance = new BulldozeController();			
			theInstance.Show();
			theInstance.Activate();
		}

		private static BulldozeController theInstance;

		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			theInstance = null;
		}
		#endregion

		private Bitmap previewBitmap;

		protected BulldozeController() {
			InitializeComponent();
			previewBitmap = ResourceUtil.loadSystemBitmap("bulldozer.bmp");
			preview.Image=previewBitmap;
			LandBuilderContribution builder = (LandBuilderContribution)PluginManager.theInstance.getContribution("{AE43E6DB-39F0-49FE-BE18-EE3FAC248FDE}");
			currentController = builder.createBuilder(new ControllerSiteImpl(this));
		}

		protected override void Dispose( bool disposing ) {
			preview.Image=null;
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
			
			if( previewBitmap!=null )
				previewBitmap.Dispose();
		}

		#region Designer generated code
		private System.Windows.Forms.PictureBox preview;
		private System.ComponentModel.IContainer components = null;

		private void InitializeComponent()
		{
			this.preview = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// preview
			// 
			this.preview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.preview.Location = new System.Drawing.Point(0, 0);
			this.preview.Name = "preview";
			this.preview.Size = new System.Drawing.Size(112, 80);
			this.preview.TabIndex = 1;
			this.preview.TabStop = false;
			// 
			// BulldozeController
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(112, 80);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.preview});
			this.Name = "BulldozeController";
			this.Text = "Bulldozer";
			//! this.Text = "ブルドーザー";
			this.ResumeLayout(false);

		}
		#endregion
	}
}

