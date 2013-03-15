using System;
using System.Runtime.Serialization;
using System.Xml;
using freetrain.framework.plugin;
using freetrain.world;
using freetrain.world.structs;

namespace freetrain.contributions.population
{
	/// <summary>
	/// Computes the population from the base population.
	/// 
	/// The Population class has a special code to support de-serizliation.
	/// We'd like the de-serialization of Population not to create a fresh
	/// instance of Population, but we'd like it to resolve to the existing
	/// instance.
	/// 
	/// However, Population object doesn't know how to resolve to its running
	/// instance. Thus it takes an IObjectReference as a parameter, which should
	/// know how to resolve to the actual instance.
	/// 
	/// During the serialization, this resolver is stored in place of the
	/// population object and then asked to restore the reference.
	/// </summary>
	[Serializable]
	public abstract class Population
	{
		/// <summary>
		/// Number of population that is counted toward the total population of the world.
		/// </summary>
		public abstract int residents { get; }

		/// <summary>
		/// Computes the population of the given structure at the given time.
		/// </summary>
		public abstract int calcPopulation( Time currentTime );

		/// <summary>
		/// Loads a population from the plug-in manifest file.
		/// </summary>
		public static Population load( XmlElement e ) {
			return (Population)PluginUtil.loadObjectFromManifest(e);
		}
	}
}
