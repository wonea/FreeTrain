using System;
using System.Drawing;
using System.Windows.Forms;
using freetrain.framework.graphics;
using freetrain.views;
using freetrain.controllers;
using freetrain.world;

namespace freetrain.contributions.land
{
	/// <summary>
	/// ModalController implementation typical for most of the land builder contribution.
	/// This class is here just for the code reuse.
	/// </summary>
	public class DefaultControllerImpl : RectSelectorController, MapOverlay
	{
		public delegate Sprite SpriteBuilder();
		
		private readonly LandBuilderContribution contrib;
		private readonly SpriteBuilder spriteBuilder;


		public DefaultControllerImpl( LandBuilderContribution _contrib, IControllerSite _site,
			SpriteBuilder _spriteBuilder ) : base(_site) {
			this.contrib = _contrib;
			this.spriteBuilder = _spriteBuilder;
		}

		protected override void onRectSelected( Location loc1, Location loc2 ) {
			contrib.create(loc1,loc2, true);
		}

		public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}

		public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) {
			if( loc.z != currentLoc.z )	return;
			
			if( anchor!=UNPLACED && loc.inBetween(anchor,currentLoc) )
					spriteBuilder().drawAlpha( canvas.surface, pt );
		}

		public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}
	}
}
