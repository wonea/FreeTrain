using System;
using System.Xml;
using System.Drawing;
using freetrain.contributions.common;
using freetrain.framework.plugin;
using freetrain.framework.graphics;
using freetrain.controllers;
using freetrain.contributions.population;

namespace freetrain.contributions.rail
{
	/// <summary>
	/// Contribution that adds <c>TrafficVoxel.Accessory</c>
	/// </summary>
	[Serializable]
	public abstract class RailAccessoryContribution : Contribution, IEntityBuilder
	{
		private readonly string _name;

		public RailAccessoryContribution( XmlElement e ) : base(e) {
			_name = XmlUtil.selectSingleNode(e,"name").InnerText;
		}


		// TODO: do we need a method like
		// void create( Location loc ) ?

		#region IEntityBuilder o
		public virtual string name { get { return _name; } }

		public virtual Population population { get { return null; } }

		public virtual int price {	get {return 0;}	}
		public virtual double pricePerArea {	get {return 0;}	}

		public bool computerCannotBuild { get{ return false; } }

		public bool playerCannotBuild {	get{ return true; }	}

		public abstract freetrain.framework.graphics.PreviewDrawer createPreview(System.Drawing.Size pixelSize);

		public abstract freetrain.controllers.ModalController createBuilder(freetrain.controllers.IControllerSite site);

		public abstract freetrain.controllers.ModalController createRemover(freetrain.controllers.IControllerSite site);

		#endregion
	}
}
