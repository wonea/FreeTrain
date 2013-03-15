using System;
using System.Drawing;
using System.Diagnostics;

namespace RoadSimulator
{	
	public delegate void VoxelAreaListener( Rectangle r );
	//internal delegate void SetRoadListener( int level, int x, int y, int leng, Direction d );		
	/// <summary>
	/// 
	/// </summary>
	public class World
	{
		delegate void callback(int x, int y);

		static private int xMax = 800;
		static private int yMax = 800;		

		public VoxelAreaListener onVoxelRemoving = null;
		public VoxelAreaListener onVoxelUpdated = null;
		//internal SetRoadListener onPlayerSetRoad = null;
		private RoadExtender extender;
		protected Random rand = new Random();

		public World()
		{
			// 
			// TODO: コンストラクタ ロジックをここに追加してください。
			//
			createField(800,800);
			extender = new RoadExtender(this);
			onVoxelUpdated += new VoxelAreaListener(dummy);
		}

		static private Voxel[,] v;
		internal void createField( int x, int y)
		{
			v = new Voxel[x,y];
			for(int ix=0; ix<x; ix++ )
				for(int iy=0; iy<y; iy++)
					v[ix,iy]=new Voxel();

		}
		public int xWidth{get{ return xMax;}}
		public int yWidth{get{ return yMax;}}

		public Voxel this[int x, int y]
		{
			get {
				Debug.Assert(x>=0&& x<xMax);
				Debug.Assert(y>=0&& y<yMax);
				return v[x,y]; 
			}
		}

		public bool isInWorld(int x, int y)
		{
			return (x>=0&& x<xMax && y>=0&& y<yMax);
		}

		internal int randEx(int mean, int range)
		{
			return mean+rand.Next(range)-rand.Next(range);
		}

//		public Voxel this[int x, int y, Direction dir]
//		{
//			get 
//			{				
//				Point p = new Point(x,y);
//				switch( dir) 
//				{
//					case Direction.EAST:
//						p.X--;
//						break;
//					case Direction.WEST:
//						p.X++;
//						break;
//					case Direction.NORTH:
//						p.Y--;
//						break;
//					case Direction.SOUTH:
//						p.Y++;
//						break;
//				}
//				if( p.X<0 ) p.X=0;
//				else if(p.X>=this.xWidth) p.X=this.xWidth-1;
//				if( p.Y<0 ) p.Y=0;
//				else if(p.Y>=this.yWidth) p.Y=this.yWidth-1;
//				return this[p.X,p.Y];
//			}
//		}

		private void dummy(Rectangle r){}

		#region setting station
		public void setStationV(int level, int x, int y)
		{
			//道路の芽を駅の左右に設定
			extender.addBud((short)(x-1),(short)y,Configure.stationLevel,Direction.WEST);
			extender.addBud((short)(x+1),(short)y,Configure.stationLevel,Direction.EAST);
			//onPlayerSetRoad(Configure.stationLevel,x-1,y,2,Direction.EAST);

			//駅設置本文
			for(int iy=y-2; iy<=y+2; iy++)
				v[x,iy].buildStation(level);
			onVoxelUpdated(new Rectangle(x,y-2,x+1,y+2));

			if( Configure.stationLevel >= Configure.noTrunkLevel )
				return;
			//駅前道路の位置を決定
			Object b1=this[x-1,y].structure;
			Object b2=this[x+1,y].structure;
			int n;
			if( b1 != null )
				if( b2!= null )
					if( rand.Next(3) == 0 )
						n=1;
					else
						n=-1;
				else
					n=-1;
			else
				n=1;
			n *= Configure.MeanDistanceFromStation;
			//駅前道路(の芽)の設置
			int sleep = randEx(Configure.MeanDistanceFromStation,Configure.MeanDistanceFromStation);
			extender.addBud((short)(x+n),(short)y,Configure.stationLevel,Direction.NORTH,sleep);
			extender.addBud((short)(x+n),(short)y,Configure.stationLevel,Direction.SOUTH,sleep);

		}

		public void setStationH(int level, int x, int y)
		{
			//道路の芽を駅の上下に設定
			extender.addBud((short)x,(short)(y-1),Configure.stationLevel,Direction.NORTH);
			extender.addBud((short)x,(short)(y+1),Configure.stationLevel,Direction.SOUTH);
			//onPlayerSetRoad(Configure.stationLevel,x,y-1,2,Direction.SOUTH);

			//駅設置本文
			for(int ix=x-2; ix<=x+2; ix++)
				v[ix,y].buildStation(level);
			onVoxelUpdated(new Rectangle(x-2,y,x+2,y+1));

			if( Configure.stationLevel >= Configure.noTrunkLevel )
				return;
			//駅前平行道路の位置を決定
			Object b1=this[x,y-1].structure;
			Object b2=this[x,y+1].structure;
			int n;
			if( b1 != null )
				if( b2!= null )
					if( rand.Next(3) == 0 )
						n=1;
					else
						n=-1;
				else
					n=-1;
			else
				n=1;
			n *= Configure.MeanDistanceFromStation;
			//駅前道路(の芽)の設置
			int sleep = randEx(Configure.MeanDistanceFromStation,Configure.MeanDistanceFromStation);
			extender.addBud((short)x,(short)(y+n),Configure.stationLevel,Direction.EAST,sleep);
			extender.addBud((short)x,(short)(y+n),Configure.stationLevel,Direction.WEST,sleep);
		}
		#endregion

