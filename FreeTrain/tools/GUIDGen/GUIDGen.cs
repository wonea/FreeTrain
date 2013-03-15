using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace GUIDGen
{
	public class GUIDGen : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label text;
		private System.Windows.Forms.Button buttonNew;
		private System.Windows.Forms.Button buttonCopy;
		private System.ComponentModel.Container components = null;

		public GUIDGen() {
			InitializeComponent();
			buttonNew_Click(null,null);
		}

		protected override void Dispose( bool disposing ) {
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
			this.text = new System.Windows.Forms.Label();
			this.buttonNew = new System.Windows.Forms.Button();
			this.buttonCopy = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// text
			// 
			this.text.Location = new System.Drawing.Point(8, 8);
			this.text.Name = "text";
			this.text.Size = new System.Drawing.Size(264, 16);
			this.text.TabIndex = 0;
			// 
			// buttonNew
			// 
			this.buttonNew.Location = new System.Drawing.Point(144, 32);
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new System.Drawing.Size(128, 24);
			this.buttonNew.TabIndex = 2;
			this.buttonNew.Text = "&New ID && Copy";
			//! this.buttonNew.Text = "新しいID＆コピー(&N)";
			this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
			// 
			// buttonCopy
			// 
			this.buttonCopy.Location = new System.Drawing.Point(56, 32);
			this.buttonCopy.Name = "buttonCopy";
			this.buttonCopy.Size = new System.Drawing.Size(80, 24);
			this.buttonCopy.TabIndex = 1;
			this.buttonCopy.Text = "&Copy";
			//! this.buttonCopy.Text = "コピー(&C)";
			this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
			// 
			// GUIDGen
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(282, 61);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonCopy,
																		  this.buttonNew,
																		  this.text});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GUIDGen";
			this.Text = "Create ID";
			//! this.Text = "IDの作成";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()  {
			Application.Run(new GUIDGen());
		}

		private void buttonNew_Click(object sender, System.EventArgs e) {
			text.Text = '{'+Guid.NewGuid().ToString().ToUpper()+'}';
			buttonCopy_Click(null,null);
		}

		private void buttonCopy_Click(object sender, System.EventArgs e) {
			Clipboard.SetDataObject( text.Text, true );
		}
	}
}
