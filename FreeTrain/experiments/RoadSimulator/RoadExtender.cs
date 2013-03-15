using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;

namespace RoadSimulator
{
	/// <summary>
	/// 
	/// </summary>
	internal class RoadExtender
	{		
		//デフォルトの道路の幅(lv0〜5
		static protected int[] def_width =new int[] {  3,  2, 2, 1, 1, 1};
		//同じレベルの平行する道路との最小間隔(lv0〜5
		static protected short[] min_length=new short[] { 24, 16,12, 8, 4, 2};
		//同じレベルの平行する道路との最大間隔(lv0〜5
		static protected short[] max_length=new short[] {200,100,40,20,15,10};

		//最初に設置されたレベル毎の(道路位置÷道路の最大間隔)
		protected short[] modH; 
		protected short[] modV; 
		protected Hashtable buds = new Hashtable();
		private IEnumerator ienum;
		private World world;

		public RoadExtender(World w)
		{
			ienum = ((Hashtable)buds.Clone()).Keys.GetEnumerator();
			world = w;
			world.onVoxelRemoving += new VoxelAreaListener(callbackRemove);
			//world.onPlayerSetRoad += new SetRoadListener(callbackRoadEnd);
			modH = new short[Configure.RoadLevelMax+1];
			modV = new short[Configure.RoadLevelMax+1];
			for(int i=0; i<=Configure.RoadLevelMax; i++)
			{
				modH[i]=-1;
				modV[i]=-1;
			}
		}

		// callback on removing voxel
		public void callbackRemove( Rectangle r ) 
		{
			Debug.WriteLine(r);
		}

		#region public functions maybe called from UI
		public void buildRoad(int level, int x, int y, int length, Direction dir)
		{
			if( length < 0  || !world.isInWorld(x,y)) return;
			switch(dir) 
			{
				case Direction.EAST:
					buildRoadE(level,(short)x,(short)y,(short)length,null);
					break;
				case Direction.WEST:
					buildRoadW(level,(short)x,(short)y,(short)length,null);
					break;
				case Direction.NORTH:
					buildRoadN(level,(short)x,(short)y,(short)length,null);
					break;
				case Direction.SOUTH:
					buildRoadS(level,(short)x,(short)y,(short)length,null);
					break;
				default:
					Debug.Assert(false);
					break;
			}
		}


		// 次の拡張を進める
		public void extendStep()
		{
			if(setEnumerator())
				while(extend((vector)ienum.Current));
		}
		// 一巡するまで拡張を進める
		public void extendPhase()
		{
			if(setEnumerator())
				do
				{
					extend((vector)ienum.Current);				
				}while(ienum.MoveNext());
		}
		#endregion

		#region the core of the road extending algorithm

		// retruns true if no road is extended.
		protected bool extend( vector key )
		{
			RoadBud bud = (RoadBud)buds[key];
			if( bud == null) return false;
			// remove if the road is already exist.
			PointS at = PointS.pointFrom(key.x,key.y,key.dir,1);
			Voxel toward = world[at.x,at.y];
			if( toward.road != null && toward.road.getLevel(key.dir) <= bud.level ) 
			{
				removeBud(key);
				return true;
			}
			// sleep check
			bool f = bud.sleeping;
			if(f)
			{
				Debug.Write(".");
				bud.stepSleep();
			}
			else
			{
				removeBud(key);
				int length = min_length[bud.level];
				PointS p = adjustPoint(bud.level,key.x,key.y,length,key.dir);
				if( p==null || p.IsEmpty )	return true;
				extendRoad(bud,p,length,key.dir);
				if( bud.level < Configure.noTrunkLevel )
					makeBranch(bud,p,length,key.dir);
				Debug.WriteLine("extends from ("+p.x+","+p.y+") to "+key.dir+" by "+length+" voxels.");
			}
			return f;

		}

