using System;
using System.Diagnostics;

namespace freetrain.world.rail
{
	/// <summary>
	/// Rail road implementation for those special purpose rail roads
	/// that doesn't allow any attachment/detachment
	/// </summary>
	[Serializable]
	public class SpecialPurposeRailRoad : RailRoad
	{
		public SpecialPurposeRailRoad( TrafficVoxel voxel, Direction d )
			: base(voxel,RailPattern.get(d,d.opposite)) {
		}

		public override bool canAttach( Direction newDir ) {
			return hasRail(newDir);
		}

		public override bool attach( Direction newDir ) {
			return hasRail(newDir);
		}

		public override void detach( Direction d1, Direction d2 ) {
			// can't be detached
		}

		// ポイント分岐を伴わないので単純なロジックで計算できる
		public override Direction guide() {
			Direction d = voxel.car.state.asInside().direction;
			// we have straight rails only, so the direction must stay the same
			Debug.Assert( hasRail(d) );
			return d;
		}
	}
}
