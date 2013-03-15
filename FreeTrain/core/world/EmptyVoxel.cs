using System;
using System.Drawing;
using org.kohsuke.directdraw;

namespace freetrain.world
{
	/// <summary>
	/// Empty, in the sense that nothing will be drawn, but
	/// occupied, in the sense that the space is already in use.
	/// </summary>
	[Serializable]
	public class EmptyVoxel : AbstractVoxelImpl
	{
		private readonly Entity _entity;
		public override bool transparent { get { return true; } }

		public EmptyVoxel( Entity e, int x, int y, int z ) : this( e, new Location(x,y,z) ) {}
		public EmptyVoxel( Entity e, Location loc ) : base(loc) {
			this._entity = e;
		}

		public override void draw( DrawContext surface, Point pt, int heightCutDiff ) {
			// draw nothing
		}

		public override Entity entity { get { return _entity; } }
	}
}
