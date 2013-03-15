using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using freetrain.controllers;
using freetrain.contributions.rail;
using freetrain.framework.plugin;
using freetrain.framework.graphics;
using freetrain.world;
using freetrain.world.rail;
using freetrain.contributions.common;

namespace freetrain.world.road.accessory
{
	[Serializable]
	public class RoadAccessoryContribution : RailAccessoryContribution
	{
		public RoadAccessoryContribution( XmlElement e ) : base(e) {
			XmlElement sprite = (XmlElement)XmlUtil.selectSingleNode(e,"sprite");
			Picture picture = getPicture(sprite);
			SpriteFactory factory = SpriteFactory.getSpriteFactory(sprite);
			
			for( int y=0; y<2; y++ )
				for( int x=0; x<2; x++ )
					sprites[x,y] = factory.createSprite( picture,
						new Point(0,16), new Point( (y*2+x)*32,0 ), new Size(32,32) );
		}

		/// <summary>
		/// Sprites. Dimensinos are [x,y] where
		/// 
		/// x=0 if a sprite is for E/W direction.
		/// x=1 if a sprite is for N/S direction
		/// 
		/// y=0 if a sprite is behind a train and
		/// y=1 if a sprite is in front of a train 
		/// </summary>
		internal readonly Sprite[,] sprites = new Sprite[2,2];

		public override PreviewDrawer createPreview( Size pixelSize ) {
			PreviewDrawer drawer = new PreviewDrawer( pixelSize, new Size(10,1), 0 );
			for( int x=9; x>=0; x-- ) {
				if( x==5 )	drawer.draw( sprites[0,0], x,0 );
				//drawer.draw( RoadPattern.get((byte)Direction.EAST), x,0 );
				if( x==5 )	drawer.draw( sprites[0,1], x,0 );
			}
			return drawer;
		}

		public override ModalController createBuilder( IControllerSite site ) {
			return new ControllerImpl(this,site,false);
		}

		public override ModalController createRemover( IControllerSite site ) {
			return new ControllerImpl(this,site,true);
		}

		public bool canBeBuilt( Location loc ) {
			TrafficVoxel voxel = TrafficVoxel.get(loc);
			if( voxel==null )	return false;

			Road r = voxel.road;
			if( r==null )	return false;

			return true;
		}

		/// <summary>
		/// Create a new road accessory at the specified location.
		/// </summary>
		/// <param name="loc"></param>
		public void create( Location loc ) {
			Debug.Assert( canBeBuilt(loc) );

			int x;
			RoadPattern rp = TrafficVoxel.get(loc).road.pattern;
			if( rp.hasRoad(Direction.NORTH) )	x=1;
			else								x=0;

			new RoadAccessory( TrafficVoxel.get(loc), this, x );
		}
	}
}
