using System;
using System.Xml;
using freetrain.world;
using freetrain.world.structs;
using freetrain.framework.plugin;

namespace freetrain.contributions.population
{
	/// <summary>
	/// Always the same population
	/// </summary>
	[Serializable]
	public class ConstantPopulation : Population
	{
		public ConstantPopulation( int p ) {
			this.population = p;
		}
		public ConstantPopulation( XmlElement e ) {
			this.population = int.Parse( XmlUtil.selectSingleNode(e,"base").InnerText );
		}

		private readonly int population;

		public override int residents { get { return population; } }

		public override int calcPopulation( Time currentTime ) {
			return population;
		}
	}
}
