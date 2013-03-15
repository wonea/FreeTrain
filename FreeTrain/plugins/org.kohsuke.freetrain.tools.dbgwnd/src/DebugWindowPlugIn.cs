using System;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using freetrain.contributions.others;
using freetrain.framework;
using freetrain.framework.plugin;

namespace freetrain.views.debug
{
	/// <summary>
	/// 
	/// </summary>
	public class DebugWindowPlugIn : MenuContribution
	{
		public DebugWindowPlugIn( XmlElement e ) : base(e) {}

		public override void mergeMenu( MainMenu containerMenu ) {
			MenuItem item = new MenuItem();
			item.Text = "Debug Window";
			//! item.Text = "デバッグウィンドウ";
			item.Click += new System.EventHandler(onClick);

			containerMenu.MenuItems[1].MenuItems.Add(item);
		}

		private void onClick( object sender, EventArgs args ) {
			Form form = new TraceWindow();
			form.MdiParent = MainWindow.mainWindow;
			form.Show();
		}
	}
}
