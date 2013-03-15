using System;

namespace Heat
{
	public class Material
	{
		public Material( int x, int y ) {
			temp = new float[x,y];
			back = new float[x,y];
			source = new float[x,y];
			alpha = new float[x,y];
			for( int i=0;i<x;i++ )
				for( int j=0;j<y;j++ )
					alpha[i,j] = 1.0f;
		}

		public float[,] temp;	// temprature
		private float[,] back;	// back buffer

		public float[,] source; // heat source

		public float[,] alpha;
		
		private float getAlpha( int x, int y ) {
			if(x<0 || y<0 || x>=temp.GetLength(0) || y>=temp.GetLength(1) )
				return 0;
			else
				return alpha[x,y];
		}

		public float next() {
			{// flip the buffer
				float[,] t = temp;
				temp = back;
				back = t;
			}
			float r=0;

			int X = temp.GetLength(0);
			int Y = temp.GetLength(1);

			// compute next
			for( int x=X-1; x>=0; x-- ) {
				for( int y=Y-1; y>=0; y-- ) {
					float t = back[x,y];
					float t1 = ((x!=X-1)?back[x+1,y]:back[x,y]) - t;
					float t2 = ((x!=0  )?back[x-1,y]:back[x,y]) - t;
					float t3 = ((y!=Y-1)?back[x,y+1]:back[x,y]) - t;
					float t4 = ((y!=0  )?back[x,y-1]:back[x,y]) - t;

					temp[x,y] = back[x,y]*DIFF +
						( t1*getAlpha(x+1,y) + t2*getAlpha(x-1,y)
						 +t3*getAlpha(x,y+1) + t4*getAlpha(x,y-1) ) * ALPHA * alpha[x,y] + source[x,y];

					r += temp[x,y];
				}
			}

			return r;
		}

		// no more than 0.25. Otherwise the model becomes chaotic.
		private const float ALPHA = 0.24f;
		
		// diffuse. no more than 1.
		private const float DIFF = 1f-0.001f;
	}
}
