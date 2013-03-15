using System;
using System.Drawing;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.framework.graphics
{
	/// <summary>
	/// SpriteLoaderContribution encapsulates the details of how a set of sprites
	/// are produced from a Picture.
	/// 
	/// This is a contribution so it can be implemented by plug-ins.
	/// 
	/// This class has many methods but one doesn't need
	/// to implement all of them.
	/// throw NotImplementedException in case of any error.
	/// 
	/// This default implementation just implements all the methods
	/// by returning an error.
	/// </summary>
	public abstract class SpriteLoaderContribution : Contribution
	{
		public SpriteLoaderContribution( XmlElement e ) : base(e) {}

		/// <summary>
		/// Load a single sprite from the given manifest.
		/// </summary>
		public virtual Sprite load0D( XmlElement sprite ) {
			throw new NotImplementedException(this.GetType().FullName+".load0D");
		}

		/// <summary>
		/// Load a set of sprites of size (x) from the given manifest.
		/// </summary>
		public virtual Sprite[] load1D( XmlElement sprite, int x ) {
			throw new NotImplementedException(this.GetType().FullName+".load1D");
		}

		/// <summary>
		/// Load a set of sprites of size (x,y) from the given manifest.
		/// </summary>
		public virtual Sprite[,] load2D( XmlElement sprite, int x, int y, int height ) {
			throw new NotImplementedException(this.GetType().FullName+".load2D");
		}

		public Sprite[,] load2D( XmlElement sprite, Size sz, int height ) {
			return load2D( sprite, sz.Width, sz.Height, height );
		}

		/// <summary>
		/// Load a set of sprites of size (x,y,z) from the given manifest.
		/// </summary>
		public virtual Sprite[,,] load3D( XmlElement sprite, int x, int y, int z ) {
			throw new NotImplementedException(this.GetType().FullName+".load3D");
		}
	}
}
