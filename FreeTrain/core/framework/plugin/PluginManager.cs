using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using freetrain.util;
using freetrain.contributions.sound;
using freetrain.contributions.others;
using freetrain.contributions.rail;
using freetrain.contributions.land;
using freetrain.contributions.train;
using freetrain.contributions.common;
using freetrain.contributions.structs;
using freetrain.contributions.road;

namespace freetrain.framework.plugin
{
	public interface PluginErrorHandler {
		bool OnNameDuplicated(Plugin p_1st, Plugin p_2nd, Exception e);
		bool OnPluginLoadError(Plugin p, Exception e);
		bool OnContributionInitError(Contribution c, Exception e);
		bool OnContribIDDuplicated(Contribution c_1st, Contribution c_2nd, Exception e);
		bool OnFinal(IDictionary errorPlugins, int totalErrorCount);
	}

	public class SilentPluginErrorHandler : PluginErrorHandler {
		#region PluginErrorHandler o
		public bool OnNameDuplicated(Plugin p_1st, Plugin p_2nd, Exception e) {
			return false;
		}

		public bool OnPluginLoadError(Plugin p, Exception e) {
			return false;
		}

		public bool OnContributionInitError(Contribution c, Exception e) {
			return false;
		}

		public bool OnContribIDDuplicated(Contribution c_1st, Contribution c_2nd, Exception e) {
			return false;
		}
		
		public bool OnFinal(IDictionary errorPlugins, int totalErrorCount){
			return true;
		}
		#endregion

	}


	/// <summary>
	/// Loads plug-ins.
	/// </summary>
	public class PluginManager
	{
		
		/// <summary> The singleton instance. </summary>
		public static PluginManager theInstance;

		/// <summary>
		/// All loaded plug-ins.
		/// </summary>
		private Plugin[] plugins;

		/// <summary>
		/// Plugins keyed by their names.
		/// </summary>
		private readonly IDictionary pluginMap = new Hashtable();
		
		/// <summary>
		/// Contribution factories that are used to load contributions.
		/// </summary>
		private readonly IDictionary contributionFactories = new Hashtable();

		/// <summary>
		/// Contributions keyed by their IDs.
		/// </summary>
		private readonly IDictionary contributionMap = new Hashtable();



		public PluginManager() {
			theInstance = this;

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(onAssemblyResolve);
		}

		private static string getModuleName( string name ) {
			return name.Substring(0,name.IndexOf(','));
		}

		private static Assembly onAssemblyResolve( object sender, ResolveEventArgs args) {
			// TODO: improve performance by having a dictionary from name to Assemblies.
			// TODO: what is the correct way to use an application specific logic to resolve assemblies
			Trace.WriteLine("onAssemblyResolve resolving "+args.Name);
			
			string name = getModuleName(args.Name);

			if( Core.plugins.plugins==null )	return null;

			// try assemblies of plug-ins
			foreach( Contribution cont in Core.plugins.publicContributions ) {
				Assembly asm = cont.assembly;

				if(getModuleName(asm.FullName)==name)
					return asm;
			}
			
			Trace.WriteLine("onAssemblyResolve failed");
			return null;
		}

