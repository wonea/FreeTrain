using System;
using System.Xml;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.contributions.others;
using freetrain.util.command;

namespace freetrain.framework.plugin.generic
{
	/// <summary>
	/// Adds "signal" menu to the main window
	/// </summary>
	public class MenuContributionImpl : MenuContribution
	{
		public MenuContributionImpl( XmlElement e ) : base(e) {}

		public override void mergeMenu( MainMenu containerMenu ) {
            //! MenuItem item = new MenuItem("建物総合(&S)...");
			MenuItem item = new MenuItem("&Structure Type Tree...");
			item.Click += new System.EventHandler(onClick);

			containerMenu.MenuItems[4].MenuItems.Add(0,item);
			MainWindow.mainWindow.SetToolBarButtonHandler("toolBar1",10,new CommandHandlerNoArg(this.ShowControllerForm));
		}

		private void onClick( object sender, EventArgs args ) {
			ShowControllerForm();
		}

		public void ShowControllerForm()
		{
			MultiSelectorController.create();
		}

	}
}
