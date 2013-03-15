using System;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.contributions.population
{
	/// <summary>
	/// HourlyPopulation with a typical distribution for
	/// agricultural lands
	/// </summary>
	[Serializable]
	public class AgriculturalPopulation : HourlyPopulation
	{
		public AgriculturalPopulation( int baseP ) : base(baseP,weekdayDistribution,weekdayDistribution) {}

		public AgriculturalPopulation( XmlElement e )
			: this( int.Parse( XmlUtil.selectSingleNode(e,"base").InnerText) ) {}

		// TODO: parameter calibration
		private static readonly int[] weekdayDistribution = new int[]{
			 10,  5,  5,  5,  5,  5,	//  0:00- 5:00
			 10, 60,100, 80, 60, 40,	//  6:00-11:00
			 40, 30, 30, 40, 40, 30,	// 12:00-17:00
			 30, 25, 25, 20, 20, 15,	// 18:00-23:00
		};

		// TODO: weekend distribution
	}
}
