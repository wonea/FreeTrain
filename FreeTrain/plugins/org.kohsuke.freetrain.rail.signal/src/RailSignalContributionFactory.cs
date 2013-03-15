using System;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.world.rail.signal
{
	/// <summary>
	/// Loader of RailSignalContribution
	/// </summary>
	public class RailSignalContributionFactory : ContributionFactory
	{
		public RailSignalContributionFactory( XmlElement e ) {}

		public Contribution load( Plugin owner, XmlElement e ) {
			return new RailSignalContribution(e);
		}
	}
}
