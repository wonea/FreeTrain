using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using freetrain.util;
using freetrain.controllers;
using freetrain.contributions.common;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.world;
using freetrain.world.structs;
using org.kohsuke.directdraw;

namespace freetrain.contributions.structs
{
	/// <summary>
	/// Building of a variable height.
	/// </summary>
	[Serializable]
	public class VarHeightBuildingContribution : StructureContribution, IPreviewWorldBuilder
	{
		public VarHeightBuildingContribution( XmlElement e ) : base(e) {
			_price = int.Parse( XmlUtil.selectSingleNode(e,"price").InnerText );
			
			size = XmlUtil.parseSize( XmlUtil.selectSingleNode(e,"size").InnerText );
			_ppa = _price/Math.Max(1,size.x*size.y);
			minHeight = int.Parse( XmlUtil.selectSingleNode(e,"minHeight").InnerText );
			maxHeight = int.Parse( XmlUtil.selectSingleNode(e,"maxHeight").InnerText );

			XmlElement pics = (XmlElement)XmlUtil.selectSingleNode(e,"pictures");

			tops    = loadSpriteSets( pics.SelectNodes("top"   ) );
			bottoms = loadSpriteSets( pics.SelectNodes("bottom") );

			XmlElement m = (XmlElement)XmlUtil.selectSingleNode(pics,"middle");
			middle = PluginUtil.getSpriteLoader(m).load2D( m, size, 16 );
		}

		public VarHeightBuildingContribution(AbstractExStructure master, XmlElement pic, XmlElement main, bool opposite ) : base (main) 
		{
			_price = master.unitPrice;
			if( opposite )
				size = new SIZE(master.size.y, master.size.x);
			else
				size = master.size;
			_ppa = _price/Math.Max(1,size.x*size.y);
			minHeight = master.minHeight;
			maxHeight = master.maxHeight;


			tops    = loadSpriteSets( pic.SelectNodes("top"   ) );
			bottoms = loadSpriteSets( pic.SelectNodes("bottom") );

			XmlElement m = (XmlElement)XmlUtil.selectSingleNode(pic,"middle");
			XmlAttribute a = m.Attributes["overlay"];
			if(a!=null && a.InnerText.Equals("true"))
				overlay = true;
			middle = PluginUtil.getSpriteLoader(m).load2D( m, size, size.x*8 );
		}

		protected override StructureGroup getGroup( string name ) {
			return PluginManager.theInstance.varHeightBuildingsGroup[name];
		}

		private Sprite[][,] loadSpriteSets( XmlNodeList list ) {
			Sprite[][,] sprites = new Sprite[list.Count][,];

			int idx=0;
			foreach( XmlElement e in list ) {
				sprites[idx++] = PluginUtil.getSpriteLoader(e).load2D( e, size, size.x*8 );
			}
			return sprites;
		}

		/// <summary>Price of this structure per height.</summary>
		protected readonly int _price;
		public override int price {	get { return _price; } }
		protected readonly double _ppa;
		public override double pricePerArea { get { return _price; } }

		/// <summary>Sprite sets.</summary>
		private readonly Sprite[][,] tops,bottoms;
		private readonly Sprite[,]   middle;
		private bool overlay = false;

		/// <summary> Sprite to draw the structure </summary>
		public Sprite[] getSprites( int x, int y, int z, int height ) {
			if( z>=height-tops.Length ) {
				if(overlay && z == bottoms.Length-1 )
					return new Sprite[]{bottoms[z][x,y],tops[height-z-1][x,y]};
				else
					return new Sprite[]{tops[height-z-1][x,y]};
			}
			if( z<bottoms.Length ) {
				if(overlay && z == bottoms.Length-1 )
					return new Sprite[]{bottoms[z][x,y],middle[x,y]};
				else
					return new Sprite[]{bottoms[z][x,y]};
			}
			return new Sprite[]{middle[x,y]};
		}

		/// <summary> Size of the basement of this structure in voxel by voxel. </summary>
		public readonly SIZE size;

		/// <summary> Range of the possible height of the structure in voxel unit. </summary>
		public readonly int minHeight,maxHeight;



		/// <summary>
		/// Creates a new instance of this structure type to the specified location.
		/// </summary>
		public Structure create( WorldLocator wLoc, int height, bool initiallyOwned ) {
			return new VarHeightBuilding(this, wLoc , height,initiallyOwned);
		}

		public Structure create( Location baseLoc, int height, bool initiallyOwned ){
			Debug.Assert( canBeBuilt(baseLoc,height) );
			return create(new WorldLocator(World.world,baseLoc),height,initiallyOwned);
		}

		/// <summary>
		/// Returns true iff this structure can be built at the specified location.
		/// </summary>
		public bool canBeBuilt( Location baseLoc, int height ) {
			for( int z=0; z<height; z++ )
				for( int y=0; y<size.y; y++ )
					for( int x=0; x<size.x; x++ )
						if( World.world[ baseLoc.x+x, baseLoc.y+y, baseLoc.z+z ]!=null )
							return false;

			return true;
		}

		public override PreviewDrawer createPreview( Size pixelSize ) {
			PreviewDrawer drawer = new PreviewDrawer( pixelSize, size, tops.Length+bottoms.Length+1/*middle*/ );

			int z=0;
			for( int i=bottoms.Length-1; i>=0; i-- )
				drawer.draw( bottoms[i], 0, 0, z++ );
			if(overlay)
				drawer.draw( middle, 0,0, z-1 );
			drawer.draw( middle, 0,0, z++ );
			for( int i=tops.Length-1; i>=0; i-- )
				drawer.draw( tops[i], 0, 0, z++ );

			return drawer;
		}

		public PreviewDrawer createPreview( Size pixelSize ,int height ) 
		{
			PreviewDrawer drawer = new PreviewDrawer( pixelSize, size, maxHeight/*middle*/ );
			int mh = height-2;
			int z=0;
			for( int i=bottoms.Length-1; i>=0; i-- )
				drawer.draw( bottoms[i], 0, 0, z++ );
			if(overlay) {
				z--;
				mh++;
			}
			for( int i = 0; i< mh; i++ )
				drawer.draw( middle, 0,0, z++ );
			for( int i=tops.Length-1; i>=0; i-- )
				drawer.draw( tops[i], 0, 0, z++ );

			return drawer;
		}

		public override ModalController createBuilder( IControllerSite site ) {
			// TODO
			throw new NotImplementedException();
		}
		public override ModalController createRemover( IControllerSite site ) {
			// TODO
			throw new NotImplementedException();
		}
		#region IPreviewWorldBuilder o

		public World CreatePreviewWorld(Size minsizePixel, IDictionary options) {
			Distance d = new Distance(size.x*2+1,size.y*2+1,maxHeight);
			World w = World.CreatePreviewWorld(minsizePixel,d);
			int v = w.size.y-size.y-2;
			Location l = w.toXYZ((w.size.x-size.x-size.y-1)/2,v,0);
			create(new WorldLocator(w,l),maxHeight,false);
			l = w.toXYZ((w.size.x)/2,v,0);
			create(new WorldLocator(w,l),minHeight,false);
			return w;
		}

		#endregion
	}
}
