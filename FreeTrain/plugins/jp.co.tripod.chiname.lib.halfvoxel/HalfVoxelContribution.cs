using System;
using System.Drawing;
using System.Xml;
using freetrain.contributions.common;
using freetrain.contributions.population;
using freetrain.framework;
using freetrain.framework.plugin;
using freetrain.framework.plugin.graphics;
using freetrain.framework.graphics;
using freetrain.world;
using freetrain.world.structs;
using freetrain.controllers;
using freetrain.contributions.structs;

namespace freetrain.world.structs.hv
{
	public enum PlaceSide : int {Fore,Back};
	public enum SideStored : int {None,Fore,Back,Both};
	/// <summary>
	/// Class1 の概要の説明です。
	/// </summary>
	[Serializable]
	public class HalfVoxelContribution : StructureContribution
	{
		static protected readonly int hl_patterns = 6;
		static protected StructureGroup _group = new StructureGroup("HalfVoxel");
		static protected readonly Point[] offsets = new Point[]
		{
			new Point(0,-8), new Point(-8,-8),new Point(0,-8),new Point(-8,-8),
			new Point(-8,-4), new Point(0,-4),new Point(-8,-4),new Point(0,-4)
		};

			/// <summary>
			/// Parses a commercial structure contribution from a DOM node.
			/// </summary>
			/// <exception cref="XmlException">If the parsing fails</exception>
		public HalfVoxelContribution( XmlElement e ):base(e)
		{			
			_price = int.Parse( XmlUtil.selectSingleNode(e,"price").InnerText );			
			height = int.Parse( XmlUtil.selectSingleNode(e,"height").InnerText );
			subgroup = XmlUtil.selectSingleNode(e,"subgroup").InnerText;
			XmlElement spr = (XmlElement)XmlUtil.selectSingleNode(e,"sprite");
			XmlElement pic =  (XmlElement)XmlUtil.selectSingleNode(spr,"picture");
			variation = spr.SelectSingleNode("map");
			if( variation!=null )
			{
				String idc = variation.Attributes["to"].Value;
				colors = PluginManager.theInstance.getContribution(idc) as ColorLibrary;
				sprites = new SpriteSet[colors.size];
				for( int i=0; i<colors.size; i++ )
					sprites[i] = new SpriteSet(8);
			}
			else
			{
				colors = ColorLibrary.NullLibrary;
				sprites = new SpriteSet[1];
				sprites[0] = new SpriteSet(8);
			}
			loadSprites(spr,pic);
			XmlElement hle =(XmlElement)spr.SelectSingleNode("highlight");
			if(hle!=null)
			{
				hilights = new SpriteSet[hl_patterns];
				loadHighSprites(spr,hle);
			}
			else
				hilights = null;
		}

		#region helper methods used on reading XML
		protected virtual void loadSprites(XmlElement e,XmlElement ep)
		{
			Picture pic = getPicture(ep,null);
			XmlNode cn = e.FirstChild;
			while(cn!=null)
			{
				if(cn.Name.Equals("pattern"))
				{
					SideStored ss = parseSide(cn);
					Direction d = parseDirection(cn);
					Point orgn = XmlUtil.parsePoint(cn.Attributes["origin"].Value);
					Point offF = getOffset(d,PlaceSide.Fore);
					Point offB = getOffset(d,PlaceSide.Back);
					Size sz = new Size(24,8+height*16);
					if( variation!=null )
					{
						for(int i=0; i<colors.size; i++ )
						{
							Color c = colors[i];
							string v = c.R.ToString()+","+c.G.ToString()+","+c.B.ToString();
							variation.Attributes["to"].Value = v;
							SpriteFactory factory =  new HueTransformSpriteFactory(e);
							if( (ss&SideStored.Fore) != 0 )
								sprites[i][d,PlaceSide.Fore] = factory.createSprite(pic,offF,orgn,sz);
							if( (ss&SideStored.Back) != 0 )
								sprites[i][d,PlaceSide.Back] = factory.createSprite(pic,offB,orgn,sz);							
						}
					}
					else
					{
						SpriteFactory factory =  new SimpleSpriteFactory();
						if( (ss&SideStored.Fore) != 0 )
							sprites[0][d,PlaceSide.Fore] = factory.createSprite(pic,offF,orgn,sz);
						if( (ss&SideStored.Back) != 0 )
							sprites[0][d,PlaceSide.Back] = factory.createSprite(pic,offB,orgn,sz);						
					}
				}
				cn = cn.NextSibling;
			}
		}

