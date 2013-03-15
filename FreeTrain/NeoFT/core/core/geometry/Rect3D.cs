using System;
using System.Drawing;
using System.Collections;

namespace nft.core.geometry
{
	/// <summary>
	/// Rect3D の概要の説明です。
	/// </summary>
	public struct Rect3D : IEnumerable
	{
		private Location loc;
		public Location Location{ get{ return loc; } }
		private Size3D size;
		public Size3D Size{ get{ return size; } }

		public int X { get{ return loc.X; } set{ loc.X = (short)value; } }
		public int Y { get{ return loc.Y; } set{ loc.Y = (short)value; } }
		public int Z { get{ return loc.Z; } set{ loc.Z = (short)value; } }
		public int X2 { get{ return loc.X+size.sx; } set{ size.sx = (short)(value-loc.X); } }
		public int Y2 { get{ return loc.Y+size.sy; } set{ size.sy = (short)(value-loc.Y); } }
		public int Z2 { get{ return loc.Z+size.sz; } set{ size.sz = (short)(value-loc.Z); } }
		public int WidthX { get{ return size.sx; } set{ size.sx = (short)value; } }
		public int WidthY { get{ return size.sy; } set{ size.sy = (short)value; } }
		public int Height { get{ return size.sz; } set{ size.sz = (short)value; } }

		public Location DiagonalLocation{ get{ return new Location(X2,Y2,Z2); } }
		public int Volume{ get{ return size.Volume; } }
		public int Area{ get{ return size.Area; } }
		public Rectangle PlaneBounds{ get{ return new Rectangle(loc.PlaneLocation,size.PlaneSize);} }

		public Rect3D(int x, int y, int z, int sx, int sy, int sz )
		{ loc = new Location(x,y,z); size = new Size3D(sx,sy,sz); }

		public Rect3D(Location l, Size3D s )
		{ loc = l; size = s; }

		/// <summary>
		/// Get minimum external rectangle larger than the rectangle specified by 'l' and 's'
		/// Each axis value in 's' must be positive.
		/// </summary>
		public static Rect3D MinLargerRect( LocationF l, Size3DF s )
		{
			Location loc = Location.Floor(l);
			int x2 = (int)Math.Ceiling(l.X+s.sx);
			int y2 = (int)Math.Ceiling(l.Y+s.sy);
			int z2 = (int)Math.Ceiling(l.Z+s.sz);
			Size3D size = new Size3D(x2-loc.X,y2-loc.Y,z2-loc.Z);
			return new Rect3D(loc,size);
		}

		/// <summary>
		/// Get maximum internal rectangle smaller than the rectangle specified by 'l' and 's'
		/// Each axis value in 's' must be positive.
		/// </summary>
		public static Rect3D MaxSmallerRect( LocationF l, Size3DF s )
		{
			Location loc = Location.Ceiling(l);
			int x2 = (int)Math.Floor(l.X+s.sx);
			int y2 = (int)Math.Floor(l.Y+s.sy);
			int z2 = (int)Math.Floor(l.Z+s.sz);
			Size3D size = new Size3D(x2-loc.X,y2-loc.Y,z2-loc.Z);
			return new Rect3D(loc,size);
		}

		/// <summary>
		/// If size is negative, invert location diagonally.
		/// Please normalize first before using any caluculation.
		/// </summary>
		public void Normalize()
		{
			if(size.sx<0)
			{	size.sx = (short)-size.sx;	loc.X -= size.sx;	}
			if(size.sy<0)
			{	size.sy = (short)-size.sy;	loc.Y -= size.sy;	}
			if(size.sz<0)
			{	size.sz = (short)-size.sz;	loc.Z -= size.sz;	}
		}

		/// <summary>
		/// Get holizontal minimumu distance to specified location
		/// Negative value means internal point and it's distance from neareset border.
		/// </summary>
		/// <param name="l"></param>
		/// <returns></returns>
		public double GetDistanceTo(Location l)
		{
			int dx = Math.Max(X-l.X,l.X-X2);
			int dy = Math.Max(Y-l.Y,l.Y-Y2);
			int dz = Math.Max(Y-l.Y,l.Y-Y2);
			if( dx<0 || dy <0 || dz < 0)
			{
				if( dx<0 && dy<0 && dz<0 )
					return Math.Max(Math.Max(dx,dy),dz);
				dx = Math.Max(0,dx);
				dy = Math.Max(0,dy);
				dz = Math.Max(0,dz);					
			}
			return Math.Sqrt(dx*dx+dy*dy+dz*dz);
		}

