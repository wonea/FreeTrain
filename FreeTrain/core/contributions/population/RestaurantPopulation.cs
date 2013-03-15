using System;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.contributions.population
{
	/// <summary>
	/// HourlyPopulation with a typical distribution for
	/// restaurants.
	/// </summary>
	[Serializable]
	public class RestaurantPopulation : HourlyPopulation
	{
		public RestaurantPopulation( int baseP ) : base(baseP,weekdayDistribution,weekdayDistribution) {}

		public RestaurantPopulation( XmlElement e )
			: this( int.Parse( XmlUtil.selectSingleNode(e,"base").InnerText) ) {}

		private static readonly int[] weekdayDistribution = new int[]{
			  0,  0,  0,  0,  0,  0,	//  0:00- 5:00
			  0,  0,  0,  5, 20, 75,	//  6:00-11:00
			100, 75, 30, 10,  0, 10,	// 12:00-17:00
			 50, 80, 80, 40, 10,  0,	// 18:00-23:00
		};

		// TODO: weekend distribution
	}
}
