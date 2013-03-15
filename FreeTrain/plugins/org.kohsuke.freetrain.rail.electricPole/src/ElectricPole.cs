using System;
using System.Drawing;
using freetrain.world;

namespace freetrain.world.rail.pole
{
	/// <summary>
	/// Accessory implementation.
	/// </summary>
	[Serializable]
	public class ElectricPole : TrafficVoxel.Accessory
	{
		private readonly byte index;
		private readonly ElectricPoleContribution contrib;

		public ElectricPole( TrafficVoxel target, ElectricPoleContribution _contrib, int _index ) {
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
		
		public static ElectricPole get( Location loc ) {
			TrafficVoxel v = TrafficVoxel.get(loc);
			if(v==null)		return null;
			return v.accessory as ElectricPole;
		}
	}
}
