using System;
using System.Diagnostics;
using freetrain.contributions.common;
using freetrain.world.rail;
using freetrain.framework.plugin;

namespace freetrain.world.structs
{
	/// <summary>
	/// Structure that has population.
	/// </summary>
	// TODO: this doesn't work quite well. for example, VarHeightBuilding is a populated structure
	// but doesn't derive from this class. Needs to be fixed.
	[Serializable]
	public abstract class PopulatedStructure : PThreeDimStructure
	{
		public PopulatedStructure( FixedSizeStructureContribution type, WorldLocator wloc )
			: base(type,wloc) {

			if( type.population!=null && wloc.world==World.world)
				stationListener = new StationListenerImpl( type.population, wloc.location );
		}

		/// <summary>
		/// Station to which this structure sends population to.
		/// </summary>
		private readonly StationListenerImpl stationListener;

		public override object queryInterface( Type aspect ) {
			// if type.population is null, we don't have any population
			if( aspect==typeof(rail.StationListener) )
				return stationListener;
			else
				return base.queryInterface(aspect);
		}


		public override void remove() {
			base.remove();

			if( stationListener!=null )
				stationListener.onRemoved();
		}
	}
}
