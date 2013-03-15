using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml;
using freetrain.util;
using freetrain.contributions.population;
using freetrain.controllers;
using freetrain.framework.plugin;
using freetrain.framework.graphics;
using freetrain.world;
using freetrain.world.structs;
using org.kohsuke.directdraw;

namespace freetrain.contributions.common {
	public interface AbstractExStructure {
		int unitPrice { get; }
		SIZE size { get; }
		int minHeight { get; }
		int maxHeight { get; }
	}

	/// <summary>
	/// Generic structure contribution.
	/// 
	/// Structure is an object that occupies a cubic area in the World,
	/// has sprites to draw it.
	/// </summary>
	[Serializable]
	public abstract class StructureContribution : Contribution, IEntityBuilder {

		/// <summary>
		/// Parses a structure contribution from a DOM node.
		/// </summary>
		/// <exception cref="XmlException">If the parsing fails</exception>
		protected StructureContribution( XmlElement e ) : base(e) {
			core = new EntityBuilderInternal(e, this.id);
			XmlNode groupNode = e.SelectSingleNode("group");
			string groupName = (groupNode!=null)? groupNode.InnerText : core.name;
			group = getGroup(groupName);
			group.add(this);

		}

		protected StructureContribution( StructureContribution original, string type, string id ) : base(type, id) {
			core = original;
			group = original.group;
		}

		internal protected IEntityBuilder core;

		public readonly StructureGroup group;

		/// <summary>
		/// Implemented by the derived class and
		/// used to determine which group this structure should go.
		/// </summary>
		protected abstract StructureGroup getGroup( string name );

		/// <summary>
		/// Name of this entity builder. Primarily used as the display name.
		/// Doesn't need to be unique.
		/// </summary>
		public virtual string name { get { return core.name; } }
		
		public virtual Population population { get { return core.population; } }

		/// <summary>
		/// True if the computer (the development algorithm) is not allowed to
		/// build this structure.
		/// </summary>
		// TODO: make EntityBuilderContribution responsible for creating a new Plan object.
		public bool computerCannotBuild { get { return core.computerCannotBuild; } }

		/// <summary>
		/// True if the player is not allowed to build this structure.
		/// </summary>
		public bool playerCannotBuild { get { return core.playerCannotBuild;} }

		public virtual int price { get { return core.price; } }

		/// <summary>
		/// price par area (minimum).
		/// </summary>
		public virtual double pricePerArea { get { return core.pricePerArea; } }

		/// <summary>
		/// Creates a preview
		/// </summary>
		/// <param name="pixelSize"></param>
		/// <returns></returns>
		public virtual PreviewDrawer createPreview( Size pixelSize ) { return core.createPreview(pixelSize); }

		public virtual ModalController createBuilder( IControllerSite site ) { return core.createBuilder(site); }
		public virtual ModalController createRemover( IControllerSite site ) { return core.createRemover(site); }

		public override string ToString() { return core.name; }

		[Serializable]
			internal protected class EntityBuilderInternal : IEntityBuilder {
			public EntityBuilderInternal(XmlElement e, string ownerId) {
				XmlNode nameNode = e.SelectSingleNode("name");
				XmlNode groupNode = e.SelectSingleNode("group");

				_name = (nameNode!=null)? nameNode.InnerText : (groupNode!=null ? groupNode.InnerText : null );
				if(name==null)
					throw new FormatException("<name> and <group> are both missing");
				_price = int.Parse( XmlUtil.selectSingleNode(e,"price").InnerText );
				_computerCannotBuild = (e.SelectSingleNode("computerCannotBuild")!=null);
				_playerCannotBuild   = (e.SelectSingleNode("playerCannotBuild")!=null);

				XmlElement pop = (XmlElement)e.SelectSingleNode("population");
				if(pop!=null)
					_population = new PersistentPopulation(	Population.load(pop),
						new PopulationReferenceImpl(ownerId));
			}
			
			
			private readonly Population _population;
			private readonly bool _computerCannotBuild;
			private readonly bool _playerCannotBuild;
			private readonly string _name;
			protected readonly int _price;

			#region IEntityBuilder o
			public Population population { get{ return _population; }}

			public bool computerCannotBuild { get{ return _computerCannotBuild; }}

			public bool playerCannotBuild { get{ return _playerCannotBuild; }}

			public string name { get{ return _name; }}

			public int price { get{ return _price; }}

			public virtual double pricePerArea { get{ return _price; }}

			public virtual freetrain.framework.graphics.PreviewDrawer createPreview(System.Drawing.Size pixelSize) {
				throw new NotImplementedException();		
			}

			public virtual freetrain.controllers.ModalController createBuilder(freetrain.controllers.IControllerSite site) {
				throw new NotImplementedException();		
			}

			public virtual freetrain.controllers.ModalController createRemover(freetrain.controllers.IControllerSite site) {
				throw new NotImplementedException();		
			}

			#endregion
		}
	
		/// <summary>
		/// Used to resolve references to the population object.
		/// </summary>
		[Serializable]
			internal sealed class PopulationReferenceImpl : IObjectReference {
			internal PopulationReferenceImpl( string id ) { this.id=id; }
			private string id;
			public object GetRealObject(StreamingContext context) {
				return ((StructureContribution)PluginManager.theInstance.getContribution(id)).population;
			}
		}
	}
}
