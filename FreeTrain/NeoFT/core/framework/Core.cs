using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
//using Microsoft.Win32;
using nft.util;
using nft.ui;
using nft.ui.mainframe;
using nft.framework.plugin;

namespace nft.framework
{

	/// <summary>
	/// Entry point to other static instances in the NeoFreeTrain framework.
	/// </summary>
	public sealed class Core
	{
		// no instantiation
		private Core() {}		

		/// <summary> the main window frame </summary>
		private static MainFrame theFrame;
		public static IBarHostFrame mainFrame
		{	get{ return theFrame; } 	}

		/// <summary> Core resources. </summary>
		public static readonly Properties resources = Properties.LoadFromFile(Directories.AppBaseDir+"nftfw.resource.xml",false);

		/// <summary> Service modules. </summary>
		public static readonly ServiceManager services = new ServiceManager();

		/// <summary> Plug-ins. </summary>
		public static readonly PluginManager plugins = new PluginManager();

		#region sound and BGM
/*		
		/// <summary>
		/// Handles BGM playback.
		/// Should be instanciated by attaching the main window.
		/// </summary>
		public static BGMManager bgmManager { get { return _bgmManager; } }

		/// <summary>
		/// Handles SFX.
		/// Should be instanciated by attaching the main window.
		/// </summary>
		public static SoundEffectManager soundEffectManager { get { return _soundEffectManager; } }
		

		private static SoundEffectManager _soundEffectManager;
		private static BGMManager _bgmManager;
*/
		#endregion

		/// <summary>
		/// Initializes the framework.
		/// Should be called once and only once.
		/// </summary>
		/// <param name="additionalPluginDirs">
		/// additional directories from which plug-ins are loaded.
		/// </param>
		/// <param name="owner">application's main window.</param>
		/// <param name="bgmMenuItem">"BGM" sub-menu</param>
		/// <param name="progressHandler">
		/// Receives initializtion progress report. Can be null.
		/// </param>
		public static void init( string[] args, MainFrame frame ) 
		{
			
			Hashtable h_args = paresArgs(args);
			Directories.Initialize(h_args);

			//InformationService service = new InformationService();
			theFrame = frame;
			// To avoid ReBar trouble, we need to show MainFrame
			// before Splash window created.
			
			using(Splash s = new Splash()) 
			{
				s.Show();
				ProgressMonitor monitor = new ProgressMonitor(2);
				monitor.SetMaximum(1,5);
				monitor.OnProgress += new ProgressHandler(s.updateMessage);
				Application.DoEvents();
				// load plug-ins
				Core.plugins.init(new string[]{Directories.PluginDir}, monitor);
			}

			//new PluginListDialog().ShowDialog();

			
//			_soundEffectManager = new SoundEffectManager(owner);
//			_bgmManager = new BGMManager(bgmMenuItem);
		}

		private static Hashtable paresArgs(string[] args)
		{
			Hashtable table = new Hashtable();
			for(int i=0;i<args.Length;i++)
			{
				string[] v = args[i].Split(new char[]{'='},2);
				if(v.Length==2)
					table.Add(v[0].ToUpper(),v[1]);
				else
					table.Add(v[0].ToUpper(),string.Empty);
			}
			return table;
		}
		
	}
}
