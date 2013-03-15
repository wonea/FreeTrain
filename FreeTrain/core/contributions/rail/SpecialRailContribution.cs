using System;
using System.Drawing;
using System.Xml;
using freetrain.contributions.common;
using freetrain.world;

namespace freetrain.contributions.rail
{
	/// <summary>
	/// Contribution that adds a special kind of railroads, like tunnels or bridges.
	/// </summary>
	[Serializable]
	public abstract class SpecialRailContribution : LineContribution
	{
		protected SpecialRailContribution( XmlElement e ) : base("specialRail",e.Attributes["id"].Value) {}
	}
}
