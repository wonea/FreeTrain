using System;
using freetrain.world;

namespace freetrain.controllers
{
	/// <summary>
	/// Stores a single Location object that requires screen update.
	/// Whenever a new location is set, the screen will be correctly updated
	/// </summary>
	public class LocationStore
	{
		private Location loc = Location.UNPLACED;

		public LocationStore() {
		}

		public Location location {
			get {
				return loc;
			}
			set {
				if( loc!=Location.UNPLACED )
					World.world.onVoxelUpdated(loc);
				loc = value;
				if( loc!=Location.UNPLACED )
					World.world.onVoxelUpdated(loc);
			}
		}
	}
}
