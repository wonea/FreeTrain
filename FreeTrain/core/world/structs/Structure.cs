using System;
using System.Drawing;
using freetrain.framework;
using freetrain.util;
using freetrain.world.terrain;
using org.kohsuke.directdraw;

namespace freetrain.world.structs
{
	/// <summary>
	/// Base implementation of the generic "structure"
	/// that occupies a square-shaped block on the ground.
	/// </summary>
	[Serializable]
	public abstract class Structure : Entity
	{
		public Structure() {
		}


		// actually none of the methods are implemented.
		// we just require Structure to implement Entity
		#region Entity implementation
		public abstract bool isSilentlyReclaimable { get; }
		public abstract bool isOwned { get; }
		public abstract void remove();
		public abstract int entityValue { get; }
		public abstract event EventHandler onEntityRemoved;
		#endregion


		/// <summary>
		/// This method is called when one of the voxel is clicked.
		/// </summary>
		public abstract bool onClick();

		/// <summary>
		/// Name of the structure.
		/// </summary>
		public abstract string name { get; }

		/// <summary>
		/// Returns true if there is enough space in the spcified location
		/// to built a structure of a given size.
		/// 
		/// Usually, derived classes override this method and add necessary
		/// checks specific to that structure.
		/// </summary>
//		public static bool canBeBuilt( Location loc, Distance sz ) 
//		{
//			return canBeBuilt(loc,sz,ControlMode.player);
//		}
		public static bool canBeBuilt( Location loc, Distance sz, ControlMode mode ) 
		{
			if(mode == ControlMode.com)
			{
				foreach( Voxel v in Cube.createExclusive(loc,sz).voxels )
					if( !v.entity.isOwned )
						return false;
				return true;
			}
			else
			{
				foreach( Voxel v in Cube.createExclusive(loc,sz).voxels )
					if( !v.entity.isSilentlyReclaimable )
						return false;
				return true;
			}
		}

		/// <summary>
		/// Make sure all the relevant voxels are on the ground
		/// </summary>
		/// <param name="loc"></param>
		/// <param name="sz"></param>
		/// <returns></returns>
		public static bool isOnTheGround( Location loc, Distance sz ) {
			for( int y=0; y<sz.y; y++ )
				for( int x=0; x<sz.x; x++ ) {
					if( World.world.getGroundLevel(loc.x+x,loc.y+y)!=loc.z )
						return false;
					if(World.world[loc.x+x,loc.y+y,loc.z] is MountainVoxel)
						return false;
				}
			return true;
		}

		public virtual object queryInterface( Type aspect ) { return null; }


		/// <summary>
		/// Individual voxel that a structure occupies.
		/// </summary>
		[Serializable]
		protected internal abstract class StructureVoxel : AbstractVoxelImpl {
			protected StructureVoxel( Structure _owner, Location _loc ) : base(_loc) {
				this.owner = _owner;
			}

			protected StructureVoxel( Structure _owner, WorldLocator wloc ) : base(wloc) {
				this.owner = _owner;
			}

			/// <summary>
			/// The structure object to which this voxel belongs.
			/// </summary>
			public readonly Structure owner;

			public override Entity entity { get { return owner; } }

			/// <summary>
			/// onClick event is delegated to the parent.
			/// </summary>
			public override bool onClick() {
				return owner.onClick();
			}
		}
	}
}
