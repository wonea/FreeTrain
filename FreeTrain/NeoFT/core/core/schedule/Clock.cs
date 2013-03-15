using System;
using System.Diagnostics;
using nft.core.game;

namespace nft.core.schedule
{
	/// <summary>
	/// Handles a clock event.
	/// </summary>
	public delegate void ClockHandler();

	/// <summary>
	/// Clock that governs the time of the world.
	/// 
	/// Because of the way Windows Forms work, this class is not self-sufficient.
	/// The main window needs to run a timr and periodically call the tick method
	/// of this class to make this class work.
	/// </summary>
	[Serializable]
	public class Clock : Time 
	{
		protected Time START_TIME;

		// creatable only from the World class.
		// Initialize the value to April, 1st 8:00 AM.
		public Clock(Time origin) : base(0)
		{
			START_TIME = origin;
			OnTick = new EventHandler(NullHandler);
			for( int i=0; i<7; i++)
				weeklyQueue[i] = new ClockEventQueue();
		}
		
		public void SetTime( long t ) 
		{
			this.Ticks = t;
			// notify the time change
			//PictureManager.reset();
			//World.world.onAllVoxelUpdated();
		}

		public override long Ticks
		{
			get	{ return ticks+START_TIME.Ticks; }
			set	{ ticks = value-START_TIME.Ticks; }
		}


		/// <summary>
		/// Handlers that are waiting for the clock notification.
		/// </summary>
		private readonly ClockEventQueue dailyQueue = new ClockEventQueue(4);
		private readonly ClockEventQueue[] weeklyQueue = new ClockEventQueue[7];
		private readonly ClockEventQueue yearlyQueue = new ClockEventQueue();
		private readonly ClockEventQueue oneshotQueue = new OneShotClockEventQueue();

		#region handler maintainance
		/// <summary>
		/// Registers an one-shot timer, which will be fired after
		/// the specified time span.
		/// </summary>
		public void RegisterOneShot( ClockHandler handler, TimeLength time ) 
		{
			Debug.Assert(time.TotalSeconds>0);
			oneshotQueue.Add( this+time, handler );
		}

		/// <summary>
		/// Registers an one-shot timer, which will be fired at
		/// the specified time.
		/// </summary>
		public void RegisterOneShot( ClockHandler handler, Time time ) 
		{
			Debug.Assert(this<time);
			oneshotQueue.Add( time, handler);
		}

		/// <summary>
		/// Registers an one-shot timer, which will be fired at
		/// the specified time.
		/// </summary>
		public void RegisterOneShot( ClockHandler handler, TimeOfDay time ) 
		{
			RegisterOneShot( handler, UntilTheTime(time) );
		}

		/// <summary>
		/// Registers a daily-timer, which will be fired
		/// everyday at specified time.
		/// the value second of the 'time' is ignored.
		/// </summary>
		public void RegisterDailyHandler( ClockHandler handler, TimeOfDay time ) 
		{
			Debug.Assert(time.Ticks>=0);
			dailyQueue.Add( time, handler );
		}

		/// <summary>
		/// Registers a weekly-timer, which will be fired
		/// everyweek at specified time.
		/// the value second of the 'time' is ignored.
		/// </summary>
		public void RegisterWeeklyHandler( ClockHandler handler, DayOfWeek week, TimeOfDay time ) 
		{
			Debug.Assert(time.Ticks>=0);
			weeklyQueue[(int)week].Add( time, handler );
		}

		/// <summary>
		/// Registers a yearly-timer, which will be fired
		/// everyyear at specified time.
		/// the value second and year of the 'time' is ignored.
		/// </summary>
		public void RegisterYearlyHandler( ClockHandler handler, Time time ) 
		{
			Debug.Assert(time.Ticks>=0);
			yearlyQueue.Add( new Time(time.Ticks%YEAR), handler );
		}

		/// <summary>
		/// Unregisters a daily repeated timer.
		/// </summary>
		public void UnregisterDailyHandler( ClockHandler handler, TimeOfDay time ) 
		{
			dailyQueue.Remove(time,handler);
		}

