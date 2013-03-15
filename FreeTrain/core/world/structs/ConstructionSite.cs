using System;
using System.Diagnostics;
using System.Drawing;
using freetrain.framework;
using freetrain.framework.sound;
using freetrain.framework.plugin;
using freetrain.framework.graphics;
using org.kohsuke.directdraw;

namespace freetrain.world.structs
{
	/// <summary>
	/// Construction site.
	/// 
	/// Construction site is used to provide visual feedback
	/// before the actual structure is placed.
	/// </summary>
	[Serializable]
	public class ConstructionSite : Structure
	{
		/// <summary>
		/// Builds a new construction site.
		/// </summary>
		/// <param name="baseLoc">North-western voxel at the ground level.</param>
		/// <param name="type">structure to build</param>
		public ConstructionSite( Location _baseLoc, EventHandler _completionHandler, Distance size ) {

			this.baseLoc = _baseLoc;
			this.completionHandler = _completionHandler;

			voxels = new VoxelImpl[size.x,size.y,size.z];
			for( int h=0; h<size.z; h++ )
				for( int x=0; x<size.x; x++ )
					for( int y=0; y<size.y; y++ ) {
						Location l = new Location(baseLoc.x+x, baseLoc.y+y, baseLoc.z+h);
						if( World.world.isInsideWorld(l) )
							voxels[x,y,h] = new VoxelImpl(this, l,
								new bool[]{ y!=0, x!=size.x-1, y!=size.y-1, x!=0 } );
					}
			
			uncompletedVoxels = size.volume;
		}

		/// <summary>
		/// Handler invoked when the construction finishes.
		/// </summary>
		private EventHandler completionHandler;

		/// <summary>
		/// Voxels
		/// </summary>
		private readonly VoxelImpl[,,] voxels;

		/// <summary>
		/// Base location of the structure to build.
		/// </summary>
		private readonly Location baseLoc;

		/// <summary>
		/// Number of uncompleted voxels.
		/// This variables counts down and the construction will be
		/// completed if it reaches to 0.
		/// </summary>
		private int uncompletedVoxels;


		// don't react to a mouse click
		public override bool onClick() { return false; }

		public override string name { get { return "Construction site"; } }
		//! public override string name { get { return "建設現場"; } }

		private void onFinished() {
			// construction is finished
			Debug.Assert(uncompletedVoxels==0);

			// remove all voxels
			foreach( VoxelImpl v in voxels )
				World.world.remove(v);

			// then fire the event
			completionHandler(null,null);
		}

		#region Entity implementation
		public override bool isSilentlyReclaimable { get { return false; } }
		public override bool isOwned { get { return false; } }

		public override void remove() {
			// remove all voxels
			foreach( VoxelImpl v in voxels )
				World.world.remove(v);
			// then fire the event
			completionHandler = null;
			// TODO: not sure what to do. ConstructionSite is not removable.
			if(onEntityRemoved!=null)	onEntityRemoved(this,null);
		}

		public override event EventHandler onEntityRemoved;

		// TODO: value?
		public override int entityValue { get { return 0; } }
		#endregion


		// random number generator
		private static Random random = new Random();


		/// <summary>
		/// Individual voxel
		/// </summary>
		[Serializable]
		internal class VoxelImpl : StructureVoxel {
			internal VoxelImpl( ConstructionSite _owner, Location _loc, bool[] _connection ) : base(_owner,_loc) {
				this.connection = _connection;
				state = State.empty;
				ground= Ground.ground;

				if( !connection[2] && !connection[3] )
					needsDoor = false;	// south west corner. if we allow a door, we will get two, which doesn't look nice.
				else
					needsDoor = random.Next(4)==0;	// door will be placed with 25% probability

				registerClockHandler();	// start receiving clocks
			}

			public override void onRemoved() {
				state = State.abandoned;
			}

			/// <summary>
			/// Connectivity to four adjacent voxels.
			/// 
			/// True if connected, false if not. Pictures change depending on this.
			/// </summary>
			private readonly bool[] connection;

