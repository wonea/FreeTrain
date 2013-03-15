using System;
using System.Drawing;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.framework.graphics
{
	/// <summary>
	/// Create a sprite from a picture.
	/// 
	/// SpriteFactory encapsulates the logic to instanciate sprite implementations.
	/// </summary>
	public abstract class SpriteFactory
	{
		public abstract Sprite createSprite( Picture picture, Point offset, Point origin, Size size );


		/// <summary>
		/// Locate the SpriteFactory that should be used to load sprites.
		/// </summary>
		/// <param name="sprite">&lt;sprite> element in the manifest.</param>
		/// <returns>non-null valid object.</returns>
		public static SpriteFactory getSpriteFactory( XmlNode sprite ) {
			XmlElement type = (XmlElement)sprite.SelectSingleNode("spriteType");
			if(type==null)
				type = (XmlElement)sprite.SelectSingleNode("colorVariation");
			if(type==null)
				// if none is specified, use the default sprite factory
				return new SimpleSpriteFactory();
			else {
				string name = type.Attributes["name"].Value;
				// otherwise load from the spriteType element.
				SpriteFactoryContribution contrib = (SpriteFactoryContribution)
					PluginManager.theInstance.getContribution( "spriteFactory:"+name );
				if(contrib==null)
						throw new FormatException("unable to locate spriteFactory:"+ name);
				return contrib.createSpriteFactory(type);
			}
		}
	}
}