		protected virtual void loadHighSprites(XmlElement e,XmlElement hle)
		{
			Picture pic = getPicture(hle,"HL");
			if( pic==null || hle.Attributes["src"]==null) 
				throw new FormatException("highlight picture not found.");
			string baseFileName = XmlUtil.resolve(hle, hle.Attributes["src"].Value).LocalPath;
			using(Bitmap bit = new Bitmap(baseFileName) )  {
				for( int i=0; i<hl_patterns; i++ )
					hilights[i] =  new SpriteSet(8);

				XmlNode cn = e.FirstChild;
				while(cn!=null)
				{
					if(cn.Name.Equals("pattern"))
					{
						SideStored ss = parseSide(cn);
						Direction d = parseDirection(cn);
						Point orgn = XmlUtil.parsePoint(cn.Attributes["origin"].Value);
						Point offF = getOffset(d,PlaceSide.Fore);
						Point offB = getOffset(d,PlaceSide.Back);
						Size sz = new Size(24,8+height*16);

						// create highlight patterns
						XmlNode hlp = cn.SelectSingleNode("highlight");
						if( hlp != null)
						{
							HueShiftSpriteFactory factory =  new HueShiftSpriteFactory(hl_patterns);
							if( (ss&SideStored.Fore) != 0 ) 
							{
								Sprite[] arr = factory.createSprites(bit,pic,offF,orgn,sz);
								for( int i=0; i<hl_patterns; i++ )
									hilights[i][d,PlaceSide.Fore] =arr[i];
							}
							if( (ss&SideStored.Back) != 0 )
							{
								Sprite[] arr = factory.createSprites(bit,pic,offB,orgn,sz);
								for( int i=0; i<hl_patterns; i++ )
									hilights[i][d,PlaceSide.Back] =arr[i];
							}
						}
					}//if(cn.Name.Equals("pattern"))
					cn = cn.NextSibling;
				}//while
			}//using
		}

		protected Point getOffset(Direction d, PlaceSide s)
		{
			Point o = offsets[d.index/2+(int)s*4];
			return new Point(o.X,o.Y+height*16);
		}

		protected SideStored parseSide(XmlNode n)
		{
			String s = n.Attributes["side"].Value;
			if( s==null || s.Equals("either"))
				return SideStored.Both;
			if( s.Equals("fore") )
				return SideStored.Fore;
			if( s.Equals("back") )
				return SideStored.Back;
			return SideStored.None;
		}

		protected Direction parseDirection(XmlNode n)
		{
			String s = n.Attributes["direction"].Value;
			if( s==null )
				throw new FormatException("missing direction attribute.");
			if( s.Equals("north") )
				return Direction.NORTH;
			if( s.Equals("south") )
				return Direction.SOUTH;
			if( s.Equals("west") )
				return Direction.WEST;
			if( s.Equals("east") )
				return Direction.EAST;
			throw new FormatException("invalid direction attribute.");
			//return null;
		}

		internal static Picture getPicture( XmlElement pic, string suffix ) 
		{
			//XmlElement pic = (XmlElement)XmlUtil.selectSingleNode(sprite,suffix);			
			XmlAttribute r = pic.Attributes["ref"];
			if(r!=null)
				// reference to externally defined pictures.
				return PictureManager.get(r.Value);

			// otherwise look for local picture definition
			XmlAttribute s = pic.Attributes["src"];
			if(s==null)
				return null;
			if( suffix != null )
				return new Picture(pic,
					pic.SelectSingleNode("ancestor-or-self::contribution/@id").InnerText +"#"+suffix);
			else
				return new Picture(pic,
					pic.SelectSingleNode("ancestor-or-self::contribution/@id").InnerText);
		}
		#endregion

		public int getHighlihtPatternCount()
		{
			if( hilights==null ) return 1;
			else return hl_patterns;
		}

		public Sprite getSprite(Direction d, PlaceSide s, int col )
		{
			return sprites[col][d,s];
		}

		public Sprite getHighLightSprite(Direction d, PlaceSide s, int col )
		{
			if(hilights!=null)
				return hilights[col][d,s];
			else
				return null;
		}


		internal SpriteSet[] sprites;
		internal SpriteSet[] hilights;
		protected readonly int _price;
		public override int price {	get { return _price; } }
		public override double pricePerArea { get { return _price<<1; } }

		public readonly int height;

		[NonSerialized]
		public readonly string subgroup;
		[NonSerialized]
		public readonly ColorLibrary colors;
		[NonSerialized]
		protected readonly XmlNode variation;
		[NonSerialized]
		protected int _currentCol;
		public int currentColor
		{
			get{ return _currentCol; }
			set
			{
				if(value>=0 && value<colors.size)
					_currentCol = value; 
			}
		}
		[NonSerialized]
		protected int _currentHLIdx;
		public int currentHighlight
		{
			get{ return _currentHLIdx; }
			set
			{
				if(value>=0 && value<hl_patterns)
					_currentHLIdx = value; 
			}
		}


