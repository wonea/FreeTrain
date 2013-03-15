using System;
using System.Drawing;
using System.Diagnostics;
using System.Xml;
using System.Collections;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.controllers;
using freetrain.controllers.structs;
using freetrain.contributions;
using freetrain.contributions.common;
using freetrain.contributions.population;
using freetrain.contributions.structs;
using freetrain.views;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.structs;

namespace freetrain.framework.plugin.generic
{
	[Serializable]
	public enum SpriteTableType :int{ UNKNOWN, BASIC, VARHEIGHT };

	/// <summary>
	/// Contribution of rail signal.
	/// </summary>
	[Serializable]
	public class GenericStructureContribution : StructureContribution, AbstractExStructure
	{
		protected static readonly Char[] spliter = new Char[]{'|'};
		/// <summary> sub type of this structure. </summary>
		protected StructCategories _categories;
		public StructCategories categories{ get { return _categories; } }
		/// <summary> sub type of this structure. </summary>
		protected string _design;
		public string design{ get { return _design;} }
		/// <summary> unit price of this structure. equals to whole price for fixed height. </summary>
		protected int _unitPrice;
		public int unitPrice{ get { return _unitPrice;} }
		public override int price { get { return _unitPrice; } }
		protected int _areaPrice;
		public override double pricePerArea { get { return _areaPrice; } }

		/// <summary> size of this structur. </summary>
		protected SIZE _size;
		public SIZE size{ get { return _size;} }
		/// <summary> valid for variable height structure only. </summary>
		protected int _minHeight;
		public int minHeight{ get { return _minHeight;} }
		/// <summary> used as well as fixed height structure. </summary>
		protected int _maxHeight;
		public int maxHeight{ get { return _maxHeight;} }
		/// <summary> sprite table type of this structure. </summary>
		protected SpriteTableType stType = SpriteTableType.UNKNOWN;
		public SpriteTableType patternType { get { return stType;} }

		internal Contribution[,] contribs;
		protected int[] dirTable = new int[8];

		/// <summary> counts of color variations. </summary>
		protected int _colorMax;
		public int colorVariations{ get { return _colorMax;} }
		/// <summary> counts of direction variations. </summary>
		protected int _dirMax;
		public int directionVariations{ get { return _dirMax;} }

		protected int _colorIndex = 0;
		public int colorIndex{ 
			get { return _colorIndex; } 
			set { 
				_colorIndex = value;
				if( _colorIndex >= _colorMax )
					_colorIndex = _colorMax-1;
			} 
		}
		protected int _dirIndex = 0;
		public int dirIndex
		{ 
			get { return _dirIndex; } 
			set 
			{ 
				_dirIndex = value;
					if( _dirIndex >= _dirMax )
						_dirIndex = _dirMax-1;
			} 
		}
		public IEntityBuilder current 
		{
			get { return (IEntityBuilder)contribs[_dirIndex,_colorIndex]; }
		}
		
		private static StructureGroup _group=null;

		static GenericStructureContribution()
		{
			// prepare category tree
			if( StructCategoryTree.theInstance==null )
				StructCategoryTree.loadDefaultTree();
		}

		// the constructor
		public GenericStructureContribution( XmlElement e ) : base(e) {
			loadPrimitiveParams(e);
			for(int i=0;i<8;i++)
				dirTable[i]=-1;
			loadSprites(e);
		}

		public override string ToString() {
			return name;
		}
		
		protected override StructureGroup getGroup( string name ) 
		{
			if( _group == null )
				_group = new StructureGroup("GenericStructure");
			return _group;
		}
		
		protected virtual void loadPrimitiveParams( XmlElement e )
		{
			XmlNode xn = e.SelectSingleNode("structure");
			if( xn!=null )
				_categories = new StructCategories(xn,this.id);
			else 
				_categories = new StructCategories();

			if( _categories.Count==0 )
			{
				StructCategory.Root.Entries.Add(this.id);
				_categories.Add(StructCategory.Root);
			}

			try {
				_design = e.SelectSingleNode("design").InnerText;
			} catch {
                //! _design = "標準";
                _design = "default";
			}
			
			_unitPrice = int.Parse( XmlUtil.selectSingleNode(e,"price").InnerText );
			_size = XmlUtil.parseSize( XmlUtil.selectSingleNode(e,"size").InnerText );
			_areaPrice = _unitPrice/Math.Max(1,size.x*size.y);

			_minHeight = 2;
			try {
				_maxHeight = int.Parse( e.SelectSingleNode("maxHeight").InnerText );
				try {
					// if minHeight is not defined, use default.
					_minHeight = int.Parse( e.SelectSingleNode("minHeight").InnerText );
				} catch {}
			} catch	{
				// if maxHeight tag is nod find, height tag must be exist.
				_maxHeight = int.Parse( XmlUtil.selectSingleNode(e,"height").InnerText );
			}
		}

