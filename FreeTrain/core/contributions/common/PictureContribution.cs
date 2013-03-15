using System;
using System.Xml;
using freetrain.framework.plugin;
using freetrain.framework.graphics;

namespace freetrain.contributions.common
{
	/// <summary>
	/// Picture can be contributed.
	/// </summary>
	[Serializable]
	public class PictureContribution : Contribution
	{
		public readonly Picture picture;

		public PictureContribution( XmlElement e ) : base(e) {
			picture = new Picture(
				(XmlElement)XmlUtil.selectSingleNode(e,"picture"),
				this.id);
			// picture object will register itself to the manager.
		}
	}
}
