using System;
using System.Net;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.contributions.sound
{
	/// <summary>
	/// Code that lists up BGMContribution programatically.
	/// </summary>
	[Serializable]
	public abstract class BGMFactoryContribution : Contribution
	{
		protected BGMFactoryContribution( XmlElement e ) : base(e) {}

		/// <summary>
		/// Lists up the BGM contributions, which will be incorporated
		/// into the set of BGMs.
		/// </summary>
		/// <returns></returns>
		public abstract BGMContribution[] listContributions();

		/// <summary>
		/// Gets the group name of these BGMContributions.
		/// </summary>
		public abstract string title { get; }
	}

	internal class BGMFactoryContributionFactory : ContributionFactory {
		public BGMFactoryContributionFactory( XmlElement e ) {}
		public BGMFactoryContributionFactory() {}

		public Contribution load( Plugin owner, XmlElement e ) {
			BGMFactoryContribution contrib = 
				(BGMFactoryContribution)PluginUtil.loadObjectFromManifest(e);

			// let the factory load BGMs
			foreach( BGMContribution bgm in contrib.listContributions() ) {
				owner.contributions.Add(bgm);
				bgm.init(owner,new Uri(owner.dirName));
			}
			
			return contrib;
		}
	}
}