		protected virtual void loadSprites( XmlElement e )
		{		
			IEnumerator ie = e.ChildNodes.GetEnumerator();
			ArrayList colors = new ArrayList();
			ArrayList spriteNodes = new ArrayList();
			while(ie.MoveNext())
			{
				XmlNode child = (XmlNode)ie.Current;
				if( child.Name.Equals("spriteType") ) 
				{
					colors.Add( child) ;
				}
				else if( child.Name.Equals("colorVariation"))
				{
					colors.Add( child);
					//throw new NotSupportedException("prease use <spriteType> tag.");
				}				
				else if( child.Name.Equals("picture") || child.Name.Equals("sprite"))
				{
					switch( stType )
					{
						case SpriteTableType.UNKNOWN:
							stType = SpriteTableType.BASIC;
							break;
						case SpriteTableType.BASIC:
							break;
						default:
							throw new FormatException("<sprite> tag is not available together with <pictures> or <sprites> tags.");
					}
					spriteNodes.Add( child );
				}
				else if( child.Name.Equals("pictures") || child.Name.Equals("sprites") )
				{
					switch( stType )
					{
						case SpriteTableType.UNKNOWN:
							stType = SpriteTableType.VARHEIGHT;
							break;
						case SpriteTableType.VARHEIGHT:
							break;
						default:
							throw new FormatException("<"+ child.Name +"> tag is not available together with <sprite> tag.");
					}
					spriteNodes.Add( child );
				}
			}
			if( colors.Count == 0 ) 
			{
				colors.Add( e.FirstChild.Clone() ); 
			}
			_colorMax = colors.Count;
			_dirMax = spriteNodes.Count;
			contribs = new Contribution[_dirMax,_colorMax];
			int defaultDir=_dirMax;
			for( int i=0; i< _colorMax; i++ )
			{
				for( int j=0; j < _dirMax; j++ ) 
				{
					e.Attributes["id"].Value = this.id+"-"+i+":"+j;
					XmlNode temp = ((XmlNode)spriteNodes[j]).Clone();
					if( parseDirection(temp,j)==0 && defaultDir>j )
						defaultDir=j;
					Contribution newContrib 
						= createPrimitiveContrib( (XmlElement)temp, (XmlNode)colors[i], e );
					PluginManager.theInstance.addContribution(newContrib);
					contribs[j,i] = newContrib;
				}
			}

			// set unassigned direction table
			for( int j=0; j < dirTable.Length; j++ )
			{
				if( dirTable[j]==-1 )
					dirTable[j] = defaultDir;
			}
		}

		protected virtual Contribution createPrimitiveContrib(XmlElement sprite, XmlNode color, XmlElement contrib )
		{
			bool opposite = ( sprite.Attributes["opposite"] != null && sprite.Attributes["opposite"].Value.Equals("true"));
			Contribution newContrib;
			if( stType == SpriteTableType.VARHEIGHT ) 
			{
				foreach( XmlNode child in sprite.ChildNodes )
					child.AppendChild(color.Clone());						 
				newContrib = new VarHeightBuildingContribution(this, sprite, contrib, opposite);
			}
			else 
			{
				sprite.AppendChild(color.Clone());
				newContrib = new CommercialStructureContribution(this, sprite, contrib, opposite);
			}
			return newContrib;
		}
		
		/// <summary>
		/// parse direction information
		/// </summary>
		/// <param name="node">parent node contains 'direction' tags</param>
		/// <param name="refindex">first index of contribs array</param>
		protected int parseDirection( XmlNode node, int refindex )
		{
			int c=0;
			foreach(XmlNode cn in node.ChildNodes)
			{
				if(cn.Name.Equals("direction"))
				{
					string front = cn.Attributes["front"].Value;					
					string[] dirs = front.ToUpper().Split(spliter);
					for(int i=0; i<dirs.Length; i++)
					{
						c++;
						if(dirs[i].Equals("ALL"))
						{
							dirTable[Direction.NORTH.index]=refindex;
							dirTable[Direction.SOUTH.index]=refindex;
							dirTable[Direction.EAST.index]=refindex;
							dirTable[Direction.WEST.index]=refindex;
						}
						else if(dirs[i].Equals("SOUTH"))
							dirTable[Direction.SOUTH.index]=refindex;
						else if(dirs[i].Equals("EAST"))
							dirTable[Direction.EAST.index]=refindex;
						else if(dirs[i].Equals("WEST"))
							dirTable[Direction.WEST.index]=refindex;
						else if(dirs[i].Equals("NORTH"))
							dirTable[Direction.NORTH.index]=refindex;
					}
				}
			}
			return c;
		}

//		protected SpriteFactory createFactory( XmlNode node, string name )
//		{
//			SpriteFactoryContribution contrib = (SpriteFactoryContribution)
//				PluginManager.theInstance.getContribution( "spriteFactory:"+name );
//			if(contrib==null)
//				throw new FormatException("unable to locate spriteFactory:"+ name);
//			return contrib.createSpriteFactory((XmlElement)node); 
//		}

		public Contribution GetPrimitiveContrib(Direction dir,int color)
		{
			return contribs[dirTable[dir.index],color];
		}

		/// <summary>
		/// Creates a preview
		/// </summary>
		/// <param name="pixelSize"></param>
		/// <returns></returns>
		public override PreviewDrawer createPreview( Size pixelSize )
		{
			return current.createPreview( pixelSize );
		}

		public override ModalController createBuilder( IControllerSite site )
		{
			return current.createBuilder(site);
		}
		public override ModalController createRemover( IControllerSite site )
		{
			return current.createRemover(site);
		}

	}
}
