using System;
using System.Drawing;

namespace nft.core.geometry
{
	/// <summary>
	/// Better "Size" class.
	/// Each coordinate value must within 2byte range.
	/// </summary>
	public struct Size3D
	{
		public Size3D( Size h_area ) : this( h_area.Width, h_area.Height, 0 ) {}
		public Size3D( Size h_area, int height ) : this( h_area.Width, h_area.Height, height ) {}
		public Size3D( int w_x, int w_y, int w_z ) { sx = (short)w_x; sy = (short)w_y; sz = (short)w_z; }
		public Size3D( Distance d ) { sx = (short)d.X; sy = (short)d.Y; sz = (short)d.Z; }
		public Size3D( Location l ) { sx = l.X; sy = l.Y; sz = l.Z; }

		public short sx;
		public short sy;
		public short sz;

		/// Convert each coordinate to integer which is smallest but not lesser than original. ///
		public static Size3D Ceiling(Size3DF s)
		{
			return new Size3D((int)Math.Ceiling(s.sx),(int)Math.Ceiling(s.sy),(int)Math.Ceiling(s.sz));
		}

		/// Convert each coordinate to integer which is largest but not greater than original. ///
		public static Size3D Floor(Size3DF s)
		{
			return new Size3D((int)Math.Floor(s.sx),(int)Math.Floor(s.sy),(int)Math.Floor(s.sz));
		}

		public override bool Equals( object o ) 
		{
			if(!(o is Size3D))	return false;
			Size3D r = (Size3D)o;
			return sx==r.sx && sy==r.sy && sz==r.sz;
		}
		public override int GetHashCode() 
		{
			return sx^(sy<<13)^(sz<<26);
		}

		/// <summary>
		/// Plane Area of this size.
		/// </summary>
		public int Area { get {	return sx*sy;	} }

		/// <summary>
		/// Volume of this size.
		/// </summary>
		public int Volume { get { return sx*sy*sz; } }

		public Size PlaneSize {	get { return new Size( sx, sy ); } }

		public static implicit operator Size3D( Size sz ) 
		{
			return new Size3D( sz );
		}

		public static Size3D operator / ( Size3D lhs, int rhs ) 
		{
			return new Size3D( lhs.sx/rhs, lhs.sy/rhs, lhs.sz/rhs );
		}

		public static Size3D operator * ( Size3D lhs, int rhs ) 
		{
			return new Size3D( lhs.sx*rhs, lhs.sy*rhs, lhs.sz*rhs );
		}

		public override string ToString() 
		{
			return string.Format("Size3D({0},{1},{2})",sx,sy,sz);
		}

	}
}