		protected PointS adjustPoint(int level, short x, short y, int length, Direction dir)
		{
			PointS p = new PointS(x,y);
			short start;
			if(dir == Direction.SOUTH || dir == Direction.NORTH )
				start = y;
			else
				start = x;			

			int l = min_length[level];
			Direction dirR = DirConvertor.rotR(dir);
			Direction dirL = DirConvertor.rotL(dir);
			int[] bR = getNearestBranches(PointS.pointFrom(p,dirR,1),dirR,l*4/3);
			int[] bL = getNearestBranches(PointS.pointFrom(p,dirL,1),dirL,l*4/3);
			int dR = bR[level];
			int dL = bL[level];
			int sR = Math.Abs(dR-start);
			int sL = Math.Abs(dL-start);
			int s = Math.Abs(dR-dL);
			if( dR>=0 )
			{
				// too close branch on ether side
				if( dL>=0 && s<l)
				{
					Debug.WriteLine("bud canceled 0");
					return null;
				}
				// close branch on right side
				if( sR<l ) 
				{
					// can shift left side?
					if( sL-l>l-sR )
					{
						p = PointS.pointFrom(p,DirConvertor.rotL(dir),(short)(l-sR));
						bR[Configure.RoadLevelMax]=-1;
						return adjustPoint2(p,level,dir,ref bR,ref bL);
					}
					else 
					{
						Debug.WriteLine("bud canceled R");
						return null;
					}
				}
			}
			if( dL>=0 )
			{
				// close branch on left side
				if( sL<l ) 
				{
					// can shift right side?
					if( sR-l>l-sL )
					{
						p = PointS.pointFrom(p,DirConvertor.rotL(dir),(short)(l-sL));
						bL[Configure.RoadLevelMax]=-1;
						return adjustPoint2(p,level,dir,ref bR,ref bL);
					}
					else
					{
						Debug.WriteLine("bud canceled L");
						return null;
					}
				}
			}
			return adjustPoint2(p,level,dir,ref bR,ref bL);
		}

		protected PointS adjustPoint2(PointS src, int level, Direction dir, ref int[] right, ref int[] left)
		{
			PointS p = src;
			// overlap to nearest lower level road (if exist)
			if( level < Configure.RoadLevelMax )
			{
				int start;
				if(dir == Direction.SOUTH || dir == Direction.NORTH )
					start = p.y;
				else
					start = p.x;			
				int l = min_length[Configure.RoadLevelMax];
				int dR = right[Configure.RoadLevelMax];
				int dL = left[Configure.RoadLevelMax];
				int sR = Math.Abs(dR-start);
				int sL = Math.Abs(dL-start);
				if(dR>=0)
				{
					if( dL>=0)
					{
						if( sL<sR ) 
						{
							if(sL<l)
								p = PointS.pointFrom(p,DirConvertor.rotL(dir),(short)(l-sL));
						}
						else
						{
							if(sR<l)
								p = PointS.pointFrom(p,DirConvertor.rotR(dir),(short)(l-sR));
						}
					}
					else if(sR<l)
						p = PointS.pointFrom(p,DirConvertor.rotR(dir),(short)(l-sR));
				}
				else if( dL>=0 )
				{
					if(sL<l)
						p = PointS.pointFrom(p,DirConvertor.rotL(dir),(short)(l-sL));
				}
					
			}
			return p;
		}

