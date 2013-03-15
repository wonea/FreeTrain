using System;
using System.Drawing;

namespace freetrain.world.road
{
	/// <summary>
	/// Bus
	/// </summary>
	[Serializable]
	public class Bus : Car
	{
		public Bus() {
		}

		public override void draw( DrawContext display, Point pt ) {
			throw new NotImplementedException();
		}
	}
}
