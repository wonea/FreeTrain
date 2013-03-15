using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using freetrain.contributions.rail;
using freetrain.framework;
using freetrain.framework.plugin;
using freetrain.util;
using freetrain.world.accounting;
using freetrain.world.structs;
using freetrain.world.development;

namespace freetrain.world.rail
{
	/// <summary>
	/// Station
	/// </summary>
	[Serializable]
	public class Station : PThreeDimStructure, PlatformHost, TrainHarbor
	{
		/// <summary>
		/// Creates a new station object with its left-top corner at
		/// the specified location.
		/// </summary>
		/// <param name="_type">
		/// Type of the station to be built.
		/// </param>
		public Station( StationContribution _type, WorldLocator wloc  ) : base( _type, wloc ) {
			this.type = _type;
			this._name = string.Format("ST{0,2:d}",iota++);
			if(wloc.world == World.world){
				World.world.stations.add(this);
				World.world.clock.registerRepeated( new ClockHandler(clockHandlerHour), TimeLength.fromHours(1) );
				World.world.clock.registerRepeated( new ClockHandler(clockHandlerDay),  TimeLength.fromHours(24) );
			}
			Distance r = new Distance( REACH_RANGE, REACH_RANGE, REACH_RANGE );

			// advertise listeners in the neighborhood that a new station is available
			foreach( Entity e in Cube.createInclusive( baseLocation-r, baseLocation+r ).getEntities() ) {
				StationListener l = (StationListener)e.queryInterface(typeof(StationListener));
				if( l!=null )
					l.advertiseStation(this);
			}
		}

		private new readonly StationContribution type;

		/// <summary>
		/// sequence number generator for automatic name generation.
		/// </summary>
		private static int iota=1;

		/// <summary> Name of this station. </summary>
		private string _name;
		
		public override string name { get { return _name; } }

		public void setName( string name ) {
			this._name = name;
		}


		public Location location { get { return baseLocation; } }

		public override bool onClick() {
			new StationPropertyDialog(this).ShowDialog(MainWindow.mainWindow);
			return true;
		}

		public override string ToString() { return name; }


		#region Entity implementation
		public override bool isSilentlyReclaimable { get { return false; } }
		public override bool isOwned { get { return true; } }

		// TODO: value?
		public override int entityValue { get { return 0; } }

		public override void remove() {
			
			World.world.clock.unregister(new ClockHandler(clockHandlerHour));
			World.world.clock.unregister(new ClockHandler(clockHandlerDay));

			// first, remove this station from the list of all stations.
			// this will allow disconnected structures to find the next
			// nearest station.
			World.world.stations.remove(this);

			// notify listeners
			foreach( StationListener l in listeners )
				l.onStationRemoved(this);

			// notify nodes that this host is going to be destroyed.
			// we need to copy it into array because nodes will be updated
			// as we notify children
			foreach( Platform p in nodes.toArray(typeof(Platform)) )
				p.onHostDisconnected();
			Debug.Assert(nodes.isEmpty);
			
			base.remove();
		}
		#endregion


		public override object queryInterface( Type aspect ) {
			if( aspect==typeof(TrainHarbor) )
				return this;

			return base.queryInterface(aspect);
		}


		private readonly Set nodes = new Set();
		public void addNode( Platform child ) {
			nodes.add(child);
		}
		public void removeNode( Platform child ) {
			nodes.remove(child);
		}
		public Station hostStation {
			get {
				return this;
			}
		}

		internal protected override Color heightCutColor { get { return Color.Gray; } }


		private void onDayClock() {
			// called once a day. charge the operation cost
			AccountManager.theInstance.spend( type.operationCost, AccountGenre.RAIL_SERVICE );
		}


		#region listeners
		//
		//
		// Listener handling
		//
		//

		[Serializable]
		public class ListenerSet {
			private readonly Set core = new Set();

			public void add( StationListener listener ) {
				core.add(listener);
			}

			public void remove( StationListener listener ) {
				core.remove(listener);
			}

			public System.Collections.IEnumerator GetEnumerator() {
				return core.GetEnumerator();
			}
		}
		
		/// <summary> StationListeners that are attached to this staion. </summary>
		public readonly ListenerSet listeners = new ListenerSet();

		/// <summary>
		/// Gets the total sum of the population of this station.
		/// </summary>
		public int population {
			get {
				int p = 0;
				foreach( StationListener l in listeners )
					p += l.getPopulation(this);
				return p;
			}
		}

		// FIXME: probably there's no need to maintain the average values any longer


		/// <summary>
		/// The number of passengers that is "gone".
		/// Those are people that live in this station but are on the road.
		/// </summary>
		private int gonePassengers = 0;

		/// <summary>
		/// Weighted average of # of people that are unloaded in this station.
		/// Multiplied by AVERAGE_PASSENGER_RATIO for every hour.
		/// </summary>
		private int accumulatedUnloadedPassengers = 0;

		public int averageUnloadedPassengers { get {
			return (int)(accumulatedUnloadedPassengers*AVERAGE_PASSENGER_PER_DAY_FACTOR);
		} }

		/// <summary>
		/// Weighted average of # of people that are loaded in this station.
		/// Multiplied by AVERAGE_PASSENGER_RATIO for every hour.
		/// </summary>
		private int accumulatedLoadedPassengers = 0;

