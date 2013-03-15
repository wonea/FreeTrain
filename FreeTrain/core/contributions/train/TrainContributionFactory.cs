using System;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.contributions.train
{
	/// <summary>
	/// LoadsTrainContributionFactory.
	/// 
	/// When a class element is present, it is used. Otherwise
	/// defaults to ParamTrainImpl
	/// </summary>
	public class TrainContributionFactory : ContributionFactory
	{
		public TrainContributionFactory() {}
		public TrainContributionFactory( XmlElement e ) {}

		public Contribution load( Plugin owner, XmlElement e ) {
			if( e.SelectSingleNode("class")==null )
				// default to ParamTrainImpl
				return new ParamTrainImpl(e);
			else
				// use the class element
				return (Contribution)PluginUtil.loadObjectFromManifest(e);
		}
	}
}
