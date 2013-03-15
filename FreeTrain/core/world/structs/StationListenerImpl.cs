using System;
using System.Collections;
using System.Diagnostics;
using freetrain.contributions.population;
using freetrain.framework.plugin;
using freetrain.world.rail;

namespace freetrain.world.structs
{
	/// <summary>
	/// StationListener implementation that uses
	/// Population object to calculate population.
	/// </summary>
	[Serializable]
	public class StationListenerImpl : rail.StationListener
	{
		public const int MaxStationCount = 4;
		/// <param name="pop">Population pattern</param>
		/// <param name="loc">The location used to decide if this object
		/// can subscribe to a given station.</param>
		public StationListenerImpl( Population pop, Location loc ) {
			this.population = pop;
			this.location = loc;
			stations = new ArrayList(MaxStationCount);
			if( population!=null )
				attachToStation();	// attach to the existing station if any
		}

		/// <summary>
		/// Station to which this structure sends population to.
		/// </summary>
		//private Station station;
		private ArrayList stations;

		private readonly Location location;

		private readonly Population population;


		/// <summary>
		/// Should be called when the owner is removed.
		/// </summary>
		public void onRemoved() {
			// remove from the currently attached station
			foreach(Station station in stations)
			{
				station.listeners.remove(this);
			}
			stations.Clear();
		}

		public int getPopulation( Station s ) {
			int v = World.world.landValue[location];
			int p =population.calcPopulation(World.world.clock);
			p /= stations.Count;
			return Math.Min(p,v+10);
		}

		public bool advertiseStation( Station s ) {
			// keep stations within 4
			if(stations.Count<MaxStationCount)
				s.listeners.add(this);
			else	
			{
				int dmax = location.distanceTo(s.baseLocation);
				Station remove = null;
				foreach(Station station in stations)
				{
					int d = location.distanceTo(station.baseLocation);
					if(d>dmax)
					{
						remove = station;
						dmax = d;
					}
				}
				if(remove!=null)
				{
					remove.listeners.remove(this);
					stations[stations.IndexOf(remove)]=s;
				}
				return false;
			}			
			stations.Add(s);
			return true;
		}

		public void onStationRemoved( Station s ) {
			stations.Remove(s);
		}

		/// <summary>
		/// Finds the nearest station and attaches to it.
		/// </summary>
		private void attachToStation() {
			foreach( Station s in World.world.stations ) {
				if( !s.withinReach(location) )
					continue;
				advertiseStation(s);					
			}
		}
	}
}
