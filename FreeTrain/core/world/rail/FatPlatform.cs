using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using org.kohsuke.directdraw;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.util;

namespace freetrain.world.rail
{
	/// <summary>
	/// "Take the A-train" style fat platform.
	/// </summary>
	[Serializable]
	public class FatPlatform : Platform
	{
		/// <summary>
		/// Returns true if a platform can be built under the specified condition.
		/// This includes room for lane 0.
		/// </summary>
		public static bool canBeBuilt( Location loc, Direction d, int length ) {
			if(!d.isSharp)	return false;	// incorrect direction

			Location laneLoc = loc + d.right90;
			if(!canAddLane(laneLoc,d,length))	return false;	// can't have lane 0

			for( ; length>0; length-- ) {
				if( World.world[loc]!=null )
					return false;	// already occupied
				loc += d;
			}

			return true;	// enough space
		}
		

		public FatPlatform( Location loc, Direction dir, int len) : base(loc,dir,len) {
			Debug.Assert( canBeBuilt(loc,dir,length) );

			voxels = new FatPlatformVoxel[length];
			for( int i=0; i<length; i++ ) {
				voxels[i] = new FatPlatformVoxel( this, loc, getSprite(dir) );
				loc += dir;
			}

			addLane(0);
		}


		/// <summary>
		/// Checks if this platform can be removed.
		/// </summary>
		public override bool canRemove {
			get {
				// make sure that there are no trains on either lanes
				foreach( YardRailRoad[] yrrs in lanes ) {
					if(yrrs!=null) {
						foreach( YardRailRoad yrr in yrrs ) {
							if(yrr.voxel.car!=null)
								return false;
						}
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Removes this platform from the world.
		/// </summary>
		public override void remove() {
			World world = World.world;

			onHostDisconnected();


			foreach( YardRailRoad[] yrrs in lanes ) {
				if(yrrs!=null) {
					foreach( YardRailRoad yrr in yrrs ) {
						// canRemove must be true before this method is called.
						Debug.Assert( yrr.voxel.car==null );

						Location loc = yrr.location;
						yrr.voxel.railRoad = null;
						new SingleRailRoad(
							TrafficVoxel.getOrCreate(loc),
							RailPattern.get(direction, direction.opposite));
						world.onVoxelUpdated(loc);
					}
				}
			}

			// remove the platform itself
			foreach( FatPlatformVoxel pv in voxels ) {
				world.remove(pv.location);
				world.onVoxelUpdated(pv.location);
			}

			base.remove();
		}





		/// <summary>
		/// Platform voxels that constitutes a platform.
		/// </summary>
		private readonly FatPlatformVoxel[] voxels;


		/// <summary>Attached RRs or null.</summary>
		private YardRailRoad[][] lanes = new YardRailRoad[2][];

		public bool hasLane( int idx ) {
			return lanes[idx]!=null;
		}

		private Location getLaneBaseLocation(int idx) {
			if(idx==0)	return location+direction.right90;
			else		return location+direction.left90;
		}



		/// <summary>
		/// Returns true if a lane can be attached.
		/// </summary>
		public bool canAddLane( int idx ) {
			// already attached?
			if(hasLane(idx))	return false;

			return canAddLane( getLaneBaseLocation(idx), direction, length );
		}

		private static bool canAddLane( Location loc, Direction direction, int length ) {

			for( int i=0; i<length; i++, loc+=direction ) {
				Voxel v = World.world[loc];
				if(v==null)	continue;	// OK
				if(v is TrafficVoxel) {
					TrafficVoxel tv = (TrafficVoxel)v;
					// TODO can't add a lane if there is a car road
					if(tv.car!=null)
							return false;	// there is a obstacle
					if(tv.railRoad is SingleRailRoad) {
						if( Direction.angle( tv.railRoad.dir1, direction )%4 ==0
						&&  Direction.angle( tv.railRoad.dir2, direction )%4 ==0 )
							continue;	// this RR can be converted to a platform
					}
				}

				// otherwise there is an obstacle
				return false;
			}

			return true;
		}

		/// <summary> Adds a new lane to this railroad. </summary>
		public void addLane( int idx ) {
			Debug.Assert(canAddLane(idx));
			
			YardRailRoad[] rr = new YardRailRoad[length];
			lanes[idx] = rr;

			Location loc = getLaneBaseLocation(idx);
			for( int i=0; i<length; i++, loc+=direction )
				// change to the new rail.
				rr[i] = new RailRoadImpl( TrafficVoxel.getOrCreate(loc), this, i );
		}

		public bool canRemoveLane( int idx ) {
			if(!hasLane(idx))	return false;

			foreach( YardRailRoad rr in lanes[idx] )
				if(rr.voxel.car!=null)	return false;
			
			return true;
		}

		/// <summary>
		/// Removes an existing lane.
		/// </summary>
		public void removeLane( int idx ) {
			Debug.Assert(hasLane(idx));

			YardRailRoad[] rrs = lanes[idx];
			lanes[idx]=null;
			foreach( YardRailRoad rr in rrs ) {
				Location loc = rr.location;
				rr.voxel.railRoad = null;

				new SingleRailRoad(
					TrafficVoxel.getOrCreate(loc),
					rr.getPattern());
				World.world.onVoxelUpdated(loc);
			}
		}


		const int HOST_RANGE = 5;
		/// <summary>
		/// Lists available platform hosts for this platform.
		/// </summary>
		internal protected override PlatformHost[] listHosts() {
			return listHosts(HOST_RANGE);
		}

		/// <summary>Sprites of the platform voxel. indexed by the direction</summary>
		private static readonly Sprite[] sprites;
		public static Sprite getSprite( Direction d ) {
			return sprites[d.index/2];
		}
		static FatPlatform() {
			sprites = new Sprite[4];
			Picture bmp = ResourceUtil.loadSystemPicture("FatPlatform.bmp");	// don't dispose this surface
			sprites[0] = sprites[2] = new SimpleSprite( bmp, new Point(0,16), new Point( 0,0), new Size(32,32) );
			sprites[1] = sprites[3] = new SimpleSprite( bmp, new Point(0,16), new Point(32,0), new Size(32,32) );
		}
		

		/// <summary>
		/// Platform voxel.
		/// </summary>
		[Serializable]
		class FatPlatformVoxel : AbstractVoxelImpl, IDeserializationCallback
		{
			public FatPlatformVoxel( FatPlatform p, int x, int y, int z, Sprite _sprite )
				: this( p, new Location(x,y,z), _sprite ) {}

			public FatPlatformVoxel( FatPlatform p, Location loc, Sprite _sprite ) : base(loc) {
				this.owner = p;
				this.sprite = _sprite;
			}

			public readonly FatPlatform owner;

			public override Entity entity { get { return owner; } }
			
			[NonSerialized]
			private Sprite sprite;

			public void OnDeserialization(object sender) {
				// restore the sprite field
				sprite = getSprite(owner.direction);
			}

			public override void draw(DrawContext dc, Point pt, int heightCutDiff) {
				Surface display = dc.surface;

				sprite.draw(display,pt);
				if(owner.host==null && location==owner.location) {
					pt.X += 8;
					display.blt( pt, warningIcon );
				}
			}

			public override bool onClick() {
				owner.onClick();
				return true;
			}

			public override object queryInterface( Type aspect ) {
				if( aspect==typeof(TrainHarbor) )
					return owner.hostStation;
				return base.queryInterface(aspect);
			}
		}


		/// <summary>
		/// Gets a platform from its location, if any, or null otherwise.
		/// </summary>
		public new static FatPlatform get( Location loc ) {
			Voxel v = World.world[loc];
			if(v is FatPlatformVoxel)
				return ((FatPlatformVoxel)v).owner;
			else
				return null;
		}

		public new static FatPlatform get( int x, int y, int z ) { return get(new Location(x,y,z)); }



		[Serializable]
		internal class RailRoadImpl : YardRailRoad
		{
			public RailRoadImpl( TrafficVoxel v, FatPlatform _owner, int _idx ) : base(v,_owner,_idx) {}

			private static readonly Sprite theImage = 
				new SimpleSprite( ResourceUtil.loadSystemPicture("YardChip.bmp"),
					new Point(0,0), new Point(0,0), new Size(32,16) );

			public override void drawBefore( DrawContext display, Point pt ) {
				theImage.draw( display.surface, pt );
				base.drawBefore(display,pt);
			}
		}
	}
}