		protected override StructureGroup getGroup( string name ) 
		{			
			return _group;
		}

		public override ModalController createBuilder( IControllerSite site ) 
		{
			return new HVControllerImpl(this,site,false);
		}

		public override ModalController createRemover( IControllerSite site ) 
		{
			return new HVControllerImpl(this,site,true);
		}

		public Structure create( Location baseLoc, Direction front, PlaceSide side ) 
		{
			ContributionReference reffer = new ContributionReference(this,currentColor,currentHighlight,front,side);
			HalfDividedVoxel v = World.world[baseLoc] as HalfDividedVoxel;
			if(v==null)
				return new HVStructure( reffer, baseLoc );
			else
			{
				if(!v.owner.add(reffer))
					MainWindow.showError("Not enough space or no fit");
					//! MainWindow.showError("設置スペースが無いか、一致しません");
				return v.owner;
			}
		}

		public void destroy( Location baseLoc, Direction front, PlaceSide side ) 
		{
			HalfDividedVoxel v = World.world[baseLoc] as HalfDividedVoxel;
			if(v!=null)
				v.owner.remove(side);
		}

		public static bool canBeBuilt( Location baseLoc ) 
		{
			Voxel v = World.world[baseLoc];
			if (v!=null)
			{
				HalfDividedVoxel hv = v as HalfDividedVoxel;
				if(hv!=null)
				{
					return hv.hasSpace;
				}
				else
					return false;
			}
			else
				return true;
		}

		public override PreviewDrawer createPreview( Size pixelSize )
		{
			PreviewDrawer drawer = new PreviewDrawer( pixelSize, new Size(7,1), 1 );
			drawer.draw( sprites[currentColor][Direction.WEST,PlaceSide.Fore], 3,1 );
			drawer.draw( sprites[currentColor][Direction.EAST,PlaceSide.Back], 2,0 );
			if(hilights!=null)
			{
				drawer.draw( hilights[currentHighlight][Direction.WEST,PlaceSide.Fore], 3,1 );
				drawer.draw( hilights[currentHighlight][Direction.EAST,PlaceSide.Back], 2,0 );
			}
			return drawer;
		}

	}
	#region ContributionReference
	[Serializable]
	public class ContributionReference
	{
		private readonly HalfVoxelContribution contrib;
		private readonly int patternIdx;
		private readonly int colorIdx;
		private readonly int hilightIdx;
		public readonly PlaceSide placeSide;
		public readonly Direction frontface;

		public ContributionReference(HalfVoxelContribution hvc, int color, int hilight, Direction front, PlaceSide side) 
		{
			this.contrib = hvc;
			this.colorIdx = color;
			this.hilightIdx = hilight;
			this.placeSide = side;
			this.frontface = front;
			this.patternIdx = SpriteSet.getIndexOf(front,side);
		}

		public virtual Sprite getSprite()
		{
			return contrib.sprites[colorIdx][patternIdx];
		}

		public virtual Sprite getHighlightSprite()
		{
			if( contrib.hilights!=null )
				return contrib.hilights[hilightIdx][patternIdx];
			else
				return null;
		}

		public virtual int height
		{
			get{ return contrib.height; }
		}
		public virtual int price
		{
			get{ return contrib.price; }
		}
		public virtual Population population
		{
			get{ return contrib.population; }
		}
		public virtual string name
		{
			get{ return contrib.subgroup; }
		}
	}

	internal class EmptyReference: ContributionReference
	{
		public EmptyReference(HalfVoxelContribution hvc, int color, int hilight, Direction front, PlaceSide side)
			: base(null,-1,-1,front,side){}
		public override Sprite getSprite(){ return null; }

		public override int height{get{ return 0; }}
		public override int price{get{ return 0; }}
		public override Population population{get{ return null; }}
	}
	#endregion
	#region SpriteSet
	[Serializable]
	public class SpriteSet
	{
		public SpriteSet(int size)
		{
			sprites = new Sprite[size];
		}

		public SpriteSet(SpriteSet org, Color variation)
		{
		}

		static internal int getIndexOf(Direction d, PlaceSide s)
		{
			return d.index/2+(int)s*4;
		}

		internal Sprite this[int idx]
		{
			get{return sprites[idx];}
		}

		public Sprite this[Direction d, PlaceSide s]
		{
			get
			{
				return sprites[getIndexOf(d,s)];
			}
			set
			{
				sprites[getIndexOf(d,s)] = value;
			}
		}

		private Sprite[] sprites;
	}
	#endregion
}
