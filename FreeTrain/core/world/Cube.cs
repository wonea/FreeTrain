using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using freetrain.util;
using freetrain.world.terrain;

namespace freetrain.world
{
	/// <summary>
	/// Cubic space in the world.
	/// </summary>
	[Serializable]
	public struct Cube
	{
		/// <summary>
		/// The north-western bottom corner of the cube.
		/// The location of the voxel that has the smallest (x,y,z)
		/// value in the cube.
		/// </summary>
		public Location corner;

		/// <summary>
		/// Size of the cube.
		/// </summary>
		public int sx,sy,sz;

		public Cube( int x, int y, int z, int _sx, int _sy, int _sz )
			: this( new Location(x,y,z), _sx, _sy, _sz ) {}
		public Cube( Location _corner, int _sx, int _sy, int _sz ) {
			this.corner = _corner;
			this.sx=_sx; this.sy=_sy; this.sz=_sz;
		}
		public Cube( Location _corner, SIZE sz, int z )
			: this( _corner, sz.x, sz.y, z ) {}
			
		#region factory methods
		public static Cube createExclusive( Location loc, Distance d ) {
			return createExclusive( loc, loc+d );
		}

		/// <summary>
		/// Create a cube represented by two locations [loc1,loc2)
		/// The voxel pointed by loc1 is inside the cube but that by loc2
		/// is not. (Hence the name "exclusive")
		/// </summary>
		public static Cube createExclusive( Location loc1, Location loc2 ) {
			int x,y,z;

			if( loc1.x <= loc2.x )	x = loc1.x;
			else					x = loc2.x+1;
			
			if( loc1.y <= loc2.y )	y = loc1.y;
			else					y = loc2.y+1;
			
			if( loc1.z <= loc2.z )	z = loc1.z;
			else					z = loc2.z+1;

			return new Cube(x,y,z,
				Math.Abs(loc2.x-loc1.x),
				Math.Abs(loc2.y-loc1.y),
				Math.Abs(loc2.z-loc1.z) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="loc1"></param>
		/// <param name="loc2"></param>
		/// <returns></returns>
		public static Cube createInclusive( Location loc1, Location loc2 ) {
			Debug.Assert( loc1!=Location.UNPLACED && loc2!=Location.UNPLACED );

			return new Cube(
				Math.Min(loc1.x,loc2.x),
				Math.Min(loc1.y,loc2.y),
				Math.Min(loc1.z,loc2.z),

				Math.Abs(loc2.x-loc1.x)+1,
				Math.Abs(loc2.y-loc1.y)+1,
				Math.Abs(loc2.z-loc1.z)+1 );
		}
		#endregion

		public int x1 { get { return corner.x; } }
		public int y1 { get { return corner.y; } }
		public int z1 { get { return corner.z; } }
		public int x2 { get { return corner.x+sx; } }
		public int y2 { get { return corner.y+sy; } }
		public int z2 { get { return corner.z+sz; } }
		
		public Distance size { get { return new Distance(sx,sy,sz); } }
		
		public int volume { get { return sx*sy*sz; } }
		
		/// <summary>
		/// Return true if this cube is on the ground.
		/// This property can be used to check if a structure can be built
		/// in this cube.
		/// </summary>
		public bool isOnGround {
			get {
				int mx = x2;
				int my = y2;
				for( int x=x1; x<mx; x++ ) {
					for( int y=y1; y<my; y++ ) {
						if( World.world.getGroundLevel(x,y)!=z1 )
							return false;
						if(World.world[x,y,z1] is MountainVoxel)
							return false;
					}
				}
				return true;
			}
		}

		
		/// <summary>
		/// Checks if this cube contains the given location.
		/// </summary>
		public bool contains( Location loc ) {
			return corner.x<=loc.x && loc.x<corner.x+sx
				&& corner.y<=loc.y && loc.y<corner.y+sy
				&& corner.z<=loc.z && loc.z<corner.z+sz;
		}


		/// <summary>
		/// Computes the rectangle in the (A,B) axis that completely contains
		/// all the voxels in this cube.
		/// </summary>
		public Rectangle boundingABRect {
			get {
				// calculate the correct top left corner.
				int a1 = World.world.fromXYZToAB( corner ).X;
				int b1 = World.world.fromXYZToAB( x2-1, y1, z2-1 ).Y-16;

				int xyDiff = sx+sy;

				int width = xyDiff*16;
				int height= (xyDiff+sz*2)*8;

				return new Rectangle( a1, b1, width, height );
			}
		}

		/// <summary>
		/// Lists up all the entities whose voxels intersect with this cube.
		/// </summary>
		public Entity[] getEntities() {
			int mx = x2;
			int my = y2;
			int mz = z2;

			Set r = new Set();

			for( int x=corner.x; x<mx; x++ ) {
				for( int y=corner.y; y<my; y++ ) {
					for( int z=corner.z; z<mz; z++ ) {
						Voxel v = World.world[x,y,z];
						if(v!=null)		r.add(v.entity);
					}
				}
			}

			return (Entity[])r.toArray(typeof(Entity));
		}
		
		/// <summary>
		/// Enumerates all the voxels inside a cube.
		/// </summary>
		public ICollection voxels {
			get {
				ArrayList a = new ArrayList(this.volume);
				for( int x=0; x<sx; x++ ) {
					for( int y=0; y<sy; y++ ) {
						for( int z=0; z<sz; z++ ) {
							Voxel v = World.world[ corner.x+x, corner.y+y, corner.z+z ];
							if(v!=null)		a.Add(v);
						}
					}
				}
				return a;
			}
		}

//		/// <summary>
//		/// Computes the sume of entity values in this cube.
//		/// </summary>
//		public int getEntityValueSum() {
//			int r=0;
//			foreach( Entity e in getEntities() )
//				r += e.entityValue;
//			return r;
//		}

		public override int GetHashCode() {
			return corner.GetHashCode() ^ size.GetHashCode();
		}
		public override bool Equals( object o ) {
			if(!(o is Cube))	return false;
			Cube rhs = (Cube)o;
			return this.corner==rhs.corner
				&& this.size==rhs.size;
		}
	}
}
