using System;
using System.Diagnostics;

namespace nft.core.schedule
{
	/// <summary>
	/// Time instant that represent a time of a day.
	/// The Year, Month, Day is always 0 for instances of this class.
	/// </summary>
	[Serializable]
	public class TimeOfDay : Time
	{
		internal protected TimeOfDay( long ticks ) : base(ticks%Time.DAY) {}

		public TimeOfDay( int hour, int minute ) : this ( hour, minute, 0 )	{}

		public TimeOfDay( int hour, int minute, int second ) : base ( hour, minute, second ) {}

		public override long Ticks
		{
			get	{ return base.Ticks; }
			set { base.Ticks = value%DAY; }
		}

		public override int Year { get { return 0; } }
		public override int Month { get { return 0; } }
		public override int Day { get { return 0; } }
	}
}