		/// <summary>
		/// Unregisters a weekly repeated timer.
		/// </summary>
		public void UnregisterWeeklyHandler( ClockHandler handler, DayOfWeek week, TimeOfDay time ) 
		{			
			weeklyQueue[(int)week].Remove(time,handler);
		}

		/// <summary>
		/// Unregisters a yearly repeated timer.
		/// </summary>
		public void UnregisterYearlyHandler( ClockHandler handler, Time time ) 
		{
			yearlyQueue.Remove(new Time(time.Ticks%YEAR), handler);
		}
		

		[Serializable]
			protected class RepeatedTimer 
		{
			public RepeatedTimer( Clock _clock, ClockHandler _handler, TimeLength first, TimeLength _interval ) 
			{
				this.clock = _clock;
				this.handler = _handler;
				this.interval = _interval;
				clock.RegisterOneShot( new ClockHandler(onClock), first );
			}

			private readonly Clock clock;
			private readonly ClockHandler handler;
			private readonly TimeLength interval;

			public void onClock() 
			{
				handler();
				clock.RegisterOneShot( new ClockHandler(onClock), interval );
			}
		}
		#endregion

		/// <summary>
		/// One-time call back at the end of a turn.
		/// 
		/// To get continuous call back after each end of turn,
		/// keep registering handlers at the end of each callback.
		/// </summary>
		[NonSerialized]
		public EventHandler OnTick;

		#region utility function 'Until???'
		// create a time span object that represents the period until tomorrow's 0:00.
		// if the current time is just 0:00, it returns "24hours" rather than 0.
		public TimeLength UntilTomorrow() 
		{
			return new TimeLength( (24-Hour)*HOUR+(60-Minute)*MINUTE+(60-Second)*SECOND );
		}

		// The shortest positive TimeLength to the time matched to the value.
		public TimeLength UntilTheTime( TimeOfDay time )
		{
			long s = Ticks%DAY;
			long n = time;
			if( s<n ) 
				s+=DAY;
			return new TimeLength(s-n);
		}

		// The shortest positive TimeLength to the time matched to the value.
		public TimeLength UntilTheTime( int hour, int minute )
		{
			long n = hour*HOUR+minute*MINUTE;
			n %= DAY;
			long s = Ticks%DAY;

			if( s<n ) 
				s+=DAY;
			return new TimeLength(s-n);
		}

		// The shortest positive TimeLength to the time matched to the value.
		public TimeLength UntilTheDay( int month, int day )
		{
			long n = day;
			for(int i=0; i<month; i++)
				n+=daysOfMonth[i];
			n %= YEAR;
			long s = (Ticks%YEAR)/DAY;
			if( s<n ) 
				s+=YEAR;
			return new TimeLength(s-n);
		}
		#endregion

		/// <summary>
		/// Make the clock tick.
		/// </summary>
		public void Tick()  
		{
			Tick(1);
		}

		/// <summary>
		/// Make the clock tick.
		/// </summary>
		public void Tick(long n)  
		{
			ticks+=n;
			
			//			long m = (Ticks%DAY);
			//			if( m==6*HOUR || m==18*HOUR ) 
			//			{
			//				PictureManager.reset();
			//				World.world.onAllVoxelUpdated();	// time change
			//			}
			int iw = indexOfWeek;
			if(Ticks%DAY == 0)
			{
				weeklyQueue[iw].Reset();
				dailyQueue.Reset();
				if(Ticks%YEAR == 0)
					yearlyQueue.Reset();
			}
			// fire yearly
			Time dy = GetDayOfYear();
			yearlyQueue.Dispatch(dy);
			// fire weekly and daily
			TimeOfDay td = GetTimeOfTheDay();
			weeklyQueue[iw].Dispatch(td);
			dailyQueue.Dispatch(td);
			// fire one shot
			oneshotQueue.Dispatch(this);

			if(OnTick!=null) OnTick(this,null);
		}
		
		public void NullHandler(object sender, EventArgs arg ){}
	}
}
