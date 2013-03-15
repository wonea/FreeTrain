using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace freetrain.views.map
{
	/// <summary>
	/// Window that controls the height cut mode of the given quarter view drawer.
	/// </summary>
	public class HeightCutWindow : System.Windows.Forms.Form
	{
		private readonly MapViewWindow mapView;
		private readonly QuarterViewDrawer drawer;

		public HeightCutWindow( MapViewWindow mapView, QuarterViewDrawer drawer ) {
			this.mapView = mapView;
			this.drawer = drawer;
			InitializeComponent();

			trackBar.Minimum = 0;
			trackBar.Maximum = world.World.world.size.z-1;
			trackBar.Value = drawer.heightCutHeight;

			drawer.OnHeightCutChanged += new EventHandler(onHeightCutChange);
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		private void trackBar_Scroll(object sender, EventArgs e) {
			drawer.heightCutHeight = trackBar.Value;
		}
		#region Windows Form Designer generated code
		private freetrain.controls.TrackBarEx trackBar;
		private System.ComponentModel.Container components = null;

		private void InitializeComponent()
		{
			this.trackBar = new freetrain.controls.TrackBarEx();
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// trackBar
			// 
			this.trackBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.trackBar.Name = "trackBar";
			this.trackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar.Size = new System.Drawing.Size(42, 96);
			this.trackBar.TabIndex = 0;
			this.trackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar.ValueChanged += new System.EventHandler(this.trackBar_Scroll);
			// 
			// HeightCutWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(42, 96);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.trackBar});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "HeightCutWindow";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			mapView.heightCutWindow = null;
			drawer.OnHeightCutChanged -= new EventHandler(onHeightCutChange);
		}

		private void onHeightCutChange( object sender, EventArgs e ) {
			trackBar.Value = drawer.heightCutHeight;
		}

	}
}
