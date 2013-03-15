using System;
using System.Drawing;
using System.Windows.Forms;
using freetrain.controllers;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.views;
using freetrain.world;
using freetrain.world.structs;

namespace freetrain.contributions.common
{
	/// <summary>
	/// FixedSizeStructureController
	/// </summary>
	public class FixedSizeStructurePlacementController : CubeSelectorController, MapOverlay
	{
		protected readonly FixedSizeStructureContribution contrib;

		private readonly AlphaBlendSpriteSet alphaSprites;



		public FixedSizeStructurePlacementController( FixedSizeStructureContribution _contrib, IControllerSite _site )
			: base( _contrib.size, _site ) {
			this.contrib = _contrib;
			this.alphaSprites = new AlphaBlendSpriteSet( contrib.sprites );
		}

		protected override void onSelected( Cube cube ) {
			if( contrib.canBeBuilt(cube.corner,ControlMode.player) ) {
				MainWindow.showError("Can not build");
				//! MainWindow.showError("設置できません");
			} else {
				CompletionHandler handler = new CompletionHandler(contrib,cube.corner,true);
				new ConstructionSite( cube.corner, new EventHandler(handler.handle), contrib.size );
			}
		}

		public override void onDetached() {
			alphaSprites.Dispose();
		}


		public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}

		public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) {
			if( currentCube.contains(loc) )
				alphaSprites.getSprite( loc-this.location ).drawAlpha( canvas.surface, pt );
		}

		public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}


		[Serializable]
		private class CompletionHandler {
			internal CompletionHandler( FixedSizeStructureContribution contribution, Location loc, bool owned ) {
				this.contribution = contribution;
				this.loc = loc;
				this.owned = owned;
			}
			private readonly FixedSizeStructureContribution contribution;
			private readonly Location loc;
			private readonly bool owned;
			public void handle( object sender, EventArgs args ) {
				contribution.create(loc,owned);
			}
		}
	}

	public class FixedSizeStructureRemovalController : CubeSelectorController
	{
		protected readonly FixedSizeStructureContribution contrib;

		public FixedSizeStructureRemovalController( FixedSizeStructureContribution _contrib, IControllerSite _site )
			: base( _contrib.size, _site ) {
			this.contrib = _contrib;
		}

		protected override void onSelected( Cube cube ) {
			PThreeDimStructure s = World.world.getEntityAt(cube.corner) as PThreeDimStructure;
			if( s==null || s.type!=contrib ) {
				MainWindow.showError("Wrong type");
				//! MainWindow.showError("種類が違います");
				return;
			}
			s.remove();
		}
	}
}
