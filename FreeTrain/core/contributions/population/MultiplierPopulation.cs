using System;
using System.Runtime.Serialization;
using freetrain.world;
using freetrain.framework.plugin;

namespace freetrain.contributions.population
{
	/// <summary>
	/// Multiplies another population by a constant factor.
	/// This object is not-persistent.
	/// </summary>
	[Serializable]
	public class MultiplierPopulation : Population
	{
		private readonly int factor;
		private readonly Population core;

		public MultiplierPopulation( int f, Population _core ) {
			this.factor = f;
			this.core = _core;

		}

		public override int residents {
			get {
				return core.residents*factor;
			}
		}

		public override int calcPopulation( Time currentTime ) {
			return core.calcPopulation(currentTime)*factor;
		}
	}
}
