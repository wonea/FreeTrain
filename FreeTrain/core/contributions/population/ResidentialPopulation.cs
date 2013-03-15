using System;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.contributions.population
{
	/// <summary>
	/// HourlyPopulation with a typical distribution for
	/// residential structures (such as houses, apartments, etc.)
	/// </summary>
	[Serializable]
	public class ResidentialPopulation : HourlyPopulation
	{
		public ResidentialPopulation( int baseP ) : base(baseP,weekdayDistribution,weekdayDistribution) {}

		public ResidentialPopulation( XmlElement e )
			: this( int.Parse( XmlUtil.selectSingleNode(e,"base").InnerText) ) {}

		private static readonly int[] weekdayDistribution = new int[]{
			 10,  5,  5,  5,  5,  5,	//  0:00- 5:00
			 10, 60,100, 80, 60, 40,	//  6:00-11:00
			 40, 30, 30, 40, 40, 30,	// 12:00-17:00
			 30, 25, 25, 20, 20, 15,	// 18:00-23:00
		};

		// TODO: weekend distribution
	}
}
