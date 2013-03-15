using System;
using nft.core.game;

namespace nft.core.schedule
{
	/// <summary>
	/// Span of time
	/// </summary>
	[Serializable]
	public struct TimeLength
	{
		internal TimeLength( long sec ) { TotalSeconds=sec; }
		
		// create new time span objects that correspond to the specified period of the time.
		public static TimeLength FromMinutes( long min   ) { return new TimeLength(min*Time.MINUTE); }
		public static TimeLength FromHours  ( long hours ) { return new TimeLength(hours*Time.HOUR); }
		public static TimeLength FromDays   ( long days)   { return new TimeLength(days*Time.DAY); }
		
		//
		// time constants
		//
		public static readonly TimeLength ZERO = new TimeLength(0);
		public static readonly TimeLength ONEDAY = new TimeLength(Time.DAY);
		public static readonly TimeLength ONEHOUR = new TimeLength(Time.HOUR);

		/// <summary>
		/// Time span in seconds.
		/// </summary>
		public long TotalSeconds;

		public int TotalDays { get{ return (int)(TotalSeconds/Time.DAY); } }
		public int TotalHours { get{ return (int)(TotalSeconds/Time.HOUR); } }
		public int TotalMinutes { get{ return (int)(TotalSeconds/Time.MINUTE); } }
		
		public static TimeLength operator + ( TimeLength a, TimeLength b ) 
		{
			return new TimeLength( a.TotalSeconds + b.TotalSeconds );
		}
		public static TimeLength operator - ( TimeLength a, TimeLength b ) 
		{
			return new TimeLength( a.TotalSeconds - b.TotalSeconds );
		}
	}
}
