using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;

namespace freetrain.framework.plugin.generic
{
	/// <summary>
	/// StructCategory の概要の説明です。
	/// </summary>
	[Serializable]
	public class StructCategory
	{
		public readonly int idnum;
		public readonly int iconIdx;
		public readonly String name;
		static protected ImageList images = new ImageList();
		static protected Hashtable files = new Hashtable();
		static public ImageList icons { get{ return images; } }

		public StructCategories Subcategories = new StructCategories();
		internal ArrayList Entries = new ArrayList();

		internal StructCategory( int idnum, String name, int icon )
		{
			this.idnum = idnum;
			this.name = name;
			this.iconIdx = icon;
		}

		internal static int ImageBaseInt(String filename)
		{			
			if( !files.ContainsKey(filename) )
			{
				int i = images.Images.Count;
				if( i==0 )
				{
					images.ImageSize = new Size(16,15);
					images.TransparentColor = Color.White;
				}
				images.Images.AddStrip(new System.Drawing.Bitmap(filename));
				files.Add(filename,i);
				return i;
			}
			else
				return (int)files[filename];
		}
		
		public StructCategory(XmlNode xn)
		{
			name = XmlUtil.selectSingleNode(xn,"name").InnerText;
			XmlAttribute att = xn.Attributes["id"];
			if(att!=null)
				idnum = int.Parse(att.Value,NumberStyles.HexNumber);
			
			XmlNode ni = xn.SelectSingleNode("icon");
			if(ni==null)
				iconIdx = Parent.iconIdx;
			else
			{
				String fn = ni.Attributes["src"].Value;				
				String path;
				if(xn.BaseURI.Length>0)
					path = new Uri(new Uri(xn.BaseURI),fn).LocalPath;
				else
					path = Path.Combine( StructCategoryTree.BaseDir, fn);
				int idx = int.Parse(ni.Attributes["index"].Value);
				iconIdx = idx+ImageBaseInt(path);
			}
			if(HasParent())
				Parent.Subcategories.Add(this);
		}

		public bool HasParent()
		{ return (idnum&0xffffff00)!=0;}
		public StructCategory Parent
		{
			get {
				StructCategory cat = StructCategoryTree.theInstance.getParent(this);
				if(cat==null)
					throw new FormatException("no parent for "+name+" : maybe ID is wrong."+idnum);
				return cat;
			}
		}
		static public StructCategory Root{ get{ return StructCategoryTree.theInstance[0]; }}
	}

	[Serializable]
	public class StructCategories 
	{
		protected ArrayList array = new ArrayList();

		internal StructCategories(){}

		public StructCategories(StructCategory cat){
			this.Add(cat);
		}

		// give <structure> tag, which contains <category> as children.
		public StructCategories(XmlNode node, string idContrib)
		{
			foreach( XmlNode cn in node )
			{
				if( cn.Name.Equals("category"))
				{
					XmlAttribute a = cn.Attributes["byid"];
					StructCategory cat = null;
					if( a!=null )
						cat = StructCategoryTree.theInstance[int.Parse(a.Value,NumberStyles.HexNumber)];
					else
					{
						a = cn.Attributes["byname"];
						if( a!=null )
							cat = StructCategoryTree.theInstance[a.Value];
					}
					if( cat==null ) cat = StructCategory.Root;
					if( !array.Contains(cat.idnum) )
						array.Add(cat.idnum);
					a = cn.Attributes["hide"];
					if( a==null || !a.Value.Equals("true") )
						cat.Entries.Add(idContrib);
				}
			}
		}

		public int Count { get{ return array.Count; }}
		public int Add( StructCategory cat )
		{
			return array.Add(cat.idnum);
		}
		public StructCategory this[int i]
		{
			get{ return StructCategoryTree.theInstance[(int)array[i]]; }
		}
		public bool Contains(StructCategory cat)
		{
			return array.Contains(cat.idnum);
		}
	}

	public class StructCategoryTree //: Contribution
	{
		static protected StructCategoryTree _theInstance;
		static public StructCategoryTree theInstance {get{ return _theInstance; } }
		static protected string _baseDir;
		static public string BaseDir{get{ return _baseDir; } }
        //! static private StructCategory hidden = new StructCategory(-1, "--無効--", 0);
        static private StructCategory hidden = new StructCategory(-1, "--N/A--", 0);

		protected Hashtable idTbl = new Hashtable();		
		protected Hashtable nameTbl = new Hashtable();

		static public void loadDefaultTree()
		{
			Plugin p = PluginManager.theInstance.getPlugin("jp.co.tripod.chiname.lib.generic");
			_baseDir = p.dirName;
			string filename = Path.Combine(p.dirName,"CategoryTree.xml");
			using( Stream file = p.loadStream(filename) ) 
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(file);
				new StructCategoryTree(doc);
			}
		}

		public StructCategoryTree(XmlDocument e)//:base(e)
		{
			_theInstance = this;
			XmlNode root = e.SelectSingleNode("tree");
			makeRoot(e);
			if(root!=null)
				parseTree(root,0);
		}

		private void makeRoot(XmlDocument e)
		{			
			String fn = Path.Combine(BaseDir,"icons.bmp");
			StructCategory.ImageBaseInt(fn);
            StructCategory cat = new StructCategory(0, "Uncategorized", 0);
            //! StructCategory cat = new StructCategory(0, "未分類", 0);
            idTbl.Add(cat.idnum, cat);
			nameTbl.Add(cat.name,cat);
		}

		protected void parseTree(XmlNode nd, int level)
		{
			XmlNodeList xnl = nd.ChildNodes;
			foreach( XmlNode cn in xnl )
			{
				if( cn.Name.Equals("category") )
				{
					StructCategory cat = new StructCategory(cn);
					idTbl.Add(cat.idnum,cat);
					nameTbl.Add(cat.name,cat);
					parseTree( cn,level+1);
				}
			}	
		}

		public StructCategory this[int idnum]
		{
			get{
				if( idnum == -1 )
					return hidden;
				else
					return (StructCategory) idTbl[idnum];
			}
		}
		public StructCategory this[String name]
		{
			get{return (StructCategory) nameTbl[name];}
		}
		public StructCategory getParent( StructCategory cat )
		{
			int pid = cat.idnum>>8;
			return this[pid];
		}
		public ICollection Categories
		{
			get{return idTbl.Values;}
		}
	}

}
