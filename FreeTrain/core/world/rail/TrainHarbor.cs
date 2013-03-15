using System;

namespace freetrain.world.rail
{
	/// <summary>
	/// Interface implemented by an entity that allows a train
	/// to stop.
	/// 
//	/// Train harbors can be organized into a tree hierarchy
//	/// (for example, 
	/// </summary>
	public interface TrainHarbor
	{
		// This interface can be queried from voxels

		// TODO: replace by moniker
		Location location { get; }

//		/// <summary>
//		/// Get the parent train harbor, or null if no such thing exists.
//		/// </summary>
//		TrainHarbor parentHarbor { get; }
	}
}
