using System;
using System.Drawing;
using freetrain.views;
using freetrain.world;
using org.kohsuke.directdraw;

namespace freetrain.controllers
{
	/// <summary>
	/// Modifies the image of map view window
	/// by overlaying additional data to it.
	/// </summary>
	public interface MapOverlay
	{
		/// <summary>
		/// Called before any voxel is drawn.
		/// </summary>
		void drawBefore( QuarterViewDrawer view, DrawContextEx canvas );

		/// <summary>
		/// Called for each voxel that the view is trying to draw.
		/// </summary>
		void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt );

		/// <summary>
		/// Called after all the images are drawn by MapView.
		/// This can be used to draw things that will never be
		/// hidden by any objects in the World.
		/// </summary>
		void drawAfter( QuarterViewDrawer view, DrawContextEx canvas );
	}

	public class DrawContextEx : DrawContext {
		public DrawContextEx( Surface surface ) : base(surface) {}

		/// <summary>
		/// MapOverlay can use this property to pass parameters among
		/// various callbacks.
		/// </summary>
		public object tag;
	}
}