			private enum State {
				empty=0, bone1=1, bone2=2, bone3=3, walled=4, abandoned=5
			}
			/// <summary>
			/// The current state of the construction.
			/// </summary>
			private State state;

			/// <summary>
			/// If this voxel is the ground level, this variable
			/// specifies the object used when it's empty.
			/// </summary>
			private Ground ground;

			private enum Ground {
				ground=0, woods=1, steel=2, hole=3, machine1=4, machine2=5, machine3=6
			}

			/// <summary>
			/// True if this ground voxel needs a fence with a door.
			/// Takes effect only when isGroundLevel==true.
			/// </summary>
			private readonly bool needsDoor;

			/// <summary>
			/// Gets the min(v.state) for all v in the specified level.
			/// </summary>
			private State getMinFloorState( int z ) {
				z -= ((ConstructionSite)owner).baseLoc.z;
				VoxelImpl[,,] voxels = ((ConstructionSite)owner).voxels;

				State s = State.walled;

				for( int x=0; x<voxels.GetLength(0); x++ )
					for( int y=0; y<voxels.GetLength(1); y++ )
						if( voxels[x,y,z].state < s )
							s = voxels[x,y,z].state;
				return s;
						
			}


			/// <summary>
			/// Gets the max(v.state) for all v in the specified level.
			/// </summary>
			private State getMaxFloorState( int z ) {
				z -= ((ConstructionSite)owner).baseLoc.z;
				VoxelImpl[,,] voxels = ((ConstructionSite)owner).voxels;

				State s = State.empty;

				for( int x=0; x<voxels.GetLength(0); x++ )
					for( int y=0; y<voxels.GetLength(1); y++ )
						if( voxels[x,y,z].state > s )
							s = voxels[x,y,z].state;
				return s;
						
			}


			/// <summary>
			/// Returns true if this voxel is at the ground level
			/// </summary>
			private bool isGroundLevel { get { return World.world.getGroundLevel(location)==location.z; } }

			/// <summary> Construction voxel under this voxel, or null if none. </summary>
			private VoxelImpl below {
				get {
					return World.world[ location.x, location.y, location.z-1 ] as VoxelImpl;
				}
			}

			/// <summary> Construction voxel above this voxel, or null if none. </summary>
			private VoxelImpl above {
				get {
					return World.world[ location.x, location.y, location.z+1 ] as VoxelImpl;
				}
			}

			/// <summary>
			/// Returns true if the construction of this voxel can advance one step.
			/// 
			/// This method is used to make sure that the construction of the entire
			/// building will proceed with some degree of order.
			/// </summary>
			/// <returns></returns>
			private bool canProceed() {

				Time ct = World.world.clock;
				if( ct.isWeekend )	return false;	// no construction work during the weekends
				int h = ct.hour;
				if( h<9 || 17<h )	return false;	// no work during the night

				VoxelImpl b = this.below;
				VoxelImpl a = this.above;

				State sa=0,sb=0;
				if(b!=null)	sb = getMinFloorState(location.z-1);
				if(a!=null)	sa = getMinFloorState(location.z+1);

				if( b!=null && b.state < State.bone2 )
					// can't construct this voxel unless the voxel below is done to a certain degree.
					return false;
				if( b!=null && sb<=this.state )
					// can't go faster than the floor below
					return false;
				if( this.state==State.bone1 && b!=null && sb<State.bone3 )
					// the walling can't start unless bones are built on top of it
					return false;
				if( this.state==State.bone2 && a!=null && sa<State.bone1 )
					// the walling can't complete unless bones are built on top of it
					return false;
				if( this.state==State.bone3 && a!=null && sa<State.bone3 )
					// the final exterior work won't start until the voxel above reaches to State.bone3.
					return false;

				return true;
			}

