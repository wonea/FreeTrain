using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using nft.framework;

namespace nft.core.game
{
	/// <summary>
	/// A region in the world, the map that can play at one time.
	/// </summary>
	public interface IDistrict : IHasNameAndID
	{
		
		/// <summary>
		/// Set container <code>World</code>, with grid location of this district. 
		/// caller should check size of district before set the world. 
		/// </summary>
		/// <param name="w"></param>
		/// <param name="wx">World X location (in grid).</param>
		/// <param name="wy">World Y location (in grid).</param>
		void SetWorld(World w, int wx, int wy );

		IOffGameProxy Proxy { get; }

		/// <summary>
		/// The world contains this district
		/// </summary>
		World OwnerWorld { get; }

		/// <summary>
		/// The location in the world (in grid);
		/// </summary>
		Point WorldLocation { get; }

		/// <summary>
		/// Map size in grid
		/// </summary>
		Size SizeInGrid {get;}

		/// <summary>
		/// Mao size in voxel
		/// </summary>
		Size SizeInVoxel {get;}
	}
}
