using System;
using System.Drawing;

namespace nft.core.geometry
{
	/// <summary>
	/// Better "Size" class.
	/// Each coordinate value must within 2byte range.
	/// </summary>
	public struct Size3DF
	{
		#region constructors
		/// <summary>
		/// Construct with specified size.
		/// Each parameters are cast down to 'short' value (2 byte).
		/// </summary>
		/// <param name="_x"></param>
		/// <param name="_y"></param>
		/// <param name="_z"></param>
		public Size3DF( double w_x, double w_y, double w_z ) { sx = (float)w_x; sy = (float)w_y; sz = (float)w_z; }
		public Size3DF( Size3D sz ) { this.sx = sz.sx; this.sy = sz.sy;	this.sz = sz.sz; }
		public Size3DF( Distance d ) { sx = d.X; sy = d.Y; sz = d.Z; }
		public Size3DF( DistanceF d ) { sx = d.X; sy = d.Y; sz = d.Z; }
		public Size3DF( Location l ) { sx = l.X; sy = l.Y; sz = l.Z; }
		public Size3DF( LocationF l ) { sx = l.X; sy = l.Y; sz = l.Z; }
		#endregion

		public float sx;
		public float sy;
		public float sz;

		public override bool Equals( object o ) 
		{
			if(!(o is Size3D))	return false;
			Size3D r = (Size3D)o;
			return sx==r.sx && sy==r.sy && sz==r.sz;
		}
		public override int GetHashCode() 
		{
			return new PointF(sx,sy).GetHashCode()^sz.GetHashCode();
		}

		/// <summary>
		/// Plane Area of this size.
		/// </summary>
		public double Area { get {	return sx*sy;	} }

		/// <summary>
		/// Volume of this size.
		/// </summary>
		public double Volume { get { return sx*sy*sz; } }

		public SizeF PlaneSize {	get { return new SizeF( sx, sy ); } }

		#region operators
		public static implicit operator Size3DF( Size3D sz ) 
		{
			return new Size3DF( sz );
		}

		public static Size3DF operator / ( Size3DF lhs, double rhs ) 
		{
			return new Size3DF( lhs.sx/rhs, lhs.sy/rhs, lhs.sz/rhs );
		}

		public static Size3DF operator * ( Size3DF lhs, double rhs ) 
		{
			return new Size3DF( lhs.sx*rhs, lhs.sy*rhs, lhs.sz*rhs );
		}
		#endregion

		public override string ToString() 
		{
			return string.Format("Size3D({0},{1},{2})",sx,sy,sz);
		}

	}
}
