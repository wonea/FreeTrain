using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Xml;
using nft.framework;
using nft.framework.plugin;
using nft.util;
using nft.core;
using nft.core.structure;

namespace nft.core.game
{
	public interface IMapGenerator
	{
		// returns false if Setup is not available.
		bool IsSetupEnable { get; }
		// set setup parameters
		void Setup(ParamSet param);
		// ProgressMonitor that monitor generation process.
		ProgressMonitor Monitor { get; }		
	}

	/// <summary>
	/// generate terrain map
	/// </summary>
	public interface ITerrainGenerator : IMapGenerator
	{
		ITerrainMap Generate(Size sz);
	}

	/// <summary>
	/// divide world into districts (<code>IDistrict</code>).
	/// </summary>
	public interface IWorldDivider : IMapGenerator
	{
		Rectangle[] Divide(ITerrainMap map);
	}

	/// <summary>
	/// Generate cities, industries, and plants.
	/// </summary>
	public interface ILandGenerator : IMapGenerator
	{
		Lot[] Generate(ITerrainMap terrain);
	}

	public interface ICompositGenerator : ITerrainGenerator, ILandGenerator
	{
	}
}
