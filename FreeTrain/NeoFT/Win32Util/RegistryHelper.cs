using System;
using Microsoft.Win32;

namespace nft.win32util
{
	/// <summary>
	/// RegistryHelper の概要の説明です。
	/// </summary>
	public class RegistryHelper
	{
		private RegistryHelper()
		{
		}

		// Read data string of specified key and value from system registry.
		public static string ReadKey(string key,string valname)
		{
			int n = key.IndexOf('\\');
			string baseKey = key.Substring(5,n-5);
			string subKey = key.Substring(n+1);
			RegistryKey rKey;
			switch(baseKey)
			{
				case "CLASSES_ROOT":
					rKey = Registry.ClassesRoot;
					break;
				case "LOCAL_MACHINE":
					rKey = Registry.LocalMachine;
					break;
				case "CURRENT_USER":
					rKey = Registry.CurrentUser;
					break;
				case "CURRENT_CONFIG":
					rKey = Registry.CurrentConfig;
					break;
				default:
					return null;
			}
			if(valname==null)
				valname="";
			object o = rKey.OpenSubKey(subKey,false).GetValue(valname);
			if(o==null)
				return null;
			else
				return o.ToString();
		}
	}
}
