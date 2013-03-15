using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using freetrain.framework.graphics;

namespace freetrain.world.rail
{
	/// <summary>
	/// Slope rail road.
	/// 
	/// Consists of 8 voxels. Four voxels are TrafficVoxels,
	/// Two more are SlopeFillerVoxels, which are invisible,
	/// and the other two are SlopeSupport voxels, which are visible.
	/// </summary>
	[Serializable]
	public class SlopeEntity : Entity
	{
		private Cube cube;
		public SlopeEntity(Location start,Direction dir) {
			Location end = new Location(start.x+dir.offsetX*3,start.y+dir.offsetY*3,start.z+1);
			cube = Cube.createInclusive(start, end);
		}
		public int entityValue { get { return 0; } }
		public bool isOwned { get { return true; } }
		public bool isSilentlyReclaimable { get { return false; } }
		public void remove() {
			foreach(Voxel v in cube.voxels){
				if(v.entity==this && !(v is BridgePierVoxel))
					World.world.remove(v);
				else {
					TrafficVoxel tv = v as TrafficVoxel;
					if(tv!=null){
						SlopeRailRoad srr = tv.railRoad as SlopeRailRoad;
						if(srr!=null && srr.entity==this)
							tv.remove();						
					}
				}
//				if(v.location.z==cube.z1)
//					BridgePierVoxel.teardownBridgeSupport(v.location,this);
			}
			if(onEntityRemoved!=null)
				onEntityRemoved(this,null);
		}
		public object queryInterface(Type aspect) { return null; }
		public event EventHandler onEntityRemoved;
//		public object GetRealObject(StreamingContext sc) { return theInstance; }
	}

	/// <summary>
	/// Railroad with slope.
	/// </summary>
	[Serializable]
	public class SlopeRailRoad : RailRoad
	{
		
		internal SlopeEntity entity;
		private SlopeRailRoad(SlopeEntity e,TrafficVoxel v,RailPattern rp) :base(v,rp) {
			entity = e;
		}


		public override void invalidateVoxel() {
			// this voxel and the voxel below/above needs to be updated
			World.world.onVoxelUpdated(voxel);
			
			Location loc = location;
			if( pattern.level<2 )	// the voxel above
				loc.z++;
			else
				loc.z--;
			World.world.onVoxelUpdated(loc);
		}

		public Direction climbDir { get { return pattern.climbDir; } }
		public int level { get { return pattern.level; } }

		// can't attach rail to slope. So the only possibility is that
		// we already have a railroad to that direction
		public override bool attach( Direction dir ) { return hasRail(dir); }
		public override bool canAttach( Direction dir ) { return hasRail(dir); }

		// similarly, can't detach
		public override void detach( Direction d1, Direction d2 ) {
			;
		}

		public override Direction guide() {
			// slop rails don't curve. so a car should be
			// able to go to the same direction
			Direction d = voxel.car.state.asInside().direction;
			Debug.Assert( hasRail(d) );
			return d;
		}




		private const int SLOPE_CONSTRUCTION_UNIT_COST = 400000;	// taken from A4.
		private const int SLOPE_DESTRUCTION_UNIT_COST  =  80000;




		/// <summary>
		/// Gets the SlopeRailRoad object of the specified location, if any.
		/// Otherwise null.
		/// </summary>
		public static new SlopeRailRoad get( Location loc ) {
			RailRoad rr = RailRoad.get(loc);
			if(rr is SlopeRailRoad)	return (SlopeRailRoad)rr;
			else	return null;
		}

		/// <summary>
		/// Creates a new slope. A slope consists of four consective
		/// blocks of railroads. The base parameter specifies the location
		/// of the lowest railroad and the direction parameter
		/// specifies the direction to climb.
		/// 
		/// The caller must use the canCreateSlope method to check
		/// if this method can be invoked.
		/// </summary>
		public static void createSlope( Location _base, Direction dir ) {
			Debug.Assert(canCreateSlope(_base,dir));

			// charge the cost before we alter something
			accounting.AccountGenre.RAIL_SERVICE.spend( calcCostOfNewSlope(_base,dir) );
			
			SlopeEntity entity = new SlopeEntity(_base, dir);

			for( int i=0; i<4; i++ ) {
				if( _base.z < World.world.getGroundLevel(_base))
				{
					new SlopeRailRoad(entity, TrafficVoxel.getOrCreate(
						_base.x, _base.y, _base.z+(i/2) ),
						RailPattern.getUGSlope( dir, i ) );
					if(i<2) 
					{
						// space filler
						new SlopeFillerVoxel( entity, _base.x, _base.y, _base.z+1, i );
					} 
					else 
					{
						new SlopeSupportVoxel( entity, _base.x, _base.y, _base.z, i,
							RailPattern.slopeWalls[ dir.index+i-2 ] );
					}
				}
				else
				{
					new SlopeRailRoad(entity, TrafficVoxel.getOrCreate(
						_base.x, _base.y, _base.z+(i/2) ),
						RailPattern.getSlope( dir, i ) );
					if(i<2) 
					{
						// space filler
						new SlopeFillerVoxel( entity, _base.x, _base.y, _base.z+1, i );
					} 
					else 
					{
						new SlopeSupportVoxel( entity, _base.x, _base.y, _base.z, i,
							RailPattern.slopeSupports[ dir.index+(i-2) ] );
					}
				}

				Type bridgeStyle;
				if( dir==Direction.NORTH || dir==Direction.EAST )
					bridgeStyle = typeof(BridgePierVoxel.DefaultImpl);
				else
					bridgeStyle = typeof(BridgePierVoxel.SlopeNEImpl);
				BridgePierVoxel.electBridgeSupport(_base,bridgeStyle,entity);

				_base += dir;
			}
		}

