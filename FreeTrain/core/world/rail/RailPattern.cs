using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.util;
using org.kohsuke.directdraw;

namespace freetrain.world.rail
{
	/// <summary>
	/// レールの形状のパターンに関する情報を表現するオブジェクト
	/// </summary>
	[Serializable]	// serialization by reference
	public sealed class RailPattern : SimpleSprite, ISerializable {
		/// <summary>
		/// Sprite that contains the images of all patterns.
		/// </summary>
		private static readonly Picture railChips = ResourceUtil.loadSystemPicture("RailRoads.bmp","RailRoads_n.bmp");
		private static readonly Picture ugChips = ResourceUtil.loadSystemPicture("ugslope.bmp");

		// single rail road
		private static readonly RailPattern[] singlePatterns = new RailPattern[] {
			RailPattern.createNormal( 0, 1, 4 ),
			RailPattern.createNormal( 1, 1, 5 ),
			RailPattern.createNormal( 2, 1, 6 ),
			RailPattern.createNormal( 3, 2, 5 ),
			RailPattern.createNormal( 4, 2, 6 ),
			RailPattern.createNormal( 5, 2, 7 ),
			RailPattern.createNormal( 6, 3, 6 ),
			RailPattern.createNormal( 7, 3, 7 ),
			RailPattern.createNormal( 8, 3, 0 ),
			RailPattern.createNormal( 9, 4, 7 ),
			RailPattern.createNormal(10, 4, 0 ),
			RailPattern.createNormal(11, 5, 0 )
		};

		// junctions
		private static readonly RailPattern[] junctionPatterns = new RailPattern[] {
			RailPattern.createJunction( 0, 1, 5, -1 ),
			RailPattern.createJunction( 1, 1, 5, +1 ),
			RailPattern.createJunction( 2, 2, 6, -1 ),
			RailPattern.createJunction( 3, 2, 6, +1 ),
			RailPattern.createJunction( 4, 3, 7, -1 ),
			RailPattern.createJunction( 5, 3, 7, +1 ),
			RailPattern.createJunction( 6, 4, 0, -1 ),
			RailPattern.createJunction( 7, 4, 0, +1 ),
			RailPattern.createJunction( 8, 5, 1, -1 ),
			RailPattern.createJunction( 9, 5, 1, +1 ),
			RailPattern.createJunction(10, 6, 2, -1 ),
			RailPattern.createJunction(11, 6, 2, +1 ),
			RailPattern.createJunction(12, 7, 3, -1 ),
			RailPattern.createJunction(13, 7, 3, +1 ),
			RailPattern.createJunction(14, 0, 4, -1 ),
			RailPattern.createJunction(15, 0, 4, +1 )
		};

		// slopes
		private static readonly RailPattern[] slopePatterns = new RailPattern[] {
			RailPattern.createSlope(railChips, 0,0, 0,0,true),
			RailPattern.createSlope(railChips, 1,0, 1,0,true),
			RailPattern.createSlope(railChips, 2,0,-1,4,false),
			RailPattern.createSlope(railChips, 3,0, 0,0,false),

			RailPattern.createSlope(railChips, 4,2, 0,2,true),
			RailPattern.createSlope(railChips, 5,2, 1,2,true),
			RailPattern.createSlope(railChips, 6,2,-1,6,false),
			RailPattern.createSlope(railChips, 7,2, 0,2,false),

			RailPattern.createSlope(railChips, 8,4, 0,4,true),
			RailPattern.createSlope(railChips, 9,4, 1,4,true),
			RailPattern.createSlope(railChips,10,4,-1,0,false),
			RailPattern.createSlope(railChips,11,4, 0,4,false),

			RailPattern.createSlope(railChips,12,6, 0,6,true),
			RailPattern.createSlope(railChips,13,6, 1,6,true),
			RailPattern.createSlope(railChips,14,6,-1,2,false),
			RailPattern.createSlope(railChips,15,6, 0,6,false)
		};

