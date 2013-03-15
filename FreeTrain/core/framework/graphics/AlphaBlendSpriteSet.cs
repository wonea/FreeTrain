using System;
using System.Drawing;
using freetrain.framework;
using freetrain.world;
using org.kohsuke.directdraw;

namespace freetrain.framework.graphics
{
	/// <summary>
	/// Builds a set of sprites for alpha-blending blit
	/// from a set of ordinary sprites.
	/// 
	/// This object keeps a reference to DirectDraw surfaces,
	/// so it needs to be disposed.
	/// 
	/// The crux is that we need to avoid the overlap of sprites.
	/// </summary>
	public class AlphaBlendSpriteSet : IDisposable
	{
		/// <summary>
		/// DirectDraw surface.
		/// </summary>
		private Surface[,,] surfaces;

		/// <summary>
		/// Sprites built for alpha-blending.
		/// </summary>
		public readonly Sprite[,,] sprites;

		public readonly Distance size;

		public AlphaBlendSpriteSet( Sprite[,,] src ) {
			int X = src.GetLength(0);
			int Y = src.GetLength(1);
			int Z = src.GetLength(2);
			surfaces = new Surface[X,Y,Z];
			sprites = new Sprite[X,Y,Z];
			size = new Distance(X,Y,Z);

			for( int z=0; z<Z; z++ ) {
				for( int y=0; y<Y; y++ ) {
					for( int x=0; x<X; x++ ) {
						Size sz = src[x,y,z].size;
						if( sz.Height<=0 || sz.Width<=0 ) {
							sprites[x,y,z] = NullSprite.theInstance;
							continue;	// this voxel is invisible
						}

						Surface surface = ResourceUtil.directDraw.createOffscreenSurface(sz);
						surfaces[x,y,z] = surface;
						surface.fill( Color.Magenta );
						surface.sourceColorKey = Color.Magenta;

						Point offset = src[x,y,z].offset;

						// first copy the sprite
						src[x,y,z].draw( surface, offset );

						// then mask areas that will be hidden by other sprites
						for( int xx=0; xx<=x; xx++ ) {
							for( int yy=y; yy<Y; yy++ ) {
								for( int zz=z; zz<Z; zz++ ) {
									if(xx==x && yy==y && zz==z )
										continue;	// skip this sprite

									Point pt = offset;
									pt.X += 16*( (xx-x)+(yy-y) );
									pt.Y +=  8*(-(xx-x)+(yy-y)-(zz-z)*2);
									src[xx,yy,zz].drawShape( surface, pt, Color.Magenta );
								}
							}
						}

						sprites[x,y,z] = new DirectSprite( surface, offset, new Point(0,0), surface.size );
					}
				}
			}
		}

		public void Dispose() {
			foreach( Surface s in surfaces )
				if(s!=null)		s.Dispose();
		}

		public Sprite getSprite( Distance d ) {
			return sprites[d.x,d.y,d.z];
		}
	}
}
