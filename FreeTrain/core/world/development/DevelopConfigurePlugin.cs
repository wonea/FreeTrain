using System;
using System.Windows.Forms;
using System.Xml;
using freetrain.contributions.others;
using freetrain.framework.plugin;

namespace freetrain.world.development
{
	public class DevelopConfigurePlugin : MenuContribution
	{
		public DevelopConfigurePlugin( XmlElement e ) : base(e) {}

		public override void mergeMenu( MainMenu containerMenu ) {
			MenuItem item = new MenuItem();
			item.Text = "Adjust Growth Parameters";
			//! item.Text = "発展パラメータ調整";
			item.Click += new System.EventHandler(onClick);

			containerMenu.MenuItems[1].MenuItems.Add(item);
		}
		
		private void onClick( object sender, EventArgs args ) {
			DevelopConfigure form = new DevelopConfigure();
			form.Show();			
		}
	}
}
