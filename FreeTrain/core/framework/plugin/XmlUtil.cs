using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Xml;
using freetrain.world;

namespace freetrain.framework.plugin
{
	/// <summary>
	/// Utility methods to help parsing XML documents.
	/// </summary>
	public class XmlUtil
	{
		/// <summary>
		/// Performs a node selection and throws an exception if it's not found.
		/// </summary>
		/// <exception cref="XmlException"></exception>
		public static XmlNode selectSingleNode( XmlNode node, string xpath ) {
			XmlNode n = node.SelectSingleNode(xpath);
			if(n==null)
				throw new XmlException("unable to find "+xpath,null);
			return n;
		}

		/// <summary>
		/// Resolves a relative URI.
		/// </summary>
		public static Uri resolve( XmlNode context, string relative ) {
			return new Uri(new Uri(context.BaseURI),relative);
		}

		public static Point parsePoint( string text ) {
			try {
				int idx = text.IndexOf(',');
				return new Point( int.Parse(text.Substring(0,idx)), int.Parse(text.Substring(idx+1)) );
			} catch( Exception e ) {
				throw new FormatException("Unable to parse "+text+" as point",e);
			}
		}

		public static SIZE parseSize( string text ) {
			try {
				int idx = text.IndexOf(',');
				return new SIZE( int.Parse(text.Substring(0,idx)), int.Parse(text.Substring(idx+1)) );
			} catch( Exception e ) {
				throw new FormatException("Unable to parse "+text+" as size",e);
			}
		}
	}
}
