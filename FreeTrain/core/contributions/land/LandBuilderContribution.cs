using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.world;
using freetrain.world.terrain;
using freetrain.contributions.common;
using freetrain.controllers;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;

namespace freetrain.contributions.land
{
	/// <summary>
	/// Plug-in that places land voxels.
	/// 
	/// This contribution allows the tiling algorithm to be customized.
	/// </summary>
	[Serializable]
	public abstract class LandBuilderContribution : StructureContribution
	{
		protected LandBuilderContribution( XmlElement e ) : base(e) {
			XmlNode gridNode = e.SelectSingleNode("grid");
			if( gridNode==null )
				_grid = new Size(1,1);
			else
				_grid = XmlUtil.parseSize(gridNode.InnerText);

			_price = int.Parse( XmlUtil.selectSingleNode( e, "price" ).InnerText );
		}

		private readonly Size _grid;

		public Size Grid { get { return _grid; } }

		/// <summary> Price of the land per voxel. </summary>
		protected readonly int _price;
		public override int price { get{ return _price; } }
		public override double pricePerArea { get{ return _price; } }

		protected override StructureGroup getGroup(string name) {
			return (StructureGroup)PluginManager.theInstance.landBuilderGroup[name];
		}

		public abstract void create( int x1, int y1, int x2, int y2, int z, bool owned );

		/// <summary>
		/// Fills the specified region with lands.
		/// </summary>
		public void create( Location loc1, Location loc2, bool owned ) {
			Debug.Assert( loc1.z==loc2.z );
			int z = loc1.z;
			int minx,maxx;
			int miny,maxy;
			int wx,wy;
			if(loc1.x > loc2.x ){
				wx = Math.Max(loc1.x - loc2.x,_grid.Width-1);
				maxx = loc1.x;
				minx = maxx - wx+wx%_grid.Width;
			}
			else {
				wx = Math.Max(loc2.x - loc1.x,_grid.Width-1);
				minx = loc1.x;
				maxx = minx + wx-wx%_grid.Width;
			}
			if(loc1.y > loc2.y ){
				wy = Math.Max(loc1.y - loc2.y,_grid.Height-1);
				maxy = loc1.y;
				miny = maxy - wy+wy%_grid.Height;
			}
			else {
				wy = Math.Max(loc2.y - loc1.y,_grid.Height-1);
				miny = loc1.y;
				maxy = miny + wy-wy%_grid.Height;
			}
			
			create( minx, miny, maxx, maxy, z, owned );
		}

		/// <summary> Creates a single patch. </summary>
		public void create( Location loc, bool owned ) {
			create( loc, loc, owned );
		}

		public override ModalController createRemover( IControllerSite site ) {
			return new DefaultControllerImpl(this,site,
				new DefaultControllerImpl.SpriteBuilder(getSprite));
		}

		private static Sprite getSprite() {
			return ResourceUtil.removerChip;
			//return ResourceUtil.emptyChip;
		}
	}
}
