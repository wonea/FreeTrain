using System;
using System.Xml;
using System.Drawing;
using System.Collections;
using freetrain.world;
using freetrain.framework.plugin;

namespace freetrain.world.structs.hv
{	
	/// <summary>
	/// ColorLibrary の概要の説明です。
	/// </summary>
	public class ColorLibrary :Contribution
	{
		protected static readonly string null_id="{COLORLIB-NULL}";
		protected ArrayList list;
		public ColorLibrary(XmlElement e):base(e)
		{	
			list = new ArrayList();
			XmlNode nd = e.FirstChild;
			while(nd!=null)
			{
				Color c;
				if(nd.Name.Equals("element")) 
				{
					c = PluginUtil.parseColor( nd.Attributes["color"].Value );
					list.Add(c);
				}
				nd = nd.NextSibling;
			}

			// special code for NullLibrary
			if(id.Equals(null_id))
				list.Add(Color.Transparent);
		}
		
		static public ColorLibrary NullLibrary
		{
			get{ return (ColorLibrary)PluginManager.theInstance.getContribution(null_id); }
		}

		public int size
		{
			get{ return list.Count; }
		}

		public Color this[int index]
		{
			get{ return (Color)list[index];}
		}
	}
}