		protected void makeBranch(RoadBud bud, PointS p, int length, Direction dir)
		{			
			int[] b = getLastBranches(bud,p,dir);
			int d_min = min_length[Configure.RoadLevelMax];

			// prepare arguments
			int start;
			if(dir == Direction.SOUTH || dir == Direction.NORTH )
				start = p.y;
			else
				start = p.x;			
			int last = Math.Abs(b[Configure.RoadLevelMax]-start);

			int n = length/d_min+1;
			// points proposed for branch
			short[] grid = new short[n];
			// set first point
			grid[0] = (short)world.randEx(0,2);
			if(last<d_min)
				grid[0] += (short)d_min;
			// set followed points
			for(int i=1; i<n; i++ )
				grid[i]=(short)(grid[i-1]+d_min+world.randEx(0,2));
			
			// sleep count of the bud
			int[] counter = new int[n];
						
			for(int lv=bud.level; lv<=Configure.RoadLevelMax; lv++)
			{
				for(int i=0; i<n; i++ )
					counter[i]=-1;
				last = Math.Abs(b[lv]-start);
				makeBranch2(lv, length, last, ref grid, ref counter);
				for(int i=0; i<n; i++ )
				{
					if(	counter[i]>-1 )
					{
						PointS p2 = PointS.pointFrom(p,dir,grid[i]);
						addBud(p2.x,p2.y,lv,DirConvertor.rotL(dir),counter[i]);
						addBud(p2.x,p2.y,lv,DirConvertor.rotR(dir),counter[i]);
						if( dir == Direction.EAST || dir == Direction.WEST)
							bud.lastBranch[lv]=p2.x;
						else
							bud.lastBranch[lv]=p2.y;		
					}
				}
			}
		}

		private void makeBranch2(int level, int length, int last, ref short[] grid, ref int[] counter)
		{
			int d1 = max_length[level];
			int d2 = min_length[level];
			int n = d1/d2;
			int i=0;
			int j=1;
			int c=0;
			if( last < d2 )
				i+=d2-last;
			while( i<length )
			{
				int n1 = world.randEx(n/2+1,n);
				i+= n1*d2;
				int cnt = 0;
				if( c%n>0 )
					cnt = world.randEx(c%n*50,25);
				c++;
				while(j<grid.Length && grid[j]<i )
					j++;
				if( j==grid.Length ) break;
				if( grid[j]-i < i-grid[j-1] )
					counter[j]=cnt;
				else
					counter[j-1]=cnt;
			}
			return;
		}
	
		protected int[] getLastBranches(RoadBud bud, PointS p, Direction dir)
		{
			if( !bud.sprouted || bud.lastBranch==null )
			{
				short w = min_length[0];
				int n = w*2;
				Direction r = DirConvertor.reverse(dir);
				int[] b0 = getNearestBranches(p,r,n);
				int[] bR = getNearestBranches(PointS.pointFrom(p,DirConvertor.rotR(dir),w),r,n);
				int[] bL = getNearestBranches(PointS.pointFrom(p,DirConvertor.rotL(dir),w),r,n);
				if(dir == Direction.WEST || dir == Direction.NORTH )
				{
					for( int i=0; i<b0.Length; i++ )
					{
						if( bR[i]>b0[i] ) b0[i] = bR[i];
						if( bL[i]>b0[i] ) b0[i] = bL[i];
					}
				}
				else
				{
					for( int i=0; i<b0.Length; i++ )
					{
						if( bR[i]>0 && bR[i]<b0[i] ) b0[i] = bR[i];
						if( bL[i]>0 && bL[i]<b0[i] ) b0[i] = bL[i];
					}
				}
				bud.lastBranch = b0;
				bud.sprouted = true;
			}
			// high level branch is substitutable for lower one
			int v;
			if(dir == Direction.WEST || dir == Direction.NORTH )
				v = -1;
			else
				v = 1;
			for( int i=1; i<=Configure.RoadLevelMax; i++ )
			{
				if(bud.lastBranch[i-1]!=-1 )
				{
					if(bud.lastBranch[i]==-1 )
						bud.lastBranch[i]=bud.lastBranch[i-1];
					else 
						if((bud.lastBranch[i-1]-bud.lastBranch[i])*v>0)
						bud.lastBranch[i]=bud.lastBranch[i-1];
				}
			}
			return bud.lastBranch;
		}

