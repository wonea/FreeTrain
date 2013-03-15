using System;
using System.Xml;
using freetrain.contributions.structs;
using freetrain.framework.plugin;

namespace freetrain.world.terrain.terrace
{
	/// <summary>
	/// Contribution implementation
	/// </summary>
	[Serializable]
	public class TerraceContributionImpl : SpecialStructureContribution
	{
		public TerraceContributionImpl( XmlElement e ) : base(e) {
			theInstance = this;
		}

		internal static TerraceContributionImpl theInstance;

		public override string name { get { return "Terraces and Cuts"; } }
		//! public override string name { get { return "雛壇と切り通し"; } }

		public override string oneLineDescription {
			get {
				return "Creating a flat surface on a mountainside by raising and lowering land";
				//! return "山腹に平地を盛り上げたり削ったりして平地を切り開きます";
			}
		}

		public override void showDialog() {
			TerraceController.create();
		}
	}
}
