using System;

namespace freetrain.world.rail
{
	/// <summary>
	/// Structures that "use" a station.
	/// 
	/// This interface is implemented by structures that have
	/// population that uses a station. Because of the way
	/// stations find listeners, listeners need to occupy
	/// at least one voxel.
	/// 
	/// StationListener interface should be accessible through the queryAspect method.
	/// </summary>
	public interface StationListener
	{
		/// <summary> Obtains the population that uses a station right now. </summary>
		/// <remarks>Usually this value varies depending on the current time.</remarks>
		int getPopulation( Station s );

		/// <summary>
		/// Notifies the removal of the station.
		/// </summary>
		/// <remarks>
		/// Affected listener should look for another station to attach.
		/// listeners will be removed automatically from the old station,
		/// so don't call the <code>listeners.remove</code> method.
		/// </remarks>
		void onStationRemoved( Station s );

		/// <summary>
		/// Notifies a newly created station.
		/// </summary>
		/// <remarks>
		/// This method is called by a newly created station object
		/// to "recruit" existing listeners to the new station.
		/// This method is called only when the receiving listener
		/// is eligible to attach to the new station.
		/// </remarks>
		/// <param name="s"></param>
		/// <returns>true if succesfuly advertised</returns>
		bool advertiseStation( Station s );
	}
}
