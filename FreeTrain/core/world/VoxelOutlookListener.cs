using System;

namespace freetrain.world
{
	/// <summary>
	/// Receive notifications of changes in voxel outlook.
	/// </summary>
	public interface VoxelOutlookListener
	{
		/// <summary>
		/// Called when all the voxels need to be fully updated.
		/// </summary>
		void onUpdateAllVoxels();

		/// <summary>
		/// Called when a particular voxel is updated.
		/// </summary>
		void onUpdateVoxel( Location loc ); 

		/// <summary>
		/// Called when a cube of voxels are updated.
		/// </summary>
		void onUpdateVoxel( Cube cube );
	}
}