		/// <summary>
		/// Get holizontal minimumu distance to specified location.
		/// Negative value means internal point and it's distance from neareset border.
		/// </summary>
		/// <param name="l"></param>
		/// <returns></returns>
		public double GetPlaneDistanceTo(Location l)
		{
			int dx = Math.Max(X-l.X,l.X-X2);
			int dy = Math.Max(Y-l.Y,l.Y-Y2);
			if( dx<0 || dy <0 )
				return Math.Max(dx,dy);
			else
				return Math.Sqrt(dx*dx+dy*dy);
		}

		/// <summary>
		/// Get holizontal minimumu distance to specified location
		/// Negative value means internal point and it's distance from neareset border.
		/// </summary>
		/// <param name="l"></param>
		/// <returns></returns>
		public double GetDistanceTo(Rect3D r)
		{
			int dx = Math.Max(r.X-X2,X-r.X2);
			int dy = Math.Max(r.Y-Y2,Y-r.Y2);
			int dz = Math.Max(r.Z-Z2,Z-r.Z2);
			if( dx<0 || dy <0 || dz<0)
			{
				if( dx<0 && dy<0 && dz<0 )
					return Math.Max(Math.Max(dx,dy),dz);
				dx = Math.Max(0,dx);
				dy = Math.Max(0,dy);
				dz = Math.Max(0,dz);					
			}
			return Math.Sqrt(dx*dx+dy*dy+dz*dz);
		}

		/// <summary>
		/// Get holizontal minimumu distance to specified location
		/// Negative value means internal point and it's distance from neareset border.
		/// </summary>
		/// <param name="l"></param>
		/// <returns></returns>
		public double GetPlaneDistanceTo(Rect3D r)
		{
			int dx = Math.Max(r.X-X2,X-r.X2);
			int dy = Math.Max(r.Y-Y2,Y-r.Y2);
			if( dx<0 || dy <0 )
				return Math.Max(dx,dy);
			else
				return Math.Sqrt(dx*dx+dy*dy);
		}

		public bool IsNormalized { get {return (size.sx>=0&&size.sy>=0&&size.sz>=0);} }

		/// <summary>
		/// Return true if the location is contained in this rectangle.
		/// </summary>
		/// <param name="l"></param>
		/// <returns></returns>
		public bool IsContains( Location l )
		{
			if( l.X<X || l.Y<Y || l.Z<Z )
				return false;
			return ( X2>=l.X || Y2>=l.Y || Z2>=l.Z );
		}
		
		/// <summary>
		/// Return true if the location is not on surface but concealed in this rectangle.
		/// </summary>
		/// <param name="l"></param>
		/// <returns></returns>
		public bool IsConceals( Location l )
		{
			if( l.X<=X || l.Y<=Y || l.Z<=Z )
				return false;
			return ( X2>l.X || Y2>l.Y || Z2>l.Z );
		}

		/// <summary>
		/// Return true if the location is on the surface of this rectangle.
		/// </summary>
		/// <param name="l"></param>
		/// <returns></returns>
		public bool IsOnBorder( Location l )
		{
			if(l.X == X || l.X == X2)
			{
				return (l.Y>=Y&&l.Z>=Z&&l.Y<=Y2&&l.Z<=Z2);
			}
			if(l.Y == Y || l.Y == Y2)
			{
				return (l.X>=X&&l.Z>=Z&&l.X<=X2&&l.Z<=Z2);
			}
			if(l.Z == Z || l.Z == Z2)
			{
				return (l.Y>=Y&&l.X>=X&&l.Y<=Y2&&l.X<=X2);
			}
			return false;
		}

		/// <summary>
		/// Return true if specified <code>Rect3D</code> is intersect this.
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		public bool IsIntersect( Rect3D r )
		{
			return (X2>=r.X&&X>=r.X2&&Y2>=r.Y&&Y>=r.Y2&&Z2>=r.Z&&Z>=r.Z2);
		}

