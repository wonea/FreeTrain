using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using freetrain.world.rail;
using freetrain.framework.graphics;

namespace freetrain.world
{
	/// <summary>
	/// Rail-road crossing
	/// </summary>
	[Serializable]
	class RRCrossing : TrafficVoxel.Accessory, IDeserializationCallback
	{
		/// <summary> This crossing belongs to this voxel. </summary>
		private readonly TrafficVoxel owner;

		/// <summary>
		/// 0 if the rail is going from N to S. 1 if E to W.
		/// </summary>
		private readonly int railDirIndex;

		private const int FLASH_FREQUENCY = 3 /*min*/;

		private readonly Location neighbor1Location, neighbor2Location;


		/// <summary>
		/// Sprites. Index is [x,y,z] where:
		/// 
		/// x=0 if rail is going from N to S
		/// x=1 if rail is going from E to W
		/// 
		/// y=0 if the sprite is in front of the rail
		/// y=1 if the sprite is behind the rail
		/// 
		/// z=state of the Xing
		/// z=3 for the secondary graphics.
		/// </summary>
		private static readonly Sprite[,,] sprites = new Sprite[2,2,4]; 

		public RRCrossing( TrafficVoxel _owner ) {
			this.owner = _owner;
			this.railDirIndex = owner.railRoad.dir1.isParallelToY?0:1;
			TrafficVoxel.onRailRoadChanged += new TrafficVoxelHandler(onRailRoadChanged);

			neighbor1Location = owner.location + owner.railRoad.dir1;
			neighbor2Location = owner.location + owner.railRoad.dir2;

			if( neighbor1!=null )	onRailRoadChanged(neighbor1);
			if( neighbor2!=null )	onRailRoadChanged(neighbor2);
		}

		static RRCrossing() {
			// load sprites
			Picture pic = PictureManager.get("{F4380415-A2F2-41d8-8FCD-ED25A470A84D}");
			for( int x=0; x<2; x++ ) {
				for( int y=0; y<2; y++ ) {
					for( int z=0; z<4; z++ ) {
						sprites[x,y,z] = new SimpleSprite( pic,
							new Point(0, z==0?16:8),
							new Point( ((x==0?2:0)+y)*32, z*24+(z==0?0:8) ),
							new Size(32,z==0?32:24) );
					}
				}
			}
		}

		private enum State {
			Open =0,
			HalfClosed =1,
			Closed =2
		}

		/// <summary>
		/// Gets the current state of this crossing.
		/// </summary>
		State currentState {
			get {
				if( owner.car is Train.TrainCar )	// needs to be closed
					return State.Closed;

				TrafficVoxel v;

				v = neighbor1;
				if(v!=null && v.car is Train.TrainCar)
					return State.HalfClosed;

				v = neighbor2;
				if(v!=null && v.car is Train.TrainCar)
					return State.HalfClosed;

				return State.Open;
			}
		}

		private TrafficVoxel neighbor1 { get {
			return TrafficVoxel.get(neighbor1Location);
		}}

		private TrafficVoxel neighbor2 { get {
			return TrafficVoxel.get(neighbor2Location);
		}}


		public void OnDeserialization( object sender ) {
			// make sure this object will receive notification for changes made to railroads
			TrafficVoxel.onRailRoadChanged += new TrafficVoxelHandler(onRailRoadChanged);
		}

		/// <summary>
		/// Called when a car approaches neighboring traffic voxels.
		/// </summary>
		public void onCarApproaching( TrafficVoxel v ) {
			// method needs to be public so that the delegate can be serialized
			// redraw this voxel
			World.world.onVoxelUpdated(owner);
			registerTimer();
		}

		private void registerTimer() {
			int min = FLASH_FREQUENCY-(World.world.clock.minutes%FLASH_FREQUENCY);
			if(min==0)	min=FLASH_FREQUENCY;

			if( currentState!=State.Open )	// register the handler while the Xing is closed.
				World.world.clock.registerOneShot( new ClockHandler(followUp), TimeLength.fromMinutes(min) );
		}

		public void followUp() {
			// method needs to be public so that the delegate can be serialized
			World.world.onVoxelUpdated(owner);
			registerTimer();
		}

		public void onRemoved() {
			// unregister all handlers.
			TrafficVoxel v;
			v = neighbor1;
			if(v!=null)		v.onCarChanged -= new TrafficVoxelHandler(onCarApproaching);
			v = neighbor2;
			if(v!=null)		v.onCarChanged -= new TrafficVoxelHandler(onCarApproaching);
			
			TrafficVoxel.onRailRoadChanged -= new TrafficVoxelHandler(onRailRoadChanged);
		}



		private void onRailRoadChanged( TrafficVoxel v ) {
			// if relevant voxels are affected,
			TrafficVoxel n1=neighbor1;
			TrafficVoxel n2=neighbor2;
			if(v==n1)
				setupHandler( n1, owner.railRoad.dir1 );
			if(v==n2)
				setupHandler( n2, owner.railRoad.dir2 );
		}

		private void setupHandler( TrafficVoxel neighbor, Direction d ) {
			RailRoad rr = neighbor.railRoad;
			if( rr!=null && rr.hasRail(d.opposite) ) {
				// connected?
				neighbor.onCarChanged += new TrafficVoxelHandler(onCarApproaching);
			} else {
				// disconnected?
				neighbor.onCarChanged -= new TrafficVoxelHandler(onCarApproaching);
			}
		}
		
		


		public void drawBefore( DrawContext display, Point pt ) {
			sprites[railDirIndex,1,stateSpriteIndex].draw( display.surface, pt );
			Trace.WriteLine( string.Format("{0} : {1},1,{2}", owner.location,railDirIndex,stateSpriteIndex) );
		}

		public void drawAfter( DrawContext display, Point pt ) {
			sprites[railDirIndex,0,stateSpriteIndex].draw( display.surface, pt );
			Trace.WriteLine( string.Format("{0} : {1},0,{2}", owner.location,railDirIndex,stateSpriteIndex) );
		}

		private int stateSpriteIndex {
			get {
				if( currentState!=State.Closed )
					return (int)currentState;
				else
					return ((World.world.clock.minutes/FLASH_FREQUENCY)%2==0)?2:3;
			}
		}

	}
}
