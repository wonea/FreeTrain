using System;
using System.Drawing;
using freetrain.controllers;
using freetrain.framework;
using freetrain.views;
using freetrain.views.map;

namespace freetrain.world.terrain.terrace
{
	/// <summary>
	/// Terrace removal mode
	/// </summary>
	internal class TerraceRemovalStrategy : Strategy, LocationDisambiguator {
		public LocationDisambiguator disambiguator { get { return this; } }
	
		public bool isSelectable( Location loc ) {
			return World.world[loc] is TerraceVoxel;
		}

		public void onClick(MapViewWindow view, Location loc, Point ab ) {
			TerraceVoxel tv = World.world[loc] as TerraceVoxel;
			
			if(tv==null) {
				MainWindow.showError("This is not a terrace");
				//! MainWindow.showError("雛壇ではありません");
				return;
			}
			if( World.world[loc.x,loc.y,loc.z+1]!=null ) {
				MainWindow.showError("There are obstacles uphill");
				//! MainWindow.showError("上に障害物が乗っています");
				return;
			}

			tv.remove();
		}
		public void drawVoxel( QuarterViewDrawer view, DrawContextEx dc, Location loc, Point pt ) {
			// nothing to draw
		}
	}
}
