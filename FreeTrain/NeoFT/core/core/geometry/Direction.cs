using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;

namespace nft.core.geometry
{
	/// <summary>
	/// 16 directions.
	/// </summary>
	public enum Direction16 : int 
	{
		NORTH,EAST,SOUTH,WEST,
		NORTHEAST,SOUTHEAST,SOUTHWEST,NORTHWEST,
		NORTHNORTHEAST,EASTSOUTHEAST,SOUTHSOUTHWEST,WESTNORTHWEST,
		EASTNORTHEAST,SOUTHSOUTHEAST,WESTSOUTHWEST,NORTHNORTHWEST,
		INVALID=byte.MaxValue
	}

	/// <summary>
	/// byte size 8 directions.
	/// Cardinal and inter-cardinal directions.
	/// </summary>
	public enum Direction8 : int 
	{
		NORTH,EAST,SOUTH,WEST,
		NORTHEAST,SOUTHEAST,SOUTHWEST,NORTHWEST,
		INVALID=byte.MaxValue
	}

	/// <summary>
	/// byte size 4 cardinal directions.
	/// </summary>
	[Serializable]
	public enum Direction4 : int 
	{
		NORTH,EAST,SOUTH,WEST,
		INVALID=byte.MaxValue
	}

	/// <summary>
	/// byte size 4 inter cardinal directions.
	/// </summary>
	[Serializable]
	public enum InterCardinalDirection : int 
	{
		NORTHEAST=4,SOUTHEAST,SOUTHWEST,NORTHWEST,
		INVALID=byte.MaxValue
	}

	/// <summary>
	/// direction expression and direction related utilities.
	/// </summary>
	[Serializable]
	public sealed class Direction : ISerializable
	{
		private static readonly string[] DirNames;
		private static readonly Direction[] directions;
		// rearranged by clockwise rotation order
		private static readonly Direction16[] rotateMap = {
			Direction16.NORTH,Direction16.NORTHNORTHEAST,Direction16.NORTHEAST,Direction16.EASTNORTHEAST,
			Direction16.EAST,Direction16.EASTSOUTHEAST,Direction16.SOUTHEAST,Direction16.SOUTHSOUTHEAST,
			Direction16.SOUTH,Direction16.SOUTHSOUTHWEST,Direction16.SOUTHWEST,Direction16.WESTSOUTHWEST,
			Direction16.WEST,Direction16.WESTNORTHWEST,Direction16.NORTHWEST,Direction16.NORTHNORTHWEST
														  };
		// coversion map from Direction.index to rotateMap index
		private static readonly int[] angles;

		static Direction()
		{
			string names = Core.resources["direction.names"].stringValue;
			DirNames = names.Split(new char[]{','});
			Array db = Enum.GetValues(Direction16.NORTH.GetType());
			directions = new Direction[16];
			angles = new int[16];
			for(int i=0;i<16;i++)
			{
				directions[i] = new Direction((int)(Direction16)db.GetValue(i));
				angles[(int)rotateMap[i]] = i;
			}

		}

		private Direction( int idx  ) 
		{
			this.index = idx;
		}

		/// <summary>
		/// create from <code>Direction16</code>
		/// </summary>
		public static Direction Get( Direction16 dir ) 
		{
			return directions[(int)dir];
		}

		public static Direction Get( Direction8 dir ) 
		{
			return directions[(int)dir];
		}

		public static Direction Get( Direction4 dir ) 
		{
			return directions[(int)dir];
		}

		public static Direction Get( InterCardinalDirection dir ) 
		{
			return directions[(int)dir];
		}

		internal static Direction GetByAngle( int angle ) 
		{
			return directions[(int)rotateMap[angle&0xf]];
		}

		/// <summary>
		/// index can parse to <code>Direction16</code>
		/// </summary>
		public readonly int index;

		public static Direction NORTH		{ get { return Get(Direction16.NORTH); } }
		public static Direction EAST		{ get { return Get(Direction16.EAST); } }
		public static Direction SOUTH		{ get { return Get(Direction16.SOUTH); } }
		public static Direction WEST		{ get { return Get(Direction16.WEST); } }
		public static Direction NORTHEAST	{ get { return Get(Direction16.NORTHEAST); } }
		public static Direction SOUTHEAST	{ get { return Get(Direction16.SOUTHEAST); } }
		public static Direction SOUTHWEST	{ get { return Get(Direction16.SOUTHWEST); } }
		public static Direction NORTHWEST	{ get { return Get(Direction16.NORTHWEST); } }
		public static Direction NORTHNORTHEAST	{ get { return Get(Direction16.NORTHNORTHEAST); } }
		public static Direction EASTSOUTHEAST	{ get { return Get(Direction16.EASTSOUTHEAST); } }
		public static Direction SOUTHSOUTHWEST	{ get { return Get(Direction16.SOUTHSOUTHWEST); } }
		public static Direction WESTNORTHWEST	{ get { return Get(Direction16.WESTNORTHWEST); } }
		public static Direction EASTNORTHEAST	{ get { return Get(Direction16.EASTNORTHEAST); } }
		public static Direction SOUTHSOUTHEAST	{ get { return Get(Direction16.SOUTHSOUTHEAST); } }
		public static Direction WESTSOUTHWEST	{ get { return Get(Direction16.WESTSOUTHWEST); } }
		public static Direction NORTHNORTHWEST	{ get { return Get(Direction16.NORTHNORTHWEST); } }
		
