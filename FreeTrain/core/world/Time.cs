using System;
using System.Diagnostics;

namespace freetrain.world
{
	/// <summary>
	/// Time instant.
	/// </summary>
	[Serializable]
	public class Time
	{
		internal Time( long timeVal ) {
			this.currentTime = timeVal;
		}


		/// <summary>
		/// Current time in minutes from 01/01/01 00:00am
		/// </summary>
		protected long currentTime;

		// well-defined time units.
		public const long MINUTE	= 1;
		public const long HOUR		= MINUTE*60;
		public const long DAY		= HOUR*24;
		public const long YEAR		= DAY*365;
		// the initial time when a game starts
		public const long START_TIME	= (31+28+31)*DAY+8*HOUR;



		/// <summary> Returns a string formatter for the display. </summary>
		public string displayString {
			get {
				return string.Format("Year {0} Month {1} Day {2} ({3}) {4,2:d}:{5,1:d}0",
				//! return string.Format("{0}年{1}月{2}日({3}) {4,2:d}時{5,1:d}0分",
					year, month, day,
					dayOfWeekChar(dayOfWeek),
					hour, minutes/10 );
			}
		}

		protected static readonly int[] daysOfMonth = {31,28,31,30,31,30,31, 31,30,31,30,31};

		/// <summary>
		/// Total minutes from the start of the game.
		/// </summary>
		public long totalMinutes { get { return currentTime-START_TIME; } }

		//
		// Get the current date/time
		//

		/// <summary>
		/// the current year. from 1.
		/// </summary>
		public int year { get { return (int)(currentTime/YEAR)+1; } }
		/// <summary>
		/// the current month. from 1.
		/// </summary>
		public int month {
			get {
				long days = currentTime/DAY;
				days %= 365;	// 1 year = 365 days. No leap year.

				for( int i=0; i<12; i++ ) {
					days -= daysOfMonth[i];
					if( days<0 )
						return i+1;
				}
				Debug.Assert(false);
				return -1;
			}
		}
		/// <summary>
		/// the current day of the month. from 1.
		/// </summary>
		public int day {
			get {
				long days = currentTime/DAY;
				days %= 365;	// 1 year = 365 days. No leap year.

				for( int i=0; i<12; i++ ) {
					if( days< daysOfMonth[i] )
						return (int)days+1;
					days -= daysOfMonth[i];
				}
				Debug.Assert(false);
				return -1;
			}
		}
		/// <summary>
		/// the current day of the week. from 0 to 6.
		/// </summary>
		public int dayOfWeek {
			get {
				long days = currentTime/DAY;
				return (int)days%7;
			}
		}
		public int hour { get { return (int)((currentTime/HOUR)%24); } }
		public int minutes { get { return (int)((currentTime/MINUTE)%60); } }

		public DayNight dayOrNight {
			get {
				int h = hour;
				if( 6<=h && h<18)		return DayNight.DayTime;
				else					return DayNight.Night;
			}
		}

		public Season season {
			get {
				int mon = month;
				return (Season)(((mon+9 /*effectively -3*/)%12)/3);
			}
		}

		public bool isWeekend {
			get {
				int dow = dayOfWeek;
				return dow==0 || dow==6;
			}
		}


		public static char dayOfWeekChar( int dow ) {
			return "7123456"[dow];
			//! Translator's comment: The string above is a temporary
			//! solution, since the weekday can only be one character
			//! long in the current solution.
			//! return "SunMonTueWedThuFriSat "[dow];
			//! return "日月火水木金土"[dow];
		}

		public static TimeLength operator - ( Time ta, Time tb ) {
			return TimeLength.fromMinutes( ta.currentTime - tb.currentTime );
		}

		public static Time operator + ( Time ta, TimeLength tb ) {
			return new Time( ta.currentTime + tb.totalMinutes );
		}
	}
}
