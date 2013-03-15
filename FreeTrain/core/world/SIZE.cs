using System;
using System.Drawing;

namespace freetrain.world
{
	/// <summary>
	/// Better "Size" class.
	/// </summary>
	[Serializable]
	public struct SIZE
	{
		public SIZE( Size sz ) : this( sz.Width, sz.Height ) {
		}
		public SIZE( int _x, int _y ) {
			this.x = _x;
			this.y = _y;
		}

		public int x;
		public int y;

		/// <summary>
		/// Area of this size.
		/// </summary>
		public int area {
			get {
				return x*y;
			}
		}

		public bool is1 { get { return x==1 && y==1; } }

		public static implicit operator Size( SIZE sz ) {
			return new Size( sz.x, sz.y );
		}
	}
}
