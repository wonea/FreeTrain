using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.world;
using freetrain.world.terrain;

namespace freetrain.tools.terrainloader
{
	/// <summary>
	/// Loads a terrain from a disindices bitmap.
	/// </summary>
	public class TerrainLoader
	{
		public static World loadWorld( Bitmap bitmap, Size sz, int height, int waterLevel ) {
			World w = new World( new Distance( sz.Width, sz.Height, height ), waterLevel );
			new TerrainLoader( bitmap, w ).load();
			return w;
		}

		private TerrainLoader( Bitmap bitmap, World world ) {
			this.bitmap = bitmap;
			this.world = world;
		}

		private readonly Bitmap bitmap;
		private readonly World world;


		private void load() {
			using( new util.LongTask() ) {
				displace();
				interpolate();
				byte[,] z = smooth();
				MainWindow.mainWindow.setWorld(world);
				texture(z);
			}
		}

		/// <summary>
		/// Set the height of each voxel by looking the brightness of the
		/// corresponding pixel.
		/// </summary>
		private void displace() {
			for( int v=0; v<world.size.y; v++ ) {
				for( int h=0; h<world.size.x; h++ ) {
					Location loc = world.toXYZ( h,v,0 );

					// obtain the height
					int z = (int)Math.Min( world.size.z-1, world.size.z*getDepth(h,v) );

					if( world.waterLevel < z ) {
						for( int i=world.waterLevel; i<z; i++ )
							world.raiseGround(loc);
					}
					if( world.waterLevel > z ) {
						for( int i=z; i<world.waterLevel; i++ )
							world.lowerGround(loc);
					}
				}
			}
		}

		/// <summary>
		/// Computes the interpolated color from the input bitmap
		/// </summary>
		/// <returns>value in the range [0,1]</returns>
		private float getDepth( float h, int v ) {
			// compute the pixel position to look at
			float x = ((float)h+(v%2)*0.5f)*bitmap.Width/world.size.x;
			float y = ((float)v)*bitmap.Height/world.size.y;


			int x1 = Math.Min( (int)Math.Floor(x),   bitmap.Width-1 );
			int x2 = Math.Min( (int)Math.Ceiling(x), bitmap.Width-1 );
			float dx = x-x1;

			int y1 = Math.Min( (int)Math.Floor(y),   bitmap.Height-1 );
			int y2 = Math.Min( (int)Math.Ceiling(y), bitmap.Height-1 );
			float dy = y-y1;

			
			float h0 = bitmap.GetPixel(x1,y1).GetBrightness();
			float h1 = bitmap.GetPixel(x2,y1).GetBrightness();
			float h2 = bitmap.GetPixel(x1,y2).GetBrightness();
			float h3 = bitmap.GetPixel(x2,y2).GetBrightness();

			float h01 = h0*(1.0f-dx)+h1*dx;
			float h23 = h2*(1.0f-dx)+h3*dx;

			float r = h01*(1.0f-dy) + h23*dy;

			return r;
		}


		/// <summary>
		/// Remove "crack"s (two adjacent voxels
		/// with the height difference of more than one.)
		/// 
		/// This is done by lowering neighbor voxels.
		/// </summary>
		private void interpolate() {
			// repeat while there is a change
			while( interpolate1() )
				;
		}

		/// <summary>
		/// Performs one step of interpolation.
		/// </summary>
		/// <returns>true if the world has changed as a result.</returns>
		private bool interpolate1() {
			bool changed = false;
			Location[] locs = new Location[4];

			for( int v=-1; v<world.size.y; v++ ) {
				for( int h=-1; h<world.size.x; h++ ) {
					// compute the location of four neighboring voxels
					getAdjacent4Locations( h, v, locs );

					// find the lowest voxel
					int z = 9999;
					for( int i=0; i<4; i++ ) {
						if( world.isInsideWorld(locs[i]) ) {
							z = Math.Min( z, world.getGroundLevel(locs[i]) );
						}
					}

					// then make sure that all four voxels are within +1 of this voxel
					for( int i=0; i<4; i++ ) {
						if( world.isInsideWorld(locs[i]) ) {
							int zz = world.getGroundLevel(locs[i]);
							for( int j=z+1; j<zz; j++ ) {
								changed = true;
								world.lowerGround(locs[i]);
							}
						}
					}
				}
			}

			return changed;
		}

		/// <summary>
		/// Sets the locations of four adjacent locations into the given array.
		/// </summary>
		private void getAdjacent4Locations( int h, int v, Location[] locs ) {
			getAdjacent4Locations( world.toXYZ(h,v,0), locs );
		}

		private void getAdjacent4Locations( Location loc, Location[] locs ) {
			locs[0] = locs[1] = locs[2] = locs[3] = loc;
			locs[1].x++;
			locs[3].x++;
			locs[2].y++;
			locs[3].y++;
		}


