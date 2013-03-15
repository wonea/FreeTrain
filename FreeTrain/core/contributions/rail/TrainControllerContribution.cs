using System;
using System.Xml;
using freetrain.framework.plugin;
using freetrain.world.rail;

namespace freetrain.contributions.rail
{
	/// <summary>
	/// plug-in that provides TrainController implementations
	/// </summary>
	[Serializable]
	public abstract class TrainControllerContribution : Contribution
	{
		protected TrainControllerContribution( XmlElement e ) :
			base("trainController",e.Attributes["id"].Value) {}

		/// <summary>
		/// Creates a new instance of TrainController.
		/// </summary>
		public abstract TrainController newController( string name );

		/// <summary>
		/// Gets the name of this train controller type.
		/// </summary>
		public abstract string name { get; }

		/// <summary>
		/// Gets the description of this train controller type.
		/// </summary>
		public abstract string description { get; }
	}
}
