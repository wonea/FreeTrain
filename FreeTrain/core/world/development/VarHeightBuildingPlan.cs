using System;
using freetrain.contributions.structs;
using freetrain.world.structs;

namespace freetrain.world.development
{
	/// <summary>
	/// VarHeightBuildingPlan の概要の説明です。
	/// </summary>
	[Serializable]
	class VarHeightBuildingPlan : Plan {
		private readonly VarHeightBuildingContribution contrib;
		private readonly Location loc;
		private readonly int h;
		
		internal VarHeightBuildingPlan(
			VarHeightBuildingContribution contrib,
			ULVFactory factory, Location _loc, int h )
			: base(factory.create(new Cube(_loc, contrib.size, 0 ))) {
			
			this.contrib = contrib;
			this.loc = _loc;
			this.h = h;
		}

		public override int value { get { return contrib.price*h; } }

		public override Cube cube { get { return new Cube(loc,contrib.size,h); } }

		public override void build() {
			new ConstructionSite( loc, new EventHandler(handle), new Distance(contrib.size,h) );
		}

		public void handle( object sender, EventArgs args ) {
			contrib.create( loc, h, false );
		}
	}
}
