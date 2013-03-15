using System;
using System.Drawing;
using System.Xml;
using freetrain.framework;
using freetrain.framework.plugin;
using freetrain.framework.graphics;
using freetrain.world;
using org.kohsuke.directdraw;

namespace freetrain.contributions.train
{
	/// <summary>
	/// Assymetric train car.
	/// 
	/// This train car is usually used for the head car,
	/// where a picture to go N and that to go S are different.
	/// This type requires 16 graphics on the level ground
	/// and 8 graphics for slopes.
	/// </summary>
	[Serializable]
	public class AsymTrainCarImpl : TrainCarContribution
	{
		public AsymTrainCarImpl( XmlElement e ) : base(e) {

			levelSprites = new Sprite[16];
			slopeSprites = new Sprite[8];

			XmlElement sprite = (XmlElement)XmlUtil.selectSingleNode(e,"sprite");
			Picture picture = getPicture(sprite);
			SpriteFactory factory = SpriteFactory.getSpriteFactory(sprite);

			Point origin = XmlUtil.parsePoint( sprite.Attributes["origin"].Value );

			for( int i=0; i<16; i++ ) {
				Point sprOrigin = new Point( (i%8)*32 +origin.X, (i/8)*32 +origin.Y );
				levelSprites[i] = factory.createSprite( picture, new Point(0,0), sprOrigin, new Size(32,32) );
			}
			for( int i=0; i<8; i++ ) {
				Point sprOrigin = new Point( i*32 +origin.X, 64+origin.Y );
				slopeSprites[i] = factory.createSprite( picture, new Point(0,0), sprOrigin, new Size(32,32) );
			}
		}

		/// <summary> Sprites used to draw a car on a level ground. 8-way from dir=0 to 7 </summary>
		private readonly Sprite[] levelSprites;

		/// <summary> Sprites used to draw a car on a slope. 4 way from dir=0,2,4, and 6 </summary>
		private readonly Sprite[] slopeSprites;


		public override void draw( Surface display, Point pt, int angle ) {
			levelSprites[angle].draw( display, pt );
		}

		public override void drawSlope( Surface display, Point pt, Direction angle, bool isClimbing ) {
			slopeSprites[ angle.index + (isClimbing?0:1) ].draw( display, pt );
		}

	}
}
