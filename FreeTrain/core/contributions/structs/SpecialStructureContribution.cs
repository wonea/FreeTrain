using System;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.contributions.structs
{
	/// <summary>
	/// Contribution that adds a special kind of structures, like airports.
	/// </summary>
	[Serializable]
	public abstract class SpecialStructureContribution : Contribution
	{
		protected SpecialStructureContribution( XmlElement e ) : base("specialStructure",e.Attributes["id"].Value) {}

		/// <summary>
		/// Gets the name used for the menu item.
		/// </summary>
		public abstract string name { get; }

		/// <summary>
		/// Gets a one line description of this rail.
		/// </summary>
		public abstract string oneLineDescription { get; }

		/// <summary>
		/// This method is called when the menu item is selected by the user.
		/// </summary>
		public abstract void showDialog();
	}
}
