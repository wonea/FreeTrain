using System;
using System.Diagnostics;
using System.Drawing;
using freetrain.framework.graphics;
using org.kohsuke.directdraw;

namespace freetrain.world.land
{
	/// <summary>
	/// Player-owned land property.
	/// </summary>
	[Serializable]
	public class LandPropertyVoxel : AbstractVoxelImpl, Entity
	{
		public LandPropertyVoxel( Location loc ) : base(loc) {
		}

		public override Entity entity { get { return this; } }

		public override void draw( DrawContext surface, Point pt, int heightCutDiff ) {
			sprite.draw( surface.surface, pt );
		}

		#region Entity implementation
		public bool isSilentlyReclaimable { get { return true; } }
		public bool isOwned { get { return true; } }

		public int entityValue { get { return 0; } }

		public void remove() {
			World.world.remove(this);
			if(onEntityRemoved!=null)	onEntityRemoved(this,null);
		}
		public event EventHandler onEntityRemoved;
		#endregion


		public static readonly Sprite sprite;

		static LandPropertyVoxel() {
			Picture pic = PictureManager.get("{0E7A9F09-4482-4b78-8A8D-F59F02574B1B}");
			sprite = new SimpleSprite( pic, new Point(0,0), new Point(0,0), new Size(32,16) );
		}
	}
}
