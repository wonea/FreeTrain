using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.util;
using freetrain.framework;
using freetrain.framework.graphics;

namespace freetrain.world.rail
{
	/// <summary>
	/// Thin platform that doesn't oocupy any additional pixels
	/// </summary>
	[Serializable]
	public class ThinPlatform : Platform
	{
		/// <summary>
		/// Returns true if a platform can be built under the specified condition.
		/// This includes room for lane 0.
		/// </summary>
		public static bool canBeBuilt( Location loc, Direction dir, int length ) {
			if(!dir.isSharp)	return false;	// incorrect direction

			for( ; length>0; length--, loc+=dir ) {
				if( World.world[loc]==null )
					continue;		// this voxel is empty.OK.

				TrafficVoxel tv = TrafficVoxel.get(loc);
				if( tv==null )		return false;	// this voxel is occupied by something else.

				if(tv.car!=null)	return false;	// there is a car on RR

				if(tv.railRoad is SingleRailRoad) {
					if( Direction.angle( tv.railRoad.dir1, dir )%4 ==0
					&&  Direction.angle( tv.railRoad.dir2, dir )%4 ==0 )
						continue;	// this RR can be converted to a platform
				}

				return false;	// other RRs are not acceptable
			}

			return true;	// enough space
		}

		public ThinPlatform( Location loc, Direction dir, int len) : base(loc,dir,len) {
			Debug.Assert( canBeBuilt(loc,dir,length) );

			for( int i=0; i<len; i++, loc+=dir ) {
				bool hasRoof = ( length/4 <= i && i<length-length/4 );
				new RailRoadImpl( TrafficVoxel.getOrCreate(loc) ,this, i, hasRoof, plainPlatform );
			}
		}

		/// <summary>
		/// Checks if this platform can be removed.
		/// </summary>
		public override bool canRemove {
			get {
				// make sure that there are no trains
				Location loc = this.location;
				for( int i=0; i<length; i++, loc+=direction )
					if( TrafficVoxel.get(loc).car!=null )
						return false;

				return true;
			}
		}

		/// <summary>
		/// Removes this platform from the world.
		/// </summary>
		public override void remove() {
			World world = World.world;

			onHostDisconnected();

			Location loc = this.location;
			for( int i=0; i<length; i++, loc+=direction )
				new SingleRailRoad(
					TrafficVoxel.get(loc),
					RailPattern.get( direction, direction.opposite ) );

			base.remove();
		}




		/// <summary>Sprites of the platform </summary>
		private static readonly Sprite[] sprites = new Sprite[8];
		public static Sprite getSprite( Direction d, bool hasRoof ) {
			Debug.Assert( d.isSharp );
			return sprites[d.index+(hasRoof?1:0)];
		}

		static ThinPlatform() {
			Picture pic = PictureManager.get("{3FF9F902-6B2A-44A4-9C5F-DE8D82CFD37D}");
			for( int i=0; i<8; i++ )
				sprites[i] = new SimpleSprite( pic, new Point(0,8), new Point(i*32,0), new Size(32,24) );
		}






		const int HOST_RANGE = 3;
		/// <summary>
		/// Lists available platform hosts for this platform.
		/// </summary>
		internal protected override PlatformHost[] listHosts() {
			return listHosts(HOST_RANGE);
		}

		/// <summary>
		/// Obtains a reference to the RailRoadImpl of the specified index.
		/// </summary>
		private RailRoadImpl getRailRoad(int idx ) {
			Location loc = location;
			loc.x += direction.offsetX*idx;
			loc.y += direction.offsetY*idx;

			return (RailRoadImpl)RailRoad.get(loc);
		}


		public new static Platform get( Location loc ) {
			TrafficVoxel v = TrafficVoxel.get(loc);
			if(v==null)	return null;
			if(v.railRoad is RailRoadImpl)
				return ((RailRoadImpl)v.railRoad).owner;
			else
				return null;
		}

		

		/// <summary>
		/// RailRoad implementation for thin platform.
		/// </summary>
		[Serializable]
		internal class RailRoadImpl : YardRailRoad {
			internal RailRoadImpl( TrafficVoxel voxel, ThinPlatform _owner, int idx, bool _hasRoof, Outlook _outlook )
				: base(voxel,_owner,idx) {

				this._hasRoof = _hasRoof;
				this._outlook = _outlook;
			}

			public Direction direction { get { return owner.direction; } }

			private Outlook _outlook;

