using System;
using System.Drawing;
using System.Xml;
using freetrain.framework.graphics;
using freetrain.framework.plugin;

namespace freetrain.contributions.road
{
	/// <summary>
	/// RoadContribution for "org.kohsuke.freetrain.road.pc-9801fa" plug-in
	/// TODO: move to its own DLL.
	/// </summary>
	[Serializable]
	public class A3RoadContribution : AbstractRoadContributionImpl
	{
		public A3RoadContribution( XmlElement e ) : base(e) {
			// load resource, but don't dispose it as sprites will still refer to this surface.
			Picture picture = getPicture(e);

			sprites = new Sprite[3];
			for( int i=0; i<3; i++ )
				sprites[i] = new SimpleSprite( picture, new Point(0,16), new Point(i*32,0), new Size(32,32) );
		}

		/// <summary>
		/// three sprites (0:E-W, 1:N-S, 2:cross)
		/// </summary>
		private readonly Sprite[] sprites;

		protected internal override Sprite getSprite( byte idx ) {
			switch(idx) {
			case 2:
			case 8:
			case 10:
				return sprites[0];
			case 1:
			case 4:
			case 5:
				return sprites[1];
			default:
				return sprites[2];
			}
		}
	}
}
