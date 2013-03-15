using System;
using System.Drawing;
using System.Diagnostics;
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
using freetrain.world;

namespace freetrain.contributions.fence
{
	[Serializable]
	public class FenceBuilder : LandBuilderContribution , Fence
	{

		public FenceBuilder( XmlElement e ) : base(e) 
		{
			
			// pictures
			Picture picture = getPicture(e);

			XmlElement pic = (XmlElement)XmlUtil.selectSingleNode(e,"picture");
			SpriteFactory spriteFactory = SpriteFactory.getSpriteFactory(e);
			int offset = int.Parse( pic.Attributes["offset"].Value );

			Point pt = new Point(0,offset);
			Size sz = new Size(32,32);
			sprites = new Sprite[4];
			Point po;

			po = getOrigin(pic,"left_back");
			if( po.X>=0 ) 
				sprites[0] = spriteFactory.createSprite( picture, pt, po, sz );
			else if( sprites[0]==null)
				throw new ArgumentException("missing left_back sprite.");
			po = getOrigin(pic,"right_back");
			if( po.X>=0 ) 
				sprites[1] = spriteFactory.createSprite( picture, pt, po, sz );
			else if( sprites[1]==null)
				throw new ArgumentException("missing right_back sprite.");
			po = getOrigin(pic,"right_front");
			if( po.X>=0 ) 
				sprites[2] = spriteFactory.createSprite( picture, pt, po, sz );
			else if( sprites[2]==null)
				throw new ArgumentException("missing right_front sprite.");
			po = getOrigin(pic,"left_front");
			if( po.X>=0 ) 
				sprites[3] = spriteFactory.createSprite( picture, pt, po, sz );
			else if( sprites[3]==null)
				throw new ArgumentException("missing left_front sprite.");
		}

		private Point getOrigin( XmlElement e, string Subnode )
		{
			XmlNode cxe = e.SelectSingleNode(Subnode);
			if( cxe != null )
				return XmlUtil.parsePoint(cxe.InnerText);
			else
				return new Point(-1,-1);
		}

		/// <summary> Sprite of this land contribution. </summary>
		public readonly Sprite[] sprites;


		/// <summary>
		/// Gets the land that should be used to fill (x,y) within [x1,y1]-[x2,y2] (inclusive).
		/// </summary>
		public override void create( int x1, int y1, int x2, int y2, int z, bool owned ) 
		{
			for( int x=x1; x<=x2; x++ ) 
			{
				setFence( new Location(x,y1,z) ,Direction.NORTH );
				setFence( new Location(x,y2,z) ,Direction.SOUTH );
			}
			for( int y=y1; y<=y2; y++ ) 
			{
				setFence( new Location(x1,y,z) ,Direction.WEST );
				setFence( new Location(x2,y,z) ,Direction.EAST );
			}
		}

		/// <summary>
		/// Gets the land that should be used to fill (x,y) within [x1,y1]-[x2,y2] (inclusive).
		/// </summary>
		public void create( Location loc1, Location loc2,  Direction side ) 
		{
			int z = loc1.z;

			int minx = Math.Min( loc1.x, loc2.x );
			int maxx = Math.Max( loc1.x, loc2.x );
			
			int miny = Math.Min( loc1.y, loc2.y );
			int maxy = Math.Max( loc1.y, loc2.y );

			create( minx, miny, maxx, maxy, z, side );
		}

		public void create( int x1, int y1, int x2, int y2, int z, Direction side ) 
		{
			for( int x=x1; x<=x2; x++ ) 
			{
				for( int y=y1; y<=y2; y++ ) 
				{
					setFence( new Location(x,y,z) ,side );
				}
			}
		}

		protected virtual void setFence(Location loc, Direction d ) 
		{
			if( World.world[loc] == null ) 
			{
				new DummyVoxel( loc );		
				World.world[loc].setFence(d,this);			
			}
			else 
			{
				World.world[loc].setFence(d,this);
				World.world.onVoxelUpdated(loc);
			}
		}

		// draw fence
		public void drawFence( Surface surface, Point pt, Direction d ) 
		{
			sprites[d.index/2].draw(surface,pt);
		}

		public string fence_id { get { return base.id; } }

