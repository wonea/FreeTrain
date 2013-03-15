using System;
using nft.framework.plugin;
using nft.contributions.game;

namespace nft.core.game
{
	/// <summary>
	/// MapGeneratorUtil の概要の説明です。
	/// </summary>
	public class MapGeneService
	{
		/// <summary>
		/// Default unit size (ie. grid size).
		/// Should be a multiple of 256.
		/// </summary>
		public const int DefaultUnitSize = 512;
		public const int DefaultAboveSeaLevel = 12;
		public const int SystemMaxHeight = 255;
		// max area of distric which is recomended from memory or sysytem environment.
		static public int DistrictAreaMax 
		{
			get {
				int n = DefaultUnitSize/256;
				return 64/(n*n);
			}
		}

		#region singleton
		static private MapGeneService theInstance; 
		static MapGeneService()
		{
			theInstance = new MapGeneService();
		}

		static public MapGeneService TheInstance 
		{
			get { return theInstance; }
		}

		private MapGeneService()
		{
		}
		#endregion
		
		public IWorldDivider DefaultWorldDividor 
		{
			get 
			{ 
				CtbWorldDivider dv = PluginManager.theInstance.GetContribution("system:SimpleDivider") as CtbWorldDivider;
				if(dv!=null)
					return dv.Divider;
				else
					return null;
			}
		}
	}
}
