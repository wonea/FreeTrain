using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;

namespace nft.core.geometry
{
	/// <summary>
	/// location in the <code>District</code>
	/// Each coordinate value must within 2byte range.
	/// </summary>
	[Serializable]
	public struct Location
	{
		/// Each parameters are cast down to 'short' value (2 byte).
		public Location( int x, int y, int z ) { X=(short)x; Y=(short)y; Z=(short)z; }

		public short X;
		public short Y;
		public short Z;

		/// Convert each coordinate to integer which is smallest but not lesser than original. ///
		public static Location Ceiling(LocationF l)
		{
			return new Location((int)Math.Ceiling(l.X),(int)Math.Ceiling(l.Y),(int)Math.Ceiling(l.Z));
		}

		/// Convert each coordinate to integer which is largest but not greater than original. ///
		public static Location Floor(LocationF l)
		{
			return new Location((int)Math.Floor(l.X),(int)Math.Floor(l.Y),(int)Math.Floor(l.Z));
		}

//		/// <summary>
//		/// Gets the location one step closer to the given location.
//		/// </summary>
//		public Location Toward( Location to ) {
//			return this + GetDirectionTo(to);
//		}

		/// <summary>
		/// Get horizontal <code>Direction</code> toward specified <code>Location</code> 'to'.
		/// </summary>
		/// <param name="to"></param>
		/// <returns></returns>
		public Direction8 GetDirectionTo( Location to ) 
		{
			return dir_table[Math.Sign(to.X-X)+1,Math.Sign(to.Y-Y)+1];			
		}
		static private readonly Direction8[,] dir_table = new Direction8[3,3]
				  {
					  //	-1,-1				-1, 0			-1,+1
					{Direction8.NORTHWEST,Direction8.WEST,Direction8.SOUTHWEST},
					  //	 0,-1			 0, 0			 0,+1
					{Direction8.NORTH,Direction8.INVALID,Direction8.SOUTH},
					  //	+1,-1				+1, 0			+1,+1
					{Direction8.NORTHEAST,Direction8.EAST,Direction8.SOUTHEAST}
				  };

		public double GetDistanceTo( Location to ) {			
			return (to-this).Value;
		}

		public double GetPlaneDistanceTo( Location to ) 
		{			
			return (to-this).ValueInPlane;
		}

		public Point PlaneLocation { get{ return new Point(X,Y); } }

		/// <summary>
		/// Aligns this location to the specified location so that
		/// two locations will either have same X value or Y value.
		/// </summary>
		public Location AlignDirection4( Location anchor ) {
			int dx = Math.Abs( this.X-anchor.X );
			int dy = Math.Abs( this.Y-anchor.Y );

			if( dx>=dy )	return new Location( this.X, anchor.Y, this.Z );
			else		return new Location( anchor.X, this.Y, this.Z );
		}

		public Location AlignDirection8( Location anchor ) {
			int dx = Math.Abs(this.X-anchor.X);
			int dy = Math.Abs(this.Y-anchor.Y);

			if( dy/2 > dx )	return new Location( anchor.X, this.Y, this.Z );
			if( dx/2 > dy )	return new Location( this.X, anchor.Y, this.Z );
			
			return new Location( this.X, anchor.Y+dx*Math.Sign(this.Y-anchor.Y), this.Z );
		}

		#region operators
		public static Location operator + ( Location loc, Direction8 dir ) 
		{
			return new Location( loc.X+Direction.GetXOffset(dir), loc.Y+Direction.GetYOffset(dir), loc.Z );
		}
		public static Location operator - ( Location loc, Direction8 dir ) {
			return new Location( loc.X-Direction.GetXOffset(dir), loc.Y-Direction.GetYOffset(dir), loc.Z );
		}
		public static Location operator + ( Location loc, Distance dis ) {
			return new Location( loc.X+dis.X, loc.Y+dis.Y, loc.Z+dis.Z );
		}
		public static Location operator - ( Location loc, Distance dis ) {
			return new Location( loc.X-dis.X, loc.Y-dis.Y, loc.Z-dis.Z );
		}
		public static Distance operator - ( Location lhs, Location rhs ) {
			return new Distance( lhs.X-rhs.X, lhs.Y-rhs.Y, lhs.Z-rhs.Z );
		}

		public static bool operator != (Location loc1, Location loc2 ) {
			return !(loc1==loc2);
		}
		public static bool operator == (Location loc1, Location loc2 ) {
			return loc1.Equals(loc2);
		}
		#endregion
		public override bool Equals( object o ) 
		{
			if(!(o is Location))	return false;
			Location r = (Location)o;
			return X==r.X && Y==r.Y && Z==r.Z;
		}
		public override int GetHashCode() {
			return X^(Y<<13)^(Z<<26);
		}

		public override string ToString() {
			return string.Format("Location({0},{1},{2})",X,Y,Z);
		}

		/// <summary>
		/// 位置を持たないことを示す特殊な値
		/// </summary>
		public static readonly Location UNPLACED = new Location( short.MinValue, short.MinValue, short.MinValue );
	}

	/// <summary>
	/// Location同士の差分
	/// </summary>
	[Serializable]
	public struct Distance
	{
		public Distance( int x, int y, int z ) { X=x; Y=y; Z=z; }
		public Distance( Size3D sz ) { X=sz.sx; Y=sz.sy; Z=sz.sz; }

		public readonly int X;
		public readonly int Y;
		public readonly int Z;

		public override bool Equals( object o ) {
			if(!(o is Distance))	return false;
			Distance r = (Distance)o;
			return X==r.X && Y==r.Y && Z==r.Z;
		}
		public override int GetHashCode() {
			return X^(Y<<13)^(Z<<26);
		}
		public static bool operator != (Distance d1, Distance d2 ) {
			return !(d1==d2);
		}
		public static bool operator == (Distance d1, Distance d2 ) {
			return d1.Equals(d2);
		}
		public static Distance operator / ( Distance lhs, int rhs ) {
			return new Distance( lhs.X/rhs, lhs.Y/rhs, lhs.Z/rhs );
		}
		public static Distance operator * ( Distance lhs, int rhs ) 
		{
			return new Distance( lhs.X*rhs, lhs.Y*rhs, lhs.Z*rhs );
		}
		public static implicit operator Size3D (Distance d)
		{
			return new Size3D(d);
		}

		public override string ToString() 
		{
			return string.Format("Distance({0},{1},{2})",X,Y,Z);
		}

		// Distahce in int
		public double Value 
		{
			get
			{
				return Math.Sqrt(X*X+Y*Y+Z*Z);
			}
		}

		/// <summary>
		/// Distance projected on x-y plane.
		/// Sqrt(X*X+Y*Y)
		/// </summary>
		public double ValueInPlane
		{
			get
			{
				return (double)Math.Sqrt(X*X+Y*Y);
			}
		}
	}


}
