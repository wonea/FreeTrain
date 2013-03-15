using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.world;
using freetrain.controllers;
using freetrain.framework.graphics;
using freetrain.framework.plugin;

namespace freetrain.contributions.common
{
	/// <summary>
	/// Contribution that adds something the player can built through a UI.
	/// </summary>
	// TODO: use this class as the base class of more of the existing contributions.
	public abstract class EntityBuilderContribution : Contribution
	{
		protected EntityBuilderContribution( XmlElement e ) : base(e) {
			computerCannotBuild = (e.SelectSingleNode("computerCannotBuild")!=null);
			playerCannotBuild   = (e.SelectSingleNode("playerCannotBuild")!=null);
		}

		/// <summary>
		/// True if the computer (the development algorithm) is not allowed to
		/// build this structure.
		/// </summary>
		// TODO: make EntityBuilderContribution responsible for creating a new Plan object.
		public readonly bool computerCannotBuild;

		/// <summary>
		/// True if the player is not allowed to build this structure.
		/// </summary>
		public readonly bool playerCannotBuild;

		/// <summary>
		/// Name of this entity builder. Primarily used as the display name.
		/// Doesn't need to be unique.
		/// </summary>
		public abstract string name { get; }

		/// <summary>
		/// Creates a preview
		/// </summary>
		/// <param name="pixelSize"></param>
		/// <returns></returns>
		public abstract PreviewDrawer createPreview( Size pixelSize );

		public abstract ModalController createBuilder( IControllerSite site );
		public abstract ModalController createRemover( IControllerSite site );

		public override string ToString() { return name; }
	}
}
