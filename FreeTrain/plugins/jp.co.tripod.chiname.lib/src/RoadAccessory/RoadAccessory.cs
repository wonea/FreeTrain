using System;
using System.Drawing;
using freetrain.world;

namespace freetrain.world.road.accessory
{
	/// <summary>
	/// Accessory implementation.
	/// </summary>
	[Serializable]
	public class RoadAccessory : TrafficVoxel.Accessory
	{
		private readonly byte index;
		private readonly RoadAccessoryContribution contrib;

		public RoadAccessory( TrafficVoxel target, RoadAccessoryContribution _contrib, int _index ) {
			this.index = (byte)_index;
			this.contrib = _contrib;
			target.accessory = this;
		}

		public void drawBefore( DrawContext display, Point pt ) {
			contrib.sprites[index,0].draw( display.surface, pt );
		}

		public void drawAfter( DrawContext display, Point pt ) {
			contrib.sprites[index,1].draw( display.surface, pt );
		}
		
		public void onRemoved() {}
	}
}
