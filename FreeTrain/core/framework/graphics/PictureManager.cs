using System;
using System.Collections;
using System.Diagnostics;
using freetrain.world;

namespace freetrain.framework.graphics
{
	/// <summary>
	/// Repository of pictures.
	/// </summary>
	public class PictureManager
	{
		/// <summary>
		/// Event fired when a DirectDraw surface is found to be lost.
		/// </summary>
		public static EventHandler onSurfaceLost;

		/// <summary>
		/// Dictionary of id->Picture
		/// </summary>
		private static readonly IDictionary dic = new Hashtable();

			// prohibit instance creation
		private PictureManager() {}

		static PictureManager() {
			onSurfaceLost += new EventHandler(_onSurfaceLost);
			World.onNewWorld += new EventHandler(reset);
		}


		/// <summary>
		/// Get the picture with a given id, or throw an exception.
		/// </summary>
		/// <returns>
		///   Always return a non-null valid object.
		/// </returns>
		public static Picture get( string id ) {
			Picture pic = (Picture)dic[id];
			if( pic == null )
				throw new GraphicsException("unable to find picture of "+id);
			return pic;
		}

		/// <summary>
		/// Checks if a picture of the specified ID is already registered.
		/// </summary>
		public static bool contains( string id ) {
			return dic[id]!=null;
		}

		/// <summary>
		/// Add a new picture.
		/// </summary>
		public static void add( Picture pic ) {
			if( dic[pic.id]!=null )
				throw new GraphicsException("picture "+pic.id+" is already registered");
			dic.Add(pic.id,pic);
		}

		/// <summary>
		/// Called by Clock at sunrise and sunset.
		/// 
		/// invalidates all the surfaces so that they will be reloaded.
		/// Since this is a static method, it cannot be registered as an ordinary clock handler.
		/// </summary>
		public static void reset() {
			foreach( Picture pic in dic.Values )
				pic.setDirty();
		}

		private static void reset( object sender, EventArgs e ) {
			reset();
		}
		

		/// <summary>
		/// Called when DirectDraw surfaces are lost. This method releases the pictures.
		/// </summary>
		private static void _onSurfaceLost( object sender, EventArgs e ) {
			Debug.WriteLine("DirectDraw surfaces are lost");
			foreach( Picture pic in dic.Values )
				pic.release();
		}

		// TODO: priodical surface eviction
	}
}
