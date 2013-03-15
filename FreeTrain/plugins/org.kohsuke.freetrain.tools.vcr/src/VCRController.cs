using System;
using System.Drawing;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.controllers;
using freetrain.views;
using freetrain.views.map;
using freetrain.world;

namespace freetrain.tools.vcr
{
	/// <summary>
	/// Controller that selects the region to capture.
	/// 
	/// The controller will be automatically closed once deactivated.
	/// </summary>
	public class VCRController : ModalController, MapOverlay
	{
		/// <summary>
		/// Selected region in (A,B) axis.
		/// </summary>
		private Rectangle _rect;

		/// <summary>
		/// Pen used to draw the rectangle.
		/// </summary>
		private static readonly Pen pen = new Pen( Color.Black, 3 );
		
		/// <summary>
		/// If we are in the dragging mode, stores the resizing strategy.
		/// </summary>
		private ResizingStrategy strategy;
		
		/// <summary>
		/// Fired when this object is closed.
		/// </summary>
		public event EventHandler OnClosed;

		public VCRController( Rectangle r ) {
			this._rect = r;
		}

		public Rectangle rect { get { return _rect; } }

		
		public LocationDisambiguator disambiguator {
			get { return GroundDisambiguator.theInstance; }
		}

		public void close() {
			OnClosed(this,null);
		}

		public string name {
			get { return "Recording range settings"; }
			//! get { return "録画範囲の設定"; }
		}

		public MapOverlay overlay { get { return this; } }

		public void onAttached() {
			World.world.onAllVoxelUpdated();	// force redraw
		}
		public void onDetached() {
			World.world.onAllVoxelUpdated();	// force redraw
			OnClosed(this,null);	// this will shut down this object.
		}


		public void onClick( MapViewWindow mapView, Location loc, Point ab ) {
			if( strategy==null ) {
				strategy = ResizingStrategies.getStrategy(rect,ab);
				if(strategy==null) {
					// mouse is out of place.
					_rect = new Rectangle( ab, new Size(1,1) );
					strategy = ResizingStrategies.SE;
				}
			} else {
				strategy = null;
			}
			World.world.onAllVoxelUpdated();
		}

		public void onMouseMove( MapViewWindow mapView, Location loc, Point ab ) {
			if( strategy==null ) {
				ResizingStrategy s = ResizingStrategies.getStrategy(rect,ab);
				if(s!=null)		Cursor.Current = s.cursor;
			} else {
				_rect = strategy.resize( _rect, ab );
				Cursor.Current = strategy.cursor;
				World.world.onAllVoxelUpdated();
			}
		}
		public void onRightClick( MapViewWindow mapView, Location loc, Point ab ) {
			MainWindow.mainWindow.detachController();
		}

		//
		// MapOverlay implementation
		//
		public void drawBefore( QuarterViewDrawer drawer, DrawContextEx context ) {}

		public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) {}

		public void drawAfter( QuarterViewDrawer view, DrawContextEx canvas ) {
			// determine the rect
			Rectangle r = _rect;
			Point pt = view.origin;
			r.Offset( -pt.X, -pt.Y );

			canvas.graphics.DrawRectangle( pen, r );
		}
	}
}
