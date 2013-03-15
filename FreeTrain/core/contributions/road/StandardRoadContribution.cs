using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.world;
using freetrain.world.road;

namespace freetrain.contributions.road
{
	/// <summary>
	/// RoadContribution implemented by 15 sprites.
	/// </summary>
	[Serializable]
	public class StandardRoadContribution : AbstractRoadContributionImpl
	{
		public StandardRoadContribution( XmlElement e ) : base(e) {
			// load resource, but don't dispose it as sprites will still refer to this surface.
			Picture pic = getPicture( e );

			XmlElement picture = (XmlElement)XmlUtil.selectSingleNode(e,"picture");
			Size sz = XmlUtil.parseSize( picture.Attributes["size"].Value );
			int offsetY = int.Parse(picture.Attributes["offset"].Value);

			flatSprites = new Sprite[16];
			for( int i=0; i<15; i++ ) {
				flatSprites[i+1] = new SimpleSprite(
					pic, new Point(0,offsetY),
					new Point(locations[i*2]*sz.Width,locations[i*2+1]*sz.Height), sz ); 
			}
		}

		/// <summary>
		/// sprites by index
		/// </summary>
		private readonly Sprite[] flatSprites;

		private static readonly int[] locations = new int[] {
			2,4, 1,4, 1,1, 1,3, 0,1, 2,0, 0,2, 2,3,
			2,1, 0,0, 0,3, 1,0, 2,2, 1,2, 0,4
		};

		protected internal override Sprite getSprite( byte idx ) {
			return flatSprites[idx];
		}
	}
}
