using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.contributions.rail;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.util;

namespace freetrain.world.rail
{
	public enum BridgeRailMode {
		Begin	=0,
		Middle	=1,
		End		=2
	}


	/// <summary>
	/// SpecialRailContribution implementation for the BridgeRail
	/// </summary>
	[Serializable]
	public class BridgeRailContributionImpl : SpecialRailContribution {
		public BridgeRailContributionImpl(XmlElement e) : base(e) {}



		protected internal override void onInitComplete() {
			Picture surface = loadPicture("BridgeRail.bmp");
			for( int i=0; i<6; i++ ) {
				backgrounds[i] = new SimpleSprite( surface, new Point(0,16), new Point(32*i, 0), new Size(32,32) );
				foregrounds[i] = new SimpleSprite( surface, new Point(0,16), new Point(32*i,32), new Size(32,32) );
			}

			Picture bridgePierImages = loadPicture("BridgePier.bmp");
			for( int i=0; i<2; i++ )
				for( int j=0; j<2; j++ )
					bridgePierSprites[i,j] = new SimpleSprite( bridgePierImages, new Point(0,16),
						new Point(j*32,i*32), new Size(32,32) );
		}

		// sprites
		private static readonly Sprite[] foregrounds = new Sprite[6];
		private static readonly Sprite[] backgrounds = new Sprite[6];





		/// <summary>
		/// Bridge rail roads.
		/// </summary>
		[Serializable]
		internal class BridgeRail : SpecialPurposeRailRoad
		{
			internal BridgeRail( TrafficVoxel tv, Direction d, int pictIdx ) : base(tv,d) {
				this.pictureIndex = (byte)pictIdx;
			}

			//
			// drawing
			//
			private readonly byte pictureIndex;

			public override void drawBefore( DrawContext display, Point pt ) {
				backgrounds[pictureIndex].draw(display.surface,pt);
				// don't call the base class so that we won't draw the rail road unnecessarily
			}
			public override void drawAfter( DrawContext display, Point pt ) {
				foregrounds[pictureIndex].draw(display.surface,pt);
			}
		}






		public override bool canBeBuilt( Location from, Location to ) {
			if( from==to )	return false;

			Direction d = from.getDirectionTo(to);

			Location here = from;

			// there must be at least one water between two locations
			while(true) {
				if( World.world[here]!=null ) {
					TrafficVoxel v = TrafficVoxel.get(here);
					if(v==null)				return false;	// occupied
					if(v.railRoad==null)	return false;	// occupied by something other than RR

					if(!v.railRoad.hasRail(d) || !v.railRoad.hasRail(d.opposite))
						return false;	// rail is running 
				}

				if( here==to )	return true;
				here = here.toward(to);
			}
		}




		public override void build( Location here, Location to ) {
			Debug.Assert( canBeBuilt(here,to) );

			Direction d = here.getDirectionTo(to);
			bool building = false;

			bool pier = true;

			while(true) {
				if( RailRoad.get(here)==null ) {
					TrafficVoxel v = TrafficVoxel.getOrCreate(here);
					if(!building) {
						building = true;
						create( v, d, BridgeRailMode.Begin );
					} else {
						create( v, d,
							( here==to || RailRoad.get(here+d)!=null )
							? BridgeRailMode.End : BridgeRailMode.Middle );
					}

					if( pier ) {
						BridgePierVoxel.electBridgeSupport( here,
							d.isParallelToX ? typeof(PierTop1Impl)  : typeof(PierTop2Impl),
							d.isParallelToX ? typeof(PierBody1Impl) : typeof(PierBody2Impl), v );
					}

					pier = !pier;

				} else {
					building = false;
				}
				
				if( here==to )	return;
				here = here.toward(to);
			}
		}

		private static void create( TrafficVoxel v, Direction d, BridgeRailMode mode ) {
			if( d==Direction.NORTH ) {
				new BridgeRail( v, d, 3+(int)mode );
				return;
			}
			if( d==Direction.SOUTH) {
				new BridgeRail( v, d, 5-(int)mode );
				return;
			}
			if( d==Direction.EAST ) {
				new BridgeRail( v, d, 2-(int)mode );
				return;
			}
			if( d==Direction.WEST) {
				new BridgeRail( v, d, (int)mode );
				return;
			}
			Debug.Assert( false );
		}



		public override void remove( Location here, Location to ) {
			if( here==to )	return;

			Direction d = here.getDirectionTo(to);

			while(true) {
				BridgeRail brr = RailRoad.get(here) as BridgeRail;
				if( brr!=null && brr.hasRail(d) ) {
					// destroy it
					brr.voxel.railRoad = null;
					// TODO: delete piers

					BridgePierVoxel.teardownBridgeSupport( here, TrafficVoxel.get(here) );
				}

				if(here==to)	return;
				here = here.toward(to);
			}
		}




		public override string name { get { return "Bridge"; } }
		//! public override string name { get { return "鉄橋"; } }

		public override string oneLineDescription { get { return "Reinforced bridge"; } }
		//! public override string oneLineDescription { get { return "鉄筋の鉄橋"; } }

		public override Bitmap previewBitmap {
			get {
				using( Stream s = parent.loadStream("BridgePreview.bmp") ) {
					return new Bitmap(s);
				}
			}
		}







		private static readonly Sprite[,] bridgePierSprites = new Sprite[2,2];




		[Serializable]
		public class PierTop1Impl : BridgePierVoxel {
			public PierTop1Impl( int x, int y, int z, Entity e ) : base(x,y,z,e) {}
			protected override Sprite sprite { get { return bridgePierSprites[0,0]; } }
		}
		[Serializable]
		public class PierBody1Impl : BridgePierVoxel {
			public PierBody1Impl( int x, int y, int z, Entity e ) : base(x,y,z,e) {}
			protected override Sprite sprite { get { return bridgePierSprites[0,1]; } }
		}

		[Serializable]
		public class PierTop2Impl : BridgePierVoxel {
			public PierTop2Impl( int x, int y, int z, Entity e ) : base(x,y,z,e) {}
			protected override Sprite sprite { get { return bridgePierSprites[1,0]; } }
		}
		[Serializable]
		public class PierBody2Impl : BridgePierVoxel {
			public PierBody2Impl( int x, int y, int z, Entity e ) : base(x,y,z,e) {}
			protected override Sprite sprite { get { return bridgePierSprites[1,1]; } }
		}
	}
}
