using System;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.contributions.others
{
	/// <summary>
	/// Plug-in that exposes menu item.
	/// </summary>
	[Serializable]
	public abstract class MenuContribution : Contribution
	{
		protected MenuContribution( XmlElement e ) : base("menu",e.Attributes["id"].Value) {}

		/// <summary>
		/// The callee can merge menu items into container at this timing.
		/// </summary>
		public abstract void mergeMenu( MainMenu containerMenu );
	}
}
