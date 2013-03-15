using System;
using freetrain.controllers;
using freetrain.views.map;

namespace freetrain.world.terrain.terrace
{
	/// <summary>
	/// LocationDisambiguator that prefers mountain voxels.
	/// </summary>
	public class MountainDisambiguator : LocationDisambiguator
	{
		// the singleton instance
		public static LocationDisambiguator theInstance = new MountainDisambiguator();
		private MountainDisambiguator() {}

		public bool isSelectable(Location loc) {
			return World.world[loc] is MountainVoxel;
		}
	}
}
