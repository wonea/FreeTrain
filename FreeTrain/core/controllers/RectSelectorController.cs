using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using freetrain.world;
using freetrain.views.map;

namespace freetrain.controllers
{
	/// <summary>
	/// ModalController that selects the rectangular region
	/// and do something with it.
	/// </summary>
	public abstract class RectSelectorController : ModalController, LocationDisambiguator
	{
		/// <summary>Constant</summary>
		protected static readonly Location UNPLACED = world.Location.UNPLACED;

		protected Location anchor = UNPLACED;
		protected Location currentLoc = UNPLACED;

		protected readonly IControllerSite site;
		
		public RectSelectorController( IControllerSite _site ) {
			this.site = _site;
		}

	//
	// methods that can/should be overrided by derived classes
	//

		/// <summary>
		/// Called when the selection is completed.
		/// </summary>
		protected abstract void onRectSelected( Location loc1, Location loc2 );

		/// <summary>
		/// Called when the selection is changed.
		/// </summary>
		protected virtual void onRectUpdated( Location loc1, Location loc2 ) {}

		/// <summary>
		/// Called when the user wants to cancel the modal controller.
		/// </summary>
		protected virtual void onCanceled() {
			site.close();
		}

		public virtual string name { get { return site.name; } }

		// can be overrided by a derived class to return another object.
		public virtual MapOverlay overlay {
			get {
				// return this object if it implements MapOverlay by itself.
				return this as MapOverlay;
			}
		}


	//
	// convenience methods
		/// <summary>
		/// North-west corner of the selected region.
		/// </summary>
		protected Location location1 {
			get {
				Debug.Assert( currentLoc!=UNPLACED );
				return new Location(
					Math.Min( currentLoc.x, anchor.x ),
					Math.Min( currentLoc.y, anchor.y ),
					anchor.z );
			}
		}

		/// <summary>
		/// South-east corner of the selected region.
		/// </summary>
		protected Location location2 {
			get {
				Debug.Assert( currentLoc!=UNPLACED );
				return new Location(
					Math.Max( currentLoc.x, anchor.x ),
					Math.Max( currentLoc.y, anchor.y ),
					anchor.z );
			}
		}


	//
	// internal logic
	//


		public LocationDisambiguator disambiguator { get { return this; } }

		/// <summary> LocationDisambiguator implementation </summary>
		public bool isSelectable( Location loc ) {
			if(anchor!=UNPLACED)
				return loc.z==anchor.z;
			else
				// lands can be placed only on the ground
				return GroundDisambiguator.theInstance.isSelectable(loc);
		}

		public virtual void onClick(MapViewWindow view, Location loc, Point ab) {
			if(anchor==UNPLACED) {
				anchor = loc;
			} else {
				onRectSelected( this.location1, this.location2 );
				anchor = UNPLACED;
			}
		}

		public virtual void onRightClick( MapViewWindow source, Location loc, Point ab ) {
			if( anchor==UNPLACED )
				onCanceled();
			else {
				// cancel the anchor
				World.world.onAllVoxelUpdated();
				anchor = UNPLACED;
			}
		}

		public virtual void onMouseMove(MapViewWindow view, Location loc, Point ab ) {
			World w = World.world;

			if(anchor!=UNPLACED && currentLoc!=loc) {
				// the current location is moved.
				// update the screen
				currentLoc = loc;
				onRectUpdated( this.location1, this.location2 );
				w.onAllVoxelUpdated();
			}
		}

		public virtual void onAttached() {}
		public virtual void onDetached() {
			// redraw the entire surface to erase any left-over from this controller
			World.world.onAllVoxelUpdated();
		}

		public virtual void close() {
			onCanceled();
		}
	}
}
