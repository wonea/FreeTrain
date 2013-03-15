using System;
using System.Diagnostics;
using System.Windows.Forms;
using freetrain.util;
using freetrain.framework;
using freetrain.framework.graphics;

namespace freetrain.world
{
	/// <summary>
	/// Handles a clock event.
	/// </summary>
	public delegate void ClockHandler();

	public enum Season : byte {
		Spring=0,
		Summer=1,
		Autumn=2,
		Winter=3
	}
	public enum DayNight : byte {
		DayTime=0,
		Night=1
	}

	/// <summary>
	/// Clock that governs the time of the world.
	/// 
	/// Because of the way Windows Forms work, this class is not self-sufficient.
	/// The main window needs to run a timr and periodically call the tick method
	/// of this class to make this class work.
	/// </summary>
	[Serializable]
	public class Clock : Time {
		// creatable only from the World class.
		// Initialize the value to April, 1st 8:00 AM.
		public Clock() : base(START_TIME) {}

		public void setCurrentTime( long t ) {
			this.currentTime = t;
			// notify the time change
			PictureManager.reset();
			World.world.onAllVoxelUpdated();
		}

		/// <summary>
		/// Handlers that are waiting for the clock notification.
		/// </summary>
		private readonly PriorityQueue queue = new PriorityQueue();

		#region handler maintainance
		/// <summary>
		/// Registers an one-shot timer, which will be fired after
		/// the specified time span.
		/// </summary>
		public void registerOneShot( ClockHandler handler, TimeLength time ) {
			Debug.Assert(time.totalMinutes>0);
			queue.insert( currentTime+time.totalMinutes, handler );
		}

		/// <summary>
		/// Registers an one-shot timer, which will be fired at
		/// the specified time.
		/// </summary>
		public void registerOneShot( ClockHandler handler, Time time ) {
			registerOneShot( handler, time - World.world.clock );
		}


		/// <summary>
		/// Registers a repeated-timer, which will be fired
		/// periodically for every specified interval.
		/// 
		/// The first clock notification will be sent also after the
		/// specified minutes.
		/// </summary>
		/// <returns>
		/// The cookie, which shall be then used to unregister the timer.
		/// </returns>
		public ClockHandler registerRepeated( ClockHandler handler, TimeLength time ) {
			return registerRepeated( handler, time, time );
		}

		/// <summary>
		/// Registers a repeated-timer, which will be fired
		/// periodically for every specified interval.
		/// </summary>
		/// <param name="first">The first event will be sent after this interval.</param>
		/// <param name="interval">Successive events will be fired with this interval.</param>
		/// <returns>
		/// The cookie, which shall be then used to unregister the timer.
		/// </returns>
		public ClockHandler registerRepeated( ClockHandler handler, TimeLength first, TimeLength interval ) {
			Debug.Assert(interval.totalMinutes!=0 && first.totalMinutes!=0);
			RepeatedTimer rt = new RepeatedTimer( this, handler, first, interval );
			return new ClockHandler(rt.onClock);
		}

		/// <summary>
		/// Unregisters a repeated timer.
		/// </summary>
		public void unregister( ClockHandler handler ) {
			queue.remove(handler);
		}
		
		[Serializable]
		protected class RepeatedTimer {
			public RepeatedTimer( Clock _clock, ClockHandler _handler, TimeLength first, TimeLength _interval ) {
				this.clock = _clock;
				this.handler = _handler;
				this.interval = _interval;
				clock.registerOneShot( new ClockHandler(onClock), first );
			}

			private readonly Clock clock;
			private readonly ClockHandler handler;
			private readonly TimeLength interval;

			public void onClock() {
				handler();
				clock.registerOneShot( new ClockHandler(onClock), interval );
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
		public EventHandler endOfTurnHandlers;


		/// <summary>
		/// Make the clock tick.
		/// </summary>
		public void tick()  {
			if(MainWindow.mainWindow.currentController!=null)
				return;	// if a controller is active, stop the timer.

			currentTime++;
			
			long m = (currentTime%DAY);
			if( m==6*HOUR || m==18*HOUR ) {
				PictureManager.reset();
				World.world.onAllVoxelUpdated();	// time change
			}

			Debug.Assert( (long)queue.minPriority>=currentTime );

			while(queue.count!=0 && (long)queue.minPriority==currentTime) {
				((ClockHandler)queue.minValue)();
				queue.removeMin();
			}

			// call end-of-the-turn handlers
			EventHandler eot = endOfTurnHandlers;
			endOfTurnHandlers = null;	// so that callback methods can re-register themselves
			if(eot!=null)	eot(this,null);
		}

	}
}
