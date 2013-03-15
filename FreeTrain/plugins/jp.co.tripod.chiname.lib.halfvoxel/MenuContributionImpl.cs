using System;
using System.Xml;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.contributions.others;

namespace freetrain.world.structs.hv
{
	/// <summary>
	/// Adds "half voxel" menu to the main window
	/// </summary>
	public class MenuContributionImpl : MenuContribution
	{
		public MenuContributionImpl( XmlElement e ) : base(e) {}

		public override void mergeMenu( MainMenu containerMenu ) 
		{
			MenuItem item = new MenuItem("&Half-tile Construction...");
			//! MenuItem item = new MenuItem("半ボクセル建築(&H)...");
			item.Click += new System.EventHandler(onClick);

			containerMenu.MenuItems[4].MenuItems.Add(item);
		}

		private void onClick( object sender, EventArgs e ) 
		{
			new ControllerForm().Show();
		}
	}
}
