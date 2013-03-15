using System;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using freetrain.contributions.others;
using freetrain.framework;
using freetrain.framework.plugin;

namespace freetrain.tools.vcr
{
	/// <summary>
	/// 
	/// </summary>
	public class VCRContribution : MenuContribution
	{
		public VCRContribution( XmlElement e ) : base(e) {}

		public override void mergeMenu( MainMenu containerMenu ) {
			MenuItem item = new MenuItem();
			item.Text = "Video Recorder";
			//! item.Text = "ビデオレコーダ";
			item.Click += new System.EventHandler(onClick);

			containerMenu.MenuItems[1].MenuItems.Add(item);
		}

		private void onClick( object sender, EventArgs args ) {
			Form form = new VCRConsole();
			form.MdiParent = MainWindow.mainWindow;
			form.Show();
		}
	}
}
