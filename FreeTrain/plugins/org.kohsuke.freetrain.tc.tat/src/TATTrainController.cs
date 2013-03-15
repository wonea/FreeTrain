using System;
using System.Collections;
using System.Windows.Forms;
using freetrain.contributions.rail;
using freetrain.framework;
using freetrain.framework.plugin;

namespace freetrain.world.rail.tattc
{
	/// <summary>
	/// TATTrainController の概要の説明です。
	/// </summary>
	[Serializable]
	internal class TATTrainController : TrainController
	{
		public TATTrainController( string _name ) { this.name=_name; }


		public override TrainControllerContribution contribution { get { return TATTrainControllerPlugIn.theInstance; }}

		public override JunctionRoute onJunction( Train train, JunctionRailRoad railRoad ) {
			
			Junction jc = (Junction)junctions[railRoad.location];
			if(jc==null)
				// if no configuration is given, go straight as the default behavior
				return JunctionRoute.Straight;

			// ask it
			return jc.determineRoute();
		}

		/// <summary>
		/// Map from Location to Junction.
		/// </summary>
		private readonly Hashtable junctions = new Hashtable();
		
		internal Junction getJunction( Location loc ) {
			return (Junction)junctions[loc];
		}
		internal Junction getOrCreateJunction( Location loc ) {
			Junction j = getJunction(loc);
			if(j==null)
				junctions[loc] = j = new Junction(loc);
			return j;
		}

		public override TimeLength getStopTimeSpan( Train train, TrainHarbor harbor, int callCount ) {
			StationHandler handler = getStationHandler( harbor );
			
			if(handler!=null)	return handler.getStopTimeSpan(callCount);
			else				return StationHandler.defaultHandler.getStopTimeSpan(callCount);
		}

		/// <summary>
		/// Map from Location to StationInfo.
		/// </summary>
		private readonly Hashtable stations = new Hashtable();

		internal StationHandler getStationHandler( TrainHarbor harbor ) {
			return (StationHandler)stations[harbor.location];
		}
		internal void setStationHandler( TrainHarbor harbor, StationHandler handler ) {
			stations[harbor.location] = handler;
		}



		public override void config( IWin32Window owner ) {
			refresh();
			// attach a controller to start configuration
			MainWindow.mainWindow.attachController( new TATModalController(this) );
		}

		/// <summary>
		/// Removes unused entries from the junctions table.
		/// </summary>
		private void refresh() {
			ArrayList tbr = new ArrayList();

			foreach( Location loc in junctions.Keys )
				if( JunctionRailRoad.get(loc)==null )
					tbr.Add(loc);
			
			foreach( Location loc in tbr )
				junctions.Remove(loc);
		}
	}
}
