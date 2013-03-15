using System;
using System.Collections;

namespace freetrain.world.rail.tattc
{
	/// <summary>
	/// Remembers a setting of a junction.
	/// </summary>
	[Serializable]
	internal class Junction
	{
		internal Junction( Location loc ) {
			location = loc;
		}

		/// <summary> Location of the junction </summary>
		internal readonly Location location;

		/// <summary> Default route when no advanced rule applies. </summary>
		internal JunctionRoute defaultRoute = JunctionRoute.Straight;

		internal JunctionRailRoad railRoad { get { return JunctionRailRoad.get(location); } }

		/// <summary>
		/// Determines the route to take.
		/// </summary>
		internal JunctionRoute determineRoute() {
			foreach( AdvJunctionRule rule in advancedRules )
				if( rule.matches(World.world.clock) )
					return rule.route;
			return defaultRoute;
		}

		internal readonly RuleCollection advancedRules = new RuleCollection();

		[Serializable]
		internal class RuleCollection : CollectionBase {
			public void add( AdvJunctionRule rule ) {
				this.List.Add(rule);
			}
			public void remove( AdvJunctionRule rule ) {
				this.List.Remove(rule);
			}
			public void insert( int idx, AdvJunctionRule rule ) {
				this.List.Insert( idx, rule );
			}
			public void set( int idx, AdvJunctionRule rule ) {
				this.List[idx] = rule;
			}
		}
	}

	[Serializable]
	internal class AdvJunctionRule : TimeMask {
		internal JunctionRoute route = JunctionRoute.Straight;
	}
}
