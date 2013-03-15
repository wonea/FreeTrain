using System;
using System.Drawing;
using freetrain.world.land;
using freetrain.world.rail;
using freetrain.world.structs;

namespace freetrain.world.land.vinylhouse
{
	/// <summary>
	/// Land voxel with a fixed graphics and its population.
	/// </summary>
	[Serializable]
	public class VinylHouseVoxel : LandVoxel, Entity
	{
		public VinylHouseVoxel( Location loc, VinylHouseBuilder contrib, int index ) : base(loc) {
			this.contrib = contrib;
			this.index = index;

			this.stationListener = new StationListenerImpl( contrib.population, loc );
		}

		public override Entity entity { get { return this; } }

		public override int entityValue { get { return contrib.price; } }

		public override void onRemoved() {
			stationListener.onRemoved();
		}


		private readonly StationListenerImpl stationListener;

		private readonly VinylHouseBuilder contrib;

		private readonly int index;

		public override void draw( DrawContext surface, Point pt, int heightCutDiff ) {
			// always draw it regardless of the height cut
			contrib.sprites[index].draw( surface.surface, pt );
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
