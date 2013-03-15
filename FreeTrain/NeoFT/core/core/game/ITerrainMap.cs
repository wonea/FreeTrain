using System;
using System.Collections;
using System.Drawing;
using nft.core.geometry;
using nft.core.structure;
using nft.util;

namespace nft.core.game
{
	/// <summary>
	/// The map 
	/// </summary>
	public interface ITerrainMap
	{
		Size Size { get; }
		
		short MaxHeight { get; }

		/// <summary>
		/// 'd' specifies the vertex of the voxel at the location(x,y) by direction.
		/// The neighboring voxel can have different height, which will translated as cliff bounds.
		/// If <see cref="IsDetaledHeight">IsDetailedHeight</see> is false,
		/// The height at specifed location has the same value regardless of 'd'.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="d"></param>
		/// <returns></returns>
		short Height(int x,int y, InterCardinalDirection d);

		/// <summary>
		/// true means that <see cref="Height">Height</see> will returns different value
		/// for the same location, according to direction 'd'.
		/// </summary>
		bool IsDetailedHeight { get; }
		// depth of water. positive value means the location is ander the water.
		short WaterDepth(int x, int y);
		// the array of rectangles each represent an district of map.
		Rectangle[] Districts { get; set; }
//		// the array of lot (buildings and lands) to be build.
//		Lot[] Lots { get; set; }

//		ParamSet GlobalParams { get; }
	}

}