		protected int[] getNearestBranches(PointS p, Direction dir, int length)
		{
			int[] ret = new int[Configure.RoadLevelMax+1];
			for( int i=0; i<ret.Length; i++ ) ret[i]=-1;

			if( p.x < 0 || p.x >= world.xWidth ) return ret;
			if( p.y < 0 || p.y >= world.yWidth ) return ret;

			short vx = 0;
			short vy = 0;
			switch(dir) 
			{
				case Direction.EAST:
					vx =+1;
					break;
				case Direction.WEST:
					vx =-1;
					break;
				case Direction.NORTH:
					vy =-1;
					break;
				case Direction.SOUTH:
					vy =+1;
					break;
				default:
					Debug.Assert(false);
					break;
			}
			// find flag: if all level branch found, becomes zero.
			int f = (1<<Configure.RoadLevelMax)-1;
			// scanning range max.
			int x = p.x;
			int y = p.y;
			Direction dirL = DirConvertor.rotL(dir);
			Direction dirR = DirConvertor.rotR(dir);
			for( int i=0; i<length; i++ )
			{
				Road r = world[x,y].road;
				if( r != null )
				{
					int lv = Math.Min(r.getLevel(dirR),r.getLevel(dirL));
					if( lv <= Configure.RoadLevelMax && ret[lv]==-1 )
					{
						// store x if dir is E or W otherwise store y.
						ret[lv]=Math.Abs(x*vx+y*vy);
						// reset flag
						f ^= (1<<lv);
						// all nearest branch found
						if( f== 0 ) break;
					}
				}
				x+=vx;
				y+=vy;
				if( !world.isInWorld(x,y) ) break;
			}
			return ret;
		}
		#endregion

		#region non-public functions which set road on voxels
		protected void extendRoad(RoadBud bud, PointS p, int length, Direction dir)
		{
			if( length < 0  || !world.isInWorld(p.x,p.y)) return;
			switch(dir) 
			{
				case Direction.EAST:
					buildRoadE(bud.level,p.x,p.y,(short)length,bud);
					break;
				case Direction.WEST:
					buildRoadW(bud.level,p.x,p.y,(short)length,bud);
					break;
				case Direction.NORTH:
					buildRoadN(bud.level,p.x,p.y,(short)length,bud);
					break;
				case Direction.SOUTH:
					buildRoadS(bud.level,p.x,p.y,(short)length,bud);
					break;
				default:
					Debug.Assert(false);
					break;
			}
		}

		protected void buildRoadN(int level, short x, short y, int length, RoadBud bud)
		{
			if( world[x,y].canBuildRoad ) 
			{
				if( bud==null)
					addBud(x,y,level,Direction.SOUTH);
				// north bound check
				int y2 = y-length;
				if( y2 <= 0 )
				{
					length = y;
					if( length <= 0 ) return;
					y2 = 0;
				}
				// set road
				world[x,y].buildRoad(level,Direction.NORTH,Direction.NORTH);
				int iy;
				for(iy=0; iy<length; iy++)
				{
					Voxel v = world[x,y-iy];
					if( !v.canBuildRoad ) break;
					v.buildRoad(level,Direction.NORTH,Direction.SOUTH);
				}
				if(iy == length)
				{
					world[x,y2].buildRoad(level,Direction.SOUTH,Direction.SOUTH);
					if(bud==null)
						addBud(x,(short)y2,level,Direction.NORTH);
					else
						addBud(bud,x,(short)y2,Direction.NORTH);
				}
				if( modV[level] == -1 )
					modV[level] = (short)(x%max_length[level]);
				world.onVoxelUpdated(new Rectangle(x,y2,1,length));
			}
		}
		protected void buildRoadS(int level, short x, short y, int length, RoadBud bud)
		{
			if( world[x,y].canBuildRoad ) 
			{
				if( bud==null)
					addBud(x,y,level,Direction.NORTH);
				// south bound check
				int y2 = y+length;
				if( y2 >= world.yWidth )
				{
					length = world.yWidth-y-1;
					if( length <= 0 ) return;
					y2 = world.yWidth-1;
				}
				// set road
				world[x,y].buildRoad(level,Direction.SOUTH,Direction.SOUTH);
				int iy;
				for(iy=0; iy<length; iy++)
				{
					Voxel v = world[x,y+iy];
					if( !v.canBuildRoad ) break;
					v.buildRoad(level,Direction.NORTH,Direction.SOUTH);
				}

				if(iy == length)
				{
					world[x,y2].buildRoad(level,Direction.NORTH,Direction.NORTH);
					if( bud==null)
						addBud(x,(short)y2,level,Direction.SOUTH);
					else
						addBud(bud,x,(short)y2,Direction.SOUTH);
				}
				if( modV[level] == -1 )
					modV[level] = (short)(x%max_length[level]);
				world.onVoxelUpdated(new Rectangle(x,y,1,length));
			}
		}

