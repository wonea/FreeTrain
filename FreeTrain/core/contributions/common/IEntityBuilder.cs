using System;
using System.Drawing;
using freetrain.framework.graphics;
using freetrain.controllers;
using freetrain.contributions.population;

namespace freetrain.contributions.common
{
	/// <summary>
	/// IEntityBuilder の概要の説明です。
	/// </summary>
	public interface IEntityBuilder
	{
		
		/// <summary> 
		/// Population of this structure, or null if this structure is not populated. 
		/// </summary>
		Population population { get; }

		/// <summary>
		/// True if the computer (the development algorithm) is not allowed to
		/// build this structure.
		/// </summary>
		// TODO: make IEntityBuilder responsible for creating a new Plan object.
		bool computerCannotBuild { get; }

		/// <summary>
		/// True if the player is not allowed to build this structure.
		/// </summary>
		bool playerCannotBuild { get; }

		/// <summary>
		/// Name of this entity builder. Primarily used as the display name.
		/// Doesn't need to be unique.
		/// </summary>
		string name { get; }

		int price { get; }

		/// <summary>
		/// price par area (minimum).
		/// </summary>
		double pricePerArea { get; }

		/// <summary>
		/// Creates a preview
		/// </summary>
		/// <param name="pixelSize"></param>
		/// <returns></returns>
		PreviewDrawer createPreview( Size pixelSize );

		ModalController createBuilder( IControllerSite site );
		ModalController createRemover( IControllerSite site );
	}
}
