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
	public class RemoverImpl : PointSelectorController
	{
		protected readonly ElectricPoleContribution contribution;

		public RemoverImpl( ElectricPoleContribution _contrib, IControllerSite _site ) : base(_site) {
			this.contribution = _contrib;
		}


		protected override void onLocationSelected( Location loc ) {
			ElectricPole e = ElectricPole.get(loc);
			if(e!=null)
				TrafficVoxel.get(loc).accessory = null;
		}

		public override void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt) {
			if( base.currentPos!=loc )		return;
			
			ElectricPole e = ElectricPole.get(loc);
			if( e!=null )
				ResourceUtil.emptyChip.drawShape( canvas.surface, pt, Color.Red );
		}

		public override LocationDisambiguator disambiguator {
			get {
				return RailRoadDisambiguator.theInstance;
			}
		}
	}
}
