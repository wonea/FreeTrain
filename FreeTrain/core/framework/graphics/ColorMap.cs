using System;
using System.Drawing;

namespace freetrain.framework.graphics
{
	/// <summary>
	/// Determines the color mapping between daylight time and night time.
	/// </summary>
	public class ColorMap
	{
		public static Color getNightColor( Color src ) {
			// handle three light colors
			for( int i=0; i<lightColorMap.Length; i+=2 )
				if( lightColorMap[i]==src )
					return lightColorMap[i+1];

			// TODO: handle season colors

			return Color.FromArgb( ratio(src.R), ratio(src.G), ratio(src.B) );
		}

		private static int ratio( int i ) { return i/4; }

		private static readonly Color[] lightColorMap = new Color[] {
			// original color			light color
			Color.FromArgb(8,0,0),		Color.FromArgb(255,  8,  8),
			Color.FromArgb(0,8,0),		Color.FromArgb(252,243,148),
			Color.FromArgb(0,0,8),		Color.FromArgb(255,227, 99)
		};

	}
}
