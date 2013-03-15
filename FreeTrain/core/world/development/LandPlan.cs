using System;
using freetrain.contributions.land;

namespace freetrain.world.development
{
	/// <summary>
	/// Plan of land surfaces such as crop fields.
	/// </summary>
	[Serializable]
	class LandPlan : Plan
	{
		private readonly LandBuilderContribution contrib;
		private readonly Location loc;
		private readonly SIZE size;

		internal LandPlan( LandBuilderContribution _contrib, ULVFactory factory, Location _loc, SIZE _size )
			: base(factory.create(new Cube(_loc,_size.x,_size.y,0))) {
			this.contrib = _contrib;
			this.loc = _loc;
			this.size = _size;
		}

		public override int value { get { return contrib.price*4; } }

		public override Cube cube { get { return new Cube(loc,size.x,size.y,1); } }

		public override void build() {
			contrib.create(loc,loc+new Distance(size.x-1,size.y-1,0),false);	// inclusive
		}
	}

}
