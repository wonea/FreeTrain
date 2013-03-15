using System;
using System.Xml;

namespace freetrain.framework.plugin
{
	/// <summary>
	/// Loads a Contribution from a designated class by passing the XmlElement
	/// to its constructor
	/// </summary>
	public class FixedClassContributionFactory : ContributionFactory
	{
		/// <param name="concreteType">
		/// Type of the class to be used to load the class.
		/// </param>
		public FixedClassContributionFactory( Type concreteType ) {
			this.concreteType = concreteType;
		}
		/// <summary>
		/// Constructor for the use in plugin.xml
		/// </summary>
		public FixedClassContributionFactory( XmlElement e ) :
			this( PluginUtil.loadTypeFromManifest(
					(XmlElement)XmlUtil.selectSingleNode(e,"implementation") ) ) {}

		private readonly Type concreteType;

		public Contribution load( Plugin owner, XmlElement e ) {
			return (Contribution)Activator.CreateInstance(concreteType,new object[]{e});
		}
	}
}
