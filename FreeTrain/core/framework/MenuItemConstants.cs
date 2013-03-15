using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace freetrain.framework
{
	/// <summary>
	/// Enumeration of the menus.
	/// </summary>
	public sealed class MenuItemConstants {
		public readonly string displayName;

		private MenuItemConstants( string _displayName ) {
			this.displayName = _displayName;
		}

		/// <summary>
		/// Obtain a menu item constants from its display name.
		/// Throws an exception if it fails to parse.
		/// </summary>
		public static MenuItemConstants parse( string name ) {
			foreach( MenuItemConstants c in
				new MenuItemConstants[]{ FILE, VIEW, RAIL, ROAD, CONSTRUCTION, CONFIG, HELP } ) {

				if( c.displayName.Equals(name) )
					return c;
			}

			throw new Exception("undefined menu item:"+name);
		}

		/// <summary>
		/// Associated menu item.
		/// </summary>
		public MenuItem menuItem {
			get {
				MainWindow mw = MainWindow.mainWindow;
				// TODO: is there any better way to implement this?
				if(this==FILE)			return mw.menuItem_file;
				if(this==VIEW)			return mw.menuItem_view;
				if(this==RAIL)			return mw.menuItem_rail;
				if(this==ROAD)			return mw.menuItem_road;
				if(this==CONSTRUCTION)	return mw.menuItem_construction;
				if(this==CONFIG)		return mw.menuItem_config;
				if(this==HELP)			return mw.menuItem_help;

				Debug.Fail("undefined menu item constant");
				return null;
			}
		}

		//
		// constants
		// 
		public static readonly MenuItemConstants FILE			= new MenuItemConstants("file");
		public static readonly MenuItemConstants VIEW			= new MenuItemConstants("view");
		public static readonly MenuItemConstants RAIL			= new MenuItemConstants("rail");
		public static readonly MenuItemConstants ROAD			= new MenuItemConstants("road");
		public static readonly MenuItemConstants CONSTRUCTION	= new MenuItemConstants("construction");
		public static readonly MenuItemConstants CONFIG			= new MenuItemConstants("config");
		public static readonly MenuItemConstants HELP			= new MenuItemConstants("help");

		private static readonly MenuItemConstants[] all = {
			FILE, VIEW, RAIL, ROAD, CONSTRUCTION, CONFIG, HELP
		};
	}
}
