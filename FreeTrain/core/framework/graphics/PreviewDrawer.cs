using System;
using System.Drawing;
using freetrain.framework;
using freetrain.world;
using org.kohsuke.directdraw;

namespace freetrain.framework.graphics
{
	/// <summary>
	/// Utility class that draws a preview image programatically.
	/// 
	/// This helper class it typically useful to prepare an image
	/// for a controller dialog box.
	/// </summary>
	public class PreviewDrawer : IDisposable
	{
		public readonly Surface surface;

		/// <summary>
		/// pixelSize of the canvas.
		/// </summary>
		private readonly Size pixelSize;
		
		/// <summary>
		/// The point in the surface of (X,Y)=(0,0).
		/// </summary>
		private readonly Point ptOrigin = new Point();

		public PreviewDrawer( Size pixelSize, Distance size )
			: this( pixelSize, new Size(size.x,size.y), size.z ) {}

		/// <summary>
		/// Creates an empty canvas with the given pixel size.
		/// </summary>
		/// <param name="pixelSize">Pixel size of the canvas</param>
		/// <param name="objSize">
		///		Chip size of the object that we'd like to draw.
		///		All the successive method calls will use this size as offset.
		/// </param>
		public PreviewDrawer( Size pixelSize, Size objSize, int height ) {
			surface = ResourceUtil.directDraw.createOffscreenSurface(pixelSize);
			this.pixelSize = pixelSize;

			int P = (objSize.Width + objSize.Height)*8;

			ptOrigin.X = (pixelSize.Width-P*2)/2;
			ptOrigin.Y = (pixelSize.Height-P-height*16)/2 /*top*/ + (8*objSize.Width+height*16) -8;

			clear();
		}

		public void Dispose() {
			surface.Dispose();
		}

		/// <summary> Clears the canvas by tiling empty chips. </summary>
		public void clear() {
			Sprite empty = ResourceUtil.getGroundChip(World.world);
			for( int y=(ptOrigin.Y%8)-16; y<pixelSize.Height; y+=8 ) {
				int x=(ptOrigin.X%32)-64;
				if( (((y-ptOrigin.Y)/8)%2)!=0 )	x+= 16;
				
				for( ; x<pixelSize.Width; x+= 32 )
					empty.draw( surface, new Point(x,y) );
			}
		}

		public Point getPoint( int offsetX, int offsetY ) {
			return getPoint( offsetX, offsetY, 0 );
		}

		public Point getPoint( int offsetX, int offsetY, int offsetZ ) {
			Point o = ptOrigin;
			o.X += ( offsetX+offsetY)*16;
			o.Y += (-offsetX+offsetY)*8;
			o.Y -= offsetZ*16;
			return o;
		}

		public void draw( Sprite sprite, int offsetX, int offsetY ) {
			sprite.draw( surface, getPoint(offsetX,offsetY) );
		}


		public void drawCenter( Sprite[,] sprites ) {
			draw( sprites, 0, 0 );
		}

		public void draw( Sprite[,] sprites, int offsetX, int offsetY ) {
			draw( sprites, offsetX, offsetY, 0 );
		}
		public void draw( Sprite[,] sprites, int offsetX, int offsetY, int offsetZ ) {
			int X = sprites.GetLength(0);
			int Y = sprites.GetLength(1);

			Point o = getPoint(offsetX,offsetY,offsetZ);

			for( int y=0; y<Y; y++ ) {
				for( int x=0; x<X; x++ ) {
					Point pt = o;
					pt.X += ( x+y)*16;
					pt.Y += (-x+y)*8;

					sprites[x,y].draw( surface, pt );
				}
			}
		}


		public void drawCenter( Sprite[,,] sprites ) {
			draw( sprites, 0, 0 );
		}

		public void draw( Sprite[,,] sprites, int offsetX, int offsetY ) {
			draw( sprites, offsetX, offsetY, 0 );
		}
		public void draw( Sprite[,,] sprites, int offsetX, int offsetY, int offsetZ ) {
			int X = sprites.GetLength(0);
			int Y = sprites.GetLength(1);
			int Z = sprites.GetLength(2);

			Point o = getPoint(offsetX,offsetY,offsetZ);

			for( int z=0; z<Z; z++ ) {
				for( int y=0; y<Y; y++ ) {
					for( int x=0; x<X; x++ ) {
						Point pt = o;
						pt.X += ( x+y)*16;
						pt.Y += (-x+y)*8  -z*16;

						sprites[x,y,z].draw( surface, pt );
					}
				}
			}
		}

//		/// <summary>
//		/// Gets the pixel location from (h,v) coordinate.
//		/// </summary>
//		public Point getLocation( int h, int v ) {
//			Point pt = new Point(h*32-16,v*8-8);
//			if((v%2)==1)	pt.X += 16;
//
//			return pt;
//		}
//
//		public Point getCenterChip( Size sz ) {
//			// TODO: think about this equation more
//			return getLocation( (size.Width-sz.x-sz.y)/2, size.Height/2 + (sz.x-sz.y) );
//		}




		/// <summary>
		/// Makes the bitmap of the current picture.
		/// 
		/// The caller needs to dispose the bitmap.
		/// </summary>
		public Bitmap createBitmap() {
			return surface.createBitmap();
		}
	}
}
