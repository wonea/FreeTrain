using System;
using System.Drawing;
using freetrain.controllers;
using freetrain.views;
using freetrain.views.map;

namespace freetrain.world.terrain.terrace
{
	/// <summary>
	/// We have four different strategies depending on the subject and the mode.
	/// </summary>
	internal interface Strategy {
		LocationDisambiguator disambiguator { get; }
		void onClick(MapViewWindow view, Location loc,Point ab);
		void drawVoxel( QuarterViewDrawer view, DrawContextEx dc, Location loc, Point pt );
	}
}
