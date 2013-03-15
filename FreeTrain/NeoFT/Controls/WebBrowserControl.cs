using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace nft.controls
{
	/// <summary>
	/// WebBrowserControl の概要の説明です。
	/// </summary>
	public class WebBrowserControl : System.Windows.Forms.UserControl
	{
		private AxSHDocVw.AxWebBrowser axWebBrowser;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WebBrowserControl()
		{
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();

			Navigate("about:blank");
		}

		public void Navigate(string url)
		{
			object o = null;
			axWebBrowser.Navigate(url,ref o,ref o,ref o,ref o);
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region コンポーネント デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WebBrowserControl));
			this.axWebBrowser = new AxSHDocVw.AxWebBrowser();
			((System.ComponentModel.ISupportInitialize)(this.axWebBrowser)).BeginInit();
			this.SuspendLayout();
			// 
			// axWebBrowser
			// 
			this.axWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.axWebBrowser.Enabled = true;
			this.axWebBrowser.Location = new System.Drawing.Point(0, 0);
			this.axWebBrowser.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWebBrowser.OcxState")));
			this.axWebBrowser.Size = new System.Drawing.Size(136, 112);
			this.axWebBrowser.TabIndex = 0;
			// 
			// WebBrowserControl
			// 
			this.AutoScroll = true;
			this.Controls.Add(this.axWebBrowser);
			this.Name = "WebBrowserControl";
			this.Size = new System.Drawing.Size(136, 112);
			((System.ComponentModel.ISupportInitialize)(this.axWebBrowser)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

	}
}
