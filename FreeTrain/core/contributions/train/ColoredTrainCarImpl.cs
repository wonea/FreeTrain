using System;
using System.Drawing;
using System.Xml;
using freetrain.contributions.common;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.world;
using org.kohsuke.directdraw;

namespace freetrain.contributions.train
{
	// TODO: this code needs to be re-written by using ColorMapSprite

	/// <summary>
	/// Train car that uses the same picture but
	/// use different colors to differenciate.
	/// 
	/// Six colors are used.
	/// </summary>
	[Serializable]
	public class ColoredTrainCarImpl : TrainCarContribution
	{
		/// <summary> Picture of this train. </summary>
		private ColoredTrainPictureContribution _picture;

		/// <summary> Color mapping. </summary>
		private Color[] _colors;

		/// <summary> Sprites used to draw a car on a level ground. 8-way from dir=0 to 7 </summary>
		private Sprite[] levelSprites;

		/// <summary> Sprites used to draw a car on a slope. 4 way from dir=0,2,4, and 6 </summary>
		private Sprite[] slopeSprites;
		


		public ColoredTrainCarImpl( XmlElement e ) : base(e) {
			XmlElement colorMap = (XmlElement)XmlUtil.selectSingleNode(e,"colorMap");

			Color cb  = getColor( colorMap, "base" );
			Color cl1 = getColor( colorMap, "line1" );	// used to be "stripe"
			Color cl2 = getColor( colorMap, "line2" );	// used to be "line"
			Color cl3 = getColor( colorMap, "line3" );	//
			
			this._picture = ColoredTrainPictureContribution.get(colorMap.Attributes["picture"].Value);
			this.colors = new Color[]{cb,cl1,cl2,cl3};
		}

		protected ColoredTrainCarImpl( ColoredTrainPictureContribution _picture,
			Color cb, Color cl1, Color cl2, Color cl3, string id, int cap ) :base(id,cap) {
			
			this._picture = _picture;
			this.colors = new Color[]{cb,cl1,cl2,cl3};
		}
		
		/// <summary>
		/// Get four colors of the train.
		/// </summary>
		public Color[] colors {
			get {
				return new Color[]{_colors[0],_colors[2],_colors[4],_colors[6]};
			}
			set {
				_colors = new Color[]{
					value[0],
					reduce(value[0]),
					value[1],
					reduce(value[1]),
					value[2],
					reduce(value[2]),
					value[3],
					reduce(value[3]) };
				createSprites();
			}
		}

		/// <summary>
		/// Picture of this train.
		/// </summary>
		public ColoredTrainPictureContribution picture {
			get {
				return _picture;
			}
			set {
				_picture = value;
				createSprites();
			}
		}






		private static Color getColor( XmlElement e, string name ) {
			// TODO: better error handling
			string value = ((XmlAttribute)XmlUtil.selectSingleNode(e,'@'+name)).Value;
			string[] cmp = value.Split(',');
			return Color.FromArgb( int.Parse(cmp[0]), int.Parse(cmp[1]), int.Parse(cmp[2]) );
		}

		private static Color reduce( Color c ) {
			return Color.FromArgb( c.R*180/255, c.G*180/255, c.B*180/255 );
		}


		


		public override void draw( Surface display, Point pt, int angle ) {
			levelSprites[angle&7].draw( display, pt );
		}

		public override void drawSlope( Surface display, Point pt, Direction angle, bool isClimbing ) {
			if(!isClimbing)		angle = angle.opposite;

			slopeSprites[ angle.index/2 ].draw( display, pt );
		}









		/// <summary>
		/// Create sprites from a new picture and using the current color set.
		/// </summary>
		/// <param name="pic"></param>
		private void createSprites() {
			levelSprites = new Sprite[8];
			slopeSprites = new Sprite[4];

			SpriteFactory factory = new ColorMappedSpriteFactory(dayColorPallete,_colors);
			for( int i=0; i<8; i++ ) {
				Point sprOrigin = new Point( i*32, 0 );
				levelSprites[i] = factory.createSprite( picture.picture, new Point(0,0), sprOrigin, new Size(32,32) );
			}
			for( int i=0; i<4; i++ ) {
				Point sprOrigin = new Point( i*32, 32 );
				slopeSprites[i] = factory.createSprite( picture.picture, new Point(0,0), sprOrigin, new Size(32,32) );
			}
		}

		
		//
		//
		// Source color palette
		//
		//

		private static readonly Color[] dayColorPallete = new Color[]{
			Color.FromArgb(0,0,255),   Color.FromArgb(0,0,180),
			Color.FromArgb(0,255,0),   Color.FromArgb(0,180,0),
			Color.FromArgb(255,0,0),   Color.FromArgb(180,0,0),
			Color.FromArgb(255,255,0), Color.FromArgb(180,180,0)
		};
	}




	/// <summary>
	/// PictureContribution that holds a picture for color-map train.
	/// This class just serves as a marker, and all the functionalities are
	/// given by the base class.
	/// </summary>
	[Serializable]
	public class ColoredTrainPictureContribution : PictureContribution
	{
		public readonly string name;

		public ColoredTrainPictureContribution( XmlElement e ) : base(e) {
			this.name = XmlUtil.selectSingleNode(e,"name").InnerText;
		}

		public override string ToString() {
			return name;
		}

		/// <summary>
		/// Get the ColoredTrainPictureContribution of the given ID.
		/// </summary>
		public static ColoredTrainPictureContribution get( string id ) {
			return (ColoredTrainPictureContribution)
				PluginManager.theInstance.getContribution(id);
		}

		/// <summary>
		/// Get all contributions of this class.
		/// </summary>
		public static ColoredTrainPictureContribution[] list() {
			return (ColoredTrainPictureContribution[])
				PluginManager.theInstance.listContributions(typeof(ColoredTrainPictureContribution));
		}
	}

}
