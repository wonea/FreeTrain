using System;
using System.Drawing;
using System.Xml;
using freetrain.contributions.others;
using freetrain.framework.plugin;
using freetrain.world;

namespace freetrain.tools.terrainloader
{
	/// <summary>
	/// Specialized new game contribution that allows other plug-ins to
	/// fix a picture and size, etc and add it to the new game list.
	/// </summary>
	public class TerrainOnlyNewGameContribution : NewGameContribution
	{
		private readonly string _author;
		private readonly string _name;
		private readonly string _description;

		/// <summary> Full name of the image (usually bitmap) file. </summary>
		private readonly string imageFileName;
		private readonly SIZE size;
		private readonly int height;
		private readonly int waterLevel;

		public TerrainOnlyNewGameContribution( XmlElement e ) : base(e) {
			_author = XmlUtil.selectSingleNode(e,"author").InnerText;
			_name = XmlUtil.selectSingleNode(e,"name").InnerText;
			_description = XmlUtil.selectSingleNode(e,"description").InnerText.Trim();

			XmlElement image = (XmlElement)XmlUtil.selectSingleNode(e,"image");
			imageFileName = XmlUtil.resolve( image, XmlUtil.selectSingleNode(image,"@href").InnerText ).LocalPath;
			
			size = XmlUtil.parseSize(XmlUtil.selectSingleNode(e,"size").InnerText);
			height = int.Parse(XmlUtil.selectSingleNode(e,"height").InnerText);
			waterLevel = int.Parse(XmlUtil.selectSingleNode(e,"waterLevel").InnerText);
		}

		public override string author { get { return _author; } }
		public override string name { get { return _name; } }
		public override string description { get { return _description; } }

		public override World createNewGame() {
			using( Bitmap bmp = new Bitmap(imageFileName) ) {
				return TerrainLoader.loadWorld(
					bmp,
					new Size( size.x, size.y ),
					height, waterLevel );
			}
		}
	}
}
