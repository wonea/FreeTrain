using System;
using System.Diagnostics;
using System.Drawing;
using freetrain.framework;
using freetrain.world.land;
using freetrain.world.rail;
using freetrain.world.structs;

namespace freetrain.world.land.forest
{
	/// <summary>
	/// Forest voxel 
	/// </summary>
	[Serializable]
	public class ForestVoxel : LandVoxel, Entity
	{
		public ForestVoxel( Location loc, ForestBuilder contrib, byte[] patterns ) : base(loc) {
			Debug.Assert(patterns.Length!=0);
			this.contrib = contrib;
			this.patterns = patterns;
		}
		private readonly ForestBuilder contrib;

		/// <summary>
		/// Dx = patterns[i*3  ];
		/// Dy = patterns[i*3+1];
		/// idx= patterns[i*3+2];
		/// </summary>
		private readonly byte[] patterns;

		public override Entity entity { get { return this; } }
		public override int entityValue { get { return contrib.price; } }


		public override void draw( DrawContext surface, Point pt, int heightCutDiff ) {
			if( contrib.ground != null )
				contrib.ground.draw( surface.surface, pt );
			for( int i=0; i<patterns.Length; i+=3 )
				contrib.sprites[ patterns[i+2] ].draw( surface.surface,
					new Point( pt.X+patterns[i+0], pt.Y+patterns[i+1] ) );
		}
	}
}
