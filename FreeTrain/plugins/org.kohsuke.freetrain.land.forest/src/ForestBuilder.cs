using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.contributions.population;
using freetrain.contributions.land;
using freetrain.controllers;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.world.terrain;
using freetrain.views;

namespace freetrain.world.land.forest
{
	/// <summary>
	/// Forest builder
	/// </summary>
	[Serializable]
	public class ForestBuilder : LandBuilderContribution
	{		
		public ForestBuilder( XmlElement e ) : base(e) {
			
			this.density = int.Parse( XmlUtil.selectSingleNode(e,"density").InnerText );
			
			int count = int.Parse( XmlUtil.selectSingleNode(e,"count").InnerText );
			SIZE sz = XmlUtil.parseSize( XmlUtil.selectSingleNode(e,"size").InnerText );

			// get the picture
			Picture picture = getPicture(e);
			SpriteFactory spriteFactory = SpriteFactory.getSpriteFactory(e);

			size = new Size(sz.x/count,sz.y);
			Point offset = new Point( size.Width/2, size.Height);
	
			sprites = new Sprite[count];
			for( int i=0; i<count; i++ )
				sprites[i] = spriteFactory.createSprite( picture, offset, new Point(i*size.Width,0), size );

			// create ground
			XmlNode node = e.SelectSingleNode("ground");
			if( node != null ) 
			{
				XmlElement pic = (XmlElement)XmlUtil.selectSingleNode(node,"picture");
				Point origin = XmlUtil.parsePoint( pic.Attributes["origin"].Value );
				picture = getPicture((XmlElement)node);
				spriteFactory =SpriteFactory.getSpriteFactory(node);
				ground = spriteFactory.createSprite( picture, new Point(0,0), origin, new Size(32,16) );
				if( ground == null ) ground = ResourceUtil.emptyChip;
			}
		}

		/// <summary> Sprite of trees. </summary>
		public readonly Sprite[] sprites;

		public readonly Sprite ground;

		/// <summary>
		/// Average density of trees in one voxel.
		/// </summary>
		public readonly int density;

		/// <summary>
		/// Size of one sprite.
		/// </summary>
		private readonly Size size;

		private static readonly Random rnd = new Random();

		private static new bool canBeBuilt(Location loc,ControlMode cm) 
		{
			if( World.world.getGroundLevel(loc) != loc.z ) 
			{
				return false;
			}
			else if(!World.world.isReusable(loc))
			{
				return (World.world[loc] is MountainVoxel );
			}
			else return true;
		}

		/// <summary>
		/// Gets the land that should be used to fill (x,y) within [x1,y1]-[x2,y2] (inclusive).
		/// </summary>
		public override void create( int x1, int y1, int x2, int y2, int z, bool owned ) {
			for( int x=x1; x<=x2; x++ ) {
				for( int y=y1; y<=y2; y++ ) {
					Location loc = new Location(x,y,z);

					if( canBeBuilt(loc, ControlMode.player) ) 
					{
						byte[] patterns = createRandomTrees();
						if( patterns.Length!=0 ) 
						{
							MountainVoxel v;
							if( World.world[loc] is MountainVoxel ) 
								v = (MountainVoxel)World.world[loc];
							else
								v = new MountainVoxel( loc, 0,0,0,0 );
							v.setTrees(ground, sprites, patterns,price);
							v.isOwned = owned;
							World.world.onVoxelUpdated(loc);
						}
					} 
				}
			}
		}

		/// <summary>
		/// Places trees randomly into the array.
		/// r[i*3+0] = offset X
		/// r[i*3+1] = offset Y
		/// r[i*3+1] = sprite index
		/// 
		/// To add more randomness, the binomial distribution is used
		/// to determine the number of trees to be created.
		/// </summary>
		public byte[] createRandomTrees() {
			// determine the # of trees
			int count = 0;
			for( int i=0; i<density*2; i++ )
				if( rnd.Next(2)==0 )	count++;

			// determine their locations and patterns
			byte[] r = new byte[count*3];
			for( int i=0; i<count; i++ ) {
				byte x = (byte)rnd.Next(16);
				byte y = (byte)rnd.Next(16);
				r[i*3+0] = (byte)(x+y);
				r[i*3+1] = (byte)(((-x+y)>>1) +8);
				r[i*3+2] = (byte)rnd.Next(sprites.Length);
			}

			// sort them by Y
			for( int i=0; i<count-1; i++ )
				for( int j=i+1; j<count; j++ )
					if( r[i*3+1] > r[j*3+1 ] ) { // swap
						byte t;
						t = r[j*3+1];
						r[j*3+1] = r[i*3+1];
						r[i*3+1] = t;

						t = r[j*3+0];
						r[j*3+0] = r[i*3+0];
						r[i*3+0] = t;

						// no need to swap the patterns. They are random any way.
					}

			return r;
		}


		/// <summary>
		/// Creates the preview image of the land builder.
		/// </summary>
		public override PreviewDrawer createPreview( Size pixelSize ) {
			const int r = 8;
			PreviewDrawer drawer = new PreviewDrawer( pixelSize, new Size(r,r), 0 );

			for( int y=0; y<r; y++ ) {
				for( int x=0; x<r; x++ ) {
					Point pt = drawer.getPoint(x,y,0);
					byte[] tree = createRandomTrees();
					
					for( int i=0; i<tree.Length; i+=3 )
						sprites[ tree[i+2] ].draw( drawer.surface,
							new Point( pt.X+tree[i+0], pt.Y+tree[i+1] ) );
				}
			}

			return drawer;
		}

		public override ModalController createBuilder( IControllerSite site ) {
			return new ControllerImpl( this, site );
		}
	}


	internal class ControllerImpl : RectSelectorController, MapOverlay
	{
		private readonly ForestBuilder contrib;

		public ControllerImpl( ForestBuilder _contrib, IControllerSite site ) : base(site) {
			this.contrib = _contrib;
		}

		protected override void onRectSelected( Location loc1, Location loc2 ) {
			contrib.create(loc1,loc2,true);
		}

		public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}

		public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) {
			if( loc.z != currentLoc.z )	return;
			
			if( anchor!=UNPLACED && loc.inBetween(anchor,currentLoc) ) {
				byte[] tree = contrib.createRandomTrees();
				
				if( contrib.ground != null )
					contrib.ground.draw( canvas.surface, pt );
				for( int i=0; i<tree.Length; i+=3 )
					contrib.sprites[ tree[i+2] ].draw( canvas.surface,
						new Point( pt.X+tree[i+0], pt.Y+tree[i+1] ) );
			}
		}

		public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}
	}
}