		/// <summary>
		/// Return true if specified <code>Rect3D</code> is entirely included in this rectangle.
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		public bool IsIncludes( Rect3D r )
		{
			return (X<=r.X&&X2>=r.X2&&Y<=r.Y&&Y2>=r.Y2&&Z<=r.Z&&Z2>=r.Z2);
		}

		public override bool Equals( object o ) 
		{
			if(!(o is Rect3D))	return false;
			Rect3D r = (Rect3D)o;
			return loc.Equals(r.loc) && size.Equals(r.size);
		}

		public override int GetHashCode() 
		{
			return loc.GetHashCode()^size.GetHashCode();
		}

		/// <summary>
		/// inflate evely bounds by 'n'.
		/// </summary>
		/// <param name="n"></param>
		public void Inflate(int n)
		{
			Inflate(n,n,n);
		}

		/// <summary>
		/// inflate each bounds by 'x','y' and 'z' respectively.
		/// </summary>
		/// <param name="n"></param>
		public void Inflate(int x, int y, int z)
		{
			X -=x; Y -=y; Z -=z;
			WidthX+=x*2; WidthY+=y*2; Height+=z*2;
		}

		/// <summary>
		/// Retruns rectangle which include both r1 and r2.
		/// </summary>
		/// <param name="r1"></param>
		/// <param name="r2"></param>
		/// <returns></returns>
		public static Rect3D Union(Rect3D r1,Rect3D r2)
		{
			int x = Math.Min(r1.X,r2.X);
			int y = Math.Min(r1.Y,r2.Y);
			int z = Math.Min(r1.Z,r2.Z);
			int x2 = Math.Max(r1.X2,r2.X2);
			int y2 = Math.Max(r1.Y2,r2.Y2);
			int z2 = Math.Max(r1.Z2,r2.Z2);
			return new Rect3D(x,y,z,x2-x,y2-y,z2-z);
		}

		/// <summary>
		/// Returns intersected part of r1 and r2.
		/// </summary>
		/// <param name="r1"></param>
		/// <param name="r2"></param>
		/// <returns></returns>
		public static Rect3D Intersect(Rect3D r1,Rect3D r2)
		{
			int x = Math.Max(r1.X,r2.X);
			int y = Math.Max(r1.Y,r2.Y);
			int z = Math.Max(r1.Z,r2.Z);
			int x2 = Math.Min(r1.X2,r2.X2);
			int y2 = Math.Min(r1.Y2,r2.Y2);
			int z2 = Math.Min(r1.Z2,r2.Z2);
			Rect3D res = new Rect3D(x,y,z,x2-x,y2-y,z2-z);
			// if not intersect at all, the result will be unnormalized one.
			if(res.IsNormalized) 
				return res;
			else
				return NULL;
		}

		public override string ToString() 
		{
			return string.Format("Rect3D({0},{1},{2})+({3},{4},{5})",X,Y,Z,WidthX,WidthY,Height);
		}

		/// <summary>
		/// Retrurns IEnumerator wichi lists all <code>Location</code> in this Rect3D
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return new LocEnumerator(this);

		}

		public static Rect3D NULL = new Rect3D(int.MinValue,int.MinValue,int.MinValue,0,0,0);

		/// <summary>
		/// IEnumerator implementation
		/// </summary>
		private class LocEnumerator : IEnumerator
		{
			private Rect3D rect;
			private int ox = 0;
			private int oy = 0;
			private int oz = 0;

			public LocEnumerator(Rect3D owner)	{ rect = owner; }

			#region IEnumerator メンバ

			public void Reset()
			{
				ox = oy = oz = 0;
			}

			public object Current {	get { return new Location(rect.X+ox,rect.Y+oy,rect.Z+oz); } }

			public bool MoveNext()
			{
				ox++;
				if(ox>=rect.WidthX)
				{
					ox = 0;
					oy++;
					if(oy>=rect.WidthY)
					{
						oy = 0;
						oz++;
					}
				}
				return ( oz < rect.Height );
			}

			#endregion
		}
	}


}
