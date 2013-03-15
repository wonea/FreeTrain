using System;
using freetrain.world;

namespace freetrain.controllers
{
	/// <summary>
	/// Used by the MapViewController to disambiguate
	/// stacked voxels.
	/// 
	/// When an user clicks a screen, there are many locations
	/// that can match. Depending on the context, the program needs
	/// to select one of them. For example, when an user is placing
	/// a train, we'd like to select a voxel with a railroad.
	/// 
	/// This interface does this.
	/// </summary>
	public interface LocationDisambiguator
	{
		/// <summary>
		/// Returns true if the callee prefers this location
		/// to be selected.
		/// </summary>
		bool isSelectable( Location loc );
	}
}
