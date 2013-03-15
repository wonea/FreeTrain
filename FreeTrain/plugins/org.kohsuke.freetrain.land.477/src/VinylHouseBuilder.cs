using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.contributions.land;
using freetrain.contributions.population;
using freetrain.controllers;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.views;

namespace freetrain.world.land.vinylhouse
{
	[Serializable]
	public abstract class VinylHouseBuilder : LandBuilderContribution
	{
		public VinylHouseBuilder( XmlElement e ) : base(e) {
			// pictures
			Picture picture = getPicture( e );
			SpriteFactory spriteFactory = SpriteFactory.getSpriteFactory(e);

			
			XmlElement pic = (XmlElement)XmlUtil.selectSingleNode(e,"picture");
			int offset = int.Parse( pic.Attributes["offset"].Value );

			Point pt = new Point(0,8);
			Size sz = new Size(32,24);

			sprites = new Sprite[3];
			sprites[0] = spriteFactory.createSprite( picture, pt, new Point(offset, 0), sz );
			sprites[1] = spriteFactory.createSprite( picture, pt, new Point(offset,24), sz );
			sprites[2] = spriteFactory.createSprite( picture, pt, new Point(offset,48), sz );

		}


		/// <summary> Sprite of this land contribution. </summary>
		public readonly Sprite[] sprites;



		/// <summary>
		/// Gets the land that should be used to fill (x,y) within [x1,y1]-[x2,y2] (inclusive).
		/// </summary>
		public override void create( int x1, int y1, int x2, int y2, int z, bool owned ) {
			for( int x=x1; x<=x2; x++ ) {
				for( int y=y1; y<=y2; y++ ) {
					Location loc = new Location(x,y,z);
					if( VinylHouseVoxel.canBeBuilt(loc) )
						new VinylHouseVoxel( loc, this,
							getSpriteIndex(x,y,x1,y1,x2,y2) ).isOwned =owned;
				}
			}
		}

		protected abstract int getSpriteIndex( int x, int y, int x1, int y1, int x2, int y2 );



		/// <summary>
		/// Creates the preview image of the land builder.
		/// </summary>
		public override PreviewDrawer createPreview( Size pixelSize ) {
			PreviewDrawer drawer = new PreviewDrawer( pixelSize, new Size(3,3), 0 );

			for( int y=0; y<3; y++ )
				for( int x=2; x>=0; x-- )
					drawer.draw( sprites[getSpriteIndex(x,y,0,2,0,2)], x,y );

			return drawer;
		}


		public override ModalController createBuilder( IControllerSite site ) {
			return new Logic(this,site);
		}

		private class Logic : RectSelectorController, MapOverlay
		{
			private readonly VinylHouseBuilder contrib;

			public Logic( VinylHouseBuilder _contrib, IControllerSite site) : base(site) {
				this.contrib = _contrib;
			}

			protected override void onRectSelected( Location loc1, Location loc2 ) {
				contrib.create(loc1,loc2,true);
			}

			public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}

			public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) {
				if( loc.z != currentLoc.z )	return;
				
				if( anchor!=UNPLACED && loc.inBetween(anchor,currentLoc) ) {
					Location loc1 = base.location1;
					Location loc2 = base.location2;
					contrib.sprites[contrib.getSpriteIndex( loc.x,loc.y, loc1.x,loc1.y, loc2.x,loc2.y )]
						.drawAlpha( canvas.surface, pt );
				}
			}

			public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}
		}	
	}



	[Serializable]
	public class XVinylHouseBuilder : VinylHouseBuilder {
		public XVinylHouseBuilder( XmlElement e ) : base(e) {}

		protected override int getSpriteIndex( int x, int y, int x1, int y1, int x2, int y2 ) {
			if(x==x1)	return 2;
			if(x==x2)	return 0;
			return 1;
		}
	}



	[Serializable]
	public class YVinylHouseBuilder : VinylHouseBuilder {
		public YVinylHouseBuilder( XmlElement e ) : base(e) {}

		protected override int getSpriteIndex( int x, int y, int x1, int y1, int x2, int y2 ) {
			if(y==y2)	return 2;
			if(y==y1)	return 0;
			return 1;
		}
	}
}
