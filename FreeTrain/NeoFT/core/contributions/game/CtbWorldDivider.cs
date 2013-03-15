using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Xml;
using nft.framework;
using nft.framework.plugin;
using nft.core.game;
using nft.util;

namespace nft.contributions.game
{
	/// <summary>
	/// CommandEntityContributuion の概要の説明です。
	/// </summary>
	public abstract class CtbWorldDivider : Contribution
	{
		public CtbWorldDivider( XmlElement contrib ) : base(contrib) {}
		public abstract IWorldDivider Divider { get; }
		public static Array ListEnabled()
		{ 
			Type t = Type.GetType("nft.contributions.game.CtbWorldDivider");
			return PluginManager.theInstance.ListContributions(t,true);
		}
		public override bool IsDetachable { get	{ return true; } }
	}

	public class CtbSimpleDivider : CtbWorldDivider, IWorldDivider
	{
		public CtbSimpleDivider( XmlElement contrib ) : base(contrib) {}		
		public override IWorldDivider Divider { get	{ return this; } }

		#region IWorldDivider メンバ

		protected ProgressMonitor monitor = new ProgressMonitor(2);
		public ProgressMonitor Monitor { get { return monitor; } }

		public Rectangle[] Divide(ITerrainMap map)
		{
			// TODO:  CtbSimpleDivider.Divide 実装を追加します。
			return null;
		}
		
		public bool IsSetupEnable { get{ return false; } }

		public void Setup(ParamSet param)
		{
		}
		#endregion
	}

}
