using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.controllers;
using freetrain.contributions.population;
using freetrain.contributions.land;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.views;

namespace freetrain.world.land.rice
{
	/// <summary>
	/// RiceFieldBuilder の概要の説明です。
	/// </summary>
	[Serializable]
	public class RiceFieldBuilder : LandBuilderContribution
	{
		public RiceFieldBuilder( XmlElement e ) : base(e) {
			// pictures
			Picture picture = getPicture(e);

			XmlElement pic = (XmlElement)XmlUtil.selectSingleNode(e,"picture");
			int offset = 0;
			XmlAttribute attr = null;
			attr = pic.Attributes["offset"];
			if(attr != null )
				offset = int.Parse( attr.Value );
			Point orig = XmlUtil.parsePoint(pic.Attributes["origin"].Value);
			SpriteFactory spriteFactory = SpriteFactory.getSpriteFactory(e);

			Point pt = new Point(0,0);
			Size sz = new Size(32,16+offset);

			sprites = new Sprite[3,3];
			sprites[0,0] = spriteFactory.createSprite( picture, pt, new Point( orig.X, orig.Y ), sz );
			sprites[1,0] = spriteFactory.createSprite( picture, pt, new Point( orig.X, orig.Y+16 ), sz );
			sprites[2,0] = spriteFactory.createSprite( picture, pt, new Point( orig.X+32, orig.Y ), sz );
			sprites[0,1] = spriteFactory.createSprite( picture, pt, new Point( orig.X+32, orig.Y+16 ), sz );
			sprites[1,1] = spriteFactory.createSprite( picture, pt, new Point( orig.X+128, orig.Y+16 ), sz );
			sprites[2,1] = spriteFactory.createSprite( picture, pt, new Point( orig.X+64, orig.Y+16 ), sz );
			sprites[0,2] = spriteFactory.createSprite( picture, pt, new Point( orig.X+96, orig.Y ), sz );
			sprites[1,2] = spriteFactory.createSprite( picture, pt, new Point( orig.X+96, orig.Y+16 ), sz );
			sprites[2,2] = spriteFactory.createSprite( picture, pt, new Point( orig.X+64, orig.Y ), sz );

		}

		/// <summary> Sprite of this land contribution. </summary>
		public readonly Sprite[,] sprites;



		/// <summary>
		/// Gets the land that should be used to fill (x,y) within [x1,y1]-[x2,y2] (inclusive).
		/// </summary>
		public override void create( int x1, int y1, int x2, int y2, int z, bool owned ) {
			for( int x=x1; x<=x2; x++ ) {
				for( int y=y1; y<=y2; y++ ) {
					Location loc = new Location(x,y,z);

					if( RiceFieldVoxel.canBeBuilt(loc) )
						new RiceFieldVoxel( loc, this, getIndex(x1,x,x2), getIndex(y1,y,y2) ).isOwned =owned;
				}
			}
		}

		private int getIndex( int min, int value, int max ) {
			if(min==value)	return 0;
			if(max==value)	return 2;
			return 1;
		}



		/// <summary>
		/// Creates the preview image of the land builder.
		/// </summary>
		public override PreviewDrawer createPreview( Size pixelSize ) {
			PreviewDrawer drawer = new PreviewDrawer( pixelSize, new Size(3,3), 0 );

			for( int y=0; y<3; y++ )
				for( int x=0; x<3; x++ )
					drawer.draw( sprites[getIndex(0,x,2),getIndex(0,y,2)], x,y );

			return drawer;
		}


		public override ModalController createBuilder( IControllerSite site ) {
			return new Logic(this,site);
		}

		private class Logic : RectSelectorController, MapOverlay
		{
			private readonly RiceFieldBuilder contrib;

			public Logic( RiceFieldBuilder _contrib, IControllerSite site ) : base(site) {
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
					contrib.sprites[contrib.getIndex(loc1.x,loc.x,loc2.x), contrib.getIndex(loc1.y,loc.y,loc2.y)]
						.drawAlpha( canvas.surface, pt );
				}
			}

			public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}
		}

	}
}