		private static readonly RailPattern[] slopeUGPatterns = new RailPattern[] {
			RailPattern.createUGSlope(ugChips, 0,0, 0,0,true),
			RailPattern.createUGSlope(ugChips, 1,0, 1,0,true),
			RailPattern.createUGSlope(ugChips, 2,0,-1,4,false),
			RailPattern.createUGSlope(ugChips, 3,0, 0,0,false),

			RailPattern.createUGSlope(ugChips, 4,2, 0,2,true),
			RailPattern.createUGSlope(ugChips, 5,2, 1,2,true),
			RailPattern.createUGSlope(ugChips, 6,2,-1,6,false),
			RailPattern.createUGSlope(ugChips, 7,2, 0,2,false),

			RailPattern.createUGSlope(ugChips, 8,4, 0,4,true),
			RailPattern.createUGSlope(ugChips, 9,4, 1,4,true),
			RailPattern.createUGSlope(ugChips,10,4,-1,0,false),
			RailPattern.createUGSlope(ugChips,11,4, 0,4,false),

			RailPattern.createUGSlope(ugChips,12,6, 0,6,true),
			RailPattern.createUGSlope(ugChips,13,6, 1,6,true),
			RailPattern.createUGSlope(ugChips,14,6,-1,2,false),
			RailPattern.createUGSlope(ugChips,15,6, 0,6,false)
		};


		public static readonly Sprite[] slopeWalls = new Sprite[] {
			RailPattern.createSlopeSupport(ugChips, 2),
			RailPattern.createSlopeSupport(ugChips, 3),

			RailPattern.createSlopeSupport(ugChips, 6),
			RailPattern.createSlopeSupport(ugChips, 7),

			RailPattern.createSlopeSupport(ugChips,10),
			RailPattern.createSlopeSupport(ugChips,11),

			RailPattern.createSlopeSupport(ugChips,14),
			RailPattern.createSlopeSupport(ugChips,15)
		};

		// slope support voxels
		public static readonly Sprite[] slopeSupports = new Sprite[] {
			RailPattern.createSlopeSupport(railChips, 2),
			RailPattern.createSlopeSupport(railChips, 3),

			RailPattern.createSlopeSupport(railChips, 6),
			RailPattern.createSlopeSupport(railChips, 7),

			RailPattern.createSlopeSupport(railChips,10),
			RailPattern.createSlopeSupport(railChips,11),

			RailPattern.createSlopeSupport(railChips,14),
			RailPattern.createSlopeSupport(railChips,15)
		};


		/// <summary>
		/// 指定されたアングルを持つ非分岐レールパターンを返す
		/// </summary>
		public static RailPattern get( Direction a, Direction b ) {
			// 線路は急にはまがれない
			Debug.Assert( Direction.angle(a,b)>=3 );

			foreach( RailPattern rp in singlePatterns ) {
				if( rp.dir[a.index] && rp.dir[b.index] )
					return rp;
			}

			Debug.Assert(false);	// 全てのパターンがテーブル中に存在するはず
			return null;
		}

		/// <summary>
		/// Gets the pattern that has three desired directions.
		/// </summary>
		public static RailPattern getJunction( Direction a, Direction b, Direction c ) {
			Debug.Assert( a!=b && b!=c && c!=a );

			foreach( RailPattern rp in junctionPatterns ) {
				if(rp.dir[a.index] && rp.dir[b.index] && rp.dir[c.index])
					return rp;
			}

			Debug.Assert(false);
			return null;
		}

		/// <summary>
		/// Gets the pattern that has the specified direction to climb and the height.
		/// </summary>
		/// <param name="climb"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static RailPattern getSlope( Direction climb, int height ) {
			Debug.Assert( 0<=height && height<4 );
			Debug.Assert( climb.isSharp );

			return slopePatterns[climb.index*2+height];
		}

		public static RailPattern getUGSlope( Direction climb, int height ) 
		{
			Debug.Assert( 0<=height && height<4 );
			Debug.Assert( climb.isSharp );

			return slopeUGPatterns[climb.index*2+height];
		}

		// for junction rail
		private static RailPattern createJunction( int imageIndexX, int angle1, int angle2, int angle3offset ) {
			RailPattern p = new RailPattern(12+imageIndexX, imageIndexX,1,angle1,angle2,new Size(32,16),0);
			p.dir[(angle2+angle3offset+8)%8]=true;
			return p;
		}
		// for normal rail
		private static RailPattern createNormal( int imageIndexX, int angle1, int angle2 ) {
			return new RailPattern(imageIndexX,imageIndexX,0,angle1,angle2,new Size(32,16),0);
		}
		// for angle rail
		private static RailPattern createSlope(Picture pic, int imageIndexX, int upDir, int zdiff, int zangle, bool isVisible ) {
			return createSlope(pic, 28, imageIndexX, upDir, zdiff, zangle, isVisible );
		}