		/// <summary>
		/// This method should be called after the object is created.
		/// </summary>
		/// <param name="dirs">
		/// collection of strings (directory names)
		/// for each directory in this collection, its sub-directories
		/// are scanned for plugin.xml
		/// </param>
		public void init( ICollection dirs, ProgressHandler progressHandler, PluginErrorHandler errorHandler ) {

			Set pluginSet = new Set();
			Hashtable errorPlugins = new Hashtable();
			int errCount = 0;
			int count = 0;
			float c_max = dirs.Count*4;
			bool errBreak = false;
			if(errorHandler==null)
				errorHandler = new SilentPluginErrorHandler();

			// locate plugins
			foreach( string dir in dirs ) {
				progressHandler("Searching for plugins...\n"+Path.GetFileName(dir),++count/c_max);
				//! progressHandler("プラグインを検索中\n"+Path.GetFileName(dir),++count/c_max);
				
				if( !File.Exists(Path.Combine(dir,"plugin.xml")) )
					continue;	// this directory doesn't have the plugin.xml file.
				Plugin p = null;
				try {
					p = new Plugin(dir);
					p.loadContributionFactories();
				} catch( Exception e ) {
					errCount++;
					p = Plugin.loadFailSafe(dir);
					errorPlugins.Add(p,e);
					errBreak = errorHandler.OnPluginLoadError(p,e);
					if(errBreak)
						break;
					else
						continue;
				}
				if( pluginMap.Contains(p.name) ) {
					errCount++;
					// loaded more than once
					Exception e = new Exception( string.Format(
						"Plugin \"{0}\" is loaded from more than one place ({1} and {2})",
						//! "プラグイン「{0}」は{1}と{2}の二箇所からロードされています",
						p.name, p.dirName, ((Plugin)pluginMap[p.name]).dirName) );
					errBreak = errorHandler.OnNameDuplicated(pluginMap[p.name] as Plugin,p,e);
					errorPlugins.Add(p,e);
					if(errBreak)
						break;
					else
						continue;
				}
				pluginMap.Add( p.name, p );
				pluginSet.add( p );
			}
			if(errBreak)
				Environment.Exit(-1);
			
			{// convert it to an array by sorting them in the order of dependency
				this.plugins = new Plugin[pluginSet.count];
				int ptr=0;
				Plugin p = null;
				while( !pluginSet.isEmpty ) {
					progressHandler("Sorting dependencies...",++count/c_max);
					//! progressHandler("依存関係を整理中",++count/c_max);
					p = (Plugin)pluginSet.getOne();
					try {
						while(true) {
							Plugin[] deps = p.getDependencies();
							int i;
							for( i=0; i<deps.Length; i++ )
								if( pluginSet.contains(deps[i]) )
									break;
							if(i==deps.Length)
								break;
							else
								p = deps[i];
						}
					} catch( Exception e ) {
						errCount++;
						errBreak = errorHandler.OnPluginLoadError(p,e);
						if(!errorPlugins.ContainsKey(p))
							errorPlugins.Add(p,e);
						if(errBreak)
							break;
					}
					pluginSet.remove(p);
					plugins[ptr++] = p;
				}
			}
			if(errBreak)
				Environment.Exit(-2);

			//	 load all the contributions			
			foreach( Plugin p in plugins ) {
				progressHandler("Loading contributions...\n"+Path.GetFileName(p.dirName),++count/c_max);
				//! progressHandler("コントリビューションをロード中\n"+Path.GetFileName(p.dirName),++count/c_max);
				try {
					p.loadContributions();
				} catch( Exception e ) {
					errCount++;
					errBreak = errorHandler.OnPluginLoadError(p,e);
					if(!errorPlugins.ContainsKey(p))
						errorPlugins.Add(p,e);
					if(errBreak)
						break;
				}
			}
			if(errBreak)
				Environment.Exit(-3);

			// initialize contributions
			count = (int)c_max;
			c_max += publicContributions.Length;			
			foreach( Contribution contrib in publicContributions ) {
				progressHandler("Initializing contributions...\n"+contrib.baseUri,++count/c_max);
				//! progressHandler("コントリビューションを初期化中\n"+contrib.baseUri,++count/c_max);
				try {
					contrib.onInitComplete();
				} catch( Exception e ) {
					errCount++;
					errBreak = errorHandler.OnContributionInitError(contrib, e);
					Plugin p = contrib.parent;
					if(!errorPlugins.ContainsKey(p))
						errorPlugins.Add(p,e);
					if(errBreak)
						break;
				}
			}
			if(errBreak)
				Environment.Exit(-4);

			{// make sure there's no duplicate id
				progressHandler("Checking for duplicate IDs...",1.0f);
				//! progressHandler("重複IDのチェック中",1.0f);
				IDictionary dic = new Hashtable();
				foreach( Contribution contrib in publicContributions ) {
					if( dic[contrib.id]!=null ) {
						errCount++;
						Exception e = new FormatException("ID:"+contrib.id+" is not unique");
						//! Exception e = new FormatException("ID:"+contrib.id+"が一意ではありません");
						errBreak = errorHandler.OnContribIDDuplicated(dic[contrib.id] as Contribution, contrib, e);
						Plugin p = contrib.parent;
						if(!errorPlugins.ContainsKey(p))
							errorPlugins.Add(p,e);
						if(errBreak)
							break;							
					} else
						dic[contrib.id] = contrib;
				}
			}			
			if(errBreak)
				Environment.Exit(-5);
			if(errCount>0){
				if(errorHandler.OnFinal(errorPlugins,errCount))
					Environment.Exit(errCount);
			}
		}


		/// <summary>
		/// Gets the default plug-in directory.
		/// </summary>
		/// <returns></returns>
		public static string getDefaultPluginDirectory() {
			// try the IDE directory first
			string pluginDir = Path.GetFullPath(Path.Combine(
				Core.installationDirectory, @"..\..\plugins" ));
			if(Directory.Exists(pluginDir))
				return pluginDir;

			// if we can't find it, try the directory under the executable directory
			pluginDir = Path.GetFullPath(Path.Combine(
				Core.installationDirectory, @"plugins" ));
			if(Directory.Exists(pluginDir))
				return pluginDir;
				
			throw new IOException("unable to find the plug-in directory: "+pluginDir);
		}

		/// <summary>
		/// Registers a <c>ContributionFactory</c>.
		/// This method has to be called before the initialization.
		/// Normally, this method is called by <c>Plugin</c> but the caller
		/// can invoke this method before calling the init method.
		/// </summary>
		public void addContributionFactory( string name, ContributionFactory factory ) {
			if( contributionFactories.Contains(name) )
				throw new Exception(string.Format(
					"contribution type \"{0}\" is already registered.",name));

			contributionFactories.Add(name,factory);
		}

		public ContributionFactory getContributionFactory( string name ) {
			ContributionFactory factory = (ContributionFactory)
				contributionFactories[name];

			if(factory==null)
				throw new Exception(name+" is an unknown contribution");
				//! throw new Exception(name+"は未知のコントリビューションです");
			else
				return factory;
		}


