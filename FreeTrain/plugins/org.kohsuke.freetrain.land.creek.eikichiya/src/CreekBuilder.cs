using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.controllers;
using freetrain.contributions.land;
using freetrain.framework.graphics;
using freetrain.views;

namespace freetrain.world.land.creek.eikichiya
{
	/// <summary>
	/// Bit-wise representation of connected directions.
	/// </summary>
	public enum Dir {
		N	= 8,
		E	= 4,
		S	= 2,
		W	= 1
	}

	/// <summary>
	/// Builds river.
	/// </summary>
	[Serializable]
	public class CreekBuilder : LandBuilderContribution
	{
		public CreekBuilder( XmlElement e ) : base(e) {
			// pictures
			Picture picture = getPicture( e );
			SpriteFactory spriteFactory = SpriteFactory.getSpriteFactory(e);
			
			sprites = new Sprite[12];
			for( int y=0; y<3; y++ ) {
				for( int x=0; x<4; x++ ) {
					sprites[x+y*4] = spriteFactory.createSprite( picture,
						new Point(0,0),
						new Point(x*32,y*16),
						new Size(32,16) );
				}
			}
		}

		private readonly Dir[] directions = new Dir[]{
			0,
			Dir.N|Dir.S,
			Dir.E|Dir.W,
			Dir.N|Dir.E|Dir.S|Dir.W,
			Dir.N|Dir.E,
			Dir.E|Dir.S,
			Dir.S|Dir.W,
			Dir.W|Dir.N,
			Dir.W|Dir.N|Dir.E,
			Dir.N|Dir.E|Dir.S,
			Dir.E|Dir.S|Dir.W,
			Dir.S|Dir.W|Dir.N
		};
		
		/// <summary> Sprite of this land contribution. </summary>
		private readonly Sprite[] sprites;





		public Sprite getSprite( Dir dir ) {
			for( int i=directions.Length-1; i>=0; i-- )
				if(directions[i]==dir)	return sprites[i];

			switch(dir) {
			case Dir.N:	case Dir.S:		return sprites[1];
			case Dir.E:	case Dir.W:		return sprites[2];
			default:					Debug.Fail("assertion failed");	return null;
			}
		}

		private Dir toDir( Direction d ) {
			if( d==Direction.NORTH )		return Dir.N;
			if( d==Direction.EAST  )		return Dir.E;
			if( d==Direction.SOUTH )		return Dir.S;
			if( d==Direction.WEST  )		return Dir.W;
			Debug.Fail("assertion failed");
			return (Dir)0;
		}



		private Random createRandomGenerator( int x1, int y1, int x2, int y2 ) {
			return new Random((x1*128+y1)^(x2*128+y2));
		}


		/// <summary>
		/// Build creek from (x1,y1) to (x2,y2) (inclusive).
		/// </summary>
		public override void create( int x1, int y1, int x2, int y2, int z, bool owned ) {
			foreach( DictionaryEntry e in computeRoute( new Location(x1,y1,z), new Location(x2,y2,z) ) ) {
				apply( (Location)e.Key, (Dir)e.Value, owned );
			}
		}

		/// <summary>
		/// Apply the given combination to the current location.
		/// </summary>
		private void apply( Location loc, Dir d, bool owned ) {
			if( World.world.isReusable(loc) && loc.z==World.world.getGroundLevel(loc) ) {
				CreekVoxel current = World.world[loc] as CreekVoxel;
				if( current!=null )		d |= current.dir;

				new CreekVoxel( loc, this, d ).isOwned = owned;
			}
		}





		/// <summary>
		/// Creates the preview image of the land builder.
		/// </summary>
		public override PreviewDrawer createPreview( Size pixelSize ) {
			PreviewDrawer drawer = new PreviewDrawer( pixelSize, new Size(1,8), 0 );

			Sprite sp = getSprite( Dir.N|Dir.S );

			for( int y=-4; y<=4; y++ )
				drawer.draw( sp, 0,y );

			return drawer;
		}
		
		/// <summary>
		/// Compute a route of the creek as map&lt;Location,Dir>.
		/// </summary>
		private IDictionary computeRoute( Location loc1, Location loc2 ) {
			// compute the route
			Random rnd = createRandomGenerator( loc1.x,loc1.y, loc2.x,loc2.y );
			Location here = loc1;
			Location there   = loc2;

			Hashtable route = new Hashtable();
			

			Direction d = null;

			while( here!=there ) {
				Direction dd = here.getDirectionTo(there);
				if( !dd.isSharp ) {// make it pararell to axis.
					if( rnd.Next(2)==0 )	dd = dd.left;
					else					dd = dd.right;
				}
				
				Dir dir = toDir(dd);
				if(d!=null)	dir|=toDir(d.opposite);
				route[here] = dir;

				here += dd;
				d = dd;
			}

			// finish up the last voxel
			if( d==null )	route[here] = (Dir)0;
			else			route[here] = toDir(d.opposite);

			return route;
		}


		
		public override ModalController createBuilder( IControllerSite site ) {
			return new Logic(this,site);
		}

		private class Logic : RectSelectorController, MapOverlay
		{
			private readonly CreekBuilder contrib;
			private IDictionary route;

			public Logic( CreekBuilder _contrib, IControllerSite site ) : base(site) {
				this.contrib = _contrib;
			}

			protected override void onRectSelected( Location loc1, Location loc2 ) {
				// don't use normalized loc1/loc2
				contrib.create(anchor,currentLoc, true);
			}

			public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {
				if( anchor!=UNPLACED && currentLoc!=UNPLACED ) {
					// compute the route
					route = contrib.computeRoute(anchor,currentLoc);
				} else
					route = null;
			}

			public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) {
				if( loc.z != currentLoc.z )	return;
				
				if( route!=null && route.Contains(loc) )
					contrib.getSprite( (Dir)route[loc] ).drawAlpha( canvas.surface, pt );
			}

			public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {
				route = null;
			}
		}

	
	}
}
