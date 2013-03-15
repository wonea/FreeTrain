using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using freetrain.util;
using org.kohsuke.directdraw;

namespace freetrain.framework
{
	/// <summary>
	/// Global Configuration.
	/// 
	/// This is an application-wide configuration, which will be used across
	/// all the games.
	/// 
	/// Use freetrain.framework.Core.options to access the instance.
	/// </summary>
	[XmlTypeAttribute(Namespace="http://www.kohsuke.org/freetrain/globalConfig")]
	[XmlRootAttribute(Namespace="http://www.kohsuke.org/freetrain/globalConfig", IsNullable=false)]
	public class GlobalOptions : PersistentOptions
	{
		public GlobalOptions() {}

		/// <summary>
		/// If true, show a message box for errors.
		/// If false, show a message into the status bar.
		/// </summary>
		public bool showErrorMessageBox = false;

		public DDSurfaceAllocation surfaceAlloc = DirectDraw.SurfeceAllocation;
		public DDSurfaceAllocation SurfaceAlloc
		{
			get{ return surfaceAlloc; }
			set{ DirectDraw.SurfeceAllocation = value; 
				 surfaceAlloc = value; }
		}
		
		public double[] devParams = new double[11];

		/// <summary>
		/// Length of the time (in seconds) 
		/// while a message is displayed.
		/// </summary>
		public int messageDisplayTime = 3;

		public bool enableSoundEffect = true;

		/// <summary>
		/// Debug option to draw the bounding box
		/// </summary>
		public bool drawBoundingBox = false;

		private bool _drawStationNames = true;

		public bool drawStationNames {
			get {
				return _drawStationNames;
			}
			set {
				if( _drawStationNames!=value && world.World.world!=null )
					world.World.world.onAllVoxelUpdated();	// redraw
				_drawStationNames = value;
			}
		}
		
		/// <summary>
		/// If false, draw trees.
		/// If true, speed up drawing by ignore drawing trees.
		/// </summary>
		private bool _hideTrees = false;

		public bool hideTrees 
		{
			get 
			{
				return _hideTrees;
			}
			set 
			{
				if( _hideTrees!=value && world.World.world!=null )
					world.World.world.onAllVoxelUpdated();	// redraw
				_hideTrees = value;
			}
		}

		public new GlobalOptions load() 
		{
			GlobalOptions opt = (GlobalOptions)base.load();
			DirectDraw.SurfeceAllocation = opt.SurfaceAlloc; 
			return opt;
		}

		// Maintain backward-compatibility
		protected override string Stem { get { return ""; } }
	}
}
