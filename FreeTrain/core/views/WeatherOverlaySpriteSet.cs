using System;
using System.Drawing;
using freetrain.framework.graphics;

namespace freetrain.views
{
	/// <summary>
	/// sprite images.
	/// </summary>
	public class WeatherOverlaySpriteSet
	{
		public readonly Sprite[] overlayImages;
		public readonly Size imageSize;
		
		public WeatherOverlaySpriteSet( string pictureId, int frameLength, Size sz ) {
			Picture pic = PictureManager.get(pictureId);
			imageSize = sz;
			overlayImages = new Sprite[frameLength];
			for( int i=0; i<frameLength; i++ )
				overlayImages[i] = new SimpleSprite(pic,new Point(0,0),new Point(sz.Width*i,0),sz);
		}
	}
}
