using System;
using System.Collections;
using System.Diagnostics;

namespace freetrain.world.rail
{
	/// <summary>
	/// ポイント分岐を伴わない線路
	/// </summary>
	[Serializable]
	public class SingleRailRoad : RailRoad
	{
		public SingleRailRoad( TrafficVoxel tv, RailPattern p ) : base(tv,p) {}

		// ポイント分岐を伴わないので単純なロジックで計算できる
		public override Direction guide() {
			Direction d = voxel.car.state.asInside().direction;
			if(hasRail(d))	return d;	// 進路変更なし

			Direction l = d.left;
			if(hasRail(l))	return l;

			Debug.Assert(hasRail(d.right));
			return d.right;
		}

		private bool is1or3or4( int i ) { return i==1 || i==3 || i==4; }

		public override bool canAttach( Direction newDir ) {
			if(hasRail(newDir))		return true;	// already added

			Direction d1 = dir1, d2 = dir2;

			if(isWellConnected) {
				return is1or3or4(Direction.angle(d1,newDir)) && is1or3or4(Direction.angle(d2,newDir));
			} else {
				return Direction.angle(d1,newDir)>=3 || Direction.angle(d2,newDir)>=3;
			}
		}

		public override bool attach( Direction newDir ) {
			if(hasRail(newDir))		return true;	// already added

			Direction d1 = dir1, d2 = dir2;
			
			if(isWellConnected) {
				Debug.Assert( is1or3or4(Direction.angle(d1,newDir)) && is1or3or4(Direction.angle(d2,newDir)) );
				// if the line is already well connected, make it a junction
				TrafficVoxel v = voxel;
				v.railRoad = new JunctionRailRoad( v, RailPattern.getJunction(d1,d2,newDir) );
				World.world.onVoxelUpdated(voxel);
				return true;
			} else {
				// if the line is not well connected, change the
				// direction
				if( Direction.angle(d1,newDir)>=3 ) {
					pattern = RailPattern.get( d1, newDir );
					World.world.onVoxelUpdated(voxel);
					return true;
				}
				if( Direction.angle(d2,newDir)>=3 ) {
					pattern = RailPattern.get( d2, newDir );
					World.world.onVoxelUpdated(voxel);
					return true;
				}
			}

			return false;	// unable to attach this.
		}

		/// <summary>
		/// If this RR has any rail in the specified direction,
		/// remove the entire RR voxel.
		/// </summary>
		public override void detach( Direction d1, Direction d2 ) {
			if(hasRail(d1) || hasRail(d2)) {
				voxel.railRoad = null;
			} else {
				;	// noop
			}
		}




		/// <summary>
		/// Compute the cost of building rail road with two given directions
		/// on the specified location. If a RR cannot be placed, return 0.
		/// </summary>
		/// <param name="d1">can be null</param>
		/// <param name="d2">can be null</param>
		private static int calcRailRoadCost( Location loc, Direction d1, Direction d2 ) {
			int waterLevel = World.world.waterLevel;
			int glevel = World.world.getGroundLevel(loc);
//			int multiplier = Math.Max( loc.z-World.world.waterLevel, 1 );
			int multiplier = Math.Abs( loc.z-glevel )+1;
			

			if( glevel<=loc.z && loc.z<=waterLevel && glevel<waterLevel )
				return 0;	// underwater or on water.

			Voxel v = World.world[loc];
			if(v==null)		return RAILROAD_CONSTRUCTION_UNIT_COST*multiplier;
			
			// TODO: incorrect compuattion
			if(!(v is TrafficVoxel)) {
				// something else is occupying the voxel
				if(v.entity.isSilentlyReclaimable)
					// we can reclaim this voxel and build a road.
					return RAILROAD_CONSTRUCTION_UNIT_COST*multiplier + v.entity.entityValue;
				else
					return 0;	// obstacle.
			}

			TrafficVoxel tv = (TrafficVoxel)v;

			if(tv.car!=null)
				return 0;	// there's a car. Can't build.
			
			if( tv.railRoad!=null ) {
				// can't attach two RR to an existing rail road.
				if( d1!=null && d2!=null && tv.railRoad!=null )	return 0;

				if( d1!=null && !tv.railRoad.canAttach(d1) )	return 0;
				if( d2!=null && !tv.railRoad.canAttach(d2) )	return 0;
			}

			// TODO: add a check about auto road.

			return RAILROAD_CONSTRUCTION_UNIT_COST*multiplier;
		}