		/// <summary>
		/// Displayable name in local language.
		/// </summary>
		public string Name 
		{
			get{return DirNames[index];}
		}

		/// <summary>
		/// Returns true if the direction is one of N,E,S, or W.
		/// </summary>
		public bool IsCardinal { get { return index<4; } }

		/// <summary>
		/// Retruns true if the direction is one of NE,SE,NW, or SW.
		/// </summary>
		public bool IsInterCardinal { get { return (index>=4)&&(index<8); } }

		/// <summary>
		/// Retruns true if the direction is cardinal or Inter-cardinal points.
		/// </summary>
		public bool IsMajor{ get { return index<8; } }

		/// <summary>
		/// Returns true if the direction is parallel.
		/// </summary>
		public bool IsParallel(Direction d) { return IsParallel(this,d); }
		public static bool IsParallel(Direction a, Direction b) { return ((a.index^b.index)|2)==2; }

		/// <summary>
		/// Returns true if the direction is opposite.
		/// </summary>
		public bool IsOpposite(Direction d) { return IsOpposite(this,d); }
		public static bool IsOpposite(Direction a, Direction b) { return (a.index^b.index)==2; }

		/// <summary>
		/// Returns true if the direction is right angle to specified one.
		/// </summary>
		public bool IsRightAngle(Direction d) { return IsRightAngle(this,d); }
		public static bool IsRightAngle(Direction a, Direction b) { return ((a.index^b.index)|2)==3; }

		/// <summary>
		/// Returns true if the direction is EAST or WEST
		/// </summary>
		public bool IsParallelToX { get { return (index==1)||(index==3); } }

		/// <summary>
		/// Returns true if the direction is NORTH or SOUTH
		/// </summary>
		public bool IsParallelToY { get { return (index==0)||(index==2); } }

		/// <summary>
		/// Get direction which rotate left.
		/// </summary>
		public Direction Left		{ get { return RotateL(4); } }
		public Direction LeftHalf 	{ get { return RotateL(2); } }
		public Direction LeftQuater { get { return RotateL(1); } }

		/// <summary>
		/// Get direction which rotate right.
		/// </summary>
		public Direction Right		 { get { return RotateR(4); } }
		public Direction RightHalf 	 { get { return RotateR(2); } }
		public Direction RightQuater { get { return RotateR(1); } }

		/// <summary>
		/// Get direction which rotate right by step*22.5 degree.
		/// </summary>
		public Direction RotateR(int step) { return GetByAngle(angles[index]+step); }
		/// <summary>
		/// Get direction which rotate left by step*22.5 degree.
		/// </summary>
		public Direction RotateL(int step) { return GetByAngle(angles[index]-step); }
		
		/// <summary>Gets the opposite direction.</summary>
		public Direction Opposite { get { return directions[index^2]; } }

		/// <summary>
		/// returns angle between two directions, by a quotient of 22.5 degree.
		/// </summary>
		public static int AngleStepCount( Direction a, Direction b ) 
		{
			int d = angles[b.index] - angles[a.index];
			return d&0xf;
		}

		/// <summary>
		/// Cast operators
		/// </summary>
		//		public static implicit operator Distance ( Direction d ) {
		//			return new Distance( d.offsetX, d.offsetY, 0 );
		//		}

		public static implicit operator Direction16 ( Direction d ) 
		{
			return (Direction16)d.index;
		}
		public static implicit operator Direction ( Direction16 d ) 
		{
			return Get(d);
		}
		public static implicit operator Direction ( Direction8 d ) 
		{
			return Get(d);
		}
		public static implicit operator Direction ( Direction4 d ) 
		{
			return Get(d);
		}

		public static implicit operator Direction ( InterCardinalDirection d ) 
		{
			return Get(d);
		}

		#region coordinate caluculation helper
		static readonly int[] x_table =new int[]{0,1,0,-1,1,1,-1,-1};
		static readonly int[] y_table =new int[]{-1,0,1,0,-1,1,1,-1};
		internal static int GetXOffset(Direction8 dir)
		{
			return x_table[(int)dir];
		}
		internal static int GetYOffset(Direction8 dir)
		{
			return y_table[(int)dir];
		}		
		#endregion
		#region ISerializable メンバ

		public void GetObjectData( SerializationInfo info, StreamingContext context) 
		{
			info.SetType(typeof(ReferenceImpl));
			info.AddValue("index",index);
		}
		
		[Serializable]
			internal sealed class ReferenceImpl : IObjectReference 
		{
			private int index=0;
			public object GetRealObject(StreamingContext context) 
			{
				return Direction.Get((Direction16)index);
			}
		}

		#endregion
	}
}
