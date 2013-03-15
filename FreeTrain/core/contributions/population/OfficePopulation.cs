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
	public class OfficePopulation : HourlyPopulation
	{
		public OfficePopulation( int baseP ) : base(baseP,weekdayDistribution,weekdayDistribution) {}

		public OfficePopulation( XmlElement e )
			: this( int.Parse( XmlUtil.selectSingleNode(e,"base").InnerText) ) {}

		private static readonly int[] weekdayDistribution = new int[]{
			 10,  5,  5,  5,  5,  5,	//  0:00- 5:00
			 10, 10, 20, 20, 20, 20,	//  6:00-11:00
			 20, 25, 25, 25, 30, 80,	// 12:00-17:00
			100, 80, 50, 30, 20, 10,	// 18:00-23:00
		};

		// TODO: weekend distribution
	}
}
