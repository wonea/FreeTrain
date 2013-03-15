using System;
using System.Xml;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.contributions.others;

namespace freetrain.world.road.accessory
{
	/// <summary>
	/// Adds "accessory" menu to the main window
	/// </summary>
	public class MenuContributionImpl : MenuContribution
	{
		public MenuContributionImpl( XmlElement e ) : base(e) {}

		public override void mergeMenu( MainMenu containerMenu ) 
		{
			MenuItem item = new MenuItem("&Accessories...");
			//! MenuItem item = new MenuItem("アクセサリ(&A)...");
			item.Click += new System.EventHandler(onClick);

			containerMenu.MenuItems[3].MenuItems.Add(item);
		}

		private void onClick( object sender, EventArgs e ) 
		{
			new ControllerForm().Show();
		}
	}
}
