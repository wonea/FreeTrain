using System;
using System.Drawing;
using System.Reflection;
using System.Xml;
using freetrain.framework.graphics;

namespace freetrain.framework.plugin
{
	/// <summary>
	/// Utility code
	/// </summary>
	public class PluginUtil
	{
		/// <summary>
		/// Parse a color from a string of the form "100,53,26"
		/// </summary>
		public static Color parseColor( string value ) {
			string[] cmp = value.Split(',');
			return Color.FromArgb( int.Parse(cmp[0]), int.Parse(cmp[1]), int.Parse(cmp[2]) );
		}

		/// <summary>
		/// Load a new object by reading a type from the manifest XML element.
		/// The "codeBase" attribute and the "name" attribute of
		/// a class element are used to determine the class to be loaded.
		/// </summary>
		public static object loadObjectFromManifest( XmlElement contrib ) {
			XmlElement el = (XmlElement)XmlUtil.selectSingleNode(contrib,"class");
			Type t = loadTypeFromManifest(el);

			try {
				object result = Activator.CreateInstance(t,new object[]{contrib});
				if( result==null )
					throw new FormatException("Designated class can not be loaded: "+t.FullName);
					//! throw new FormatException("指定されたクラスはロードできません:"+t.FullName);

				return result;
			} catch( TargetInvocationException e ) {
				throw new FormatException("Designated class can not be loaded: "+t.FullName,e);
				//! throw new FormatException("指定されたクラスはロードできません:"+t.FullName,e);
			}
		}
		
		/// <summary>
		/// Load a type from the name attribute and the codebase attribute .
		/// </summary>
		/// <param name="e">Typically a "class" element</param>
		public static Type loadTypeFromManifest( XmlElement e ) {
			string typeName = e.Attributes["name"].Value;

			Assembly a;

			if( e.Attributes["codebase"]==null ) {
				// load the class from the FreeTrain.Core.dll
				a = Assembly.GetExecutingAssembly();
			} else {
				// load the class from the specified assembly
				Uri codeBase = XmlUtil.resolve( e, e.Attributes["codebase"].Value );

				if( !codeBase.IsFile )
					throw new FormatException("Designated codebase is not a filename: "+codeBase);
					//! throw new FormatException("指定されたコードベースはファイル名ではありません:"+codeBase);

				a = Assembly.LoadFrom( codeBase.LocalPath );
			}

			return a.GetType(typeName,true);
		}

		public static SpriteLoaderContribution getSpriteLoader( XmlElement sprite ) {
			string loader;
			
			if( sprite.Attributes["loader"]!=null )
				loader = sprite.Attributes["loader"].Value;
			else
				loader = "default";

			SpriteLoaderContribution contrib = (SpriteLoaderContribution)
				PluginManager.theInstance.getContribution("spriteLoader:"+loader);
			if( contrib==null )
				throw new Exception("unable to find spriteLoader:"+loader);
			return contrib;
		}
	}
}
