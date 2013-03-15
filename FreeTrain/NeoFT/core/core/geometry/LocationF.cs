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
	public struct LocationF
	{
		/// Each parameters are cast down to 'short' value (2 byte).
		public LocationF( double x, double y, double z ) { X=(float)x; Y=(float)y; Z=(float)z; }
		public LocationF( Location l ) { X=l.X; Y=l.Y; Z=l.Z; }

		public float X;
		public float Y;
		public float Z;

		/// <summary>
		/// Get horizontal <code>Direction</code> toward specified <code>Location</code> 'to'.
		/// Retrun value is the nearest one in 16 directions.
		/// </summary>
		/// <param name="to"></param>
		/// <returns></returns>
		public Direction16 GetDirectionTo( LocationF to ) {
			double v = Math.Atan2(to.Y-Y,to.X-X)/Math.PI;
			if( v<0 ) 
				v+= 2.5;
			else
				v+=0.5;
			// 180deg = 1, 22.5deg=1/8. so multiply v 8 times to make 22.5deg=1
			int n = (int)Math.Round(v*8);
			return Direction.GetByAngle(n);			
		}

		public double GetDistanceTo( LocationF to ) {			
			return (to-this).Value;
		}

		public double GetPlaneDistanceTo( LocationF to ) 
		{			
			return (to-this).ValueInPlane;
		}

		public PointF PlaneLocation { get{ return new PointF(X,Y); } }

		#region operators
		public static LocationF operator + ( LocationF loc, DistanceF dis ) {
			return new LocationF( loc.X+dis.X, loc.Y+dis.Y, loc.Z+dis.Z );
		}
		public static LocationF operator - ( LocationF loc, DistanceF dis ) {
			return new LocationF( loc.X-dis.X, loc.Y-dis.Y, loc.Z-dis.Z );
		}
		public static DistanceF operator - ( LocationF lhs, LocationF rhs ) {
			return new DistanceF( lhs.X-rhs.X, lhs.Y-rhs.Y, lhs.Z-rhs.Z );
		}

		public static bool operator != (LocationF loc1, LocationF loc2 ) {
			return !(loc1==loc2);
		}
		public static bool operator == (LocationF loc1, LocationF loc2 ) {
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
			return new PointF(X,Y).GetHashCode()^Z.GetHashCode();
		}

		public override string ToString() {
			return string.Format("Location({0},{1},{2})",X,Y,Z);
		}
	}

	/// <summary>
	/// LocationìØémÇÃç∑ï™
	/// </summary>
	[Serializable]
	public struct DistanceF
	{
		public DistanceF( double x, double y, double z ) { X=(float)x; Y=(float)y; Z=(float)z; }
		public DistanceF( Size3D sz ) { X=sz.sx; Y=sz.sy; Z=sz.sz; }
		public DistanceF( Size3DF sz ) { X=sz.sx; Y=sz.sy; Z=sz.sz; }
		public DistanceF( Distance d ) { X=d.X; Y=d.Y; Z=d.Z; }

		public readonly float X;
		public readonly float Y;
		public readonly float Z;

		public override bool Equals( object o ) {
			if(!(o is Distance))	return false;
			Distance r = (Distance)o;
			return X==r.X && Y==r.Y && Z==r.Z;
		}
		public override int GetHashCode() {
			return new PointF(X,Y).GetHashCode()^Z.GetHashCode();
		}

		#region operators
		public static bool operator != (DistanceF d1, DistanceF d2 ) 
		{
			return !(d1==d2);
		}
		public static bool operator == (DistanceF d1, DistanceF d2 ) {
			return d1.Equals(d2);
		}
		public static DistanceF operator / ( DistanceF lhs, int rhs ) {
			return new DistanceF( lhs.X/rhs, lhs.Y/rhs, lhs.Z/rhs );
		}
		public static DistanceF operator * ( DistanceF lhs, int rhs ) 
		{
			return new DistanceF( lhs.X*rhs, lhs.Y*rhs, lhs.Z*rhs );
		}
		public static implicit operator Size3DF (DistanceF d)
		{
			return new Size3DF(d);
		}
		#endregion

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
