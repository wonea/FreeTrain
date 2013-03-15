using System;
using System.Diagnostics;
using System.Drawing;

namespace freetrain.world.land
{
	/// <summary>
	/// Land filler that occupies only one voxel.
	/// </summary>
	[Serializable]
	public abstract class LandVoxel : AbstractVoxelImpl, Entity
	{
		public LandVoxel( Location loc ) : base(loc) {
			Debug.Assert( canBeBuilt(loc) );
			Debug.Assert( loc.z==World.world.getGroundLevel(loc) );
		}

		public override bool transparent { get { return true; } }

		public override Entity entity { get { return this; } }
		
		#region Entity implementation
		public virtual bool isSilentlyReclaimable { get { return true; } }
		public bool isOwned { get { return owned; } set{ owned = value; } }
		protected bool owned = false;

		public abstract int entityValue { get; }

		public void remove() {
			World.world.remove(this);
			if(onEntityRemoved!=null)	onEntityRemoved(this,null);
		}
		public event EventHandler onEntityRemoved;
		#endregion


		/// <summary>
		/// Utility method for derived classes. Returns true
		/// if a land voxel can be placed at the specified location
		/// </summary>
		public static bool canBeBuilt( Location loc ) {
			if( World.world.getGroundLevel(loc) != loc.z )
				return false;	// can only be placed on the ground
			return World.world.isReusable(loc);
		}
	}
}