			public void clockHandler() {
				if( !canProceed() ) {
					// can't construct this voxel
					registerClockHandler();
					return;
				}

				// TODO: spend materials and abort if none is available

				if( ground==Ground.ground && state==State.empty && random.Next(2)==0 ) {
					// put something on the ground
					switch(random.Next(4)) {
					case 0:	ground = Ground.hole;  break;
					case 1:	ground = Ground.steel; break;
					case 2:	ground = Ground.woods; break;
					case 3:
						switch(random.Next(3)) {
						case 0:	ground = Ground.machine1; break;
						case 1: ground = Ground.machine2; break;
						case 2: ground = Ground.machine3; break;
						}
						break;
					}
					World.world.onVoxelUpdated(this);
					registerClockHandler();
					return;
				}

				// proceed one step
				state++;
				World.world.onVoxelUpdated(this);
				registerClockHandler();
				theSound.play(location);
				return;
			}

			// registers a next clock handler.
			private void registerClockHandler() {
				if( state==State.abandoned )
					return;
				if( state==State.walled ) {
					// this voxel is finished. don't register another handler
					if( --((ConstructionSite)owner).uncompletedVoxels == 0 ) {
						// the entire construction has finished.
						((ConstructionSite)owner).onFinished();
					}
					return;
				}

				// TODO: change the time span
				World.world.clock.registerOneShot( new ClockHandler(clockHandler),
					TimeLength.fromMinutes( 40 + random.Next(80) ) );
			}

			//
			//
			// drawing
			//
			//

			public override void draw( DrawContext context, Point pt, int heightCutDiff ) {
				Surface surface = context.surface;

				if( isGroundLevel ) {
					if(!connection[0])	drawFence(surface,pt,0);
					if(!connection[1])	drawFence(surface,pt,1);
				}

				switch(state) {
				case State.empty:
					if( isGroundLevel )
						drawGroundObject(surface,pt);
					break;
				case State.bone1:
				case State.bone2:
				case State.bone3:
					if( isGroundLevel )
						groundSprites[0].draw(surface,pt);
					drawBone( surface, pt, (int)state-1 );
					break;
				case State.walled:
					if(!connection[0])	drawWall(surface,pt,0);
					if(!connection[1])	drawWall(surface,pt,1);
					drawBone( surface, pt, 2 );
					if(!connection[2])	drawWall(surface,pt,2);
					if(!connection[3])	drawWall(surface,pt,3);
					break;

				default:
					Debug.Assert(false);
					break;
				}

				if( isGroundLevel ) {
					if(!connection[2])	drawFence(surface,pt,2);
					if(!connection[3])	drawFence(surface,pt,3);
				}
			}


			/// <summary> Draws a fence </summary>
			private void drawFence( Surface surface, Point pt, int dir ) {
				fenceSprites[dir,needsDoor?1:0].draw( surface, pt );
			}

			/// <summary> Draws a ground object. </summary>
			private void drawGroundObject( Surface surface, Point pt ) {
				groundSprites[(int)ground].draw( surface, pt );
			}

			/// <summary> Draws a bone. </summary>
			private void drawBone( Surface surface, Point pt, int level ) {
				boneSprites[level,
					connection[0]?1:0,
					connection[1]?1:0,
					connection[2]?1:0,
					connection[3]?1:0 ].draw( surface, pt );
			}

			private void drawWall( Surface surface, Point pt, int dir ) {
				wallSprites[dir].draw( surface,pt);
			}



			/// <summary>
			/// Sound-effect of construction.
			/// </summary>
			private static readonly SoundEffect theSound = new RepeatableSoundEffectImpl(
				ResourceUtil.loadSystemSound("construction.wav"),5,200);


			//
			//
			// Sprites
			//
			//

			private readonly static Sprite[/*direction*/,/*0:no door, 1:door*/] fenceSprites;

			private readonly static Sprite[/*Ground*/] groundSprites;

			private readonly static Sprite[/*level*/,/*N*/,/*E*/,/*S*/,/*W*/] boneSprites;

			private readonly static Sprite[/*direction*/] wallSprites;


