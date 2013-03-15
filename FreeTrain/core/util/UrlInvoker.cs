using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace freetrain.util
{
	/// <summary>
	/// UrlInvoker の概要の説明です。
	/// (summary for UrlInvoker)
	/// </summary>
	public class UrlInvoker
	{
		// 指定のURLを標準ブラウザで開く
		// Open the URL with the default browser
		static public void openUrl(String targetUrl) {
			ProcessStartInfo info = new ProcessStartInfo();
			// URLに関連づけられたアプリケーションを探す
			// Look for the application that is bound to the URL by default
			RegistryKey rkey = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command");
			String val = rkey.GetValue("").ToString();
			// レジストリ値には、起動パラメータも含まれるので、
			// 実行ファイル名と起動パラメータを分離する
			// The registry value contains run parameters, so detach
			// executable file name and those parameters
			if(val.StartsWith("\"")) {
				int n = val.IndexOf("\"",1);
				info.FileName = val.Substring(1,n-1);
				info.Arguments = val.Substring(n+1);
			}
			else {
				string[] a = val.Split(new char[]{' '});
				info.FileName = a[0];
				info.Arguments = val.Substring(a[0].Length+1);
			}
			// 作業ディレクトリも指定しないとダメなようだ・・・
			// we also need working directory...
			info.WorkingDirectory = Path.GetDirectoryName(info.FileName);
			// 引数の最後にURLを加える
			// add the URL at the end of the parameters
			info.Arguments += targetUrl;
			Process.Start(info);
		}
	
	}
}
