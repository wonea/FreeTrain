using System;
using System.Diagnostics;
using System.Drawing;
using org.kohsuke.directdraw;
using freetrain.framework;

namespace freetrain.world.rail
{
	/// <summary>
	/// Rail road adjacent to a platform
	/// </summary>
	[Serializable]
	public class YardRailRoad : SpecialPurposeRailRoad
	{
		public YardRailRoad( TrafficVoxel v, Platform _owner, int _idx ) : base(v,_owner.direction) {
			Debug.Assert( dir1.isSharp );
			Debug.Assert( dir2.isSharp );
			this.owner = _owner;
			this.index = _idx;
		}

		/// <summary> Platform object that owns this RR. </summary>
		public readonly Platform owner;
		
		/// <summary>
		/// Index of this railroad.
		/// The one closest to the base of the platform gets 0,
		/// and the number increases as RR gets further to the base.
		/// </summary>
		protected readonly int index;






		public override TimeLength getStopTimeSpan( Train tr, int callCount ) {
			// calculate the position where the train should stop.
			int pos;

			CarState.Inside ins = tr.head.state.asInside();

			if(owner.direction==ins.direction) {
				pos = ( owner.length + tr.length )/2 -1;
			} else {
				Debug.Assert( owner.direction==ins.direction.opposite );
				pos = ( owner.length - tr.length )/2;
			}

			if( pos<0 || pos>=owner.length )
				return TimeLength.ZERO;	// longer than the platform

			if(pos!=index)
				return TimeLength.ZERO;	// not at the stop position
			
			// the train is positioned at the right place.
			if(owner.hostStation==null)
				return TimeLength.ZERO;	// platform is not connected. can't make a stop

			// ask the controller to see if it wants to stop.
			TimeLength ts = tr.controller.getStopTimeSpan(tr,owner.hostStation,callCount);

			if( callCount==0 && ts.isPositive )
				// if the train stops, unload passengers
				owner.hostStation.unloadPassengers(tr);
			
			if( callCount!=0 && !ts.isPositive ) {
				// if the train departs, load passengers
				owner.hostStation.loadPassengers(tr);
				// ring the bell, here we go!
				tr.playSound( owner.bellSound.sound );
			}

			return ts;
		}
	}
}
