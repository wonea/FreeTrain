using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using freetrain.controllers;
using freetrain.contributions.rail;
using freetrain.framework.plugin;
using freetrain.framework.graphics;
using freetrain.world;
using freetrain.world.rail;
using freetrain.contributions.common;

namespace freetrain.world.road.dummycar
{
	[Serializable]
	public class DummyCarContribution : RailAccessoryContribution
	{
		public DummyCarContribution( XmlElement e ) : base(e) {
			// pictures
			XmlElement sprite = (XmlElement)XmlUtil.selectSingleNode(e,"sprite");
			Picture picture = getPicture(sprite);
			Point origin = XmlUtil.parsePoint( sprite.Attributes["origin"].Value );
			int offset = int.Parse( sprite.Attributes["offset"].Value );
			Size sz = new Size(32,16+offset);
			Point sprOrigin0 = new Point( origin.X, origin.Y );
			Point sprOrigin1 = new Point( 32 +origin.X, origin.Y );

			XmlElement splist = (XmlElement)XmlUtil.selectSingleNode(sprite,"variations");
			colorVariations = 0;
			IEnumerator ienum = splist.ChildNodes.GetEnumerator();
			while(ienum.MoveNext()) colorVariations++;
			sprites = new Sprite[colorVariations,2];
			currentColor = 0;
			ienum.Reset();
			colorVariations = 0;
			while(ienum.MoveNext()) 
			{
				XmlNode child = (XmlNode)ienum.Current;
				if( child.Name == "colorVariation" )
				{
					SpriteFactory factory = SpriteFactory.getSpriteFactory(child);
					sprites[colorVariations,0] = factory.createSprite( picture, new Point(0,offset), sprOrigin0, sz );
					sprites[colorVariations,1] = factory.createSprite( picture, new Point(0,offset), sprOrigin1, sz );
					colorVariations++;
				}
			}
		}

		/// <summary>
		/// Sprites. Dimensinos are [x,y] where
		/// 
		/// x is an index of variations.
		/// 
		/// y=0 if a pole is for E/W direction.
		/// y=1 if a pole is for N/S direction
		/// 
		/// </summary>
		internal readonly Sprite[,] sprites = new Sprite[10,2];

		public readonly int colorVariations;
		public int currentColor;

		public Sprite getSprites() 
		{
			if(currentColor>=colorVariations) currentColor=0;
			return sprites[currentColor,0];
		}

		public override PreviewDrawer createPreview( Size pixelSize ) {
			PreviewDrawer drawer = new PreviewDrawer( pixelSize, new Size(10,1), 0 );
			for( int x=9; x>=0; x-- ) {
				if( x==5 )	drawer.draw( sprites[currentColor,0], x,0 );
				//drawer.draw( RoadPattern.get((byte)Direction.EAST), x,0 );
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
		/// Create a new car at the specified location.
		/// dir = 0 or 1
		/// </summary>
		/// <param name="loc"></param>
		public void create( Location loc) {
			Debug.Assert( canBeBuilt(loc) );

			int x;
			RoadPattern rp = TrafficVoxel.get(loc).road.pattern;
			if( rp.hasRoad(Direction.NORTH) )	x=0;
			else								x=1;

			new DummyCar( TrafficVoxel.get(loc), this, currentColor, x );
		}

	}

}