		protected void buildRoadE(int level, short x, short y, int length, RoadBud bud)
		{
			if( world[x,y].canBuildRoad ) 
			{
				if( bud==null)
					addBud(x,y,level,Direction.WEST);
				// east bound check
				int x2 = x+length;
				if( x2 >= world.xWidth )
				{
					length = world.xWidth-x-1;
					if( length <= 0 ) return;
					x2 = world.xWidth-1;
				}
				// set road
				world[x,y].buildRoad(level,Direction.EAST,Direction.EAST);
				int ix;
				for(ix = 0; ix<length; ix++)
				{
					Voxel v = world[x+ix,y];
					if( !v.canBuildRoad ) break;
					v.buildRoad(level,Direction.WEST,Direction.EAST);
				}

				if(ix == length)
				{
					world[x2,y].buildRoad(level,Direction.WEST,Direction.WEST);
					if( bud==null)
						addBud((short)x2,y,level,Direction.EAST);
					else
						addBud(bud,(short)x2,y,Direction.EAST);
				}
				if( modH[level] == -1 )
					modH[level] = (short)(y%max_length[level]);
				world.onVoxelUpdated(new Rectangle(x,y,length,1));
			}
		}
		protected void buildRoadW(int level, short x, short y, int length, RoadBud bud)
		{
			if( world[x,y].canBuildRoad ) 
			{
				if( bud==null)
					addBud(x,y,level,Direction.EAST);
				// west bound check
				int x2 = x-length;
				if( x2 <= 0 )
				{
					length = x;
					if( length <= 0 ) return;
					x2 = 0;
				}
				// set road
				world[x,y].buildRoad(level,Direction.WEST,Direction.WEST);
				int ix;
				for(ix = 0; ix<length; ix++)
				{
					Voxel v = world[x-ix,y];
					if( !v.canBuildRoad ) break;
					v.buildRoad(level,Direction.WEST,Direction.EAST);
				}

				if(ix == length)
				{
					world[x2,y].buildRoad(level,Direction.EAST,Direction.EAST);
					if( bud==null)
						addBud((short)x2,y,level,Direction.WEST);
					else
						addBud(bud,(short)x2,y,Direction.WEST);
				}
				if( modH[level] == -1 )
					modH[level] = (short)(y%max_length[level]);
				world.onVoxelUpdated(new Rectangle(x2,y,length,1));
			}
		}
		#endregion

		#region primitive functions that add, remove and check 'Bud'.
		internal bool addBud( RoadBud bud, short x, short y, Direction dir ) 
		{
			PointS at = PointS.pointFrom(x,y,dir,1);
			if( !world.isInWorld(at.x,at.y) )
				return false;
			Voxel toward = world[at.x,at.y];
			if( toward.road != null && toward.road.getLevel(dir) <= bud.level )
				return false;
			vector key = new vector(x,y,dir);
			if( buds.ContainsKey(key) )
				buds.Remove(key);
			buds.Add(key,bud);
			Debug.WriteLine("new bud at:("+x+","+y+") to "+dir);
			return true;
		}
		internal bool addBud( short x, short y, int level, Direction dir ) 
		{
			return addBud(x,y,level,dir,0);
		}
		internal bool addBud( short x, short y, int level, Direction dir, int sleep) 
		{
			// ignore if the road is already exist.
			PointS at = PointS.pointFrom(x,y,dir,1);
			if( !world.isInWorld(at.x,at.y) )
				return false;
			Voxel toward = world[at.x,at.y];
			if( toward.road != null && toward.road.getLevel(dir) <= level )
				return false;
			vector key = new vector(x,y,dir);
			if( buds.ContainsKey(key) )
				return false;
			RoadBud bud = new RoadBud(level,sleep);
			buds.Add(key,bud);
			Debug.WriteLine("new bud at:("+x+","+y+") to "+dir);
			return true;
		}

