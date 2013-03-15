using System;
using System.Drawing;
using System.Xml;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.world;
using org.kohsuke.directdraw;

namespace freetrain.contributions.train
{
	/// <summary>
	/// Symmetric train car.
	/// 
	/// This train car contribution uses the same graphics
	/// for N and S. Thus 8 graphics for level and 4 for
	/// slopes are necessary.
	/// </summary>
	[Serializable]
	public class SymTrainCarImpl : TrainCarContribution
	{
		public SymTrainCarImpl( XmlElement e ) : base(e) {

			levelSprites = new Sprite[8];
			slopeSprites = new Sprite[4];

			XmlElement sprite = (XmlElement)XmlUtil.selectSingleNode(e,"sprite");
			Picture picture = getPicture(sprite);
			SpriteFactory factory = SpriteFactory.getSpriteFactory(sprite);

			Point origin = XmlUtil.parsePoint( sprite.Attributes["origin"].Value );

			for( int i=0; i<8; i++ ) {
				Point sprOrigin = new Point( i*32 +origin.X, origin.Y );
				levelSprites[i] = factory.createSprite( picture, new Point(0,0), sprOrigin, new Size(32,32) );
			}
			for( int i=0; i<4; i++ ) {
				Point sprOrigin = new Point( i*32 +origin.X, 32+origin.Y );
				slopeSprites[i] = factory.createSprite( picture, new Point(0,0), sprOrigin, new Size(32,32) );
			}
		}

		/// <summary> Sprites used to draw a car on a level ground. 8-way from dir=0 to 7 </summary>
		private readonly Sprite[] levelSprites;

		/// <summary> Sprites used to draw a car on a slope. 4 way from dir=0,2,4, and 6 </summary>
		private readonly Sprite[] slopeSprites;


		public override void draw( Surface display, Point pt, int angle ) {
			levelSprites[angle&7].draw( display, pt );
		}

		public override void drawSlope( Surface display, Point pt, Direction angle, bool isClimbing ) {
			if(!isClimbing)		angle = angle.opposite;
			slopeSprites[ angle.index/2 ].draw( display, pt );
		}

	}
}
