using System;
using System.Xml;
using freetrain.framework.plugin;
using freetrain.contributions.structs;

namespace freetrain.world.soccerstadium
{
	[Serializable]
	public class ContributionImpl : SpecialStructureContribution
	{
		public ContributionImpl( XmlElement e ) : base(e) {
		}

		public override string name { get { return "Soccer Stadium"; } }
		//! public override string name { get { return "サッカースタジアム"; } }

		public override string oneLineDescription {
			get {
				return "Build a soccer stadium and manage a team";
				//! return "サッカースタジアムを建設してチームを経営します";
			}
		}

		public override void showDialog() {
			PlacementController.create();
		}



	}
}
