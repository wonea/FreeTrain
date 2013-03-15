using System;
using System.Windows.Forms;
using System.Xml;
using freetrain.contributions.others;

namespace freetrain.world.rail.pole
{
	// temporary
	// TODO: instead of having our own menu item,
	// merge them to rail accessory builder form.
	
	public class MenuContributionImpl : MenuContribution
	{
		public MenuContributionImpl( XmlElement e ) : base(e) {}

		public override void mergeMenu( MainMenu containerMenu ) {
			MenuItem mi = new MenuItem("Utility Poles...",new EventHandler(onClick));
			//! MenuItem mi = new MenuItem("架線柱...",new EventHandler(onClick));
			containerMenu.MenuItems[2].MenuItems.Add(mi);
		}

		private void onClick( object sender, EventArgs e ) {
			new ControllerForm().Show();
		}
	}
}
