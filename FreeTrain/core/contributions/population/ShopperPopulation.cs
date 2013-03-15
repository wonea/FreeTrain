using System;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.contributions.population
{
	/// <summary>
	/// HourlyPopulation with a typical distribution for
	/// shoppers to shops.
	/// </summary>
	[Serializable]
	public class ShopperPopulation : HourlyPopulation
	{
		public ShopperPopulation( int baseP ) : base(baseP,weekdayDistribution,weekdayDistribution) {}

		public ShopperPopulation( XmlElement e )
			: this( int.Parse( XmlUtil.selectSingleNode(e,"base").InnerText) ) {}

		private static readonly int[] weekdayDistribution = new int[]{
			  0,  0,  0,  0,  0,  0,	//  0:00- 5:00
			  0,  0,  0,  5, 20, 75,	//  6:00-11:00
			 45, 25, 15, 40, 70,100,	// 12:00-17:00
			 45, 20, 10,  5,  0,  0,	// 18:00-23:00
		};

		// TODO: weekend distribution
	}
}
