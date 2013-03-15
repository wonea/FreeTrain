using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace freetrain.world
{
	/// <summary>
	/// 3D sparse array for <c>Voxel</c>s.
	/// </summary>
	[Serializable]
	internal class SparseVoxelArray // : ISerializable
	{
		/// <summary>
		/// Size of the array.
		/// </summary>
		public readonly int H,V,Z;

		private readonly Voxel[,,][,] index;

		private const int BLOCK_H = 8;
		private const int BLOCK_V = 8;

		public SparseVoxelArray( int h, int v, int z ) {
			this.H=h; this.V=v; this.Z=z;
			this.index = new Voxel[ (H+BLOCK_H-1)/BLOCK_H, (V+BLOCK_V-1)/BLOCK_V, Z ][,];
		}

		public Voxel this [int h,int v,int z] {
			get {
				if(h>=H || v>=V)	// out of world
					throw new IndexOutOfRangeException();

				Voxel[,] b = index[ h/BLOCK_H, v/BLOCK_V, z ];
				if(b==null)		return null;
				else			return b[ h%BLOCK_H, v%BLOCK_V ];
			}
			set {
				Voxel[,] b = index[ h/BLOCK_H, v/BLOCK_V, z ];
				if(b==null)
					index[ h/BLOCK_H, v/BLOCK_V, z ] = b = new Voxel[BLOCK_H,BLOCK_V];

				Voxel old = b[ h%BLOCK_H, v%BLOCK_V ];
				if( old!=null ) {
					// removed old ones
					Entity e = old.entity;
					if( e.isSilentlyReclaimable )
						e.remove();
				}
				Debug.Assert( b[ h%BLOCK_H, v%BLOCK_V ]==null );
				b[ h%BLOCK_H, v%BLOCK_V ] = value;
			}
		}

		public void remove( int h, int v, int z ) {
			Voxel[,] b = index[ h/BLOCK_H, v/BLOCK_V, z ];
			if(b==null)		return;
			b[ h%BLOCK_H, v%BLOCK_V ] = null;
		}

		/// <summary>
		/// Release unused indexes.
		/// </summary>
		public void cleanUp() {
			for( int z=0; z<index.GetLength(2); z++ ) {
				for( int v=0; v<index.GetLength(1); v++ ) {
					for( int h=0; h<index.GetLength(0); h++ ) {
						Voxel[,] b = index[h,v,z];
						if(b==null)		continue;

						bool empty = true;
						
						foreach( Voxel vox in b ) {
							if( vox!=null ) {
								empty = false;
								break;
							}
						}

						if( empty )
							index[h,v,z] = null;
					}
				}
			}
		}


		#region serialization support
//
//		public SparseVoxelArray(SerializationInfo info, StreamingContext context) {
//			Distance d = (Distance)info.GetValue("size",typeof(Distance));
//			this.H = d.x;
//			this.V = d.y;
//			this.Z = d.z;
//
//			this.index = new Voxel[ (H+BLOCK_H-1)/BLOCK_H, (V+BLOCK_V-1)/BLOCK_V, Z ][,];
//
//			foreach( SerializationEntry e in info ) {
//				string name = e.Name;
//				if( name[0]!='v' )	continue;
//
//				string[] hvz = name.Substring(1).Split('-');
//
//				this[int.Parse(hvz[0]),int.Parse(hvz[1]),int.Parse(hvz[2])]
//					= (Voxel)e.Value;
//			}
//		}
//
//
//		public void GetObjectData( SerializationInfo info, StreamingContext context ) {
//			info.AddValue("size",new Distance(H,V,Z));
//
//			for( int z=0; z<index.GetLength(2); z++ ) {
//				for( int v=0; v<index.GetLength(1); v++ ) {
//					for( int h=0; h<index.GetLength(0); h++ ) {
//						Voxel[,] b = index[h,v,z];
//						if(b==null)		continue;
//
//						for( int sv=0; sv<BLOCK_V; sv++ ) {
//							for( int sh=0; sh<BLOCK_H; sh++ ) {
//								if( b[sh,sv]!=null )
//									info.AddValue(string.Format("v{0}-{1}-{2}",
//										h*BLOCK_H+sh, v*BLOCK_V+sv, z ),
//										b[sh,sv] );
//							}
//						}
//					}
//				}
//			}
//		}
		#endregion
	}
}
