using System;
using System.Drawing;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.framework.graphics
{
	/// <summary>
	/// DefaultSpriteLoaderContributionImpl の概要の説明です。
	/// </summary>
	public class DefaultSpriteLoaderContributionImpl : SpriteLoaderContribution
	{
		public DefaultSpriteLoaderContributionImpl( XmlElement e ) : base(e) {}

		public override Sprite load0D( XmlElement sprite ) {

			int h = int.Parse( sprite.Attributes["offset"].Value );

			XmlAttribute size = sprite.Attributes["size"];

			return SpriteFactory.getSpriteFactory(sprite).createSprite(
				getPicture(sprite),
				new Point(0,h),
				XmlUtil.parsePoint( XmlUtil.selectSingleNode(sprite,"@origin").InnerText ),
				size==null?new Size(32,32):XmlUtil.parseSize(size.Value) );
		}



		public override Sprite[,] load2D( XmlElement sprite, int X, int Y, int height ) {
			Picture picture = getPicture(sprite);
			SpriteFactory spriteFactory = SpriteFactory.getSpriteFactory(sprite);

			Sprite[,] sprites = new Sprite[X,Y];

			Point origin = XmlUtil.parsePoint( sprite.Attributes["origin"].Value );
			int h = height;
			XmlAttribute att = sprite.Attributes["offset"];
			if( att !=null)
				h = int.Parse( att.Value );
			int maxh = int.MaxValue;
			if( sprite.Attributes["height"]!=null )
				maxh = int.Parse( sprite.Attributes["height"].Value );

			for( int y=0; y<Y; y++ ) {
				for( int x=0; x<X; x++ ) {
					Point sprOrigin = new Point( (x+y)*16 + origin.X, origin.Y );
					Size sprSize = new Size(32, Math.Min( maxh, h+16+(y-x)*8 ) );
					
					if( sprSize.Height==0 )
						sprites[x,y] = NullSprite.theInstance;
					else
						sprites[x,y] = spriteFactory.createSprite(
							picture, new Point(0, h+(y-x)*8 ), sprOrigin, sprSize );
				}
			}

			return sprites;
		}

		public override Sprite[,,] load3D( XmlElement sprite, int X, int Y, int Z ) {
			Picture picture = getPicture(sprite);
			SpriteFactory spriteFactory = SpriteFactory.getSpriteFactory(sprite);

			Sprite[,,] sprites = new Sprite[X,Y,Z];

			Point origin = XmlUtil.parsePoint( sprite.Attributes["origin"].Value );
			int h = ((Z<<1)+(X-1))<<3; // calculate default offset
			XmlAttribute att = sprite.Attributes["offset"];
			if( att !=null)
                h = int.Parse( att.Value );

			// top-floor
			for( int y=0; y<Y; y++ ) {
				for( int x=0; x<X; x++ ) {
					Point sprOrigin = new Point(
						(x+y)*16 + origin.X, origin.Y+h-16*(Z-1)+(y-x)*8 );

					Size sprSize = new Size(32,16);
					Point voxelOrigin = sprOrigin;

					if(y==0 || x==X-1) {
						sprOrigin.Y -= 16;
						sprSize.Height += 16;
						if(y==0 && x==X-1) {// top of the "hat"
							;
						} else
						if(y==0 && Y>1) {// top-left edge
							sprSize.Width = 16;
						} else
						if(x==X-1 && X>1) {// top-right edge
							sprOrigin.X += 16;
							sprSize.Width -= 16;
						}
					}

					if( sprOrigin.Y<0 ) {
						sprSize.Height += sprOrigin.Y;
						sprOrigin.Y=0;
					}

					if( sprSize.Height==0 )
						sprites[x,y,Z-1] = NullSprite.theInstance;
					else
						sprites[x,y,Z-1] = spriteFactory.createSprite(
							picture,
							new Point(	voxelOrigin.X-sprOrigin.X,
										voxelOrigin.Y-sprOrigin.Y ),
							sprOrigin, sprSize );
				}
			}

			// bottom-front
			if(Z>1) {
				for( int y=0; y<Y; y++ ) {
					for( int x=0; x<X; x++ ) {
						Point voxelOrigin = new Point(
							(x+y)*16 + origin.X, origin.Y+h+(y-x)*8 );
						
						Point sprOrigin = voxelOrigin;
						sprOrigin.Y -= (Z-2)*16+8;
						Size sprSize;

						if( x==0 && y==Y-1 ) {// bottom
							sprSize = new Size( 32, 16*(Z-1)+8 );
						} else
						if( x==0 ) { // left edge
							sprSize = new Size( 16, 16*(Z-1)+8 );
						} else
						if( y==Y-1 ) {// right edge
							sprSize = new Size( 16, 16*(Z-1)+8 );
							sprOrigin.X += 16;
						} else
							continue;	// invisible

						sprites[x,y,0] = spriteFactory.createSprite(
							picture,
							new Point(	voxelOrigin.X-sprOrigin.X,
										voxelOrigin.Y-sprOrigin.Y ),
							sprOrigin, sprSize );
					}
				}
			}

			// fill-in invisible cells by NullSprite
			for( int z=0; z<Z; z++ )
				for( int y=0; y<Y; y++ )
					for( int x=0; x<X; x++ )
						if( sprites[x,y,z]==null )
							sprites[x,y,z]=NullSprite.theInstance;

			return sprites;
		}
	}
}
