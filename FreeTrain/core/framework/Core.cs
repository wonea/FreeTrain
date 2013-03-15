using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using freetrain.framework.plugin;
using freetrain.framework.sound;
using freetrain.util;

namespace freetrain.framework
{
	/// <summary>
	/// Entry point to other static instances in the FreeTrain framework.
	/// </summary>
	public sealed class Core
	{
		private Core() {}		// no instantiation


		/// <summary>
		/// Registry key where the per-user application setting
		/// should be stored.
		/// </summary>
		public static RegistryKey userRegistry {
			get {
				return Registry.CurrentUser.CreateSubKey(@"Software\FreeTrain");
			}
		}

		/// <summary>
		/// Installation directory of the FreeTrain framework.
		/// </summary>
		public static string installationDirectory {
			get {
				return (string)userRegistry.GetValue("installationDirectory");
			}
			set {
				userRegistry.SetValue("installationDirectory",value);
			}
		}


		/// <summary> Plug-ins. </summary>
		public static readonly PluginManager plugins = new PluginManager();

		/// <summary> Global options. </summary>
		public static readonly GlobalOptions options = new GlobalOptions().load();
		
		/// <summary> Game mode </summary>
		public static bool isConstructionMode {
			get {
				return _isConstructionMode;
			}
		}

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
		private static bool _isConstructionMode;



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
		public static void init( string[] args, Control owner, MenuItem bgmMenuItem,ProgressHandler progressHandler, bool constructionMode ) {
			
			_isConstructionMode = constructionMode;

			if(progressHandler==null)
				progressHandler = new ProgressHandler(silentProgressHandler);

			_soundEffectManager = new SoundEffectManager(owner);
		
			// load plug-ins
			Core.plugins.init(
				args.Length==0?getDefaultProfile():parseProfile(args[0]),
				progressHandler, new DefaultPluginErrorHandler(5));

			_bgmManager = new BGMManager(bgmMenuItem);
		}

		private static void silentProgressHandler( string msg, float progress ) {
			Trace.WriteLine(msg);
		}
		
		/// <summary>
		/// Determines the plug-ins to be used from a profile.
		/// </summary>
		private static IList parseProfile( string searchPath ) {
			// TODO: better error check
			Set r = new Set();
			string[] basedirs = searchPath.Split(';');
			for(int i=0; i<basedirs.Length; i++){
				if(!File.Exists(basedirs[i])) 
					continue;
				string[] subdirs = Directory.GetDirectories(basedirs[i]);
			}

			return new ArrayList(r);
		}

		/// <summary>
		/// Determines the default set of plug-ins. Used when no profile is specified.
		/// </summary>
		/// <returns></returns>
		private static IList getDefaultProfile() {
			IList r = new ArrayList();
			
			string baseDir = PluginManager.getDefaultPluginDirectory();
			foreach( string subdir in Directory.GetDirectories(baseDir) )
				r.Add(Path.Combine(baseDir,subdir));
			
			return r;
		}
	}

	/// <summary>
	/// Function that receives a progress in initialization.
	/// </summary>
	public delegate void ProgressHandler( string msg, float progress );

	public class DefaultPluginErrorHandler : PluginErrorHandler {
		int errCount;
		readonly int errMax;
		
		public DefaultPluginErrorHandler(int errMax){
			errCount = 0;
			this.errMax = errMax;
		}

		#region PluginErrorHandler Member
		public bool OnNameDuplicated(Plugin p_1st, Plugin p_2nd, Exception e) {
			if( errCount++ < errMax ){
				MessageBox.Show( makeErrorMessage(p_2nd,e),
					"Plugin "+p_2nd.dirName+" is loaded in more than one instance");
					//! "プラグイン"+p_2nd.dirName+"が二重に読み込まれました");
				return false;
			}else{
				MessageBox.Show("There are too many plugin loading errors");
				//! MessageBox.Show("プラグインのロードエラーが多すぎます");
				return true;
			}
		}

		public bool OnPluginLoadError(Plugin p, Exception e) {
			if( errCount++ < errMax ){
				MessageBox.Show( makeErrorMessage(p,e),
					"Plugin "+p.dirName+" can not be loaded");
					//! "プラグイン"+p.dirName+"がロードできません");
				return false;
			}else{
				MessageBox.Show("There are too many plugin loading errors");
				//! MessageBox.Show("プラグインのロードエラーが多すぎます");
				return true;
			}
		}

		public bool OnContributionInitError(Contribution c, Exception e) {
			if( errCount++ < errMax ){
				MessageBox.Show( makeErrorMessage(c,e), 
					"Contribution contained in plugin "+c.parent.dirName+" can not be initialized");
					//! "プラグイン"+c.parent.dirName+"でコントリビューションの初期化に失敗しました");
				return false;
			}else{
				MessageBox.Show("There are too many plugin loading errors");
				//! MessageBox.Show("プラグインのロードエラーが多すぎます");
				return true;
			}
		}

		public bool OnContribIDDuplicated(Contribution c_1st, Contribution c_2nd, Exception e) {
			if( errCount++ < errMax ){
				MessageBox.Show( makeErrorMessage(c_2nd,e), 
					"Contribution ID="+c_2nd.id+" is defined in more than instance");
					//! "コントリビューション ID="+c_2nd.id+" が重複定義されています");
				return false;
			}else{
				MessageBox.Show("There are too many plugin loading errors");
				//! MessageBox.Show("プラグインのロードエラーが多すぎます");
				return true;
			}
		}

		public bool OnFinal(IDictionary errorPlugins, int totalErrorCount){
			DialogResult res = MessageBox.Show(MainWindow.mainWindow, @"Some plugins could not be loaded.
Some functions may not be available and you might encounter errors.
In the worst case, game data might be lost, so it is advised that you quit now.

Do you want to quit FreeTrain now?

* Contact plugin authors in the first place to resolve errors.",
//! 			DialogResult res = MessageBox.Show(MainWindow.mainWindow, @"正常に読み込めなかったプラグインがあります。
//! 一部の機能が使えなかったり、プレイ中にエラーが発生するおそれがあります。
//! 最悪の場合ゲームデータを破壊する可能性もありますので、このまま終了することをお奨めします。
//! 
//! FreeTrainExを終了してよろしいですか？
//! 
//! ※なお、エラーの対処方法については、まずプラグイン作者にお問い合わせください。",

				string.Format("There were {0} plugin loading errors.",errCount)
				//! string.Format("プラグインのロード中に {0}個のエラーが発生しました。",errCount)
				,MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
			return res==DialogResult.Yes;
		}
		#endregion

		private static string makeErrorMessage(Plugin p, Exception e){			
			return string.Format("Title: {0}\tAuthor: {1}\n\n{2}",p.title,p.author,e.ToString());
			//! return string.Format("タイトル： {0}\t作者： {1}\n\n{2}",p.title,p.author,e.ToString());
		}

		private static string makeErrorMessage(Contribution c, Exception e){
			Plugin p = c.parent;
			return string.Format("Title: {0}\tAuthor: {1}\nID={2}\n\n{3}",p.title,p.author,c.id,e.ToString());
			//! return string.Format("タイトル： {0}\t作者： {1}\nID={2}\n\n{3}",p.title,p.author,c.id,e.ToString());
		}
	}
}