			static VoxelImpl() {
				// load fence sprites
				Picture fence = ResourceUtil.loadSystemPicture("ConstructionFence.bmp");
				fenceSprites = new Sprite[4/*direction*/,2/*door or no door*/];
				for( int i=0; i<4; i++ )
					fenceSprites[i,0] = new SimpleSprite( fence, new Point(0,16), new Point(32*i,0), new Size(32,32));
				fenceSprites[0,1] = fenceSprites[0,0];
				fenceSprites[1,1] = fenceSprites[1,0];	// no doors

				for( int i=0; i<2; i++ )
					fenceSprites[i+2,1] = new SimpleSprite(
						fence, new Point(0,16), new Point(32*(i+4),0), new Size(32,32) );


				// load ground sprites
				Picture ground = ResourceUtil.loadSystemPicture("ConstructionGround.bmp");
				groundSprites = new Sprite[7];

				groundSprites[0] = new SimpleSprite(// ground
					ground, new Point(0,0), new Point(96, 1), new Size(32,16) );
				groundSprites[1] = new SimpleSprite(// woods
					ground, new Point(0,0), new Point(64,17), new Size(32,16) );
				groundSprites[2] = new SimpleSprite(// steel
					ground, new Point(0,0), new Point(96,17), new Size(32,16) );
				groundSprites[3] = new SimpleSprite(// hole
					ground, new Point(0,1), new Point(64, 0), new Size(32,17) );
				groundSprites[4] = new SimpleSprite(// machine1
					ground, new Point(0,31),new Point( 0, 0), new Size(32,47) );
				groundSprites[5] = new SimpleSprite(// machine2
					ground, new Point(0,31),new Point(32, 0), new Size(32,47) );
				groundSprites[6] = new SimpleSprite(// machine3
					ground, new Point(0, 8),new Point(96,33), new Size(32,24) );


				// load bone sprites
				Picture bone = ResourceUtil.loadSystemPicture("ConstructionBones.bmp");
				boneSprites = new Sprite[3,2,2,2,2];

				for( int l=0; l<3; l++ )
					for( int n=0; n<2; n++ )
						for( int e=0; e<2; e++ )
							for( int s=0; s<2; s++ )
								for( int w=0; w<2; w++ ) {
									int code = 0x1000*n + 0x0100*e + 0x0010*s + 0x0001*w;
									int idx;
									for( idx=0; idx<16; idx++ )
										if( boneConnectivities[idx] == code )
											break;
									Debug.Assert(idx!=16);

									boneSprites[l,n,e,s,w] = new SimpleSprite( bone,
										new Point(0,16),
										new Point( l*128 + (idx%4)*32, (idx/4)*32 ),
										new Size(32,32) );
								}

				// loa dwall sprites
				Picture wall = ResourceUtil.loadSystemPicture("ConstructionWall.bmp");
				wallSprites = new Sprite[4];

				wallSprites[0] = new SimpleSprite( wall,
					new Point(  0,16), new Point( 0, 0), new Size(16,32) );
				wallSprites[1] = new SimpleSprite( wall,
					new Point(-16,16), new Point(16, 0), new Size(16,32) );
				wallSprites[2] = new SimpleSprite( wall,
					new Point(-16,16), new Point(16,32), new Size(16,32) );
				wallSprites[3] = new SimpleSprite( wall,
					new Point(  0,16), new Point( 0,32), new Size(16,32) );
			}

			/// <summary>
			/// Describes the connectivity of bones in "constructionBones.bmp"
			/// 
			/// 1:connected, 0:not connected.
			/// One number consists of four connectivity in the "NESW" format.
			/// </summary>
			private static readonly int[] boneConnectivities = new int[]{
				0x0011, 0x1011, 0x1001, 0x0000,
				0x0111, 0x1111, 0x1101, 0x0101,
				0x0110, 0x1110, 0x1100, 0x1010,
				0x0010, 0x0100, 0x1000, 0x0001 };
		}
	}
}
