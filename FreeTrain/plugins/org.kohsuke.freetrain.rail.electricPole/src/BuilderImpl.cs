using System;
using System.Drawing;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.views;
using freetrain.views.map;
using freetrain.controllers;

namespace freetrain.world.rail.pole
{
	/// <summary>
	/// ModalController implementation for pole contribution
	/// </summary>
	public class BuilderImpl : PointSelectorController
	{
		protected readonly ElectricPoleContribution contribution;

		public BuilderImpl( ElectricPoleContribution _contrib, IControllerSite _site ) : base(_site) {
			this.contribution = _contrib;
		}

		protected override void onLocationSelected( Location loc ) {
			if( contribution.canBeBuilt(loc) )
				contribution.create(loc);
			else
				MainWindow.showError("Can not place");
				//! MainWindow.showError("設置できません");
		}

		public override void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt) {
			if( base.currentPos!=loc )		return;
			if( !contribution.canBeBuilt(loc) )	return;
			
			int x;
			RailPattern rp = TrafficVoxel.get(loc).railRoad.getPattern();
			if( rp.hasRail(Direction.NORTH) )	x=1;
			else								x=0;

			contribution.sprites[x,0].drawAlpha( canvas.surface, pt );
			contribution.sprites[x,1].drawAlpha( canvas.surface, pt );
		}

		public override LocationDisambiguator disambiguator {
			get {
				return RailRoadDisambiguator.theInstance;
			}
		}
	}
}
