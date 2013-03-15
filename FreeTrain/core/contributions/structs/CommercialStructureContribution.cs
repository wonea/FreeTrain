using System;
using System.Xml;
using freetrain.contributions.common;
using freetrain.framework.plugin;
using freetrain.world;
using freetrain.world.structs;

namespace freetrain.contributions.structs
{
	/// <summary>
	/// commercial structure.
	/// 
	/// Including everything from convenience stores (like Seven-eleven)
	/// to shopping malls like Walmart.
	/// </summary>
	[Serializable]
	public class CommercialStructureContribution : FixedSizeStructureContribution
	{
		/// <summary>
		/// Parses a commercial structure contribution from a DOM node.
		/// </summary>
		/// <exception cref="XmlException">If the parsing fails</exception>
		public CommercialStructureContribution( XmlElement e ) : base(e) {}

		public CommercialStructureContribution( AbstractExStructure master, XmlElement pic, XmlElement main, bool opposite) 
			: base(master, pic, main,opposite) {}

		protected override StructureGroup getGroup( string name ) {
			return PluginManager.theInstance.commercialStructureGroup[name];
		}

		public override Structure create( WorldLocator wLoc, bool initiallyOwned ) {
			return new Commercial( this, wLoc, initiallyOwned );
		}

		public override bool canBeBuilt( Location baseLoc, ControlMode cm ) {
			return Commercial.canBeBuilt( baseLoc, size, cm );
		}

		// TODO: additional parameters, like population and attractiveness.
	}
}
