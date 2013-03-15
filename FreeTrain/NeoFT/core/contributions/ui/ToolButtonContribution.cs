using System;
using System.IO;
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
	public class ToolButtonContribution : Contribution
	{
		
		public readonly string barname;
		protected readonly string after = null;
		protected readonly string before = null;

		/// <summary>
		/// Contributes a menu and submenu items to mainmenu.
		/// 
		/// This contribution can add several items at once.
		/// </summary>
		public ToolButtonContribution( XmlElement e ) : base(e) 
		{
			
			foreach( XmlNode cn in e.ChildNodes )
			{
				if(!cn.Name.Equals("bar")) continue;

				barname = XmlUtil.getAttributeValue(cn,"name","MAIN");
				after = XmlUtil.getAttributeValue(cn,"after",null);
				before = XmlUtil.getAttributeValue(cn,"before",null);
			
				parseNode(cn);
			}
		}

		protected void parseNode( XmlNode node )
		{
			foreach( XmlNode cn in node.ChildNodes )
			{
				if(!cn.Name.Equals("button")) continue;
				ButtonCreationInfo info = parseButton(cn);
				Core.mainFrame.AddToolButton(info,barname,after,before);
			}
		
		}

		protected ButtonCreationInfo parseButton( XmlNode item )
		{
			string _id = XmlUtil.getAttributeValue(item,"bid","").Trim();
			string _img = XmlUtil.getAttributeValue(item,"image","").Trim();
			_img = Path.Combine(PluginUtil.GetPruginFullPath(item),_img);
			_img = Path.GetFullPath(_img);
			string _text = item.InnerText;
			int _idx = int.Parse(XmlUtil.getAttributeValue(item,"index","0"));
			return new ButtonCreationInfo( _id, _text, _img, _idx );
		}
	}
}
