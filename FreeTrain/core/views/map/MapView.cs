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
	public class MapViewWindow : Form {
		private WindowedDirectDraw ddraw;

		/// <summary> Store the MapView object attached to it. </summary>
		internal MapView controller;

		private QuarterViewDrawer drawer;
		private System.Windows.Forms.Timer weatherTimer;

		private WeatherOverlay weatherOverlay;
		

		public MapViewWindow() {
			InitializeComponent();

			World w = World.world;
			AutoScroll = true;
			// ( (X*32+16) -16 -16, Y*8 -8 -(8+16*Z) )
			// the left edge of the world is shaggy, so we need to cut the left-most 16 pixels.
			// similarly we cut the right-most 16 pixels.
			//
			// for the same reason, cut the top-most 8 pixels. bottom 8 pixels.
			// 16*Z is further cut so that the user won't see the edge of the world
			// even if the bottom edge of the world is fully raised.
			Size sz = new Size(
				w.size.x *32 -16,
				(w.size.y - 2*w.size.z -1)*8 );
			
			AutoScrollMinSize = sz;
			MaximumSize = sz;

			PictureManager.onSurfaceLost += new EventHandler(onSurfaceLost);

			// build height-cut MenuItems
			for( int i=World.world.size.z-1; i>=0; i-- )
				menuItem_heightCut.MenuItems.Add( new HeightCutMenuItem(this,i) );
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
			drawer = new QuarterViewDrawer( World.world, ddraw,
				new Rectangle( this.scrollPos, ClientSize ) );
			drawer.OnUpdated += new EventHandler(onDrawerUpdated);

			weatherOverlay = NullWeatherOverlay.theInstance;
			// TODO
			// TEST: TODO
			//			weatherOverlay = new WeatherOverlayImpl( new WeatherOverlaySpriteSet(
			//				"{9B411B87-07F4-451b-93D0-2922EE62461B}",8,new Size(64,128)));
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			weatherOverlay.Dispose();
			PictureManager.onSurfaceLost -= new EventHandler(onSurfaceLost);
			drawer.Dispose();
			ddraw.Dispose();
			base.Dispose( disposing );
		}



		#region Windows Form Designer generated code
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem_heightCut;
		private System.Windows.Forms.MenuItem menuItem_heightCutWnd;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem_heightCut = new System.Windows.Forms.MenuItem();
			this.menuItem_heightCutWnd = new System.Windows.Forms.MenuItem();
			this.weatherTimer = new System.Windows.Forms.Timer(this.components);
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem4,
																					  this.menuItem_heightCut,
																					  this.menuItem_heightCutWnd});
			this.menuItem1.MergeOrder = 1;
			this.menuItem1.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuItem1.Text = "&View";
			//! this.menuItem1.Text = "表示(&V)";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 0;
			this.menuItem4.Text = "-";
			// 
			// menuItem_heightCut
			// 
			this.menuItem_heightCut.Index = 1;
			this.menuItem_heightCut.Text = "&Height Cutting";
			//! this.menuItem_heightCut.Text = "ヘイトカット(&H)";
			this.menuItem_heightCut.Popup += new System.EventHandler(this.menuItem_heightCut_Popup);
			// 
			// menuItem_heightCutWnd
			// 
			this.menuItem_heightCutWnd.Index = 2;
			this.menuItem_heightCutWnd.Text = "Height Cut Window";
			//! this.menuItem_heightCutWnd.Text = "ヘイトカットウィンドウ";
			this.menuItem_heightCutWnd.Click += new System.EventHandler(this.menuItem_heightCutWnd_Click);
			// 
			// weatherTimer
			// 
			this.weatherTimer.Enabled = true;
			this.weatherTimer.Tick += new System.EventHandler(this.weatherTimer_Tick);
			// 
			// MapViewWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(432, 427);
			this.Menu = this.mainMenu;
			this.Name = "MapViewWindow";
			this.Text = "Map";
			//! this.Text = "マップ";

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



		protected override void OnMouseWheel( MouseEventArgs arg ) {
			if( arg.Delta<0 && drawer.heightCutHeight < World.world.size.z-1 )
				drawer.heightCutHeight ++;
			if( arg.Delta>0 && drawer.heightCutHeight > 0 )
				drawer.heightCutHeight --;
		}

		#region scroll by drag
		/// <summary>
		/// True if the drag-by-mouse mode is on.
		/// While this mode is on, mouse is captured.
		/// </summary>
		private bool dragMode = false;
		private Point dragStartMousePos;
		private Point dragStartScrollPos;
		private int dragAccel;

		/// <summary>Converts the moust pos in a MouseEventArgs into a Point obj.</summary>
		private Point getPoint( MouseEventArgs a ) {
			return new Point(a.X,a.Y);
		}

		/// <summary>Computes the distance between two points.</summary>
		private int distance( Point a, Point b ) {
			return (int)Math.Sqrt( (a.X-b.X)*(a.X-b.X) + (a.Y-b.Y)*(a.Y-b.Y) );
		}

		/// <summary> Scroll the window to follow the mouse pos. </summary>
		/// <returns>True if the scrolling was successful.</returns>
		private bool scrollByDrag( MouseEventArgs arg ) {
			Point curMousePos = getPoint(arg);
			if( (arg.Button!=MouseButtons.Left && controller==null)
				||  distance(dragStartMousePos,curMousePos)>5 ) {
				
				// change the cursor for visual feedback that panning is in progress
				this.Cursor = Cursors.SizeAll;

				// move the window accordingly
				Point pt = this.dragStartScrollPos;
				pt.X += (curMousePos.X - dragStartMousePos.X)*dragAccel;
				pt.Y += (curMousePos.Y - dragStartMousePos.Y)*dragAccel;

				pt.X *= -1;
				pt.Y *= -1;
				this.scrollPos = pt;

				return true;
			}

			return false;
		}
		#endregion

		protected override void OnMouseDown( MouseEventArgs arg ) {
			ModalController controller = MainWindow.mainWindow.currentController;

			// start the drag mode
			dragMode = true;
			dragStartMousePos = getPoint(arg);
			dragStartScrollPos = this.AutoScrollPosition;
			dragAccel = (Keyboard.isControlKeyPressed?2:1)*(Keyboard.isShiftKeyPressed?2:1);
			this.Capture = true;
		}

		protected override void OnMouseMove( MouseEventArgs arg ) {
			ModalController controller = MainWindow.mainWindow.currentController;

			if( dragMode )
				scrollByDrag(arg);
			else {
				if(controller!=null) {
					Point ab = drawer.fromClientToAB( arg.X, arg.Y );

					controller.onMouseMove( this,
						drawer.fromABToXYZ(ab,controller),
						ab );
				}
			}
		}

		protected override void OnMouseUp( MouseEventArgs arg ) {
			ModalController controller = MainWindow.mainWindow.currentController;

			if( dragMode ) {
				bool r = scrollByDrag(arg);

				// end the drag mode
				dragMode = false;
				this.Capture = false;
				this.Cursor = null;

				if(r)
					return;	// if the scroll-by-drag was successful, don't process the click event
			}

			if(controller!=null) {
				Point ab = drawer.fromClientToAB( arg.X, arg.Y );
				Location xyz = drawer.fromABToXYZ( ab, controller );

				if( arg.Button == MouseButtons.Left )
					controller.onClick( this, xyz, ab );
				if( arg.Button == MouseButtons.Right )
					controller.onRightClick( this, xyz, ab );
			} else {

				if( arg.Button==MouseButtons.Left ) {
					// dispatch this click event to the appropriate voxel.

					Location loc = drawer.fromClientToXYZ(arg,null); {
					 // print debug information
						Debug.WriteLine("mouse clicked on MapViewWindow. (x,y,z)="+loc);
						int h,v;
						World.world.toHV( loc.x, loc.y, out h, out v );
						Debug.WriteLine(string.Format(" (h,v)=({0},{1})",h,v) );
					}

					// look for voxels that can process this event
					for( int z=drawer.heightCutHeight; z>=0; z-- ) {
						Voxel v = World.world[ loc.x-z, loc.y+z, z ];
						if(v!=null && v.onClick())
							return;
					}
				}
			}
		}

		//		protected override void OnResize( EventArgs e ) {
		//			base.OnResize(e);
		//			drawer.size = this.ClientSize;
		//		}
		//
		protected override void OnClosed( EventArgs e ) {
			if(heightCutWindow!=null)
				heightCutWindow.Close();
		}

		protected override void OnGotFocus( EventArgs e ) {
			base.OnGotFocus(e);
			// register this map view as the primary map view
			MainWindow.primaryMapView = controller;
		}

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




		public class HeightCutMenuItem : MenuItem {
			public HeightCutMenuItem() {}
			public HeightCutMenuItem( MapViewWindow owner, int height ) {
				this.owner=owner;
				this.height=height;

				//				this.Checked = ( height==owner.drawer.heightCutHeight );

				if( height==World.world.size.z-1 ) {
					this.Text = "None";
					//! this.Text = "なし";
				} else {
					int h = height - World.world.waterLevel;
					if( h==0 )		this.Text = "Water level";
					//! if( h==0 )		this.Text = "地表";
					else			this.Text = h.ToString();
				}
			}
			private readonly MapViewWindow owner;
			private readonly int height;

			protected override void OnClick(EventArgs e) {
				owner.drawer.heightCutHeight = height;
			}
			internal void update() {
				this.Checked = (owner.drawer.heightCutHeight==height);
			}
		}

		private void menuItem_heightCut_Popup( object sender, EventArgs e ) {
			foreach( HeightCutMenuItem mi in menuItem_heightCut.MenuItems )
				mi.update();
		}

		internal HeightCutWindow heightCutWindow = null;

		private void menuItem_heightCutWnd_Click(object sender, System.EventArgs e) {
			if( heightCutWindow==null ) {
				heightCutWindow = new HeightCutWindow(this,drawer);
				MainWindow.mainWindow.AddOwnedForm(heightCutWindow);
			}

			heightCutWindow.Show();
			heightCutWindow.BringToFront();
		}

		private void weatherTimer_Tick(object sender, System.EventArgs e) {
			if( weatherOverlay.onTimerFired() )
				Invalidate();
		}
	}

	/// <summary>
	/// View interface implementation of the map view.
	/// </summary>
	public class MapView : AbstractView {
		public MapView() : base(new MapViewWindow()) {
			form.controller = this;
		}
		
		private new MapViewWindow form { get { return (MapViewWindow)base.form; } }

		public bool IsDisposed {
			get {
				return form.IsDisposed;
			}
		}

		/// <summary>
		/// Moves the map window to display the specified location
		/// </summary>
		public void moveTo( Location loc ) {
			form.moveTo(loc);
		}

		/// <summary>
		/// If the specified location is visible by one of the map views currently opened,
		/// return true. Otherwise false.
		/// </summary>
		public static bool isVisibleInAny( Location loc ) {
			foreach( IView view in MainWindow.mainWindow.getAllViews() ) {
				if( view is MapView && ((MapView)view).form.isVisible(loc) )
					return true;
			}
			return false;
		}
	}

}
