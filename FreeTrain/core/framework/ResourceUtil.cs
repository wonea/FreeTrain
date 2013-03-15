using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Net;
using org.kohsuke.directaudio;
using org.kohsuke.directdraw;
using freetrain.util;
using freetrain.framework.graphics;
using freetrain.world;

namespace freetrain.framework
{
	/// <summary>
	/// Simplified resource manager.
	/// </summary>
	public abstract class ResourceUtil
	{
		public static string findSystemResource( string name ) {
			string path;
			
			path = Path.Combine( Core.installationDirectory, @"res\"+name );
			if( File.Exists(path) )	return path;

			path = Path.Combine( Core.installationDirectory, @"..\..\core\res\"+name );
			if( File.Exists(path) )	return path;

			throw new FileNotFoundException("system resource: "+name);
		}

//		private static WebResponse getStream( Uri uri ) {
//			return WebRequest.Create(uri).GetResponse();
//		}

//		public static Bitmap loadBitmap( string location ) {
//			using(WebResponse res = getStream(uri)) {
//				return new Bitmap(res.GetResponseStream());
//			}
//		}
		public static Bitmap loadSystemBitmap( string name ) {
			return new Bitmap(findSystemResource(name));
		}

//		public static Icon loadIcon( Uri uri) {
//			using(WebResponse res = getStream(uri)) {
//				return new Icon(res.GetResponseStream());
//			}
//		}

		public static Segment loadSystemSound( String name ) {
			// can't read from stream
			return Segment.fromFile(findSystemResource(name));
		}

		// using URI is essentially dangerous as Segment only support file names.
		// I should limit it to file names only.
		public static Segment loadSound( Uri uri ) {
			return Segment.fromFile(uri.LocalPath);
		}

		public static Picture loadSystemPicture( string name ) {
			string id = "{8AD4EF28-CBEF-4C73-A8FF-5772B87EF005}:"+name;

			// check if it has already been loaded
			if( PictureManager.contains(id) )
				return PictureManager.get(id);

			// otherwise load a new picture
			return new Picture( id, findSystemResource(name) );
		}
		public static Picture loadSystemPicture( string dayname, string nightname ) 
		{
			string id = "{8AD4EF28-CBEF-4C73-A8FF-5772B87EF005}:"+dayname;

			// check if it has already been loaded
			if( PictureManager.contains(id) )
				return PictureManager.get(id);

			// otherwise load a new picture
			return new Picture( id, findSystemResource(dayname), findSystemResource(nightname) );
		}

		public static Surface loadTimeIndependentSystemSurface( string name ) {
			using(Bitmap bmp=loadSystemBitmap(name))
				return directDraw.createSprite(bmp);
		}



		/// <summary>
		/// DirectDraw instance for loading surface objects.
		/// </summary>
		public static readonly DirectDraw directDraw = new DirectDraw();

		private static Picture emptyChips = loadSystemPicture("EmptyChip.bmp","EmptyChip_n.bmp");
		private static Picture cursorChips = loadSystemPicture("cursorChip.bmp","cursorChip.bmp");

		public static Sprite emptyChip {
			get {
				return groundChips[0];
			}
		}
		public static Sprite getGroundChip(World w) {
			if( w.clock.season!=Season.Winter )	
				return groundChips[0];
			else											
				return groundChips[1];
		}

		private static Sprite[] groundChips = new Sprite[]{
			new SimpleSprite(emptyChips,new Point(0,0),new Point( 0,0),new Size(32,16)),
			new SimpleSprite(emptyChips,new Point(0,0),new Point(32,0),new Size(32,16))
		};
		
		public static Sprite removerChip =
			new SimpleSprite(cursorChips,new Point(0,0),new Point(0,0),new Size(32,16));
		
		public static Sprite underWaterChip =
			new SimpleSprite(emptyChips,new Point(0,0),new Point(64,0),new Size(32,16));
		public static Sprite underGroundChip =
			new SimpleSprite(emptyChips,new Point(0,0),new Point(96,0),new Size(32,16));
	}
}
