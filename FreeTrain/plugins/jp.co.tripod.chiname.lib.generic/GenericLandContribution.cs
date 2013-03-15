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
	/// <summary>
	/// GenericLandContribution の概要の説明です。
	/// </summary>
	public class GenericLandContribution : GenericStructureContribution
	{
		public GenericLandContribution(XmlElement e) : base(e)
		{
		}

		protected override void loadPrimitiveParams( XmlElement e )
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

			try 
			{
				_design = e.SelectSingleNode("design").InnerText;
			} 
			catch 
			{
                //! _design = "標準";
                _design = "default";
			}
			
			try	{
				_unitPrice = int.Parse( XmlUtil.selectSingleNode(e,"price").InnerText );
			} catch	{
				_unitPrice = 0;
			}
			
			_size = new SIZE(1,1);
			
			_minHeight = 2;
			_maxHeight = 0;
		}

		protected override Contribution createPrimitiveContrib(XmlElement sprite, XmlNode color, XmlElement contrib )
		{
			sprite.AppendChild(color.Clone());
			PluginManager manager = PluginManager.theInstance;
			ContributionFactory factory = manager.getContributionFactory("land");
			XmlNode temp = contrib.Clone();
			foreach(XmlNode cn in temp.ChildNodes)
			{
				if(cn.Name.Equals("sptite") || cn.Name.Equals("picture"))
					temp.RemoveChild(cn);
			}
			temp.AppendChild(sprite);
			contrib.AppendChild(temp);
			return factory.load(parent,(XmlElement)temp);
		}

	}
}
