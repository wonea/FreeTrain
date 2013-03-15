using System;
using System.Windows.Forms;
using freetrain.contributions.rail;
using freetrain.framework.plugin;

namespace freetrain.world.rail
{
	public enum JunctionRoute {
		Straight, Curve
	}

	/// <summary>
	/// Controls the movement of a train.
	/// 
	/// This object is responsible to determine:
	/// - When a train stop and how long
	/// - How does it proceed when it hits a junction
	/// </summary>
	[Serializable]
	public abstract class TrainController
	{
		/// <summary>
		/// At junction, this method is called to determine
		/// the direction to which a train proceeds.
		/// </summary>
		public abstract JunctionRoute onJunction( Train train, JunctionRailRoad railRoad );

		/// <summary>
		/// This method is called when a train hits the stop position
		/// of a platform (or other train harbors.)
		/// </summary>
		/// <param name="callCount">
		/// This value is 0 when this method is called
		/// first time (when a train stops at a station.)
		/// After the given time is elapsed, the onStop method will be
		/// called again but this time with callCount==1. The value
		/// keeps increasing as this process repeats.
		/// </param>
		/// <returns>
		/// returns the amount of time the train should stop.
		/// after this time span, the train will call this same method
		/// so the train controller still has a chance to postpone the
		/// departure. Return 0 to make the train proceed, or return
		/// -1 to make the train turn around.
		/// 
		/// Returning 0 when callCount==0 means the train will not stop
		/// at the specified platform.
		/// </returns>
		public abstract TimeLength getStopTimeSpan( Train train, TrainHarbor platform, int callCount );

		/// <summary>
		/// Opens a configuration dialog if necessary. Or it can
		/// attach ModalController.
		/// </summary>
		/// <param name="owner">The parent window if the callee is
		/// going to open a modal dialog box.</param>
		public abstract void config( IWin32Window owner );

		/// <summary>
		/// The user-friendly name assigned to this controller.
		/// </summary>
		public string name;

		/// <summary>
		/// Obtains a reference to the contribution from which
		/// this controller is created.
		/// 
		/// A system defined TrainController returns null.
		/// </summary>
		public abstract TrainControllerContribution contribution { get; }


		public override string ToString() { return name; }
	}
}
