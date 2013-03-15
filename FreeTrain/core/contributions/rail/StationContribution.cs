using System;
using System.Xml;
using freetrain.contributions.common;
using freetrain.framework.plugin;
using freetrain.world;
using freetrain.world.rail;
using freetrain.world.structs;

namespace freetrain.contributions.rail
{
	/// <summary>
	/// Station.
	/// </summary>
	[Serializable]
	public class StationContribution : FixedSizeStructureContribution
	{
		/// <summary>
		/// Parses a station contribution from a DOM node.
		/// </summary>
		/// <exception cref="XmlException">If the parsing fails</exception>
		public StationContribution( XmlElement e ) : base(e) {
			operationCost = int.Parse( XmlUtil.selectSingleNode(e,"operationCost").InnerText );
		}

		/// <summary> Operation cost of this station per day. </summary>
		public readonly int operationCost;

		protected override StructureGroup getGroup( string name ) {
			return PluginManager.theInstance.stationGroup[name];
		}

		public override Structure create( WorldLocator wLoc, bool initiallyOwned ) {
			return new Station( this, wLoc );
		}

		public override bool canBeBuilt( Location baseLoc, ControlMode cm ) {
			return Station.canBeBuilt( baseLoc, size, cm );
		}
	}
}
