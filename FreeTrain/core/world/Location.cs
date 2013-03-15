using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using org.kohsuke.directdraw;
using freetrain.framework;

namespace freetrain.world
{

	public class WorldLocator{
		protected World w;
		protected Location l;
		public WorldLocator(World world, int _x, int _y, int _z){
			w = world;
			l = new Location(_x, _y, _z);
		}
		public WorldLocator(World world, Location loc){
			w = world;
			l = loc;
		}
		public Location location{ get{return l;}}
		public World world { get{ return w; }}
	}

	/// <summary>
	/// 世界での位置を示すオブジェクト
	/// </summary>
	[Serializable]
	public struct Location
	{
		public Location( int _x, int _y, int _z ) { x=_x; y=_y; z=_z; }

		public int x;
		public int y;
		public int z;

		/// <summary>
		/// Gets the location one step closer to the given location.
		/// </summary>
		public Location toward( Location to ) {
			return this + getDirectionTo(to);
		}
		public Direction getDirectionTo( Location to ) {
			Debug.Assert(z==to.z);
			Debug.Assert(this!=to);
			return Direction.get( Math.Sign(to.x-x), Math.Sign(to.y-y) );
		}
		public int getDistanceTo( Location to ) {
			return (int)Math.Sqrt( (to.x-x)*(to.x-x) + (to.y-y)*(to.y-y) + (to.z-z)*(to.z-z) );
		}

		public World world { get{ return World.world; }}

		/// <summary>
		/// Computes the distance (in Euclidean sense)
		/// </summary>
		public int distanceTo( Location loc ) {
			int dx = loc.x-x;
			int dy = loc.y-y;
			int dz = loc.z-z;
			return (int)Math.Sqrt( dx*dx + dy*dy + dz*dz );
		}

		/// <summary>
		/// Returns true if this location is in the cube projected
		/// by the two locations.
		/// </summary>
		public bool inBetween( Location lhs, Location rhs ) {
			return inBetween( x, lhs.x, rhs.x )
				&& inBetween( y, lhs.y, rhs.y )
				&& inBetween( z, lhs.z, rhs.z );
		}
		private static bool inBetween( int v, int l, int r ) {
			return (l<=v && v<=r) || (l>=v && v>=r);
		}


		public static Location max( Location lhs, Location rhs ) {
			if( lhs.x<=rhs.x && lhs.y<=rhs.y && lhs.z<=rhs.z )
				return rhs;
			if( rhs.x<=lhs.x && rhs.y<=lhs.y && rhs.z<=lhs.z )
				return lhs;
			Debug.Fail("incorrect use of Location.max");
			return UNPLACED;
		}
		public static Location min( Location lhs, Location rhs ) {
			if( lhs.x<=rhs.x && lhs.y<=rhs.y && lhs.z<=rhs.z )
				return lhs;
			if( rhs.x<=lhs.x && rhs.y<=lhs.y && rhs.z<=lhs.z )
				return rhs;
			Debug.Fail("incorrect use of Location.min");
			return UNPLACED;
		}


		/// <summary>
		/// Aligns this location to the specified location so that
		/// two locations will either have same X value or Y value.
		/// </summary>
		public Location align4To( Location anchor ) {
			Debug.Assert( this.z==anchor.z );

			int X = Math.Abs( this.x-anchor.x );
			int Y = Math.Abs( this.y-anchor.y );

			if( X>=Y )	return new Location( this.x, anchor.y, this.z );
			else		return new Location( anchor.x, this.y, this.z );
		}

		public Location align8To( Location anchor ) {
			int X = Math.Abs(this.x-anchor.x);
			int Y = Math.Abs(this.y-anchor.y);

			if( Y/2 > X )	return new Location( anchor.x, this.y, this.z );
			if( X/2 > Y )	return new Location( this.x, anchor.y, this.z );
			
			return new Location( this.x, anchor.y+X*Math.Sign(this.y-anchor.y), this.z );
		}




		public static Location operator + ( Location loc, Direction dir ) {
			return new Location( loc.x+dir.offsetX, loc.y+dir.offsetY, loc.z );
		}
		public static Location operator - ( Location loc, Direction dir ) {
			return new Location( loc.x-dir.offsetX, loc.y-dir.offsetY, loc.z );
		}
		public static Location operator + ( Location loc, Distance dis ) {
			return new Location( loc.x+dis.x, loc.y+dis.y, loc.z+dis.z );
		}
		public static Location operator - ( Location loc, Distance dis ) {
			return new Location( loc.x-dis.x, loc.y-dis.y, loc.z-dis.z );
		}

		public static Distance operator - ( Location lhs, Location rhs ) {
			return new Distance( lhs.x-rhs.x, lhs.y-rhs.y, lhs.z-rhs.z );
		}

		public static bool operator != (Location loc1, Location loc2 ) {
			return !(loc1==loc2);
		}
		public static bool operator == (Location loc1, Location loc2 ) {
			return loc1.Equals(loc2);
		}
		public override bool Equals( object o ) {
			if(!(o is Location))	return false;
			Location r = (Location)o;
			return x==r.x && y==r.y && z==r.z;
		}
		public override int GetHashCode() {
			return x^y^z;
		}

		public override string ToString() {
			return x.ToString()+','+y+','+z;
		}

		/// <summary>
		/// 位置を持たないことを示す特殊な値
		/// </summary>
		public static readonly Location UNPLACED = new Location( int.MinValue, int.MinValue, int.MinValue );
	}

	/// <summary>
	/// Location同士の差分
	/// </summary>
	[Serializable]
	public struct Distance
	{
		public Distance( int _x, int _y, int _z ) { x=_x; y=_y; z=_z; }
		public Distance( SIZE sz, int _z ) { x=sz.x; y=sz.y; z=_z; }

		public readonly int x;
		public readonly int y;
		public readonly int z;

		public int volume { get { return x*y*z; } }

		public override bool Equals( object o ) {
			if(!(o is Distance))	return false;
			Distance r = (Distance)o;
			return x==r.x && y==r.y && z==r.z;
		}
		public override int GetHashCode() {
			return x^y^z;
		}
		public static bool operator != (Distance d1, Distance d2 ) {
			return !(d1==d2);
		}
		public static bool operator == (Distance d1, Distance d2 ) {
			return d1.Equals(d2);
		}
		public static Distance operator / ( Distance lhs, int rhs ) {
			return new Distance( lhs.x/rhs, lhs.y/rhs, lhs.z/rhs );
		}
	}


}
