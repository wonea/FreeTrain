using System;
using freetrain.world;
using freetrain.world.rail;

namespace freetrain.controllers
{
	/// <summary>
	/// LocationDisambiguator implementation that prefers
	/// a location with a railroad.
	/// </summary>
	public class RailRoadDisambiguator : LocationDisambiguator
	{
		// the singleton instance
		public static LocationDisambiguator theInstance = new RailRoadDisambiguator();
		private RailRoadDisambiguator() {}

		public bool isSelectable(Location loc) {
			// if there's any rail roads, fine
			if( RailRoad.get(loc)!=null )	return true;

			// or if we hit the ground
			if( World.world.getGroundLevel(loc)>=loc.z )	return true;

			return false;
		}
	}

	/// <summary>
	/// LocationDisambiguator that prefers the surface level.
	/// </summary>
	public class GroundDisambiguator : LocationDisambiguator
	{
		// the singleton instance
		public static LocationDisambiguator theInstance = new GroundDisambiguator();
		private GroundDisambiguator() {}

		public bool isSelectable(Location loc) {
			return loc.z==World.world.getGroundLevel(loc);
		}
	}
	
	/// <summary>
	/// LocationDisambiguator that only allows locations in the same level
	/// </summary>
	public class SameLevelDisambiguator : LocationDisambiguator {
		public SameLevelDisambiguator( int height ) { this.height=height; }
		private readonly int height;
		public bool isSelectable( Location loc ) {
			return loc.z == height;
		}
	}

	// TODO: other disambiguators
}
