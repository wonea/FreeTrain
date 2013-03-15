using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace nft.win32util
{
	/// <summary>
	/// ConfigureService の概要の説明です。
	/// </summary>
	public class Win32Version
	{
		[DllImport("version.dll")]
		static private extern bool GetFileVersionInfo (string sFileName,
			int handle, int size, byte[] infoBuffer);
		[DllImport("version.dll")]
		static private extern int GetFileVersionInfoSize (string sFileName,
			out int handle);
		// The third parameter - "out string pValue" - is automatically
		// marshaled from ANSI to Unicode:
		[DllImport("version.dll")]
		unsafe static private extern bool VerQueryValue (byte[] pBlock,
			string pSubBlock, out string pValue, out uint len);
		// This VerQueryValue overload is marked with 'unsafe' because 
		// it uses a short*:
		[DllImport("version.dll")]
		unsafe static private extern bool VerQueryValue (byte[] pBlock,
			string pSubBlock, out short *pValue, out uint len);

		private Win32Version()
		{
		}

		unsafe static public string GetAssemblyFileVersion(Assembly target)
		{
			string fname = (new Uri(target.CodeBase)).LocalPath;
			int handle = 0;
			// Figure out how much version info there is:
			int size =
				GetFileVersionInfoSize(fname, out handle);
 
			if (size == 0) return "N/A";
 
			byte[] buffer = new byte[size];
 
			if (!GetFileVersionInfo(fname, handle, size, buffer))
			{
				Debug.WriteLine("Failed to query file version information.");
				return "N/A";
			}
			short *subBlock = null;
			uint len = 0;
			// Get the locale info from the version info:
			if (!VerQueryValue (buffer, @"\VarFileInfo\Translation", out subBlock, out len))
			{
				Debug.WriteLine("Failed to query version information.");
				return "N/A";
			}
 
			string spv = @"\StringFileInfo\" + subBlock[0].ToString("X4") + subBlock[1].ToString("X4") + @"\ProductVersion";
 
			byte *pVersion = null;
			// Get the ProductVersion value for this program:
			string versionInfo;

			if (!VerQueryValue (buffer, spv, out versionInfo, out len))
			{
				Debug.WriteLine("Failed to query version information.");
				return "N/A";
			}
			return versionInfo;
		}
	}
}
