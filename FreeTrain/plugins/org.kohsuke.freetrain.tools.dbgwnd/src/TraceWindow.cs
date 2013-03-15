using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace freetrain.views.debug
{
	/// <summary>
	/// Displays trace messages
	/// </summary>
	public class TraceWindow : Form
	{
		private System.Windows.Forms.TextBox log;
		private System.ComponentModel.Container components = null;
		private TraceListenerImpl listener;

		public TraceWindow() {
			InitializeComponent();

			// register this object so that it can receive messages.
			listener = new TraceListenerImpl(log);
			Debug.Listeners.Add(listener);
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			Debug.Listeners.Remove(listener);

			if( disposing && components != null)
				components.Dispose();

			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.log = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// log
			// 
			this.log.Dock = System.Windows.Forms.DockStyle.Fill;
			this.log.Multiline = true;
			this.log.Name = "log";
			this.log.Size = new System.Drawing.Size(292, 271);
			this.log.TabIndex = 0;
			this.log.Text = "";
			// 
			// TraceWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(292, 271);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.log});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TraceWindow";
			this.ShowInTaskbar = false;
			this.Text = "Debug Window";
			//! this.Text = "デバッグウィンドウ";
			this.ResumeLayout(false);

		}
		#endregion

		internal class TraceListenerImpl : TraceListener {
			private readonly TextBox text;
			internal TraceListenerImpl( TextBox _text ) {
				this.text = _text;
			}
			
			public override void Write( string str ) {
				text.AppendText(str);
			}
			
			public override void WriteLine( string str ) {
				text.AppendText(str);
				text.AppendText("\n");
			}
		}
	}
}
