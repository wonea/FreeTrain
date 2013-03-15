using System;
using System.Xml;

namespace nft.framework.plugin
{
	/// <summary>
	/// Responsible for loading a contribution from an XML element.
	/// </summary>
	public interface ContributionFactory
	{
		/// <summary>
		/// Loads a contribution from an XML Element "e",
		/// which belongs to the plugin "owner".
		/// 
		/// In case of an error, this method must throw an exception
		/// instead of silently returning null.
		/// </summary>
		Contribution load( Plugin owner, XmlElement e );
	}
}
