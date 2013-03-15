using System;
using System.Drawing;
using System.Xml;
using freetrain.framework.graphics;
using freetrain.framework.plugin;

namespace freetrain.world.rail.signal
{
	/// <summary>
	/// Contribution of rail signal.
	/// </summary>
	[Serializable]
	public class RailSignalContribution : Contribution
	{
		public RailSignalContribution( XmlElement e ) : base(e) {
			this.name = XmlUtil.selectSingleNode( e, "name" ).InnerText;

			this.onLeft = XmlUtil.selectSingleNode(e,"side").InnerText.Trim()=="left";

			Picture picture = getPicture(e);
			SpriteFactory spriteFactory = SpriteFactory.getSpriteFactory(e);
			
			for( int i=0; i<4; i++ )
				sprites[i] = spriteFactory.createSprite(
					picture, new Point(0,8), new Point(i*32,0), new Size(32,24));
		}

		/// <summary> Name of this signal. </summary>
		public readonly string name;

		private readonly Sprite[] sprites = new Sprite[4];
		
		/// <summary> True if this signal is standing on the left of a railway. </summary>
		private readonly bool onLeft;

		/// <summary>
		/// Gets the sprite.
		/// </summary>
		public Sprite getSprite( Direction d ) {
			return sprites[d.index/2];
		}

		/// <summary>
		/// Returns true if the signal needs to be drawn after drawing the rail/train.
		/// </summary>
		public bool needsToDrawAfter( Direction d ) {
			bool b = d==Direction.NORTH || d==Direction.WEST;
			if( onLeft )	return !b;
			else			return b;
		}

		public override string ToString() {
			return name;
		}
	}
}