		private static RailPattern createUGSlope(Picture pic, int imageIndexX, int upDir, int zdiff, int zangle, bool isVisible ) 
		{
			return createSlope(pic, 44, imageIndexX, upDir, zdiff, zangle, isVisible );
		}

		// for angle rail
		private static RailPattern createSlope(Picture pic, int baseIndexX, int imageIndexX, int upDir, int zdiff, int zangle, bool isVisible ) 
		{
			RailPattern p = new RailPattern(pic,
				baseIndexX+imageIndexX,
				isVisible?imageIndexX:12,
				isVisible?2:0,
				upDir,(upDir+4)%8,
				isVisible?new Size(32,32):new Size(1,1), 16);
			p._climbDir = Direction.get(upDir);
			p._level = imageIndexX%4;

			p._zdiff = zdiff;
			p._zangle = Direction.get(zangle);			
			return p;
		}

		private static Sprite createSlopeSupport( Picture p, int imageIndexX ) {
			return new SimpleSprite(p, new Point(0,16), new Point(imageIndexX*32,32), new Size(32,32) );
		}

		private RailPattern( int id, int imageIndexX, int imageIndexY, int angle1, int angle2, Size sz, int offsetY )
			: base(railChips,new Point(0,offsetY),new Point(imageIndexX*32,imageIndexY*16),sz) {
			this.id = id;
			dir[angle1] = true;
			dir[angle2] = true;
		}

		private RailPattern(Picture pic, int id, int imageIndexX, int imageIndexY, int angle1, int angle2, Size sz, int offsetY )
			: base(pic,new Point(0,offsetY),new Point(imageIndexX*32,imageIndexY*16),sz) 
		{
			this.id = id;
			dir[angle1] = true;
			dir[angle2] = true;
		}

		/// <summary>
		/// どの方向にレールが伸びているか。trueなら伸びている
		/// </summary>
		private readonly bool[] dir = new bool[8];

		public bool hasRail( Direction d ) { return dir[d.index]; }
		public bool hasRail( int index )   { return dir[index]; }

		private readonly int id;
		
		/// <summary>
		/// this rail road has up/down in z-axis to this direction.
		/// </summary>
		private int _zdiff;
		private Direction _zangle;

		private Direction _climbDir =null;

		/// <summary>
		/// If this is a slope rail, the rail is climbing to
		/// this direcion. Otherwise null.
		/// </summary>
		public Direction climbDir { get { return _climbDir; } }

		private int _level;
		/// <summary>
		/// If this is a slope rail, this property holds the height
		/// of the rail.
		/// </summary>
		public int level { get { return _level; } }

		public int zdiff  { get { return _zdiff; } }
		public Direction zangle { get { return _zangle; } }

		/// <summary>
		/// Returns the number of connection edge of this pattern.
		/// Normally 2 but it will be 3 if this is a junction.
		/// </summary>
		public int numberOfRails {
			get {
				int i=0;
				foreach( bool b in dir )
					if(b)	i++;
				return i;
			}
		}

		/// <summary>
		/// These objects are serialized as singleton.
		/// </summary>
		public void GetObjectData( SerializationInfo info, StreamingContext context) {
			info.SetType(typeof(ReferenceImpl));
			info.AddValue("id",id);
		}
		
		[Serializable]
		internal sealed class ReferenceImpl : IObjectReference {
			private int id=0;
			public object GetRealObject(StreamingContext context) {
				foreach( RailPattern rp in singlePatterns )
					if(rp.id==id)	return rp;
				foreach( RailPattern rp in junctionPatterns )
					if(rp.id==id)	return rp;
				foreach( RailPattern rp in slopePatterns )
					if(rp.id==id)	return rp;
				foreach( RailPattern rp in slopeUGPatterns )
					if(rp.id==id)	return rp;
				
				Debug.Assert(false);
				return null;
			}
		}
	}
}
