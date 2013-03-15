using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Xml;
using nft.framework;
using nft.framework.plugin;
using nft.core.game;
using nft.core.geometry;
using nft.core.structure;
using nft.util;

namespace nft.contributions.game
{
	/// <summary>
	/// CommandEntityContributuion の概要の説明です。
	/// </summary>
	public abstract class CtbTerrainGenerator : Contribution
	{
		public CtbTerrainGenerator( XmlElement contrib ) : base(contrib) {}
		public abstract ITerrainGenerator Generator { get; }
		public static Array ListEnabled()
		{ 
			Type t = Type.GetType("nft.contributions.game.CtbTerrainGenerator");
			return PluginManager.theInstance.ListContributions(t,true);
		}
		public override bool IsDetachable { get	{ return true; } }
	}

	public class CtbFlatTerrainGenerator : CtbTerrainGenerator, ITerrainGenerator
	{
		public const string KEY_MAX_HEIGHT = "MaxHeoght";
		public const string KEY_GROUND_LEVEL = "GroundLevel";

		public CtbFlatTerrainGenerator( XmlElement contrib ) : base(contrib) {}		
		public override ITerrainGenerator Generator { get {	return this; } }

		#region ITerrainGenerator メンバ
		protected ParamSet param;
		protected ProgressMonitor monitor = new ProgressMonitor(2);

		public bool IsSetupEnable { get{ return false; } }

		public void Setup(ParamSet param)
		{
			this.param = param;
		}

		public ITerrainMap Generate(Size sz) 
		{ 
			int mxh = param[KEY_MAX_HEIGHT, MapGeneService.SystemMaxHeight];
			int asl = param[KEY_GROUND_LEVEL, MapGeneService.DefaultAboveSeaLevel];
			ITerrainMap map = new FlatMap(sz,(short)asl,(short)mxh);
			return map; 
		}

		public ProgressMonitor Monitor { get { return monitor; } }

		#endregion
	}

	public class FlatMap : ITerrainMap
	{
		protected short maxHeight = MapGeneService.SystemMaxHeight;
		protected short groundLv = MapGeneService.DefaultAboveSeaLevel;
		protected ParamSet param = new ParamSet();

		protected Rectangle[] districts;
		protected Lot[] lots;

		public FlatMap(Size sz, short groundLevel, short maxHeight)
		{
			this.size = sz;
			this.maxHeight = maxHeight;
			this.groundLv = groundLevel;
		}

		#region ITerrainMap メンバ
		public Size Size { get { return size; } }
        protected Size size;

		public short MaxHeight { get { return maxHeight; } }

		public bool IsDetailedHeight { get { return false; } }

		public short Height(int x, int y, InterCardinalDirection d) { 
			if( x<0 || x>= size.Width || y< 0 || y>=size.Height )
				return -1;
			else
				return groundLv; 
		}

		public short WaterDepth(int x, int y)	{ return -1; }

		public Rectangle[] Districts 
		{
			get { return districts; }
			set { districts = value; }
		}
		public Lot[] Lots 
		{
			get { return lots; }
			set { lots = value; }
		}
		public ParamSet GlobalParams { get { return param; } }
		#endregion
	}

}
