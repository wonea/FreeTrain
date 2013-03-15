using System;
using System.Diagnostics;
using freetrain.world;

namespace freetrain.world.rail
{
	/// <summary>
	/// Railroad with a junction.
	/// </summary>
	[Serializable]
	public sealed class JunctionRailRoad : RailRoad
	{
		public JunctionRailRoad( TrafficVoxel tv, RailPattern p ) : base(tv,p) {}

		public override Direction guide() {
			Train.TrainCar car = (Train.TrainCar)voxel.car;
			CarState.Placed state = car.state.asPlaced();

			Direction d = state.asInside().direction;
			if(hasRail(d)) {
				if(!hasRail(d.left) && !hasRail(d.right))
					// straight is the only option
					return d;

				// we have an option to either go straight or turn.
				Train.TrainCar previous = car.previous;

				if( previous==null) {
					// if this is the head car, ask the controoler
					if( car.parent.controller.onJunction( car.parent, this )==JunctionRoute.Straight )
						return d;	// Go straight
				} else {
					// otherwise follow the previous car
					if( previous.state.asPlaced().location == state.location+d )
						return d;
				}

				// make a turn
				if(hasRail(d.left))		return d.left;
				else					return d.right;
			} else {
				Direction l = d.left;
				if(hasRail(l))	return l;

				Debug.Assert(hasRail(d.right));
				return d.right;
			}
		}

		/// <summary>
		/// Gets the "straight" direction of this rail pattern.
		/// If this has rails to N,NE,S, then N is the straight direction.
		/// </summary>
		public Direction straightDirection {
			get {
				foreach( Direction d in Direction.directions ) {
					if( hasRail(d) && hasRail(d.opposite) ) {
						if( hasRail(d.left) || hasRail(d.right) )	return d;
						else										return d.opposite;
					}
				}
				Debug.Assert(false);
				return null;
			}
		}

		/// <summary>
		/// Gets the "curve" direction of this rail pattern.
		/// If this has rails to N,NE,S, then NE is the curve direction.
		/// </summary>
		public Direction curveDirection {
			get {
				foreach( Direction d in Direction.directions )
					if( hasRail(d) && !hasRail(d.opposite) )
						return d;
				Debug.Assert(false);
				return null;
			}
		}

		/// <summary> Returns the direction from the route to take. </summary>
		public Direction getDirection( JunctionRoute route ) {
			if(route==JunctionRoute.Curve)	return curveDirection;
			else							return straightDirection;
		}

		public override bool canAttach( Direction newDir ) {
			return hasRail(newDir);	// unless the rail is already there, we can't add.
		}

		public override bool attach( Direction newDir ) {
			return hasRail(newDir);	// unless the rail is already there, we can't add.
		}

		/// <summary>
		/// If this RR has two RRs with specified direction,
		/// remove it completely. If one RR matches, change 
		/// it to a single RR.
		/// </summary>
		public override void detach( Direction d1, Direction d2 ) {
			if(hasRail(d1) && hasRail(d2)) {
				voxel.railRoad = null;
				return;
			}
			if(hasRail(d1) || hasRail(d2)) {
				// this needs to be changed to a single RR.
				Direction a=null,b=null;	// two directions of such a single RR.
				foreach( Direction x in Direction.directions )
					if(hasRail(x) && x!=d1 && x!=d2) {
						if(a==null)	a=x;
						else		b=x;
					}
				Debug.Assert(a!=null && b!=null);

				Location loc = location;
				TrafficVoxel v = voxel;			// these references can be lost, so use local variables.

				// replace the railroad
				new SingleRailRoad( v, RailPattern.get(a,b) );
				World.world.onVoxelUpdated(loc);

				return;
			}
		}

		public static new JunctionRailRoad get( Location loc ) {
			return RailRoad.get(loc) as JunctionRailRoad;
		}
	}
}
