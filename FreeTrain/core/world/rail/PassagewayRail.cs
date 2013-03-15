using System;
using System.Diagnostics;
using System.Drawing;
using freetrain.framework;
using freetrain.framework.graphics;

namespace freetrain.world.rail
{
	/// <summary>
	/// Rail road with a raised passageway
	/// </summary>
	[Serializable]
	public class PassagewayRail : SpecialPurposeRailRoad
	{
		public PassagewayRail( TrafficVoxel v, Direction dir ) : base(v,dir) {
			Debug.Assert( dir.isSharp );
		}

		public override void drawAfter( DrawContext display, Point pt ) {
			getFloatingSprite(dir1).draw( display.surface, pt );
		}




		/// <summary>
		/// sprites for passageways.
		/// 0 : single-width north platform and bridge connecting to east
		/// 1:  double-width north platform
		/// 2:  double-width north platform and bridge connecting to east
		/// 
		/// 3-5: east
		/// 
		/// 6-8: south
		/// 
		/// 9-11: west
		/// 
		/// 12: E-W bridge
		/// 13: N-S bridge
		/// </summary>
		private static readonly Sprite[] sprites;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d">direction of the rail road</param>
		/// <param name="connected">true if a passageway is bridged</param>
		/// <param name="doubleWidth">true if a platform is double-width</param>
		/// <returns></returns>
		public static Sprite getSprite( Direction d, bool bridged, bool doubleWidth ) {
			return sprites[(d.index/2)*3+(doubleWidth?(bridged?2:1):0)];	// TODO
		}
		public static Sprite getFloatingSprite( Direction d ) {
			if( d.isParallelToX )	return sprites[12];
			else					return sprites[13];
		}



		/// <summary>
		/// Sprites for stair cases.
		/// 
		/// 8 spirtes for one direction.
		/// (upward --- stairs go upward to the direction of the platform)
		/// 0: single-width, no-roof
		/// 1: single-width, roof
		/// 2: double-width, no-roof
		/// 3: double-width, roof
		/// (downward -- stairs go downward to the direction of the platform)
		/// 4,5,6,7
		/// </summary>
		private static readonly Sprite[] stairSprites;

		public static Sprite getStairSprite( Direction d, bool upward, bool hasRoof, bool doubleWidth ) {
			return stairSprites[d.index*4 | (upward?0:4) | (doubleWidth?2:0) | (hasRoof?1:0)];	// TODO
		}

		static PassagewayRail() {
			Point offset = new Point(0,16);
			Size sz = new Size(32,32);

			sprites = new Sprite[14];
			Picture bmp = PictureManager.get("{3197A63A-89DC-4237-8C9B-045F41F31CDB}");
			for( int i=0; i<4; i++ ) {
				sprites[i*3  ] = new SimpleSprite( bmp, offset, new Point(i*32, 8), sz );
				sprites[i*3+1] = new SimpleSprite( bmp, offset, new Point((i*2+1)*32,40), sz );
				sprites[i*3+2] = new SimpleSprite( bmp, offset, new Point((i*2+2)*32,40), sz );
			}
			sprites[12] = new SimpleSprite( bmp, offset, new Point(4*32, 8), sz );
			sprites[13] = new SimpleSprite( bmp, offset, new Point(5*32, 8), sz );

			stairSprites = new Sprite[32];

			// NORTH
			stairSprites[ 0] = new SimpleSprite( bmp, new Point(+6,16), new Point( 16-6, 80), sz ); 
			stairSprites[ 1] = new SimpleSprite( bmp, new Point(+6,16), new Point( 48-6, 80), sz );
			stairSprites[ 2] = stairSprites[ 0];	// can reuse the same sprites
			stairSprites[ 3] = stairSprites[ 1];

			stairSprites[ 4] = new SimpleSprite( bmp, offset, new Point(  0,120), sz ); 
			stairSprites[ 5] = new SimpleSprite( bmp, offset, new Point( 32,120), sz );
			stairSprites[ 6] = stairSprites[ 4];	// can reuse the same sprites
			stairSprites[ 7] = stairSprites[ 5];

			// EAST
			stairSprites[ 8] = new SimpleSprite( bmp, new Point(0,20), new Point( 80, 80-4), new Size(32,36) ); 
			stairSprites[ 9] = new SimpleSprite( bmp, new Point(0,20), new Point(112, 80-4), new Size(32,36) );
			stairSprites[10] = new SimpleSprite( bmp, new Point(0,20), new Point(192,  8-4), new Size(32,36) );
			stairSprites[11] = new SimpleSprite( bmp, new Point(0,20), new Point(224,  8-4), new Size(32,36) );

			stairSprites[12] = new SimpleSprite( bmp, new Point(+6,16), new Point( 96-6,120), sz ); 
			stairSprites[13] = new SimpleSprite( bmp, new Point(+6,16), new Point(128-6,120), sz );
			stairSprites[14] = stairSprites[12];	// can reuse the same sprites
			stairSprites[15] = stairSprites[13];

			// SOUTH
			stairSprites[16] = new SimpleSprite( bmp, new Point(-6,16), new Point(160+6,120), sz ); 
			stairSprites[17] = new SimpleSprite( bmp, new Point(-6,16), new Point(192+6,120), sz );
			stairSprites[18] = stairSprites[16];	// can reuse the same sprites
			stairSprites[19] = stairSprites[17];

			stairSprites[20] = new SimpleSprite( bmp, new Point(0,20), new Point(176, 80-4), new Size(32,36)  ); 
			stairSprites[21] = new SimpleSprite( bmp, new Point(0,20), new Point(208, 80-4), new Size(32,36)  );
			stairSprites[22] = new SimpleSprite( bmp, new Point(0,20), new Point(256,  8-4), new Size(32,36) );
			stairSprites[23] = new SimpleSprite( bmp, new Point(0,20), new Point(388,  8-4), new Size(32,36) );

			// WEST
			stairSprites[24] = new SimpleSprite( bmp, offset, new Point(256,120), sz ); 
			stairSprites[25] = new SimpleSprite( bmp, offset, new Point(288,120), sz );
			stairSprites[26] = stairSprites[24];	// can reuse the same sprites
			stairSprites[27] = stairSprites[25];

			stairSprites[28] = new SimpleSprite( bmp, new Point(-6,16), new Point(240+6, 80), sz ); 
			stairSprites[29] = new SimpleSprite( bmp, new Point(-6,16), new Point(272+6, 80), sz );
			stairSprites[30] = stairSprites[28];
			stairSprites[31] = stairSprites[29];
		}
	}
}
