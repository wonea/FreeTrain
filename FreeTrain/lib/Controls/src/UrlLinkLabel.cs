using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace freetrain.controls
{
	/// <summary>
	/// LinkLabel control that launchs a browser and
	/// navigates to a specified URL.
	/// </summary>
	public class UrlLinkLabel : LinkLabel
	{
		private string targetUrl;

		[
			Description("URL to be opened")
		]
		public string TargetUrl {
			get { return targetUrl; }
			set { targetUrl=value; }
		}

		public UrlLinkLabel() {}

		protected override void OnLinkClicked(LinkLabelLinkClickedEventArgs e) {
			base.OnLinkClicked(e);
			
			// Because shell execuse doesn't work
			// we have to specify executing module directory
			ProcessStartInfo info = new ProcessStartInfo();
			// get default browser (exe) path
			RegistryKey rkey = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command");
			String val = rkey.GetValue("").ToString();
			Debug.WriteLine(val);
			if(val.StartsWith("\""))
			{
				int n = val.IndexOf("\"",1);
				info.FileName = val.Substring(1,n-1);
				info.Arguments = val.Substring(n+1);
			}
			else
			{
				string[] a = val.Split(new char[]{' '});
				info.FileName = a[0];
				info.Arguments = val.Substring(a[0].Length+1);
			}
			// Specifies working directory is necessary to run browser successfuly.
			info.WorkingDirectory = Path.GetDirectoryName(info.FileName);
			
			info.Arguments += targetUrl;
			Debug.WriteLine(info.FileName);
			Debug.WriteLine(info.WorkingDirectory);
			Debug.WriteLine(info.Arguments);
			System.Diagnostics.Process.Start(info);

			// Following code doesn't work. I don't know why...
			//System.Diagnostics.Process.Start(targetUrl);
		}
	}
}
