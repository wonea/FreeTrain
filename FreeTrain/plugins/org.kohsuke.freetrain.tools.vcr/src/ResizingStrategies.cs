using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace freetrain.tools.vcr
{
	/// <summary>
	/// Determines
	/// how the rectangle position changes according to the mouse pos.
	/// </summary>
	internal interface ResizingStrategy
	{
		/// <param name="current">
		/// Current rectangle
		/// </param>
		/// <param name="mousePos">
		/// Mouse position in the (A,B) axis
		/// </param>
		/// <returns>
		/// Updated rectangle
		/// </returns>
		Rectangle resize( Rectangle current, Point mousePos );
		
		/// <summary> Cursor that should be used. </summary>
		Cursor cursor { get; }
	}

	internal class ResizingStrategies
	{
		private static Rectangle create( int x1, int y1, int x2, int y2 ) {
			return new Rectangle( x1, y1, x2-x1, y2-y1 );
		}

		/// <summary>
		/// North-west corner resizing
		/// </summary>
		public static readonly ResizingStrategy NW = new NWImpl();
		private class NWImpl : ResizingStrategy
		{
			public Rectangle resize( Rectangle rect, Point ab ) {
				if( ab.X<rect.Right && ab.Y<rect.Bottom )
					return create( ab.X, ab.Y, rect.Right, rect.Bottom );
				else
					return rect;
			}
			public Cursor cursor { get { return Cursors.SizeNWSE; } }
		}


		/// <summary>
		/// North edge resizing
		/// </summary>
		public static readonly ResizingStrategy N = new NImpl();
		private class NImpl : ResizingStrategy
		{
			public Rectangle resize( Rectangle rect, Point ab ) {
				if( ab.Y<rect.Bottom )
					return create( rect.Left, ab.Y, rect.Right, rect.Bottom );
				else
					return rect;
			}
			public Cursor cursor { get { return Cursors.SizeNS; } }
		}

		/// <summary>
		/// North-east corner resizing
		/// </summary>
		public static readonly ResizingStrategy NE = new NEImpl();
		private class NEImpl : ResizingStrategy
		{
			public Rectangle resize( Rectangle rect, Point ab ) {
				if( ab.X>rect.Left && ab.Y<rect.Bottom )
					return create( rect.Left, ab.Y, ab.X, rect.Bottom );
				else
					return rect;
			}
			public Cursor cursor { get { return Cursors.SizeNESW; } }
		}

		/// <summary>
		/// East edge resizing
		/// </summary>
		public static readonly ResizingStrategy E = new EImpl();
		private class EImpl : ResizingStrategy
		{
			public Rectangle resize( Rectangle rect, Point ab ) {
				if( ab.X>rect.Left )
					return create( rect.Left, rect.Top, ab.X, rect.Bottom );
				else
					return rect;
			}
			public Cursor cursor { get { return Cursors.SizeWE; } }
		}

		/// <summary>
		/// South-east corner resizing
		/// </summary>
		public static readonly ResizingStrategy SE = new SEImpl();
		private class SEImpl : ResizingStrategy
		{
			public Rectangle resize( Rectangle rect, Point ab ) {
				if( ab.X>rect.Left && ab.Y>rect.Top )
					return create( rect.Left, rect.Top, ab.X, ab.Y );
				else
					return rect;
			}
			public Cursor cursor { get { return Cursors.SizeNWSE; } }
		}

		/// <summary>
		/// South edge resizing
		/// </summary>
		public static readonly ResizingStrategy S = new SImpl();
		private class SImpl : ResizingStrategy
		{
			public Rectangle resize( Rectangle rect, Point ab ) {
				if( ab.Y>rect.Top )
					return create( rect.Left, rect.Top, rect.Right, ab.Y );
				else
					return rect;
			}
			public Cursor cursor { get { return Cursors.SizeNS; } }
		}

		/// <summary>
		/// South-west corner resizing
		/// </summary>
		public static readonly ResizingStrategy SW = new SWImpl();
		private class SWImpl : ResizingStrategy
		{
			public Rectangle resize( Rectangle rect, Point ab ) {
				if( ab.X<rect.Right && ab.Y>rect.Top )
					return create( ab.X, rect.Top, rect.Right, ab.Y );
				else
					return rect;
			}
			public Cursor cursor { get { return Cursors.SizeNESW; } }
		}

		/// <summary>
		/// West edge resizing
		/// </summary>
		public static readonly ResizingStrategy W = new WImpl();
		private class WImpl : ResizingStrategy
		{
			public Rectangle resize( Rectangle rect, Point ab ) {
				if( ab.X<rect.Right )
					return create( ab.X, rect.Top, rect.Right, rect.Bottom );
				else
					return rect;
			}
			public Cursor cursor { get { return Cursors.SizeWE; } }
		}

		/// <summary>
		/// Center panning. The behavior is like when you drag the title bar of a window.
		/// </summary>
		public class CenterImpl : ResizingStrategy
		{
			private readonly int offsetX;
			private readonly int offsetY;

			public CenterImpl( int offX, int offY ) {
				this.offsetX = offX;
				this.offsetY = offY;
			}

			public Rectangle resize( Rectangle rect, Point ab ) {
				rect.X = ab.X-offsetX;
				rect.Y = ab.Y-offsetY;
				return rect;
			}
			public Cursor cursor { get { return Cursors.SizeAll; } }
		}


		/// <summary>
		/// Width of the grip.
		/// </summary>
		private const int MARGIN = 3;

		/// <summary>
		/// Gets the applicable strategy from the current rect and the mouse position.
		/// </summary>
		/// <returns>
		///	null if non is applicable (if the mouse position is completely out of the place)
		/// </returns>
		public static ResizingStrategy getStrategy( Rectangle r, Point ab ) {
			int region=0;

			if( Math.Abs(ab.Y-r.Top)<MARGIN )			region += 0;
			else
			if( Math.Abs(ab.Y-r.Bottom)<MARGIN )		region += 6;
			else
			if( r.Top<ab.Y && ab.Y<r.Bottom )			region += 3;
			else
				return null;

			if( Math.Abs(ab.X-r.Left)<MARGIN )			region += 0;
			else
			if( Math.Abs(ab.X-r.Right)<MARGIN )			region += 2;
			else
			if( r.Left<ab.X && ab.X<r.Right )			region += 1;
			else
				return null;
			
			switch(region) {
			case 0:		return NW;
			case 1:		return N;
			case 2:		return NE;

			case 3:		return W;
			case 4:		return new CenterImpl( ab.X-r.X, ab.Y-r.Y );
			case 5:		return E;

			case 6:		return SW;
			case 7:		return S;
			case 8:		return SE;

			default:	Debug.Fail("assertion failed"); return null;
			}
		}
	}
}
