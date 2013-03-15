using System;
using System.Collections;
using System.Drawing;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.controllers;
using freetrain.framework.plugin;
using freetrain.framework.graphics;
using freetrain.views;
using freetrain.world;
using freetrain.world.land;

namespace freetrain.contributions.land
{
	/// <summary>
	/// RandomLandBuilder の概要の説明です。
	/// </summary>
	[Serializable]
	public class RandomLandBuilder : LandBuilderContribution
	{
		public RandomLandBuilder( XmlElement e ) : base(e) {
			ArrayList array = new ArrayList();
			string[] guids = XmlUtil.selectSingleNode( e, "lands" ).InnerText.Split(' ','\t','\r','\n');
			for( int i=0; i<guids.Length; i++ ) {
				if(guids[i].Length!=0)
					array.Add(guids[i]);
			}
			lands = (string[])array.ToArray(typeof(string));
		}

		private static readonly Random random = new Random();

		private StaticLandBuilder getLand() {
			return (StaticLandBuilder)
				PluginManager.theInstance.getContribution(lands[random.Next(lands.Length)]);
		}

		/// <summary>
		/// Lands in this array will be placed randomly.
		/// </summary>
		private readonly string[] lands;


		public override void create( int x1, int y1, int x2, int y2, int z, bool owned ) {
			for( int x=x1; x<=x2; x++ ) {
				for( int y=y1; y<=y2; y++ ) {
					Location loc = new Location(x,y,z);
					if( LandVoxel.canBeBuilt(loc) )
						new StaticLandVoxel( loc, getLand() ).isOwned = owned;
				}
			}
		}

		/// <summary>
		/// Creates the preview image of the land builder.
		/// </summary>
		public override PreviewDrawer createPreview( Size pixelSize ) {
			PreviewDrawer drawer = new PreviewDrawer( pixelSize, new Size(10,10), 0 );

			for( int y=0; y<10; y++ )
				for( int x=0; x<10; x++ )
					drawer.draw( getLand().sprite, x, y );

			return drawer;
		}

		public override ModalController createBuilder( IControllerSite site ) {
			return new DefaultControllerImpl( this, site, new DefaultControllerImpl.SpriteBuilder(getLandSprite) );
		}

		private Sprite getLandSprite() {
			return getLand().sprite;
		}
	}
}
