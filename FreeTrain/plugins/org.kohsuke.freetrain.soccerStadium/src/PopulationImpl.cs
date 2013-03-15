using System;
using System.Xml;
using freetrain.contributions.population;

namespace freetrain.world.soccerstadium
{
	/// <summary>
	/// Population implementation for soccer stadium.
	/// </summary>
	[Serializable]
	public class PopulationImpl : Population
	{
		public PopulationImpl( XmlElement e ) {}

		// nodbody is living in a stadium.
		public override int residents { get { return 1; } }

		/// <summary>
		/// Computes the population of the given structure at the given time.
		/// </summary>
		public override int calcPopulation( Time currentTime ) {
			int h = currentTime.hour;
			if( h==16 || h==21 )	return 10000;
			else					return 100;
		}
	}
}
