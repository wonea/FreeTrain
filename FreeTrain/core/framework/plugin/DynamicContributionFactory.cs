using System;
using System.Xml;

namespace freetrain.framework.plugin
{
	/// <summary>
	/// Loads a Contribution class by using a &lt;class> element
	/// in the contribution Xml definition.
	/// </summary>
	public class DynamicContributionFactory : ContributionFactory
	{
		/// <param name="baseType">
		/// Type of the contribution to be loaded.
		/// Loaded class is judged invalid unless it is a subtype
		/// of this type.
		/// </param>
		public DynamicContributionFactory( Type baseType ) {
			this.baseType = baseType;
		}

		public DynamicContributionFactory( XmlElement e ) :
			this( PluginUtil.loadTypeFromManifest(
					(XmlElement)XmlUtil.selectSingleNode(e,"implementation") ) ) {}


		private readonly Type baseType;

		public Contribution load( Plugin owner, XmlElement e ) {
			Contribution contrib = (Contribution)PluginUtil.loadObjectFromManifest(e);
			if( baseType.IsInstanceOfType(contrib) )
				return contrib;
			else
				throw new Exception(string.Format(
					"{0} is incorrect for for this contribution (expected:{1})",
					contrib.GetType().FullName, baseType.FullName ));
		}
	}
}