		/// <summary>
		/// Computes the route of RRs between specified two points.
		/// </summary>
		/// <returns>
		///		null if it is impossible to build the route between two.
		///		Otherwise returns a map from Location to RailPattern.
		/// </returns>
		/// <param name="cost">The total cost of construction will be returned here</param>
		public static IDictionary comupteRoute( Location from, Location to, out int cost ) {
			cost = 0;
			int c;

			Hashtable route = new Hashtable();

			if(from==to)	return route;

			Direction dir = null;

			for( Location loc = from; loc!=to; dir=loc.getDirectionTo(to).opposite, loc=loc.toward(to) ) {

				Direction dd = loc.getDirectionTo(to);

				c = calcRailRoadCost( loc, dir, dd );
				cost += c;
				if( c==0 )	return null;

				route.Add( loc, RailPattern.get( dir!=null?dir:dd.opposite, dd ) );
			}

			c = calcRailRoadCost( to, dir, null );
			cost += c;
			if( c==0 )		return null;

			route.Add( to, RailPattern.get( dir, dir.opposite ) );

			return route;
		}


		/// <summary>
		/// Builds normal RR between two specified locations
		/// </summary>
		/// <returns>false if the operation was unsuccessful</returns>
		public static bool build( Location here, Location there ) {
			// ensure that nothing is on our way between "from" and "to"
			World world = World.world;
			int cost;
			
			if(comupteRoute(here,there,out cost)==null)
				return false;
			
			Direction d = here.getDirectionTo(there);

			while(true) {
				TrafficVoxel v = TrafficVoxel.getOrCreate(here);
				if(v==null) {
					Voxel vv = World.world[here];
					Debug.Assert(vv.entity.isSilentlyReclaimable);
					vv.entity.remove();
					v = TrafficVoxel.getOrCreate(here);
				}
				
				Direction dd;
				if(here!=there)	dd = here.getDirectionTo(there);
				else			dd = d;

				if(v.railRoad!=null) {
					v.railRoad.attach(here==there?d.opposite:d);
					World.world.onVoxelUpdated(here);
				} else {
					v.railRoad = new SingleRailRoad( v, RailPattern.get(d.opposite,dd) );

					// if this RR is elevated, elect a bridge support.
//					if((++cycle%2)==0)
					BridgePierVoxel.electBridgeSupport( here, typeof(BridgePierVoxel.DefaultImpl), v );
				}

				if(here==there)	break;

				d = dd;
				here = here.toward(there);
			}

			accounting.AccountGenre.RAIL_SERVICE.spend(cost);	// charge the cost
			return true;
		}

		/// <summary>
		/// Compute the cost of removing railroads.
		/// </summary>
		public static int calcCostOfRemoving( Location here, Location there ) {

			if(here==there)	return 0;

			World world = World.world;
			Direction d = here.getDirectionTo(there);
			int cost = 0;

			while(true) {
				Direction dd;
				if(here!=there)	dd = here.getDirectionTo(there);
				else			dd = d;

				TrafficVoxel v = TrafficVoxel.get(here);
				if(v!=null && v.railRoad!=null && !v.isOccupied)
					cost++;

				if(here==there)		break;

				d = dd;
				here = here.toward(there);
			}

			return cost*RAILROAD_DESTRUCTION_UNIT_COST*Math.Max(1, here.z-world.waterLevel);
		}

		/// <summary>
		/// Removes normal RR between two specified locations
		/// </summary>
		/// <returns>false if the operation was unsuccessful</returns>
		public static void remove( Location here, Location there ) {
			World world = World.world;
			Direction d = here.getDirectionTo(there);

			// charge the cost first. 
			accounting.AccountGenre.RAIL_SERVICE.spend( calcCostOfRemoving(here,there) );

			while(true) {
				Direction dd;
				if(here!=there)	dd = here.getDirectionTo(there);
				else			dd = d;

				TrafficVoxel v = TrafficVoxel.get(here);
				if(v!=null && v.railRoad!=null && !v.isOccupied)
					v.railRoad.detach(d.opposite,dd);

				BridgePierVoxel.teardownBridgeSupport(here,v);

				if(here==there)	break;

				d = dd;
				here = here.toward(there);
			}
		}

		private const int RAILROAD_DESTRUCTION_UNIT_COST  =  4000;
		// taken from A4. 6000 per each fragment.
		private const int RAILROAD_CONSTRUCTION_UNIT_COST = 12000;

	}
}
