using System;
using System.Diagnostics;
using System.Drawing;

namespace freetrain.framework.graphics
{
	/// <summary>
	/// Maintain a set of dirty rects.
	/// </summary>
	[Serializable]
	public class DirtyRect
	{
		private Rectangle _rect;
		private bool _isEmpty = true;

		public bool isEmpty { get { return _isEmpty; } }
		public Rectangle rect { get { return _rect; } }

		public void add( Rectangle r ) {
			Debug.Assert( r.Left>=0 );

			// this assertion is incorrect. a higher voxel on the top border can yield
			// a rectangle with top<0.
			// Debug.Assert( r.Top >=0 );

			if(_isEmpty) {
				_rect = r;
				_isEmpty = false;
			} else {
				_rect = Rectangle.Union(_rect,r);
			}
		}

		public void add( int x, int y, int w, int h ) {
			add( new Rectangle(x,y,w,h) );
		}

		public void add( Point pt, int w, int h ) {
			add( pt.X, pt.Y, w, h );
		}

		public void clear() {
			_isEmpty = true;
		}
	}

/*
	/// <summary>
	/// Maintain a set of dirty rects.
	/// 
	/// The algorithm is taken from "Yaneurao-SDK"
	/// </summary>
	public class DirtyRect
	{
		/// <summary> Number of rects to keep. </summary>
		private const int MAX = 4;

		/// <summary> Number of rects in the buffer. </summary>
		private int n;

		/// <summary> Dirty rects. </summary>
		private readonly Rectangle[] rects = new Rectangle[MAX];

		private readonly int[] areas = new int[MAX];

		public DirtyRect() {
			clear();
		}

		public int size { get { return n; } }

		public Rectangle getRect( int idx ) { return rects[idx]; }



		public void clear() {
			n=0;
		}

		/// <summary>
		/// Adds a new dirty rect.
		/// </summary>
		public void add( int x1, int y1, int x2, int y2 ) {
			rects[n] = new Rectangle(x1,y1,x2,y2);
			areas[n] = (x2-x1)*(y2-y1);
			if( (++n)!=MAX )	return;

			// compact the existing rects
			// find a pair of rects that minimizes the increase of area
			Rectangle minRect = new Rectangle(0,0,0,0);
			int minDelta=int.MaxValue,minArea=0,I=0,J=0;

			for( int i=MAX-2; i>=0; i-- ) {
				for( int j=MAX-1; j>i; j-- ) {
					// try merging i-th and j-th rects
					Rectangle newRect = Rectangle.Union( rects[i], rects[j] );
					int a = newRect.Height * newRect.Width;
					int delta = a-(areas[i]+areas[j]);	// increase in the area
					if( delta < minDelta ) {
						minArea = a;
						minRect = newRect;
						I=i; J=j;
					}
				}
			}

			// update rects
			areas[I] = minArea;
			rects[I] = minRect;

			// delete the J-th rect
			if(J!=MAX-1) {
				rects[J] = rects[MAX-1];
				areas[J] = areas[MAX-1];
			}
			n--;
		}

		/// <summary>
		/// Fully compact the rects.
		/// </summary>
		public void refresh() {
			for( int i=0; i<n-1; i++ ) {
				for( int j=i+1; j<n;  ) {
					Rectangle u = Rectangle.Union( rects[i], rects[j] );
					int a = u.Height*u.Width;
					int delta = a-(areas[i]+areas[j]);
					if( delta<0 ) {
						areas[i] = a;
						rects[i] = u;
						if(j!=n-1) {
							rects[j] = rects[n-1];
							areas[j] = areas[n-1];
						}
						n--;
						// i-th rect is updated, so try the same j again.
					} else {
						j++;
					}
				}
			}
		}
	}
*/

}
