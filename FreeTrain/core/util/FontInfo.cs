using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Reflection;
using System.Xml.Serialization;

namespace freetrain.util
{
	/// <summary>
	/// LOGFONT structure that keeps all the information of Font.
	/// 
	/// <c>Font</c> doesn't work with XML serialization, but this one does.
	/// </summary>
	public class FontInfo
	{
		[XmlAttribute()]
		public string fontName;
		[XmlAttribute()]
		public GraphicsUnit unit;
		[XmlAttribute()]
		public float size;
		[XmlAttribute()]
		public FontStyle style;

		public FontInfo() {}
		public FontInfo( Font f ) {
			this.fontName = f.FontFamily.Name;
			this.unit = f.Unit;
			this.size = f.Size;
			this.style = f.Style;
		}

		public Font createFont() {
			return new Font( fontName, size, style, unit );
		}
	}
}