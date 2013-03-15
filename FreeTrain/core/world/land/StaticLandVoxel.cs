using System;
using System.Drawing;
using freetrain.contributions.land;
using freetrain.framework.plugin;
using freetrain.world.rail;
using freetrain.world.structs;

namespace freetrain.world.land
{
	/// <summary>
	/// Land voxel with a fixed graphics and its population.
	/// </summary>
	[Serializable]
	public class StaticLandVoxel : LandVoxel
	{
		public StaticLandVoxel( Location loc, StaticLandBuilder contrib ) : base(loc) {
			this.contrib = contrib;

			if( contrib.population!=null )
				this.stationListener = new StationListenerImpl( contrib.population, loc );
		}

		public override void onRemoved() {
			if( stationListener!=null )
				stationListener.onRemoved();
		}

		public override int entityValue { get { return contrib.price; } }

		private readonly StationListenerImpl stationListener;

		private readonly StaticLandBuilder contrib;

		public override void draw( DrawContext surface, Point pt, int heightCutDiff ) {
			// always draw it regardless of the height cut
			contrib.sprite.draw( surface.surface, pt );
		}

		public override object queryInterface( Type aspect ) {
			// if type.population is null, we don't have any population
			if( aspect==typeof(StationListener) )
				return stationListener;
			else
				return base.queryInterface(aspect);
		}
	}
}
