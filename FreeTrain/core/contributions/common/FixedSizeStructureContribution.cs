using System;
using System.Collections;
using System.Drawing;
using System.Xml;
using freetrain.util;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.controllers;
using freetrain.world;
using freetrain.world.structs;
using freetrain.contributions.common;
using org.kohsuke.directdraw;

namespace freetrain.contributions.common
{
	/// <summary>
	/// StructureContribution for structures of a fixed size.
	/// </summary>
	[Serializable]
	public abstract class FixedSizeStructureContribution : StructureContribution, IPreviewWorldBuilder
	{
		/// <summary>
		/// Parses a structure contribution from a DOM node.
		/// </summary>
		/// <exception cref="XmlException">If the parsing fails</exception>
		protected FixedSizeStructureContribution( XmlElement e ) : base(e) 
		{
			_price = int.Parse( XmlUtil.selectSingleNode(e,"price").InnerText );
			
			SIZE sz = XmlUtil.parseSize( XmlUtil.selectSingleNode(e,"size").InnerText );
			int height = int.Parse( XmlUtil.selectSingleNode(e,"height").InnerText );

			this.size = new Distance(sz,height);
			_ppa = _price/Math.Max(1,size.x*size.y);
			XmlElement spr = (XmlElement)XmlUtil.selectSingleNode(e,"sprite");
			sprites = PluginUtil.getSpriteLoader(spr).load3D( spr, size.x, size.y, height );
		}

		public FixedSizeStructureContribution(AbstractExStructure master, XmlElement pic, XmlElement main, bool opposite ) : base (main) 
		{
			_price = master.unitPrice;			
			int height = master.maxHeight;
			if( opposite )
				size = new Distance(master.size.y, master.size.x, height);
			else
				this.size = new Distance( master.size, height);
			_ppa = _price/Math.Max(1,size.x*size.y);
			sprites = PluginUtil.getSpriteLoader(pic).load3D( pic, size.x, size.y, height );
		}
	
	/// <summary>Price of this structure.</summary>
		protected readonly int _price;			// TODO: should be moved up
		public override int price {	get { return _price; } }
		protected readonly double _ppa;
		public override double pricePerArea { get { return _ppa; } }


		/// <summary>
		/// Sprite set to draw this structure. Indexed as [x,y,z]
		/// and may contain null if there's no need to draw that voxel.
		/// </summary>
		public readonly Sprite[,,] sprites;

		/// <summary> Size of this structure in voxel by voxel. </summary>
		public readonly Distance size;

		/// <summary>
		/// Creates a new instance of this structure type to the specified location.
		/// </summary>
		/// <param name="owned">
		/// If the structure is a subsidiary, this flag controls whether the
		/// structure is initially owned or not. Otherwise this flag has no effect.
		/// </param>
		public Structure create( Location baseLoc, bool initiallyOwned ){
			return create(new WorldLocator(World.world, baseLoc), initiallyOwned);
		}

		public abstract Structure create(WorldLocator wloc, bool initiallyOwned );
		
		// this method differs from the create method in its return type.
		// delegates are so inflexible that we have to do this kind of adjustment.
		public void create2( Location baseLoc ) {
			create(new WorldLocator(World.world,baseLoc),false);
		}
		
		/// <summary>
		/// Returns true iff this structure can be built at the specified location.
		/// </summary>
		public abstract bool canBeBuilt( Location baseLoc, ControlMode cm );

		
		public Sprite getSprite( Distance d ) {
			return sprites[d.x,d.y,d.z];
		}

		public override PreviewDrawer createPreview( Size pixelSize ) {
			PreviewDrawer drawer = new PreviewDrawer( pixelSize, size );
			drawer.drawCenter( sprites );
			return drawer;
		}
		
		public override ModalController createBuilder( IControllerSite site ) {
			return new FixedSizeStructurePlacementController( this, site );
		}
		public override ModalController createRemover( IControllerSite site ) {
			return new FixedSizeStructureRemovalController( this, site );
		}
		#region IPreviewWorldBuilder o

		public World CreatePreviewWorld( Size minsizePixel, IDictionary options) {
			World w = World.CreatePreviewWorld(minsizePixel, size);
			Location l = w.toXYZ((w.size.x-size.x+size.y)/2,w.size.y-size.y-2,0);
			create(new WorldLocator(w,l),false);
			return w;
		}

		#endregion
	}
}