		/// <summary>
		/// Used for upper two invisible voxels
		/// </summary>
		[Serializable]
		internal class SlopeFillerVoxel : EmptyVoxel, HoleVoxel
		{
			internal SlopeFillerVoxel( SlopeEntity entity, int x, int y, int z, int idx )
				: base(entity,x,y,z) {
				
				int glevel = World.world.getGroundLevel(location);
				drawSurfaceBelow = !( idx<2 && glevel>=z );
				drawSurfaceAbove = !(idx>=2 && glevel>=z+1);
			}
			

			private readonly bool drawSurfaceAbove;
			private readonly bool drawSurfaceBelow;
			public bool drawGround( bool above ) {
				return above?drawSurfaceAbove:drawSurfaceBelow;
			}
		}

		/// <summary>
		/// Used for lower two voxels. Visible but not rail road.
		/// </summary>
		[Serializable]
		internal class SlopeSupportVoxel : EmptyVoxel, HoleVoxel
		{
			internal SlopeSupportVoxel( SlopeEntity entity, int x, int y, int z, int idx, Sprite s )
				: base(entity,x,y,z) {
				
				int glevel = World.world.getGroundLevel(location);
				drawSurfaceBelow = !( idx<2 && glevel>=z );
				drawSurfaceAbove = !(idx>=2 && glevel>=z+1);
				sprite = s;
			}
			public override bool transparent { get { return true; } }

			private readonly bool drawSurfaceAbove;
			private readonly bool drawSurfaceBelow;
			private readonly Sprite sprite;

			public bool drawGround( bool above ) {
				return above?drawSurfaceAbove:drawSurfaceBelow;
			}

			public override void draw( DrawContext surface, Point pt, int heightCutDiff ) {
				sprite.draw( surface.surface, pt );
			}
		}


		/// <summary>
		/// Return true if a slope RR can be built at the specified location.
		/// </summary>
		/// <param name="_base"></param>
		/// <param name="dir"></param>
		/// <returns></returns>
		public static bool canCreateSlope( Location _base, Direction dir ) {
			return calcCostOfNewSlope(_base,dir)!=0;
		}

		/// <summary>
		/// Compute a construction cost of a slope rail.
		/// </summary>
		/// <returns>If a construction is impossible, return 0</returns>
		public static int calcCostOfNewSlope( Location _base, Direction dir ) {
			if(!dir.isSharp)	return 0;

			if(_base.z==World.world.size.z-1)
				return 0;	// we can't go above the ceil

			// 8 voxels around (depth-4 height-2) must be completely available.
			// it's not even OK to have a TrafficVoxel.
			for( int i=0; i<4; i++ ) {
				if( World.world[ _base ]!=null )						return 0;
				if( World.world[ _base.x, _base.y, _base.z+1]!=null)	return 0;

				_base += dir;
			}

			return SLOPE_CONSTRUCTION_UNIT_COST*Math.Max(1, _base.z-World.world.waterLevel);
		}

		public static bool canRemoveSlope( Location loc, Direction dir ) {
			return calcCostOfTearDownSlope(loc,dir)!=0;
		}

		/// <summary>
		/// Compute the cost of destructing a slope rail.
		/// </summary>
		/// <returns>If a destruction is impossible, return 0</returns>
		public static int calcCostOfTearDownSlope( Location loc, Direction dir ) {
			// make sure the first voxel is not occupied by a car
			if( Car.get(loc)!=null )	return 0;

			// the 2nd block has a distinctive zangle and zdiff. check it.
			loc += dir;
			RailRoad rr = RailRoad.get(loc);
			if(!(rr is SlopeRailRoad))	return 0;
			SlopeRailRoad srr = (SlopeRailRoad)rr;
			
			if(!(srr.pattern.zangle==dir && srr.pattern.zdiff==1))
				return 0;

			// make sure the 2nd rail is not occupied by a car
			if( Car.get(loc)!=null )	return 0;

			// check 3rd and 4th rails.
			loc += dir;
			loc.z++;
			if( Car.get(loc)!=null )	return 0;
			loc += dir;
			if( Car.get(loc)!=null )	return 0;

			return SLOPE_DESTRUCTION_UNIT_COST*Math.Max(1, loc.z-World.world.waterLevel);
		}

		/// <summary>
		/// Removes a slope. The format of the parameters are the same
		/// as the createSlope method. Ut us 
		/// </summary>
		public static void removeSlope( Location loc, Direction dir ) {
			Debug.Assert(canRemoveSlope(loc,dir));

			// charge the cost before we alter something
			accounting.AccountGenre.RAIL_SERVICE.spend( calcCostOfTearDownSlope(loc,dir) );

			for( int i=0; i<4; i++ ) {
				TrafficVoxel v = TrafficVoxel.get( loc.x, loc.y, loc.z+(i/2) );
				v.railRoad = null;
				
				Location l = loc;
				l.z += -(i/2)+1;
				Debug.Assert( World.world[l] is EmptyVoxel );
				World.world.remove(l);

				BridgePierVoxel.teardownBridgeSupport(loc,v);

				loc += dir;
			}
		}
	}
}
