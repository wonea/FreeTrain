using System;
using System.Drawing;
using freetrain.contributions.rail;
using freetrain.world.structs;
using freetrain.framework.plugin;

namespace freetrain.world.rail
{
	/// <summary>
	/// RailStationaryStructure の概要の説明です。
	/// </summary>
	[Serializable]
	public class RailStationaryStructure : PThreeDimStructure
	{
		public RailStationaryStructure(RailStationaryContribution type, WorldLocator wloc ) : base(type,wloc) {}

		protected override StructureVoxel CreateVoxel(WorldLocator wloc) {
			if( type.size.x == 1 && type.size.y == 1 )
				return new StationaryVoxel(this, wloc);
			else
				return base.CreateVoxel(wloc);
		}

		public override bool onClick() { return false; }

		#region Entity implementation
		public override bool isSilentlyReclaimable { get { return false; } }
		public override bool isOwned { get { return true; } }

		// TODO: value?
		public override int entityValue { get { return 0; } }

		#endregion

		/// <summary>
		/// Gets the station object if one is in the specified location.
		/// </summary>
		public static RailStationaryStructure get( Location loc ) {
			return World.world.getEntityAt(loc) as RailStationaryStructure;
		}

		internal protected override Color heightCutColor { get { return Color.Gray; } }

		public static RailStationaryStructure get( int x, int y, int z ) { return get(new Location(x,y,z)); }

		[Serializable]
		protected class StationaryVoxel : VoxelImpl {
			internal StationaryVoxel( PThreeDimStructure _owner, WorldLocator wloc )
				: base(_owner,wloc) {}

		public override bool transparent { get { return true; } }
		}
	}
}