		/// <summary>
		/// Smoothes the loaded terrain by adding gentle slope.
		/// </summary>
		/// <returns>
		///		smoothing heights. result[h,v]=z indicates that the south-east
		///		corner of the voxel(h,v) should be raised z/4.
		///	</returns>
		private byte[,] smooth() {
			Location[] locs = new Location[4];

			byte[,] smoothedHeights = new byte[ world.size.x, world.size.y ];

			for( int v=0; v<world.size.y; v++ ) {
				for( int h=0; h<world.size.x; h++ ) {
					Location loc = world.toXYZ( h, v, 0 );

					int min2, max2;
					getMinMaxHeight( loc.x, loc.y, 2, 2, out min2, out max2 );
					if( min2!=max2 )
						continue;	// four adjacent voxels must be flat

					int min4, max4;
					getMinMaxHeight( loc.x-1, loc.y-1, 4, 4, out min4, out max4 );
					if( max4==max2 )
						continue;	// at least one of the surrounding voxel must be higher

					// obtain the height of the corner.
					int z = (int)Math.Min( world.size.z*4-1, world.size.z*4*getDepth(h+0.5f,v) );

					smoothedHeights[h,v] = (byte)(z%4);
				}
			}

			return smoothedHeights;
		}

		/// <summary>
		/// Computes the minimum and maximum height in the given region.
		/// </summary>
		private void getMinMaxHeight( int x0, int y0, int sx, int sy, out int min, out int max ) {
			sx += x0;
			sy += y0;
			min = int.MaxValue;
			max = int.MinValue;

			for( int y=y0; y<sy; y++ ) {
				for( int x=x0; x<sx; x++ ) {
					if( world.isOutsideWorld( new Location(x,y,0) ) )
						continue;

					int z = world.getGroundLevel(x,y);
					min = Math.Min( min, z );
					max = Math.Max( max, z );
				}
			}
		}

//		/// <summary>
//		/// Checks if the south-east corner of the given location is a flat corner.
//		/// </summary>
//		/// <param name="locs"></param>
//		/// <returns></returns>
//		private bool areSameHeight( Location loc ) {
//			Location[] locs = new Location[4];
//			getAdjacent4Locations(loc,locs);
//			return areSameHeight(locs);
//		}
//		private bool areSameHeight( int x, int y ) {
//			return areSameHeight( new Location(x,y,0) );
//		}
//
//		private bool areSameHeight( Location[] locs ) {
//			int dummy;
//			return areSameHeight( locs, out dummy );
//		}
//
//		/// <summary>
//		/// Checks if these four voxels are of the same height
//		/// </summary>
//		/// <returns>the minimum height of the four</returns>
//		private bool areSameHeight( Location[] locs, out int z ) {
//			z = -1;
//			for( int i=0; i<4; i++ ) {
//				if( world.isOutsideWorld(locs[i]) )
//					continue;
//
//				int zz = world.getGroundLevel(locs[i]);
//				if( z==-1 || z==zz ) {
//					z = zz;
//					continue;	// OK
//				}
//
//				return false;	// not the same height
//			}
//
//			return true;
//		}



		/// <summary>
		/// Adds MountainVoxels to the world as a wrap up.
		/// </summary>
		private void texture( byte[,] z ) {
			int[] heights = new int[4];

			for( int v=0; v<world.size.y; v++ ) {
				for( int h=0; h<world.size.x; h++ ) {
					Location loc = world.toXYZ( h,v, world.getGroundLevelFromHV(h,v) );

					heights[0] = getCornerHeight( loc.x  , loc.y-1, z );
					heights[1] = getCornerHeight( loc.x  , loc.y  , z );
					heights[2] = getCornerHeight( loc.x-1, loc.y  , z );
					heights[3] = getCornerHeight( loc.x-1, loc.y-1, z );
					if( heights[0]==heights[1] && heights[1]==heights[2] && heights[2]==heights[3]
					&&	heights[0]==4*loc.z )
						continue;	// flat

					new MountainVoxel( loc,
						(byte)(heights[0]-loc.z*4),
						(byte)(heights[1]-loc.z*4),
						(byte)(heights[2]-loc.z*4),
						(byte)(heights[3]-loc.z*4) );
				}
			}
		}

		/// <summary>
		/// Gets the height (4*z+offset) of the south-east corner
		/// of the given location
		/// </summary>
		private int getCornerHeight( int x, int y, byte[,] smoothHeights ) {
			int min,max;
			getMinMaxHeight( x,y, 2,2, out min, out max );

			Debug.Assert( max-min <= 1 );

			if(min!=max)
				return max*4;	// different height

			// they are of the same height
			int h,v;
			world.toHV( x,y, out h, out v );
			try {
				return max*4+smoothHeights[h,v];
			} catch( IndexOutOfRangeException ) {
				// sloppy hanling of out of the world voxels
				return max*4;
			}
		}
	}
}