			/// <summary> Outlook of this platform. </summary>
			public Outlook outlook {
				get { return _outlook; }
				set {
					if( value!=_outlook)
						World.world.onVoxelUpdated(this.voxel);
					_outlook = value;
				}
			}

			private bool _hasRoof;

			/// <summary> True if this voxel should have a roof. </summary>
			public bool hasRoof {
				get { return _hasRoof; }
				set {
					if( value!=_hasRoof )
						World.world.onVoxelUpdated(this.voxel);
					_hasRoof = value;
				}
			}

			public override void drawBefore( DrawContext display, Point pt ) {
				base.drawBefore(display,pt);

				Direction d = direction;
				if( d==Direction.EAST || d==Direction.SOUTH )
					outlook.draw( this, display, pt );
			}
			public override void drawAfter( DrawContext display, Point pt ) {
				base.drawAfter(display,pt);

				Direction d = direction;
				if( d==Direction.WEST || d==Direction.NORTH )
					outlook.draw( this, display, pt );

				outlook.drawAfter( this, display, pt );

				if(owner.host==null && index==0) {
					pt.X += 8;
					display.surface.blt( pt, warningIcon );
				}
			}
			public override bool onClick() {
				owner.onClick();	// delegate the call to the owner
				return true;
			}

			public override object queryInterface( Type aspect ) {
				if( aspect==typeof(TrainHarbor) )
					return owner.hostStation;
				return base.queryInterface(aspect);
			}


			public bool isDoubleWidth {
				get {
					Location nloc = this.location + this.direction.left90;
					RailRoadImpl nrr = RailRoadImpl.get(nloc);
					return (nrr!=null) &&
						(nrr.outlook is PassagewayPlatform) && ( nrr.direction == this.direction.opposite);
				}
			}

			public static new RailRoadImpl get( Location loc ) {
				return RailRoad.get(loc) as RailRoadImpl;
			}
		}

	
	
		/// <summary>
		/// Draws the platform.
		/// </summary>
		internal interface Outlook {
			void draw( RailRoadImpl rr, DrawContext display, Point pt );
			/// <summary>
			/// Called after the drawing of a platform is completeld.
			/// Primarily to re-draw the bridge.
			/// </summary>
			void drawAfter( RailRoadImpl rr, DrawContext display, Point pt );
		}

		internal static readonly Outlook plainPlatform = new PlainPlatform();


		/// <summary>
		/// Normal platform
		/// </summary>
		[Serializable]
		internal class PlainPlatform : Outlook, ISerializable {
			public void draw( RailRoadImpl rr, DrawContext display, Point pt ) {
				getSprite(rr.direction,rr.hasRoof).draw(display.surface,pt);
			}
			public void drawAfter( RailRoadImpl rr, DrawContext display, Point pt ) {}

			// singleton serialization
			public void GetObjectData( SerializationInfo info, StreamingContext context) {
				info.SetType(typeof(ReferenceImpl));
			}
			[Serializable]
			internal sealed class ReferenceImpl : IObjectReference {
				public object GetRealObject(StreamingContext context) { return plainPlatform; }
			}
		}

		/// <summary>
		/// Platform with a raised passageway.
		/// </summary>
		// TODO: fly-weight pattern support
		[Serializable]
		internal class PassagewayPlatform : Outlook {
			public PassagewayPlatform( bool _hasBridge ) {
				this.hasBridge = _hasBridge;
			}

			public void draw( RailRoadImpl rr, DrawContext display, Point pt ) {
				PassagewayRail.getSprite( rr.direction, hasBridge, rr.isDoubleWidth ) .draw(display.surface,pt);
			}
			public void drawAfter( RailRoadImpl rr, DrawContext display, Point pt ) {
				Direction d = rr.direction;
				if( hasBridge && ( d==Direction.SOUTH || d==Direction.EAST ) )
					PassagewayRail.getFloatingSprite(d.right90).draw(display.surface,pt);
			}

			public readonly bool hasBridge;
		}

		/// <summary>
		/// Platform with a stiar case to a passageway.
		/// </summary>
		// TODO: fly-weight pattern support
		[Serializable]
		internal class StairPlatform : Outlook {
			public StairPlatform( bool upward ) {
				this.upward = upward;
			}

			private readonly bool upward;

			public void draw( RailRoadImpl rr, DrawContext display, Point pt ) {
				PassagewayRail.getStairSprite( rr.direction, upward, rr.hasRoof, rr.isDoubleWidth ) .draw(display.surface,pt);
			}
			public void drawAfter( RailRoadImpl rr, DrawContext display, Point pt ) {}
		}
	}
}