		/// <summary>
		/// Creates the preview image of the land builder.
		/// </summary>
		public override PreviewDrawer createPreview( Size pixelSize ) 
		{
			PreviewDrawer drawer = new PreviewDrawer( pixelSize, new Size(3,3), 0 );

			for( int x=0; x<=3; x++ ) 
			{
				drawer.draw( sprites[Direction.NORTH.index/2], x, 0 );
				drawer.draw( sprites[Direction.SOUTH.index/2], x, 3 );
			}
			for( int y=0; y<=3; y++ ) 
			{
				drawer.draw( sprites[Direction.WEST.index/2], 0, y );
				drawer.draw( sprites[Direction.EAST.index/2], 3, y );
			}

			return drawer;
		}


		public override ModalController createBuilder( IControllerSite site ) 
		{
			return new LogicL(this,site);
		}

		#region LogicL class
		private class LogicL : BorderLineSelectorController, MapOverlay
		{
			private readonly FenceBuilder contrib;

			public LogicL( FenceBuilder _contrib, IControllerSite site ) : base(site) 
			{
				this.contrib = _contrib;
			}

			protected override void onLineSelected( Location loc1, Location loc2, Direction side ) 
			{
				contrib.create(loc1,loc2,side);
			}

			protected override void onLineUpdated( Location loc1, Location loc2, Direction side ) 
			{
			}

			public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}

			public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) 
			{
				//if( loc.z != currentLoc.z )	return;
								
				if( anchor!=UNPLACED && loc.inBetween(anchor,currentPos) ) 
				{								
					Location loc1 = base.location1;
					Location loc2 = base.location2;
					Direction side = base.currentSide;
					//contrib.sprites[contrib.getIndex(loc1.x,loc.x,loc2.x), contrib.getIndex(loc1.y,loc.y,loc2.y)]
					contrib.sprites[currentSide.index/2].drawAlpha( canvas.surface, pt );
				}
			}

			public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}
		}
		#endregion


		#region LogicR class
		private class LogicR : RectSelectorController, MapOverlay
		{
			private readonly FenceBuilder contrib;

			public LogicR( FenceBuilder _contrib, IControllerSite site ) : base(site) 
			{
				this.contrib = _contrib;
			}

			protected override void onRectSelected( Location loc1, Location loc2 ) 
			{
				contrib.create(loc1,loc2,true);
			}

			public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}

			public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) 
			{
								if( loc.z != currentLoc.z )	return;
								
								if( anchor!=UNPLACED && loc.inBetween(anchor,currentLoc) ) 
								{								
									Location loc1 = base.location1;
									Location loc2 = base.location2;
									//contrib.sprites[contrib.getIndex(loc1.x,loc.x,loc2.x), contrib.getIndex(loc1.y,loc.y,loc2.y)]
									if( loc1.x == loc.x) 
										contrib.sprites[Direction.WEST.index/2].drawAlpha( canvas.surface, pt );
									if( loc1.y == loc.y) 
										contrib.sprites[Direction.NORTH.index/2].drawAlpha( canvas.surface, pt );
									if( loc2.x == loc.x) 
										contrib.sprites[Direction.EAST.index/2].drawAlpha( canvas.surface, pt );
									if( loc2.y == loc.y) 
										contrib.sprites[Direction.SOUTH.index/2].drawAlpha( canvas.surface, pt );
								}
			}

			public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}
		}
		#endregion

		[Serializable]
		private class DummyVoxel : AbstractVoxelImpl, Entity									 
		{
			public DummyVoxel( int x, int y, int z ) : this( new Location(x,y,z) ) {}
			public DummyVoxel( Location loc ) : base(loc) {}
			internal protected DummyVoxel( WorldLocator wloc ) : base(wloc) {}

			public override bool transparent { get { return true; }	}

			public override void draw( DrawContext surface, Point pt, int heightCutDiff ) 
			{
				// draw nothing
			}
			
			public override Entity entity { get { return this; } }
 
			#region Entity implementation
			public virtual bool isSilentlyReclaimable { get { return true; } }
			public bool isOwned { get { return true; } }

			public int entityValue { get{ return 0; } }

			public void remove() 
			{
				World.world.remove(this);
				if(onEntityRemoved!=null)	onEntityRemoved(this,null);
			}
			public event EventHandler onEntityRemoved;
			#endregion

		}
	}
}
