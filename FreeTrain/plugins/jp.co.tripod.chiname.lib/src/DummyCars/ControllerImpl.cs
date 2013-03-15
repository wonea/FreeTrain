using System;
using System.Drawing;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.views;
using freetrain.views.map;
using freetrain.controllers;


namespace freetrain.world.road.dummycar
{
	/// <summary>
	/// ModalController implementation for DummyCar contribution
	/// </summary>
	public class ControllerImpl : PointSelectorController
	{
		protected readonly DummyCarContribution contribution;
		protected readonly int color;
		protected readonly bool remove;

		public ControllerImpl( DummyCarContribution _contrib, IControllerSite _site, bool _remover ) : base(_site) {
			this.remove = _remover;
			this.contribution = _contrib;
			this.color = _contrib.currentColor;
		}

		protected override void onLocationSelected( Location loc ) {
			if( remove )
			{
				if( contribution.canBeBuilt(loc))
				{
					if( TrafficVoxel.get(loc).accessory != null )
						TrafficVoxel.get(loc).accessory = null;
				}
				else
					MainWindow.showError("Can not remove");
					//! MainWindow.showError("撤去できません");
			}
			else
			{
				if( contribution.canBeBuilt(loc) )
					contribution.create(loc);
				else
					MainWindow.showError("Can not place");
					//! MainWindow.showError("設置できません");
			}
		}

		public override void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt) {
			if( base.currentPos!=loc )		return;
			if( !contribution.canBeBuilt(loc) )	return;
			
			int x;
			RoadPattern rp = TrafficVoxel.get(loc).road.pattern;
			if( rp.hasRoad(Direction.NORTH) )	x=0;
			else								x=1;

			contribution.sprites[color,x].drawAlpha( canvas.surface, pt );
		}

		public override LocationDisambiguator disambiguator {
			get {
				return RoadDisambiguator.theInstance;
			}
		}
	}

	public class RoadDisambiguator : LocationDisambiguator
	{
		// the singleton instance
		public static LocationDisambiguator theInstance = new RoadDisambiguator();
		private RoadDisambiguator() {}

		public bool isSelectable(Location loc) 
		{
			// if there's any rail roads, fine
			if( Road.get(loc)!=null )	return true;

			// or if we hit the ground
			if( World.world.getGroundLevel(loc)>=loc.z )	return true;

			return false;
		}
	}
}
