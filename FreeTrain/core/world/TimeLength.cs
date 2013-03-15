using System;

namespace freetrain.world
{
	/// <summary>
	/// Span of time
	/// </summary>
	[Serializable]
	public struct TimeLength
	{
		private TimeLength( long min ) { totalMinutes=min; }
		
		// create new time span objects that correspond to the specified period of the time.
		public static TimeLength fromMinutes( long min   ) { return new TimeLength(min); }
		public static TimeLength fromHours  ( long hours ) { return new TimeLength(hours*60); }
		public static TimeLength fromDays   ( long days)   { return TimeLength.fromHours(days*24); }
		public static TimeLength random( TimeLength min, TimeLength max ) {
			return new TimeLength( (long)
				(rnd.NextDouble()*(max.totalMinutes-min.totalMinutes)+min.totalMinutes) );
		}

		// create a time span object that represents the period until tomorrow's 0:00.
		// if the current time is just 0:00, it returns "24hours" rather than 0.
		public static TimeLength untilTomorrow() {
			Clock c = World.world.clock;
			return fromMinutes( ONEDAY.totalMinutes - (c.hour*60+c.minutes)%ONEDAY.totalMinutes );
		}

		//
		// time constants
		//
		public static readonly TimeLength ZERO = new TimeLength(0);
		public static readonly TimeLength ONEDAY = new TimeLength(60*24);

		private static Random rnd = new Random();

		/// <summary>
		/// Time span in minutes.
		/// </summary>
		public long totalMinutes;

		public bool isPositive { get { return totalMinutes>0; } }


		public static TimeLength operator + ( TimeLength a, TimeLength b ) {
			return new TimeLength( a.totalMinutes + b.totalMinutes );
		}
	}
}
