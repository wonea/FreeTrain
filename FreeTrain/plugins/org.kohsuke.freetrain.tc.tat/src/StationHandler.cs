using System;

namespace freetrain.world.rail.tattc
{
	/// <summary>
	/// Determines the time of stop at statins.
	/// </summary>
	[Serializable]
	internal abstract class StationHandler
	{
		internal abstract TimeLength getStopTimeSpan( int callCount );

		internal static readonly StationHandler defaultHandler =
			new FixedDurationStationHandler( TimeLength.fromMinutes(10), false );
	}



	/// <summary>
	/// Always pass.
	/// </summary>
	[Serializable]
	internal class PassStationHandler : StationHandler {
		internal override TimeLength getStopTimeSpan( int callCount ) {
			return TimeLength.ZERO;
		}
	}



	/// <summary>
	/// Always stop the fixed amount of time.
	/// </summary>
	[Serializable]
	internal class FixedDurationStationHandler : StationHandler {
		internal FixedDurationStationHandler( TimeLength ts, bool turnAround ) {
			duration = ts;
			this.turnAround = turnAround;
		}

		public readonly TimeLength duration;
		public bool turnAround;

		internal override TimeLength getStopTimeSpan( int callCount ) {
			if( callCount==0 )	return duration;
			else {
				if(turnAround)	return TimeLength.fromMinutes(-1);
				else			return TimeLength.ZERO;
			}
		}
	}



	/// <summary>
	/// Always depart at the given time
	/// </summary>
	[Serializable]
	internal class OnceADayStationHandler : StationHandler {
		internal OnceADayStationHandler( int m, bool turnAround ) {
			minutes=m;
			this.turnAround = turnAround;
		}
		
		/// <summary>
		/// Total minutes from the beginning of a day.
		/// </summary>
		public int minutes;

		public bool turnAround;

		internal override TimeLength getStopTimeSpan( int callCount ) {
			if( callCount!=0 ) {
				if(turnAround)	return TimeLength.fromMinutes(-1);
				else			return TimeLength.ZERO;
			}
			
			Clock c = World.world.clock;

			int m = c.hour*60 + c.minutes;

			if( minutes>m )		return TimeLength.fromMinutes( minutes-m );
			else				return TimeLength.fromMinutes( minutes-m ) + TimeLength.ONEDAY;
		}
	}




}
