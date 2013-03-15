using System;
using System.Drawing;
using System.Runtime.Serialization;
using org.kohsuke.directdraw;

namespace freetrain.framework.graphics
{
	/// <summary>
	/// Sprite that draws nothing.
	/// </summary>
	[Serializable]
	public class NullSprite : Sprite, IObjectReference
	{
		private NullSprite() {}
		
		public static readonly Sprite theInstance = new NullSprite();

		public void draw( Surface surface, Point pt ) {}
		public void drawShape( Surface surface, Point pt, Color color ) {}
		public void drawAlpha( Surface surface, Point pt ) {}

		public Size size { get { return new Size(0,0); } }
		public Point offset { get { return new Point(0,0); } }
		public bool HitTest( int x, int y){ return false; }
		
		public object GetRealObject( StreamingContext ctxt ) {
			return theInstance;
		}
	}
}
