using System;
using System.Drawing;
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
	public abstract class PointSelectorController : ModalController, MapOverlay
	{
		protected Location currentPos = Location.UNPLACED;

		protected readonly IControllerSite site;

		public PointSelectorController( IControllerSite _site ) {
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
		protected abstract void onLocationSelected( Location loc );





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
			onLocationSelected(loc);
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
