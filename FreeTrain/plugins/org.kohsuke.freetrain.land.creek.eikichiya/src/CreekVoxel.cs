using System;
using System.Drawing;
using freetrain.framework.graphics;

namespace freetrain.world.land.creek.eikichiya
{
	/// <summary>
	/// Land voxel with a fixed graphics and its population.
	/// </summary>
	[Serializable]
	public class CreekVoxel : LandVoxel, Entity
	{
		public CreekVoxel( Location loc, CreekBuilder contrib, Dir dir ) : base(loc) {
			this.contrib = contrib;
			this.dir = dir;
			this.sprite = contrib.getSprite(dir);
		}

		private readonly CreekBuilder contrib;

		public readonly Dir dir;

		private readonly Sprite sprite; 

		public override Entity entity { get { return this; } }

		public override void draw( DrawContext surface, Point pt, int heightCutDiff ) {
			// always draw it regardless of the height cut
			sprite.draw( surface.surface, pt );
		}

		public override int entityValue { get { return 0; } }
	}
}
