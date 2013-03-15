using System;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.framework.graphics
{
	/// <summary>
	/// Let SpriteFactories to be contributed.
	/// 
	/// SpriteFactoryContribution assigns a name to SpriteFactory,
	/// and also allows SpriteFactory to be confiugred by parameters.
	/// </summary>
	[Serializable]
	public abstract class SpriteFactoryContribution : Contribution
	{
		public abstract SpriteFactory createSpriteFactory( XmlElement e );

		public SpriteFactoryContribution( XmlElement e ) : base(e) {}
	}
}
