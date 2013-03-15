using System;
using System.Drawing;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.framework.plugin;
using freetrain.world;

namespace freetrain.framework.graphics
{
	/// <summary>
	/// Draw an image in the picture with transforming colors by keying a hue.
	/// </summary>
	[Serializable]
	public class HueTransformSprite : SimpleSprite {
		/// <summary>
		/// Source colors are transformed into a color series of this.
		/// </summary>
		private readonly Color[] targetColors = new Color[3];

		public HueTransformSprite( Picture _pic, Point _offset, Point _origin, Size _size,
			Color R_target, Color G_target, Color B_target )
			
			: base(_pic,_offset,_origin,_size) {

			SetColorMap(ColorMask.R, R_target);
			SetColorMap(ColorMask.G, G_target);
			SetColorMap(ColorMask.B, B_target);
		}

		public override void draw( Surface surface, Point pt ) {
			pt.X -= offset.X;
			pt.Y -= offset.Y;

			int idx	= (World.world.viewOptions.useNightView)?1:0;
			
			surface.bltHueTransform( pt, picture.surface, origin, size,
				RedTarget, GreenTarget, BlueTarget );
		}

		public Color RedTarget { get { return targetColors[(int)ColorMask.R]; }}
		public Color GreenTarget { get { return targetColors[(int)ColorMask.G]; }}
		public Color BlueTarget { get { return targetColors[(int)ColorMask.B]; }}

		private void SetColorMap(ColorMask channel, Color dest){ targetColors[(int)channel] = dest; }
	}


	/// <summary>
	/// SpriteFactory for HueTransformSprite.
	/// </summary>
	public class HueTransformSpriteFactory : SpriteFactory
	{
		//private readonly Color keyColor;
		//private readonly ColorMask mask;
		protected readonly Color[] targetColors = new Color[3];

		/// <summary>
		/// Load the setting from a XML manifest of the format:
		///		&lt;map from="100,200,*" to="50,30,20" />
		/// </summary>
		/// <param name="e">
		/// The parent of a &lt;map> element.
		/// </param>
		public HueTransformSpriteFactory( XmlElement e ) {
			XmlNodeList lst = e.SelectNodes("map");
			for(int i=0; i<3; i++)
				targetColors[i] = Color.Transparent;
			foreach( XmlElement map in lst ) 
			{
				string[] from = map.Attributes["from"].Value.Split(',');
				ColorMask mask;
				if(from.Length==3)
				{
					if(     from[0].Equals("*"))	mask=ColorMask.R;
					else if(from[1].Equals("*"))	mask=ColorMask.G;
					else if(from[2].Equals("*"))	mask=ColorMask.B;
					else	throw new FormatException("no mask is specified:"+map.Attributes["from"].Value);
					SetColorMap(mask, PluginUtil.parseColor(map.Attributes["to"].Value));
				}
				else
				{
					string v = from[0].ToLower();
					if( v.Equals("r") || v.Equals("red") ) mask=ColorMask.R;
					else if( v.Equals("g") || v.Equals("green") ) mask=ColorMask.G;
					else if(  v.Equals("b") || v.Equals("blue") ) mask=ColorMask.B;
					else	throw new FormatException("no mask is specified:"+v);
					SetColorMap(mask, PluginUtil.parseColor(map.Attributes["to"].Value));
				}
			}
		}

		public HueTransformSpriteFactory( Color _key, ColorMask _mask, Color _target ) {
			SetColorMap(_mask, _target);
			for(int i=0; i<3; i++)
			{
				if(i!=(int)_mask)
					targetColors[i] = Color.Transparent;
			}
		}

		private static int safeParse( string value ) {
			if(value.Equals("*"))	return 0;
			else					return int.Parse(value);
		}

		public override Sprite createSprite( Picture picture, Point offset, Point origin, Size size ) {
			return new HueTransformSprite(picture,offset,origin,size, RedTarget, GreenTarget, BlueTarget);
		}

		public Color RedTarget { get { return targetColors[(int)ColorMask.R]; }}
		public Color GreenTarget { get { return targetColors[(int)ColorMask.G]; }}
		public Color BlueTarget { get { return targetColors[(int)ColorMask.B]; }}

		private void SetColorMap(ColorMask channel, Color dest){ targetColors[(int)channel] = dest; }
	}

	[Serializable]
	public class HueTransformSpriteFactoryContributionImpl : SpriteFactoryContribution
	{
		public HueTransformSpriteFactoryContributionImpl( XmlElement e ) : base(e) {}

		public override SpriteFactory createSpriteFactory( XmlElement e ) {
			return new HueTransformSpriteFactory(e);
		}
	}
}
