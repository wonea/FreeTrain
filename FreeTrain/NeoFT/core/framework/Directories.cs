using System;
using System.IO;
using System.Collections;
using System.Reflection;
using nft.util;

namespace nft.framework
{
	/// <summary>
	/// Environment の概要の説明です。
	/// </summary>
	public class Directories
	{
		
		private Directories()
		{
		}

		static private string baseDir;
		static private string resDir;
		static private string pluginDir;
		static private string serviceDir;
		static private string dataDir;
		static private string settingDir;
		static private string defaultSaveDir;
		static private string workDir1;
		static private string workDir2;
		static private string curGameDir;

		static public void Initialize(Hashtable args)
		{
			// set application program existing path

			string current = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			baseDir = getPath(args,"-B","-BASEDIR", current);
			//baseDir = Assembly.GetAssembly(Type.GetType("nft.framework.Directories")).Location;

			serviceDir = baseDir+@"extservices\";
			GetOrCreate(serviceDir,false);
			resDir = baseDir+@"res\";
			GetOrCreate(resDir,false);			
			dataDir = baseDir+@"data\";
			curGameDir = dataDir;
			GetOrCreate(dataDir,false);
			defaultSaveDir = dataDir+@"savedata\";
			GetOrCreate(defaultSaveDir,false);
			workDir1 = baseDir+@"work\";
			workDir2 = workDir1;
			GetOrCreate(workDir2,false);
			settingDir = getPath(args,"-S","-SETTINGDIR", dataDir+@"settings\");
			pluginDir = getPath(args,"-P","-PLUGINDIR",baseDir+@"plugins\");
		}

		static private string getPath(Hashtable args, string key1, string key2, string defaultPath)
		{
			string path="";
			try
			{
				if( args.ContainsKey(key1) )
				{
					path = args[key1].ToString();
					return GetOrCreate(path,false).FullName;
				}
				else if( args.ContainsKey(key2) )
				{
					path = args[key2].ToString();
					return GetOrCreate(path,false).FullName;
				}
			}
			catch(Exception e)
			{
				string templ = Core.resources["directories.errormsg_on_dir_creation"].stringValue;
				UIUtil.ShowException(string.Format(templ,path),e,UIInformLevel.normal);
			}
			return defaultPath;
		}

		/// <summary>
		/// returns specified DirectoryInfo, createing if not exist.
		/// (when prompt=true, and if user doesn't allow create directory, returns null.)
		/// </summary>
		/// <param name="dir">full path for the directory</param>
		/// <param name="prompt">show prompt before creating directory</param>
		/// <returns></returns>
		static public DirectoryInfo GetOrCreate(string dir, bool prompt)
		{
			if(!Directory.Exists(dir))
			{
				if(prompt)
				{
					string templ = Core.resources["directories.create_dir_prompt"].stringValue;
					if(!UIUtil.YesNoMessage(string.Format(templ,dir)))
						return null;
				}
				Directory.CreateDirectory(dir);
			}
			return new DirectoryInfo(dir);
		}

		static public string AppBaseDir
		{
			get{ return baseDir; }
		}
		static public string SystemResourceDir
		{
			get{ return resDir; }
		}
		static public string PluginDir
		{
			get{ return pluginDir;	}
		}
		static public string ExternServicesDir
		{
			get{ return serviceDir;	}
		}
		static public string DataDir
		{
			get{ return dataDir;	}
		}
		static public string DefaultSaveDir
		{
			get{ return defaultSaveDir;	}
			set{ defaultSaveDir = value; }
		}
		static public string SettingDir
		{
			get{ return settingDir;	}
			set{ settingDir = value; }
		}
		static public string WorkDirPrimary
		{
			get{ return workDir1;	}
			set{ workDir1 = value; }
		}
		static public string WorkDirSecondary
		{
			get{ return workDir2;	}
			set{ workDir2 = value; }
		}
		static public string CurrentGameDir
		{
			get{ return curGameDir;	}
			set{ curGameDir = value; }
		}		
	}
}
