using System;
using System.Windows.Forms;
using System.Xml;
using freetrain.contributions.others;
using freetrain.framework.plugin;

namespace freetrain.world.development
{
	public class LandValueInspectorPlugin : MenuContribution
	{
		public LandValueInspectorPlugin( XmlElement e ) : base(e) {}

		public override void mergeMenu( MainMenu containerMenu ) {
			MenuItem item = new MenuItem();
			item.Text = "Inspect Land Value";
			//! item.Text = "地価の検査";
			item.Click += new System.EventHandler(onClick);

			containerMenu.MenuItems[1].MenuItems.Add(item);
		}

		private void onClick( object sender, EventArgs args ) {
			new LandValueInspector();
		}
	}
}
