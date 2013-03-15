using System;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.framework.plugin;

namespace nft.framework.graphics
{
	/// <summary>
	/// Wraps DirectDraw surface
	/// </summary>
	[Serializable]
	public class Picture : ISerializable
	{
		public enum BaseDir{ Absolute, SysResouce, Plugin, PrimaryWorkDir, SecondaryWorkDir, CurrentGameDir };

		/// <summary>
		/// DirectDraw instance for loading surface objects.
		/// </summary>
		public static readonly DirectDraw directDraw = new DirectDraw();
		/// <summary>
		/// DirectDraw surface.
		/// null when the surface is detached.
		/// </summary>
		internal protected Surface _surface;
		// id equals to image file path
		public readonly string id;
		public readonly BaseDir baseDir;

		// available from plugin modules
		public Picture(string filename)
		{
			baseDir = BaseDir.Absolute;
			id = filename;
		}

		// for internal use only
		internal protected Picture(string filename, BaseDir baseLocation)
		{
			baseDir = baseLocation;
			id = filename;
		}

		public string FullPath
		{
			get
			{
				if( baseDir==BaseDir.Absolute ) return id;
				else 
				{
					string bpath;
					switch(baseDir)
					{
						case BaseDir.Plugin:
							bpath = Directories.PluginDir;
							break;
						case BaseDir.SysResouce:
							bpath = Directories.SystemResourceDir;
							break;
						case BaseDir.PrimaryWorkDir:
							bpath = Directories.WorkDirPrimary;
							break;
						case BaseDir.SecondaryWorkDir:
							bpath = Directories.WorkDirSecondary;
							break;
						case BaseDir.CurrentGameDir:
							bpath = Directories.CurrentGameDir;
							break;
						default:
							bpath = Directories.AppBaseDir;
							break;
					}
					return Path.Combine(bpath,id);
				}
			}
		}

		public Surface surface 
		{
			get 
			{
				if(dirty) 
				{
					loadFile(FullPath);
					dirty = false;
				}

				return _surface;
			}
		}

		protected void loadFile(string fileName) 
		{
			using( Bitmap bmp = new Bitmap(fileName) ) 
			{
				if(_surface==null) 
				{
					_surface = directDraw.createOffscreenSurface( bmp.Size );
				}

				using( GDIGraphics g = new GDIGraphics(_surface) ) 
				{
					// without the size parameter, it doesn't work well with non-standard DPIs.
					g.graphics.DrawImage( bmp, new Rectangle( new Point(0,0), bmp.Size ) );
				}
				_surface.sourceColorKey = bmp.GetPixel(0,0);
			}
		}

		/// <summary>
		/// Dirty flag. Set true to reload the surface.
		/// </summary>
		private bool dirty;

		public void setDirty() {
			dirty = true;
		}

		/// <summary>
		/// Release any resource acquired by this picture.
		/// The picture will be automatically reloaded next time
		/// the picture is used.
		/// </summary>
		public void release() {
			if(_surface!=null) {
				_surface.Dispose();
				_surface = null;
			}
			dirty = true;
		}

		//
		// serialization
		//
		public void GetObjectData( SerializationInfo info, StreamingContext context) {
			info.SetType(typeof(ReferenceImpl));
			info.AddValue("id",id);
		}
		
		[Serializable]
		internal sealed class ReferenceImpl : IObjectReference {
			private string id=null;
			public object GetRealObject(StreamingContext context) {
				return PictureManager.get(id);
			}
		}
	}
}