		/// <summary>
		/// Enumerates all plug-in objects.
		/// </summary>
		public IEnumerator GetEnumerator() {
			return new ArrayEnumerator(plugins);
		}





		/// <summary>
		/// Gets all the station contributions.
		/// </summary>
		public StationContribution[] stations {
			get {
				return (StationContribution[])listContributions(typeof(StationContribution));
			}
		}
		public readonly StructureGroupGroup stationGroup = new StructureGroupGroup();

		/// <summary>
		/// Gets all the special rail contributions.
		/// </summary>
		public SpecialRailContribution[] specialRails {
			get {
				return (SpecialRailContribution[])listContributions(typeof(SpecialRailContribution));
			}
		}

		/// <summary>
		/// Gets all the rail stationary contributions
		/// </summary>
		public RailStationaryContribution[] railStationaryStructures {
			get {
				return (RailStationaryContribution[])listContributions(typeof(RailStationaryContribution));
			}
		}
		public readonly StructureGroupGroup railStationaryGroup = new StructureGroupGroup();

		/// <summary>
		/// Gets all the commercial structure contributions.
		/// </summary>
		public CommercialStructureContribution[] commercialStructures {
			get {
				return (CommercialStructureContribution[])listContributions(typeof(CommercialStructureContribution));
			}
		}
		public readonly StructureGroupGroup commercialStructureGroup = new StructureGroupGroup();

		/// <summary>
		/// Gets all the special structure contributions.
		/// </summary>
		public SpecialStructureContribution[] specialStructures {
			get {
				return (SpecialStructureContribution[])listContributions(typeof(SpecialStructureContribution));
			}
		}

		/// <summary>
		/// Gets all the road contributions.
		/// </summary>
		public RoadContribution[] roads {
			get {
				return (RoadContribution[])listContributions(typeof(RoadContribution));
			}
		}


		/// <summary>
		/// Gets all the BGM contributions.
		/// </summary>
		public BGMContribution[] bgms {
			get {
				return (BGMContribution[])listContributions(typeof(BGMContribution));
			}
		}

		/// <summary>
		/// Gets all the menu item contributions.
		/// </summary>
		public MenuContribution[] menus {
			get {
				return (MenuContribution[])listContributions(typeof(MenuContribution));
			}
		}

		/// <summary>
		/// Gets all the train contributions.
		/// </summary>
		public TrainContribution[] trains {
			get {
				return (TrainContribution[])listContributions(typeof(TrainContribution));
			}
		}

		/// <summary>
		/// Gets all the train controller contributions.
		/// </summary>
		public TrainControllerContribution[] trainControllers {
			get {
				return (TrainControllerContribution[])listContributions(typeof(TrainControllerContribution));
			}
		}

		public VarHeightBuildingContribution[] varHeightBuildings {
			get {
				return (VarHeightBuildingContribution[])listContributions(typeof(VarHeightBuildingContribution));
			}
		}
		public readonly StructureGroupGroup varHeightBuildingsGroup = new StructureGroupGroup();



		public LandBuilderContribution[] landBuilders {
			get {
				return (LandBuilderContribution[])listContributions(typeof(LandBuilderContribution));
			}
		}
		public readonly LandBuilderGroupGroup landBuilderGroup = new LandBuilderGroupGroup();



		/// <summary>
		/// Lists up contributions of the given type.
		/// </summary>
		public Array listContributions( Type contributionType ) {
			ArrayList list = new ArrayList();
			foreach( Plugin p in plugins ) {
				foreach( Contribution contrib in p.contributions ) {
					if( contributionType.IsInstanceOfType(contrib) )
						list.Add(contrib);
				}
			}

			return list.ToArray(contributionType);
		}


		/// <summary>
		/// Gets all contributions. except for runtime generated ones.
		/// </summary>
		public Contribution[] publicContributions {
			get {
				ArrayList list = new ArrayList();
				foreach( Plugin p in plugins )
					foreach( Contribution contrib in p.contributions )
						list.Add(contrib);

				return (Contribution[])list.ToArray(typeof(Contribution));
			}
		}

		/// <summary>
		/// Gets all contributions including runtime generat.
		/// </summary>
		public Contribution[] allContributions {
			get {
				ArrayList list = new ArrayList();
				foreach( Contribution contrib in contributionMap.Values )
					list.Add(contrib);
				return (Contribution[])list.ToArray(typeof(Contribution));
			}
		}

		public void addContribution( Contribution contrib ) {
			contributionMap.Add( contrib.id, contrib );
		}

		/// <summary>
		/// Gets the contribution with a given ID, or null if not found.
		/// </summary>
		public Contribution getContribution( string id ) {
			return (Contribution)contributionMap[id];
		}

		/// <summary>
		/// Get the plug-in of the specified name, or null if not found.
		/// </summary>
		public Plugin getPlugin( string name ) {
			return (Plugin)pluginMap[name];
		}
	}

}
