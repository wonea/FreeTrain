using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using freetrain.views;
using freetrain.views.map;
using freetrain.world;

namespace freetrain.controllers
{
	/// <summary>
	/// Partial <c>ModalController</c> implementation that selects
	/// a particular location.
	/// </summary>
	public abstract class BorderSelectorController : ModalController, MapOverlay
	{
		protected Location currentPos = Location.UNPLACED;

		protected readonly IControllerSite site;

		public BorderSelectorController( IControllerSite _site ) {
			this.site = _site;
		}

		/// <summary>
		/// Called when a selected location is changed.
		/// Usually an application doesn't need to do anything.
		/// </summary>
		/// <param name="loc"></param>
		protected virtual void onSelectionChanged( Location loc ) {
		}

		/// <summary>
		/// Called when the player selects a location.
		/// </summary>
		protected abstract void onBorderSelected( Location loc, Direction dir );


		public virtual void drawAfter( QuarterViewDrawer view, DrawContextEx canvas) {
		}

		public virtual void drawBefore( QuarterViewDrawer view, DrawContextEx canvas) {
		}

		public virtual void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt) {
		}

		public virtual void onAttached() {
		}
		public virtual void onDetached() {
			// clear the remaining image
			if( currentPos!=Location.UNPLACED )
				World.world.onVoxelUpdated(currentPos);
		}

		public virtual void onMouseMove( MapViewWindow source, Location loc, Point ab) {
			if( currentPos!=Location.UNPLACED )
				World.world.onVoxelUpdated(currentPos);
			currentPos = loc;
			World.world.onVoxelUpdated(currentPos);
			
			onSelectionChanged(currentPos);
		}

		public void onRightClick( MapViewWindow source, Location loc, Point ab) {
			close();
		}

		public void onClick( MapViewWindow source, Location loc, Point ab) {
			Point p = World.world.fromXYZToAB(loc);
			int x = ab.X-p.X;
			int y = ab.Y-p.Y;
			Debug.WriteLine("diff=("+x+","+y+")");
			Direction d;
			if( x<15 )
				if(y<15)
					d=Direction.NORTH;
				else
					d=Direction.WEST;
			else
				if(y<15)
				d=Direction.EAST;
			else
				d=Direction.SOUTH;
			onBorderSelected(loc,d);
		}


		public virtual void close() {
			site.close();
		}

		public abstract LocationDisambiguator disambiguator { get; }

		public virtual string name { get { return site.name; } }

		// can be overrided by a derived class to return another object.
		public virtual MapOverlay overlay {
			get {
				// return this object if it implements MapOverlay by itself.
				return this as MapOverlay;
			}
		}

	}
}
