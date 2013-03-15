using System;
using freetrain.contributions.structs;
using freetrain.world.structs;

namespace freetrain.world.development
{
	/// <summary>
	/// CommercialStructurePlan の概要の説明です。
	/// </summary>
	[Serializable]
	class CommercialStructurePlan : Plan
	{
		private readonly CommercialStructureContribution contrib;
		private readonly Location loc;

		internal CommercialStructurePlan(
			CommercialStructureContribution _contrib,
			ULVFactory factory, Location _loc )
			: base( factory.create(Cube.createExclusive(_loc, new Distance(_contrib.size.x, _contrib.size.y, 0) )))
		{
			this.contrib = _contrib;
			this.loc = _loc;
		}

		public override int value { get { return contrib.price; } }

		public override Cube cube { get { return Cube.createExclusive(loc,contrib.size); } }


		public override void build() {
			new ConstructionSite( loc, new EventHandler(handle), contrib.size );
		}

		public void handle( object sender, EventArgs args ) {
			contrib.create(loc,false);
		}
	}
}
