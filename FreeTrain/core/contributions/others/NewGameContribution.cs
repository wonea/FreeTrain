using System;
using System.Xml;
using freetrain.framework.plugin;
using freetrain.world;

namespace freetrain.contributions.others
{
	/// <summary>
	/// Plug-in that creates a new game.
	/// </summary>
	public abstract class NewGameContribution : Contribution
	{
		protected NewGameContribution( XmlElement e ) : base("newGame",e.Attributes["id"].Value) {}

		/// <summary>
		/// Name of the new game.
		/// </summary>
		public abstract string name { get; }

		/// <summary>
		/// Author of the new game.
		/// </summary>
		public abstract string author { get; }

		/// <summary>
		/// Human-readable description of the new game.
		/// </summary>
		public abstract string description { get; }

		/// <summary>
		/// Creates a new game by creating a new instance of the World object.
		/// </summary>
		/// <returns>null to indicate that the operation was cancelled.</returns>
		public abstract World createNewGame();
	}
}
