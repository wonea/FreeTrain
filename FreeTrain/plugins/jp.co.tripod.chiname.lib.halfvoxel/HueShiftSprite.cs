using System;
using System.Collections;
using System.Drawing;
using System.Xml;
using System.Runtime.Serialization;
using org.kohsuke.directdraw;
using freetrain.world;
using freetrain.framework.plugin;
using freetrain.framework.graphics;

namespace freetrain.world.structs.hv
{

	/// <summary>
	/// SpriteFactory for ColorMappedSprite.
	/// </summary>
	public class HueShiftSpriteFactory : SpriteFactory
	{
		private int steps;

		/// <summary>
		/// Load a color map from the XML manifest of the format:
		///		&lt;map from="100,200, 50" to="50,30,20" />
		///		&lt;map from="  0, 10,100" to="..." />
		///		...
		/// </summary>
		/// <param name="e">
		/// The parent of &lt;map> elements.
		/// </param>
		public HueShiftSpriteFactory( XmlElement e ) 
		{
			try
			{
				steps = int.Parse(e.Attributes["counts"].InnerText);
			}
			catch
			{
				steps = 6;
			}
		}

		public HueShiftSpriteFactory( int counts ) 
		{
			steps = counts;
		}

		public override Sprite createSprite( Picture picture, Point offset, Point origin, Size size )
		{
			return new SimpleSprite(picture,offset,origin,size);
		}

		public Sprite[] createSprites( Bitmap bit, Picture picture, Point offset, Point origin, Size size ) 
		{
			int sprites = steps;
			int shift = 360/sprites; // hue shift per step
			Sprite[] dest = new Sprite[sprites];
			ArrayList work = new ArrayList();
			for(int y=0; y<size.Height; y++)
			{
				for(int x=0; x<size.Width; x++)
				{					
					Color c = bit.GetPixel(x+origin.X,y+origin.Y);
					if(!work.Contains(c))
						work.Add(c);
				}
			}
			work.Remove(bit.GetPixel(0,0));
			

			dest[0] = new SimpleSprite(picture,offset,origin,size);
			if(work.Count==0) 
			{
				// no replace color
				for(int i=1; i<sprites; i++ )
					dest[i]=dest[0];
			}
			else
			{
				Color[] srcColors = new Color[work.Count];
				Color[] dstColors = new Color[work.Count];
				for( int j=0; j<work.Count; j++ )
					srcColors[j] = (Color)work[j];
				for(int i=1; i<sprites; i++ )
				{
					int s2 = shift*i;
					for( int j=0; j<srcColors.GetLength(0); j++ ){
						HSVColor c = new HSVColor(srcColors[j]);
						c.Hue += s2;
						dstColors[j] = c.ToRGBColor();
					}
					dest[i] = new ColorMappedSprite(picture,offset,origin,size,srcColors,dstColors);
				}
			}
			return dest;
		}
	}

	[Serializable]
	public class HueShiftSpriteFactoryContributionImpl : SpriteFactoryContribution
	{
		public HueShiftSpriteFactoryContributionImpl( XmlElement e ) : base(e) {}

		public override SpriteFactory createSpriteFactory( XmlElement e ) {
			return new HueShiftSpriteFactory(e);
		}
	}

	public class HSVColor
	{
		protected double _H;
		protected double _S;
		protected double _V;
		public double Hue
		{
			get{ return _H; }
			set{ _H = value % 360; }
		}
		public double Saturation
		{
			get{ return _S; }
			set{ _S = Math.Max(1,Math.Min(0,value)); }
		}
		public double Brightness
		{
			get{ return _V; }
			set{ _V = Math.Max(1,Math.Min(0,value)); }
		}

		static public HSVColor FromRGB(int R, int G, int B)
		{
			return new HSVColor(Color.FromArgb(R,G,B));
		}

		public HSVColor()
		{
			_H = _S = _V = 0;
		}

		public HSVColor(Color src)
		{
			int V = Math.Max( src.R, Math.Max(src.G, src.B));
			int Z = Math.Min( src.R, Math.Min(src.G, src.B));
			_V = V/255.0;
			double d = V-Z;
			if( V != 0 )
				_S = d/V;
			else
				_S = 0.0;

			double r;
			double g;
			double b;
			if((V-Z)!= 0 )
			{
				r = (V-src.R )/d;
				g = (V-src.G )/d;
				b = (V-src.B )/d;
			}
			else
				r = g = b = 0;

			if( V == src.R )
				_H = 60*(b-g);
			else if( V == src.G )
				_H = 60*(2+r-b);
			else
				_H = 60*(4+g-r);
			if( _H < 0.0 )
				_H += 360;
		}

		public Color ToRGBColor()
		{
			int ht = (int)Math.Floor( _H / 60 );
			double d = (_H / 60 -ht)/60;
			if((ht & 1)==0) d = 1 - d;
			int V = (int)(255*_V);
			int m = (int)(255*_V*(1-_S));
			int n = (int)(255*_V*(1-_S*d));
			switch( ht ){
			case 0: return Color.FromArgb( V, n, m );
			case 1: return Color.FromArgb( n, V, m );
			case 2: return Color.FromArgb( m, V, n );
			case 3: return Color.FromArgb( m, n, V );
			case 4: return Color.FromArgb( n, m, V );
			case 5: return Color.FromArgb( V, m, n );
			}
			return Color.Black;
		}
	}
}
