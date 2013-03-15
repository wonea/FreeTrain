using System;
using System.Drawing;
using System.Windows.Forms;
using org.kohsuke.directdraw;

namespace freetrain.views
{
	/// <summary>
	/// Draws overlay images to a QuarterViewDrawer.
	/// </summary>
	public interface WeatherOverlay : IDisposable
	{
		/// <summary>
		/// Called when the size of the QuarterViewDrawer is changed.
		/// </summary>
		void setSize( Size sz );

		/// <summary>
		/// Draws the contents of the given drawer with the overlay
		/// to the target image.
		/// </summary>
		void draw( QuarterViewDrawer drawer, Surface target, Point pt );

		/// <summary>
		/// TBD: Periodical timer notification.
		/// </summary>
		/// <returns>
		/// true if the screen needs to be updated.
		/// </returns>
		bool onTimerFired();
	}
}