		protected void removeBud( vector key) 
		{
			if( buds.ContainsKey(key) )
				buds.Remove(key);
		}

		protected void removeBud( short x, short y, Direction dir ) 
		{
			removeBud(new vector(x,y,dir));
		}

		protected void removeBud( short x, short y ) 
		{
			removeBud(x,y,Direction.EAST);
			removeBud(x,y,Direction.WEST);
			removeBud(x,y,Direction.NORTH);
			removeBud(x,y,Direction.SOUTH);
		}

		protected bool isExistBud( short x, short y, Direction dir ) 
		{
			return( buds.ContainsKey(new vector(x,y,dir)) );
		}

		protected bool isExistBud( short x, short y ) 
		{
			return(
				isExistBud(x,y,Direction.EAST) || 
				isExistBud(x,y,Direction.WEST) ||
				isExistBud(x,y,Direction.NORTH)|| 
				isExistBud(x,y,Direction.SOUTH));
		}
		#endregion

		#region other non-public functions
		private bool setEnumerator()
		{
			if( buds.Count == 0 )
				return false;
			if( !ienum.MoveNext() ) 
			{				
				ienum = ((Hashtable)buds.Clone()).Keys.GetEnumerator();
				return ienum.MoveNext();
			}
			else
				return true;
		}
		#endregion

		#region the vector class
		protected class vector 
		{
			public readonly Int16 x;
			public readonly Int16 y;
			public readonly Direction dir;

			public vector( Int16 x, Int16 y, Direction d )
			{
				this.x = x;
				this.y = y;
				this.dir = d;
			}

			// hashtableのキーとするため、EqualsとGetHashCodeをオーバーライドする。
			// 座標と方向が全て同じなら、等しいと見なす。
			public override bool Equals(Object o) 
			{
				if( o is vector ) 
				{
					vector v2 = (vector)o;
					return (v2.x==this.x && v2.y==this.y && v2.dir==this.dir );
				}
				else
					return false;
			}

			// キーは　x(15bit)+ y(15bit)+ dir(2bit) = 32bit 
			// この仕組みのためX,Yの最大値は2^14-1(=約16万)
			public override int GetHashCode()
			{
				Int32 key = (x<<17)+(y<<2)+(Int16)dir;
				return 0;
			}
		}
		#endregion

		#region the short point class
		protected class PointS
		{
			public short x;
			public short y;
			public PointS()
			{
				this.x = -1;
				this.y = -1;
			}
			public PointS(short _x, short _y)
			{
				this.x = _x;
				this.y = _y;
			}
			public PointS(int _x, int _y)
			{
				this.x = (short)_x;
				this.y = (short)_y;
			}
			public PointS(PointS src)
			{
				this.x = src.x;
				this.y = src.y;
			}
			public bool IsEmpty{ get{ return (x<0||y<0);} }

			public static PointS pointFrom( short x, short y, Direction dir, short length)
			{
				switch(dir) 
				{
					case Direction.EAST:
						x += length;
						break;
					case Direction.WEST:
						x -= length;
						break;
					case Direction.NORTH:
						y -= length;
						break;
					case Direction.SOUTH:
						y += length;
						break;
					default:
						Debug.Assert(false);
						break;
				}
				return new PointS(x,y);
			}
			public static PointS pointFrom( PointS p, Direction dir, short length)
			{
				return pointFrom(p.x,p.y,dir,length);
			}
		}
		#endregion
	}
}
