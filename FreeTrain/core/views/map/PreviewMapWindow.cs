using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.util;
using freetrain.world;
using freetrain.controllers;
using freetrain.framework;
using freetrain.framework.graphics;
using org.kohsuke.directdraw;

namespace freetrain.views.map
{
	/// <summary>
	/// Form implementation of the map view.
	/// </summary>
	public class PreviewMapWindow : UserControl {
		private WindowedDirectDraw ddraw;
		private QuarterViewDrawer drawer;
		private WeatherOverlay weatherOverlay;
		private World world;

		public PreviewMapWindow(World w) {
			world = w;
			InitializeComponent();
			
			AutoScroll = true;
			// ( (X*32+16) -16 -16, Y*8 -8 -(8+16*Z) )
			// the left edge of the world is shaggy, so we need to cut the left-most 16 pixels.
			// similarly we cut the right-most 16 pixels.
			//
			// for the same reason, cut the top-most 8 pixels. bottom 8 pixels.
			// 16*Z is further cut so that the user won't see the edge of the world
			// even if the bottom edge of the world is fully raised.
			Size sz = new Size(	w.size.x *32 -16, w.size.y *8 - 16);
			
			AutoScrollMinSize = sz;
			ClientSize = sz;

			PictureManager.onSurfaceLost += new EventHandler(onSurfaceLost);
		}

		private Point scrollPos {
			get {
				Point pt = AutoScrollPosition;
				return new Point(-pt.X+16,-pt.Y+8);
			}
			set {
				AutoScrollPosition = new Point(
					Math.Max(value.X-16,0),
					Math.Max(value.Y- 8,0) );
			}
		}


		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			ddraw = new WindowedDirectDraw(this);
			drawer = new QuarterViewDrawer( world, ddraw,
				new Rectangle( this.scrollPos, ClientSize ) );
			drawer.OnUpdated += new EventHandler(onDrawerUpdated);

			weatherOverlay = NullWeatherOverlay.theInstance;
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			if(ddraw!=null){
				weatherOverlay.Dispose();
				drawer.Dispose();
				ddraw.Dispose();
			}
			PictureManager.onSurfaceLost -= new EventHandler(onSurfaceLost);
			world = null;
			base.Dispose( disposing );
		}



		#region Windows Form Designer generated code
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent() {
			// 
			// PreviewMapWindow
			// 
			this.AutoScroll = true;
			this.Name = "PreviewMapWindow";
			this.Size = new System.Drawing.Size(432, 427);

		}
		#endregion


		protected override void OnPaint(PaintEventArgs pe) {
			drawer.size = this.ClientSize;
			drawer.origin = this.scrollPos;

			weatherOverlay.setSize(this.ClientSize);

			if( ddraw.primarySurface.handle.isLost()!=0 )	// surface is lost
				PictureManager.onSurfaceLost(this,null);

			//			drawer.draw( ddraw.primarySurface, PointToScreen(new Point(0,0)) );
			weatherOverlay.draw( drawer, ddraw.primarySurface, PointToScreen(new Point(0,0)) );

			base.OnPaint(pe);
		}
		
		protected void onDrawerUpdated( object sender, EventArgs e ) {
			Invalidate();
		}

		protected void onSurfaceLost( object sender, EventArgs e ) {
			ddraw.primarySurface.restore();
			Invalidate();
		}

		protected override void OnPaintBackground( PaintEventArgs pevent ) {
			// don't paint the background to avoid flicker
		}

		//

		/// <summary>
		/// Moves the map window to display the specified location
		/// </summary>
		public void moveTo( Location loc ) {
			// compute the new origin
			Point pt = World.world.fromXYZToAB(loc);
			Size sz = drawer.size;
			sz.Width /= 2;
			sz.Height /= 2;
			pt -= sz;

			this.scrollPos = pt;
		}

		/// <summary>
		/// Determine if the specified location is visible by this view.
		/// </summary>
		public bool isVisible( Location loc ) {
			return drawer.isVisible(loc);
		}


	}

}
