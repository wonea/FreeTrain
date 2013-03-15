using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using freetrain.world;
using freetrain.views.map;

namespace freetrain.controllers
{
	/// <summary>
	/// ModalController that selects a cube of the fixed size.
	/// </summary>
	public abstract class CubeSelectorController : ModalController
	{
		/// <summary>Constant</summary>
		protected static readonly Location UNPLACED = world.Location.UNPLACED;

		protected Location location = UNPLACED;

		protected readonly Distance size;

		protected readonly IControllerSite site;
		
		public CubeSelectorController( Distance _size, IControllerSite _site ) {
			this.size = _size;
			this.site = _site;
		}

	//
	// methods that can/should be overrided by derived classes
	//

		/// <summary>
		/// Called when the selection is completed.
		/// </summary>
		protected abstract void onSelected( Cube cube );

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
		protected Cube currentCube {
			get {
				Debug.Assert( location!=UNPLACED );
				return Cube.createExclusive( location, size );
			}
		}


	//
	// internal logic
	//


		public virtual LocationDisambiguator disambiguator { get { return GroundDisambiguator.theInstance; } }


		public virtual void onClick(MapViewWindow view, Location loc, Point ab) {
			onSelected(Cube.createExclusive(loc,size));
		}

		public virtual void onRightClick( MapViewWindow source, Location loc, Point ab ) {
			onCanceled();
		}

		public virtual void onMouseMove(MapViewWindow view, Location loc, Point ab ) {
			World w = World.world;

			if(location!=loc) {
				// the current location is moved.
				// update the screen
				w.onVoxelUpdated(currentCube);
				location = loc;
				w.onVoxelUpdated(currentCube);
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
