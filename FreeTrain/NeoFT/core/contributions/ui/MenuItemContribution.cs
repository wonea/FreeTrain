using System;
using System.Xml;
using nft.framework;
using nft.framework.plugin;
using nft.util;
using nft.ui.mainframe;

namespace nft.contributions.ui
{
	/// <summary>
	/// MenuItemContribution の概要の説明です。
	/// </summary>
	public class MenuItemContribution : Contribution
	{
		
		public readonly string location;
		protected readonly string after = null;
		protected readonly string before = null;

		/// <summary>
		/// Contributes a menu and submenu items to mainmenu.
		/// 
		/// This contribution can add several items at once.
		/// </summary>
		public MenuItemContribution( XmlElement e ) : base(e) 
		{
			location = XmlUtil.getSingleNodeText(e,"parent",null);
			XmlNode order = e.SelectSingleNode("order");
			if(order!=null)
			{
				after = XmlUtil.getAttributeValue(order,"after",null);
				before = XmlUtil.getAttributeValue(order,"before",null);
			}
			XmlNode entries = XmlUtil.selectSingleNode(e,"entries");
			if( location.Equals("ROOT") )
				parseNode(entries,"");
			else
				parseNode(entries, location);
		}

		protected void parseNode( XmlNode node, string path )
		{
			foreach( XmlNode cn in node.ChildNodes )
			{
				if(cn.Name.Equals("item"))
				{
					if(cn.ChildNodes.Count!=0)
					{
						string _id = XmlUtil.getAttributeValue(cn,"mid","").Trim();
						string _cap = XmlUtil.getAttributeValue(cn,"caption","").Trim();
						string p2 = Core.mainFrame.RegisterMenuNode(_id,path,_cap,after,before);
						parseNode(cn,p2);
					}
					else
					{
						MenuCreationInfo info = parseItem(cn);
						Core.mainFrame.AddMenuItem(info,path,after,before);
					}						
				}
			}
		
		}

		protected MenuCreationInfo parseItem( XmlNode item )
		{
			string _id = XmlUtil.getAttributeValue(item,"mid","").Trim();
			string _cap = XmlUtil.getAttributeValue(item,"caption","").Trim();
			string _text = XmlUtil.getSingleNodeText(item,"note","");
			return new MenuCreationInfo( _id, _cap, _text );
		}
	}
}