		#region gateway to the RoadExtender (contains setting road)
		public void nextStep() 
		{
			extender.extendStep();
		}

		public void nextPhase() 
		{
			extender.extendPhase();
		}

		#region setting road
		public void buildRoadV(int level, int x, int y0, int y1)
		{
			extender.buildRoad(level,x,Math.Min(y0,y1),Math.Abs(y1-y0),Direction.SOUTH);
		}

		public void buildRoadH(int level, int x0, int x1, int y)
		{
			extender.buildRoad(level,Math.Min(x0,x1),y,Math.Abs(x1-x0),Direction.EAST);
		}

		public void buildRoadRect(int level, Rectangle r )
		{
			// normalize rectangle
			if( r.Width<0 )
			{
				r.Offset(r.Width,0);
				r.Width = -r.Width;
			}
			if( r.Height<0 )
			{
				r.Offset(0,r.Height);
				r.Height = -r.Height;
			}

			extender.buildRoad(level,r.Left,r.Top,r.Width,Direction.EAST);
			extender.buildRoad(level,r.Left,r.Bottom,r.Width,Direction.EAST);
			extender.buildRoad(level,r.Left,r.Top,r.Height,Direction.SOUTH);
			extender.buildRoad(level,r.Right,r.Top,r.Height,Direction.SOUTH);
			r.Inflate(1,1);
			onVoxelUpdated(r);
		}
		#endregion
		#endregion

		#region voxel eraser
		public void eraseBox(Point p0, Point p1)
		{
			onVoxelRemoving(new Rectangle(p0,new Size(p1.X-p0.X,p1.Y-p0.Y)));
			setFilledBox(p0,p1,new callback(EraserSilent));
		}
		
		public void eraseLine(Point p0, Point p1)
		{
			if(p0.X == p1.X || p0.Y == p1.Y)
				eraseBox(p0,p1);
			else
				setLine(p0,p1,new callback(Eraser));
		}
		public void Eraser(int x, int y) 
		{
			onVoxelRemoving(new Rectangle(x,y,0,0));
			EraserSilent(x,y);
		}
		void EraserSilent(int x, int y) 
		{
			v[x,y].erase();
		}
		#endregion

		#region setting barrier
		public void barrierBox(Point p0, Point p1)
		{
			setFilledBox(p0,p1,new callback(setBarrier));
		}
		
		public void barrierLine(Point p0, Point p1)
		{
			setLine(p0,p1,new callback(setBarrier));
		}
		public void setBarrier(int x, int y) 
		{
			v[x,y].buildBarrier();
		}
		#endregion

		#region voxel setting helpers
		void setFilledBox(Point p0, Point p1, callback builder)
		{
			int x1=Math.Min(p0.X,p1.X);
			int	y1=Math.Min(p0.Y,p1.Y);
			int x2=Math.Max(p0.X,p1.X);
			int	y2=Math.Max(p0.Y,p1.Y);
			x1=Math.Max(x1,0);
			y1=Math.Max(y1,0);
			x2=Math.Min(x2,xMax);
			y2=Math.Min(y2,yMax);

			for(int x=x1; x<x2; x++)
				for(int y=y1; y<y2; y++)
					builder(x,y);
			onVoxelUpdated(new Rectangle(x1,y1,x2-x1,y2-y1));
		}

		void setLine(Point p0, Point p1, callback builder)
		{
			int w=p1.X-p0.X;
			int h=p1.Y-p0.Y;
			int x=Math.Min(p0.X,p1.X);
			int	y=Math.Min(p0.Y,p1.Y);
			int x2=Math.Max(p0.X,p1.X);
			int	y2=Math.Max(p0.Y,p1.Y);
			x=Math.Max(x,0);
			y=Math.Max(y,0);
			x2=Math.Min(x2,xMax);
			y2=Math.Min(y2,yMax);
			int aw = Math.Abs(w);
			int ah = Math.Abs(h);
			bool b = (w*h<0);

			if(aw>ah)
			{
				if(b)
					for(int i=0; i<=aw; i++)
						builder(x+i,y2-ah*i/aw);
				else
					for(int i=0; i<=aw; i++)
						builder(x+i,y+ah*i/aw);
			}
			else
			{
				if(b)
					for(int i=0; i<=ah; i++)
						builder(x2-aw*i/ah,y+i);
				else
					for(int i=0; i<=ah; i++)
						builder(x+aw*i/ah,y+i);
			}
			onVoxelUpdated(new Rectangle(x,y,aw+1,ah+1));
		}
		#endregion

	}
}
