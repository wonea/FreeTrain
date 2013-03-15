using System;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using freetrain.contributions.others;
using freetrain.framework;
using freetrain.framework.plugin;

namespace freetrain.world.rail.cttrain
{
	/// <summary>
	/// Menu item contribution that allows an user to
	/// open a color config dialog.
	/// </summary>
	public class ColorConfigCommand : MenuContribution
	{
		public ColorConfigCommand( XmlElement e ) : base(e) {}

		public override void mergeMenu( MainMenu containerMenu ) {
			MenuItem item = new MenuItem();
			item.Text = "Color Test Train Settings";
			//! item.Text = "試験列車の色設定";
			item.Click += new System.EventHandler(onClick);

			containerMenu.MenuItems[1].MenuItems.Add(item);
		}

		private void onClick( object sender, EventArgs args ) {
			Form form = new ColorConfigDialog(ColorTestTrainCar.theInstance);
			MainWindow.mainWindow.AddOwnedForm(form);
			form.Show();
		}
	}
}
