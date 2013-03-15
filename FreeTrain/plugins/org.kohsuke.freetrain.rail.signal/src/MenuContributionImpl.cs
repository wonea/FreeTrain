using System;
using System.Xml;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.contributions.others;

namespace freetrain.world.rail.signal
{
	/// <summary>
	/// Adds "signal" menu to the main window
	/// </summary>
	public class MenuContributionImpl : MenuContribution
	{
		public MenuContributionImpl( XmlElement e ) : base(e) {}

		public override void mergeMenu( MainMenu containerMenu ) {
			MenuItem item = new MenuItem("Si&gnals...");
			//! MenuItem item = new MenuItem("信号(&G)...");
			item.Click += new System.EventHandler(onClick);

			containerMenu.MenuItems[2].MenuItems.Add(5,item);
		}

		private void onClick( object sender, EventArgs args ) {
			SignalRailController.create();
		}
	}
}
