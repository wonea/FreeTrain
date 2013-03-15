using System;
using System.Xml;
using freetrain.world;

namespace freetrain.contributions.population
{
	/// <summary>
	/// Populaion that additively combines other populations.
	/// 
	/// Syntax in XML would be:
	/// &lt;population>
	///   &lt;class name="...CombinationPopulation"/>
	///   &lt;population>
	///     ...
	///   &lt;/population>
	///   &lt;population>
	///     ...
	///   &lt;/population>
	///   ...
	/// &lt;/population>
	/// </summary>
	public class CombinationPopulation : Population
	{
		private readonly Population[] children;

		public CombinationPopulation( XmlElement e ) {
			XmlNodeList nl = e.SelectNodes("population");
			children = new Population[nl.Count];
			for( int i=0; i<nl.Count; i++ )
				children[i] = Population.load( (XmlElement)nl[i] );
		}

		public override int residents {
			get {
				int r = 0;
				foreach( Population p in children )
					r += p.residents;
				return r;
			}
		}

		public override int calcPopulation( Time currentTime ) {
			int r = 0;
			foreach( Population p in children )
				r += p.calcPopulation(currentTime);
			return r;
		}
	}
}
