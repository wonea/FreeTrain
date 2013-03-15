using System;
using freetrain.world;

namespace freetrain.world.rail.tattc
{
	/// <summary>
	/// Mask for time
	/// </summary>
	[Serializable]
	public class TimeMask
	{
		/// <summary>
		/// Specified month or -1 to indicate the wildcard.
		/// </summary>
		public sbyte month;
		
		public sbyte day;
		
		public sbyte dayOfWeek;

		public sbyte hour;

		public sbyte minutes;

		public const sbyte WILDCARD = -1;

		/// <summary>
		/// Returns true if the current clock matches this mask.
		/// </summary>
		public bool matches( Clock clock ) {
			if( month!=WILDCARD && clock.month!=month )				return false;
			if( day!=WILDCARD && clock.day!=day )					return false;
			if( dayOfWeek!=WILDCARD && clock.dayOfWeek!=dayOfWeek )	return false;
			if( hour!=WILDCARD && clock.hour!=hour )				return false;
			if( minutes!=WILDCARD && clock.minutes!=minutes )		return false;

			return true;
		}

		/// <summary>
		/// Computes time necessary to the next match of this time mask.
		/// 
		/// For example, if the time mask is "any 17:00" and the current time is
		/// "2002/05/03 03:15" then this method returns "13:45".
		/// 
		/// This method returns TimeSpan.ZERO if the time mask matches with the given clock.
		/// </summary>
		/// <param name="clock"></param>
		/// <returns></returns>
		public TimeSpan nextMatch( Clock clock ) {
//			sbyte[] tcomp = new sbyte[5]{ clock.currentMonth, clock.currentDay, clock.currentDayOfWeek,
//						 clock.currentHour, clock.currentMinutes };
//
//			if( month!=WILDCARD && tcomp[0]!=month ) {
//				tcomp[i]
//			}
			throw new NotImplementedException();
		}
	}
}
