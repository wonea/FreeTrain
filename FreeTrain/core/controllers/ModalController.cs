using System;
using System.Drawing;
using System.Windows.Forms;
using freetrain.views.map;
using freetrain.world;

namespace freetrain.controllers
{
	/// <summary>
	/// User Interface by using MapViewWindow.
	/// 
	/// When active, a ModalController can receive mouse events
	/// on map windows, can modify the image of the map view,
	/// and can affect how mouse clicks are interpreted.
	/// </summary>
	public interface ModalController
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="source">sender of the event.</param>
		/// <param name="loc">(X,Y,Z) location that was clicked</param>
		/// <param name="ab">(A,B) location that was clicked.</param>
		void onClick( MapViewWindow source, Location loc, Point ab );
		void onRightClick( MapViewWindow source, Location loc, Point ab );
		void onMouseMove( MapViewWindow source, Location loc, Point ab );

		/// <summary>
		/// Called when the controller gets activated.
		/// </summary>
		void onAttached();

		/// <summary>
		/// Called when the controller gets deactivated.
		/// </summary>
		void onDetached();

//		/// <summary>
//		/// Closes the controller. A host uses this method to close
//		/// a controller.
//		/// </summary>
//		void close();

		/// <summary>
		/// Gets the display name of this controller.
		/// </summary>
		string name { get; }

		/// <summary>
		/// Gets the disambiguator associated with this controller, if any.
		/// </summary>
		LocationDisambiguator disambiguator { get; }

		/// <summary>
		/// If this controller needs to modify the map view, return non-null value.
		/// </summary>
		MapOverlay overlay { get; }
	}
}
