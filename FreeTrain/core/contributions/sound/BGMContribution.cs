using System;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.contributions.sound
{
	/// <summary>
	/// Background music.
	/// </summary>
	[Serializable]
	public class BGMContribution : Contribution
	{
		public BGMContribution( XmlElement e ) : base(e) {
			this.name = XmlUtil.selectSingleNode( e, "name" ).InnerText;

			XmlElement href = (XmlElement)XmlUtil.selectSingleNode( e, "href" );
			fileName = XmlUtil.resolve( href, href.InnerText ).LocalPath;
		}

		public BGMContribution( string name, string fileName, string id ) : base("bgm",id) {
			this.name = name;
			this.fileName = fileName;
		}

		/// <summary> Title of the music. </summary>
		public readonly string name;

		/// <summary> File name of the music. </summary>
		public readonly string fileName;
	}
}
