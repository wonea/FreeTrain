using System;
using System.Drawing;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.world;
using freetrain.framework.plugin;

namespace freetrain.framework.graphics
{
	/// <summary>
	/// Draw an image in the picture with color-mapping.
	/// </summary>
	[Serializable]
	public class ColorMappedSprite : SimpleSprite {
		protected readonly Color[][] srcColors;
		protected readonly Color[][] dstColors;

		public ColorMappedSprite( Picture _pic, Point _offset, Point _origin, Size _size, Color[] srcColors, Color[] dstColors )
			: base(_pic,_offset,_origin,_size) {
			// To map colors in the night overrid picture without reducing luminance by quarter,
			// add non-darkened color into the color table.
			// **Note that this modification is INCOMPLETE**
			// 1. There is a possibility of undesirable translation in the day&night picture at night.
			//    For example, a mapping from (r,g,b) to (r',g',b') can cause
			//    undesirable mapping from (r*4,g*4,b*4) to (R'*4,g'*4,b'*4).
			//    This trouble rarely happens because (r,g,b) each values must be under 64.
			// 2. There is a possibility of undesirable translation in the night override picture.
			//    For example, a mapping from (r,g,b) to (r',g',b') can cause
			//    undesirable mapping from (r/4,g/4,b/4) to (R'/4,g'/4,b'/4).
			//    This trouble is avoidable by choosing source color pludently, 
			//    or adding mapping from (r/4,g/4,b/4) to (r/4,g/4,b/4).
			this.srcColors = new Color[2][];
			this.srcColors[0] = new Color[srcColors.Length];
			this.srcColors[1] = new Color[srcColors.Length*2];
			this.dstColors = new Color[2][];
			this.dstColors[0] = new Color[srcColors.Length];
			this.dstColors[1] = new Color[srcColors.Length*2];
			
			for( int i=0; i<srcColors.Length; i++ ) {
				this.srcColors[0][i] = srcColors[i];
				this.srcColors[1][i+srcColors.Length] = srcColors[i];
				this.srcColors[1][i] = ColorMap.getNightColor(srcColors[i]);
				this.dstColors[0][i] = dstColors[i];
				this.dstColors[1][i+srcColors.Length] = dstColors[i];
				this.dstColors[1][i] = ColorMap.getNightColor(dstColors[i]);
			}
		}

		public override void draw( Surface surface, Point pt ) {
			pt.X -= offset.X;
			pt.Y -= offset.Y;
			int idx = (World.world.viewOptions.useNightView)?1:0;
			surface.bltColorTransform( pt, picture.surface, origin, size,
				srcColors[idx], dstColors[idx], false );
		}
	}


	/// <summary>
	/// SpriteFactory for ColorMappedSprite.
	/// </summary>
	public class ColorMappedSpriteFactory : SpriteFactory
	{
		private readonly Color[] srcColors;
		private readonly Color[] dstColors;

		/// <summary>
		/// Load a color map from the XML manifest of the format:
		///		&lt;map from="100,200, 50" to="50,30,20" />
		///		&lt;map from="  0, 10,100" to="..." />
		///		...
		/// </summary>
		/// <param name="e">
		/// The parent of &lt;map> elements.
		/// </param>
		public ColorMappedSpriteFactory( XmlElement e ) {
			XmlNodeList lst = e.SelectNodes("map");
			
			srcColors = new Color[lst.Count];
			dstColors = new Color[lst.Count];

			int i=0;
			foreach( XmlElement map in lst ) {
				srcColors[i] = PluginUtil.parseColor(map.Attributes["from"].Value);
				dstColors[i] = PluginUtil.parseColor(map.Attributes["to"  ].Value);
				i++;
			}
		}

		public ColorMappedSpriteFactory( Color[] src, Color[] dst ) {
			this.srcColors = src;
			this.dstColors = dst;
		}

		public override Sprite createSprite( Picture picture, Point offset, Point origin, Size size ) {
			return new ColorMappedSprite(picture,offset,origin,size,srcColors,dstColors);
		}
	}

	[Serializable]
	public class ColorMappedSpriteFactoryContributionImpl : SpriteFactoryContribution
	{
		public ColorMappedSpriteFactoryContributionImpl( XmlElement e ) : base(e) {}

		public override SpriteFactory createSpriteFactory( XmlElement e ) {
			return new ColorMappedSpriteFactory(e);
		}
	}
}
