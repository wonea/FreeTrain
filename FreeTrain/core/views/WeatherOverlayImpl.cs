using System;
using System.Drawing;
using freetrain.framework;
using freetrain.framework.graphics;
using org.kohsuke.directdraw;

namespace freetrain.views
{
	/// <summary>
	/// WeatherOverlayImpl の概要の説明です。
	/// </summary>
	public class WeatherOverlayImpl : WeatherOverlay
	{
		private readonly WeatherOverlaySpriteSet spriteSet;
		private Size canvasSize;
		private Surface offscreenSurface;
		private int currentFrame=0;

		public WeatherOverlayImpl( WeatherOverlaySpriteSet _spriteSet ) {
			this.spriteSet = _spriteSet;
		}

		public void Dispose() {
			if( offscreenSurface!=null ) {
				offscreenSurface.Dispose();
				offscreenSurface = null;
			}
		}

		public void setSize( Size sz ) {
			if( this.canvasSize==sz )	return;
			this.canvasSize = sz;
			if( offscreenSurface!=null )
				offscreenSurface.Dispose();
			offscreenSurface = MainWindow.mainWindow.directDraw.createOffscreenSurface(sz);
		}

		public void draw( QuarterViewDrawer drawer, Surface target, Point pt ) {
			drawer.draw( offscreenSurface, new Point(0,0) );
			for( int x=0; x<canvasSize.Width; x+=spriteSet.imageSize.Width )
				for( int y=0; y<canvasSize.Height; y+=spriteSet.imageSize.Height )
					spriteSet.overlayImages[currentFrame].draw( offscreenSurface, new Point(x,y) );

			target.blt( pt, offscreenSurface );
		}

		public bool onTimerFired() {
			currentFrame = (currentFrame+1)%spriteSet.overlayImages.Length;
			return true;
		}
	}
}