		public int averageLoadedPassengers { get {
			return (int)(accumulatedLoadedPassengers*AVERAGE_PASSENGER_PER_DAY_FACTOR);
		} }

		/// <summary>
		/// Factor that we apply to averageLoaded/UnloadedPassengers every hour.
		/// </summary>
		const float AVERAGE_PASSENGER_RATIO = 0.9996f;

		/// <summary>
		/// Factor that we need to apply to obtain average passengers per day.
		/// obtained by 24*(1-RATIO)
		/// 
		/// Justification of the above equation is that if you always carry 1 passenger
		/// for every hour, thie accumulated value should converge to C
		/// where C = C*RATIO + 1. Such C = \frac{1}{1-RATIO}
		/// </summary>
		const float AVERAGE_PASSENGER_PER_DAY_FACTOR = 24.0f*(1.0f-AVERAGE_PASSENGER_RATIO);


		protected TransportLog trains = new TransportLog(); // train numbers arrive and depart today.
		protected TransportLog import = new TransportLog(); // passengers unloaded today.
		protected TransportLog export = new TransportLog(); // passengers can be exported today.
		public double ScoreImported { get { return import.LastWeek; } }
		public double ScoreExported { get { return export.LastWeek; } }
		public double ScoreTrains { get { return trains.LastWeek; } }
		public int UnloadedToday { get { return import.Today; } }
		public int UnloadedYesterday { get { return import.Yesterday; } }
		public int TrainsToday { get { return trains.Today; } }
		public int TrainsYesterday { get { return trains.Yesterday; } }

		public void unloadPassengers( Train tr ) {
			// TODO: do something with unloaded passengers
			int r = tr.unloadPassengers();
			import.AddAmount(r);
			trains.AddAmount(1);
			Debug.WriteLine(string.Format("devQ on unload v={0} for {1} passengers.",import.LastWeek/24,r));
			World.world.landValue.addQ( location, Math.Min((float)(import.LastWeek/24),r) );
			accumulatedUnloadedPassengers += r;
			GlobalTrafficMonitor.TheInstance.NotifyPassengerTransport(this,r);
		}

		/// <summary>
		/// Obtains the number of the passenger for the train
		/// that is going to depart.
		/// </summary>
		/// <param name="capacity">train to put passengers in</param>
		public void loadPassengers( Train tr ) {
			int total = this.population;
			if(total==0)	return;		// avoid division by 0

			int avail = Math.Max(0, total - gonePassengers);
				
			// one train can't have 100% of available populations. (the number is arbitrarily set to 30%)
			int pass = Math.Min( tr.passengerCapacity, (int)(avail*0.3f) );
			export.AddAmount(tr.passengerCapacity-pass);
			trains.AddAmount(1);

			gonePassengers += pass;
			accumulatedLoadedPassengers += pass;
			World.world.landValue.addQ( location, pass );
			Debug.WriteLine(name+": # of passengers gone (up to) " + gonePassengers );

			tr.loadPassengers(this, pass);
		}

		public void clockHandlerHour() {
			// increase the passenger ratio
			gonePassengers = (int)(gonePassengers*0.8f);
			Debug.WriteLine(name+": # of passengers gone (down to) " + gonePassengers );

			// update those statistics
			accumulatedLoadedPassengers = (int)(accumulatedLoadedPassengers*AVERAGE_PASSENGER_RATIO);
			accumulatedUnloadedPassengers = (int)(accumulatedUnloadedPassengers*AVERAGE_PASSENGER_RATIO);
		}

		public void clockHandlerDay() {
			// called once a day. charge the operation cost
			AccountManager.theInstance.spend( type.operationCost, AccountGenre.RAIL_SERVICE );
			import.DailyReset();
			export.DailyReset();
			trains.DailyReset();
		}

		public const int REACH_RANGE = 10;

		/// <summary>
		/// Returns true if a listener at the given location can use this station.
		/// </summary>
		/// <param name="loc"></param>
		/// <returns></returns>
		public bool withinReach( Location loc ) {
			// TODO: maybe it's better to take Listener as a parameter
			return distanceTo(loc)<REACH_RANGE;
		}
		#endregion


		/// <summary>
		/// Gets the station object if one is in the specified location.
		/// </summary>
		public static Station get( Location loc ) {
			return World.world.getEntityAt(loc) as Station;
		}

		public static Station get( int x, int y, int z ) { return get(new Location(x,y,z)); }
	}

	[Serializable]
	public class TransportLog {
		private const double LogFactor = 5;

		private int today = 0;
		private int yesterday = 0;
		private double thisweek = 0;
		private double lastweek = 0;
		public void AddAmount(int amount){
			today+=amount;
		}

		public void DailyReset(){
			thisweek += Math.Pow(today,1/LogFactor);
			yesterday = today;
			today = 0;
			Clock c = World.world.clock;
			if(c.dayOfWeek == 6 ) {
				lastweek = Math.Pow(thisweek/7,LogFactor);
				thisweek = 0;
				Debug.WriteLine("report "+lastweek);
			}
		}
		public int Yesterday { get{ return yesterday; }}
		public int Today { get{ return today; }}
		public double ThisWeek { 
			get{ return Math.Pow(thisweek/(World.world.clock.dayOfWeek+1),LogFactor); }
		}
		public double LastWeek { get{ return lastweek; }}
	}
}
