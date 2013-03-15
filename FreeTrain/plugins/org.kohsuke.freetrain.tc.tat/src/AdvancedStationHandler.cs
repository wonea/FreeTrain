using System;
using System.Collections;

namespace freetrain.world.rail.tattc
{
	/// <summary>
	/// StationHandler that follows the detailed rules.
	/// </summary>
	[Serializable]
	internal class AdvancedStationHandler : StationHandler {
		public AdvancedStationHandler() {}


		internal readonly RuleCollection rules = new RuleCollection();

		[Serializable]
		internal class RuleCollection : CollectionBase {
			public void add( AdvStationRule rule ) {
				this.List.Add(rule);
			}
			public void remove( AdvStationRule rule ) {
				this.List.Remove(rule);
			}
			public void insert( int idx, AdvStationRule rule ) {
				this.List.Insert( idx, rule );
			}
			public void set( int idx, AdvStationRule rule ) {
				this.List[idx] = rule;
			}
		}

		private readonly TimeLength MIN_STOP_TIME = TimeLength.fromMinutes(10);

		internal override TimeLength getStopTimeSpan( int callCount ) {
			Clock clock = World.world.clock;

			if( callCount==0 ) {
				// decide whether to stop or pass
				foreach( AdvStationRule rule in rules ) {
					if( rule.action==StationAction.pass && rule.matches(clock) )
						return TimeLength.ZERO;	// pass
					if( rule.action==StationAction.stop && rule.matches(clock) )
						return MIN_STOP_TIME;	// force the train to stop at least this much
				}
				// by default, we stop.
				return MIN_STOP_TIME;

			} else {
				// TODO: do the efficient computation by using the getNextMatch method.

				// decide whether to go or sit still
				foreach( AdvStationRule rule in rules ) {
					if( rule.action==StationAction.go && rule.matches(clock) )
						return TimeLength.ZERO;	// go
					if( rule.action==StationAction.reverse && rule.matches(clock) )
						return TimeLength.fromMinutes(-1);	// turn around and go
					if( rule.action==StationAction.stop && rule.matches(clock) )
						break;		// can't go
				}

				// the unit of rules is 10 minutes. So wait until the next ten minutes break
				int next = (60-clock.minutes)%10;
				if(next==0)	next=10;
				return TimeLength.fromMinutes(next);
			}
		}
	}

	[Serializable]
	internal enum StationAction {
		pass,	// pass the station
		stop,	// sit still
		go,		// go
		reverse	// reverse
	}

	[Serializable]
	internal class AdvStationRule : TimeMask {
		internal StationAction action = StationAction.go;
	}
}
