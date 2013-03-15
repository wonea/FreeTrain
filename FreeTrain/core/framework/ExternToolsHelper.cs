using System;
using System.Collections;
using System.IO;
using freetrain.framework.plugin;
using freetrain.world;

namespace freetrain.framework
{
	/// <summary>
	/// SilentInitializer の概要の説明です。
	/// </summary>
	public class ExternToolsHelper
	{
		private static bool initialized;
		
		public static void InitializePlugins(string[] plugindirs, ProgressHandler progressHandler, PluginErrorHandler errorHandler)
		{
			if(initialized)
				Clear();
			if(MainWindow.mainWindow==null)
				MainWindow.mainWindow = new MainWindow(plugindirs,false);
			if(progressHandler==null)
				progressHandler = new ProgressHandler(SilentProgress);

			IList r = new ArrayList();
			string baseDir = PluginManager.getDefaultPluginDirectory();
			foreach( string subdir in Directory.GetDirectories(baseDir) )
				r.Add(Path.Combine(baseDir,subdir));
			// load plug-ins
			Core.plugins.init( r, progressHandler, errorHandler);
			if(World.world == null ) 
				World.world = new World(new Distance(5,5,5),0);
			initialized = true;
		}

		public static void Clear(){
			// TODO:
			//Core.plugins.
			initialized = false;
		}

		static void SilentProgress(string msg, float progress){
		}
	}
}
