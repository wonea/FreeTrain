using System;
using System.Drawing;
using freetrain.world.land;
using freetrain.world.rail;
using freetrain.world.structs;

namespace freetrain.world.land.rice
{
	/// <summary>
	/// voxel implementation for rice field
	/// </summary>
	[Serializable]
	public class RiceFieldVoxel : LandVoxel, Entity
	{
		public RiceFieldVoxel( Location loc, RiceFieldBuilder contrib, int xIndex, int yIndex ) : base(loc) {
			this.contrib = contrib;
			this.xIndex = xIndex;
			this.yIndex = yIndex;

			this.stationListener = new StationListenerImpl( contrib.population, loc );
		}

		private readonly StationListenerImpl stationListener;

		private readonly RiceFieldBuilder contrib;

		private readonly int xIndex,yIndex;

		public override Entity entity { get { return this; } }
		public override int entityValue { get { return contrib.price; } }

		public override void onRemoved() {
			stationListener.onRemoved();
		}


		public override void draw( DrawContext surface, Point pt, int heightCutDiff ) {
			// always draw it regardless of the height cut
			contrib.sprites[xIndex,yIndex].draw( surface.surface, pt );
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
