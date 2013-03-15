using System;
using System.Drawing;
using freetrain.world;

namespace freetrain.world.road.dummycar
{
	/// <summary>
	/// Accessory implementation.
	/// </summary>
	[Serializable]
	public class DummyCar : TrafficVoxel.Accessory
	{
		private readonly byte index;
		private readonly DummyCarContribution contrib;
		private readonly int color;

		public DummyCar( TrafficVoxel target, DummyCarContribution _contrib , int _color, int _index ) {
			this.index = (byte)_index;
			this.contrib = _contrib;
			this.color = _color;
			target.accessory = this;
		}

		public void drawBefore( DrawContext display, Point pt ) {
			//contrib.sprites[index].draw( display.surface, pt );
		}

		public void drawAfter( DrawContext display, Point pt ) {
			contrib.sprites[color,index].draw( display.surface, pt );
		}
		
		public void onRemoved() {}
	}
}
