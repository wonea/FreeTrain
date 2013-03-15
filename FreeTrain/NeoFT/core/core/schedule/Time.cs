using System;
using System.Diagnostics;

namespace nft.core.schedule
{
	/// <summary>
	/// Time instant.
	/// </summary>
	[Serializable]
	public class Time : IComparable
	{
		// well-defined time units.
		public const long SECOND=1;
		public const long MINUTE=60;
		public const long HOUR=3600;
		public const long DAY=24*HOUR;
		public const long WEEK=7*DAY;
		public const long DAYSinYEAR=365;
		public const long YEAR=DAYSinYEAR*DAY;

		internal protected Time( long sec ) 
		{
			this.ticks = sec;
		}

		internal protected Time( int hour, int minute, int second ) 
		{
			this.Ticks = hour*HOUR+minute*MINUTE+second;
		}

		internal protected Time( int day, int hour, int minute, int second ) 
		{
			this.Ticks = day*DAY+hour*HOUR+minute*MINUTE+second;
		}

		/// <summary>
		/// Current time in seconds from 01/01/01 00:00am
		/// </summary>
		protected long ticks;
		public virtual long Ticks { get { return ticks; } set { ticks = value; } }

		/// <summary> Returns a string formatter for the display. </summary>
		public override string ToString() {
			return string.Format("{0}/{1}/{2}({3}) {4,2:d}:{5,2:d}:{6,2:d}",
				Year, Month, Day, DayOfTheWeek,
				Hour, Minute, Second  );
		}

		protected static readonly int[] daysOfMonth = {31,28,31,30,31,30,31, 31,30,31,30,31};

		/// <summary>
		/// Total minutes from the start of the game.
		/// Use field 'ticks' instead of the property 'Ticks',
		/// so that extended class can set offset to ticks->Ticks
		/// </summary>
		public long TotalSeconds { get { return ticks/SECOND; } }
		public long TotalMinutes { get { return ticks/MINUTE; } }
		public long TotalHours { get { return ticks/HOUR; } }
		public long TotalDays { get { return ticks/DAY; } }
		public long TotalYears { get { return ticks/YEAR; } }
		public long TotalMonths { get { return TotalYears*12+Month; } }

		#region Get the current date/time
		/// <summary>
		/// the current year. from 1.
		/// </summary>
		public virtual int Year { get { return (int)(Ticks/YEAR)+1; } }
		/// <summary>
		/// the current month. from 1.
		/// </summary>
		public virtual int Month {
			get {
				long d = Ticks%YEAR/DAY;
				for( int i=0; i<12; i++ ) {
					d -= daysOfMonth[i];
					if( d<0 )
						return i+1;
				}
				return -1;
			}
		}
		/// <summary>
		/// the current day of the month. from 1.
		/// </summary>
		public virtual int Day {
			get {
				long d = Ticks%YEAR/DAY;
				for( int i=0; i<12; i++ ) 
				{
					if( d< daysOfMonth[i] )
						return (int)d+1;
					d -= daysOfMonth[i];
				}
				return -1;
			}
		}

		public int Hour { get { return (int)(Ticks%DAY/HOUR); } }
		public int Minute { get { return (int)(Ticks%HOUR/MINUTE); } }
		public int Second { get { return (int)(Ticks%MINUTE/SECOND); } }
		#endregion

		/// <summary>
		/// the current day of the week. from 0 to 6.
		/// </summary>
		public DayOfWeek DayOfTheWeek
		{ get { return (DayOfWeek)(Ticks%WEEK/DAY); } }

		protected int indexOfWeek{ get{ return (int)(Ticks%WEEK/DAY); } }

		public bool IsWeekend {
			get {
				DayOfWeek dow = DayOfTheWeek;
				return dow==DayOfWeek.Sunday || dow==DayOfWeek.Saturday;
			}
		}

		/// <summary>
		/// Get 0 o'clock of the day. 
		/// Cut off ticks smaller than 'DAY'.
		/// </summary>
		/// <returns></returns> 		 
		public Time GetZeroOClock()
		{
			return new Time(Ticks-Ticks%DAY);
		}

		public Time GetDayOfYear()
		{
			return new Time(Ticks%YEAR);
		}

		public TimeOfDay GetTimeOfTheDay()
		{
			// Ticks is cutted off to become lesser than DAY in constructor.
			return new TimeOfDay(Ticks);
		}

		#region IComparable ÉÅÉìÉo

		public int CompareTo(object obj)
		{			
			return Math.Sign(Ticks-((Time)obj).Ticks);
		}

		#endregion

		#region Equals and GetHashCode
		public override bool Equals(object obj)
		{
			Time t = obj as Time;
			if(t!=null)
				return t.Ticks==Ticks;
			return false;
		}

		public override int GetHashCode()
		{
			return (int)(Ticks%MINUTE);
		}
		#endregion

		#region operators

		public static TimeLength operator - ( Time ta, Time tb ) 
		{
			return new TimeLength( ta.Ticks - tb.Ticks );
		}
		public static Time operator + ( Time ta, TimeLength tb ) {
			return new Time( ta.Ticks + tb.TotalSeconds );
		}
		public static implicit operator long( Time ta )
		{
			return ta.Ticks;
		}
		public static implicit operator DateTime( Time ta )
		{
			// beause this class treat no leap year, the Ticks cannot compatible to that of DataTime.
			return new DateTime(ta.Year,ta.Month,ta.Day,ta.Hour,ta.Minute,ta.Second);
		}
		#endregion

	}
}
