using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using freetrain.controls;

namespace freetrain.framework
{
	/// <summary>
	/// Wraps MruMenuInline for this project.
	/// </summary>
	internal sealed class MruHelper 
	{
		private MruMenuInline mruMenu;

		/// <summary>
		/// File path to save the MRU data.
		/// </summary>
		private readonly FileInfo mruDataFile = new FileInfo(Application.ExecutablePath+".mru.dat");

		private readonly MainWindow parent;

		internal MruHelper( MainWindow _parent, MenuItem placeHolder )
		{
			this.parent = _parent;

			// load the menu
			mruMenu = new MruMenuInline( placeHolder, new MruMenu.ClickHandler(onMruClicked) );
			mruMenu.LoadFromFile(mruDataFile);

			parent.Closed += new EventHandler(onClosed);
		}

		internal void addFile( FileInfo file ) {
			mruMenu.AddFile(file);
		}

		private void onMruClicked( int index, String fileName ) {
			parent.loadGame(new FileInfo(fileName));
		}

		private void onClosed( object sender, EventArgs e ) {
			// save the setting at the exit of the app.
			mruMenu.SaveToFile(mruDataFile);
		}
	}
}
