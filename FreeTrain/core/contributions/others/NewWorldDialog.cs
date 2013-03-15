using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.world;

namespace freetrain.contributions.others
{
	/// <summary>
	/// Dialog box to create a new World object.
	/// </summary>
	class NewWorldDialog : System.Windows.Forms.Form
	{
		public NewWorldDialog() {
			InitializeComponent();
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox name;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox sizeX;
		private System.Windows.Forms.TextBox sizeY;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox sizeZ;
		private System.Windows.Forms.Button cancelButton;

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.name = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.sizeX = new System.Windows.Forms.TextBox();
			this.sizeY = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.sizeZ = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Name:";
			//! this.label1.Text = "名前(&N):";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// name
			// 
			this.name.Location = new System.Drawing.Point(72, 9);
			this.name.Name = "name";
			this.name.Size = new System.Drawing.Size(224, 20);
			this.name.TabIndex = 1;
			this.name.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(46, 20);
			this.label2.TabIndex = 2;
			this.label2.Text = "&Size:";
			//! this.label2.Text = "サイズ(&S):";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// sizeX
			// 
			this.sizeX.Location = new System.Drawing.Point(72, 35);
			this.sizeX.Name = "sizeX";
			this.sizeX.Size = new System.Drawing.Size(64, 20);
			this.sizeX.TabIndex = 3;
			this.sizeX.Text = "30";
			this.sizeX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.sizeX.Validating += new System.ComponentModel.CancelEventHandler(this.validateNumber);
			// 
			// sizeY
			// 
			this.sizeY.Location = new System.Drawing.Point(160, 35);
			this.sizeY.Name = "sizeY";
			this.sizeY.Size = new System.Drawing.Size(56, 20);
			this.sizeY.TabIndex = 4;
			this.sizeY.Text = "30";
			this.sizeY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.sizeY.Validating += new System.ComponentModel.CancelEventHandler(this.validateNumber);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(136, 35);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(24, 17);
			this.label3.TabIndex = 5;
			this.label3.Text = "x";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.okButton.Location = new System.Drawing.Point(139, 69);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(79, 25);
			this.okButton.TabIndex = 6;
			this.okButton.Text = "&OK";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cancelButton.Location = new System.Drawing.Point(224, 69);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 25);
			this.cancelButton.TabIndex = 7;
			this.cancelButton.Text = "&Cancel";
			//! this.cancelButton.Text = "ｷｬﾝｾﾙ(&C)";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(216, 35);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(24, 17);
			this.label4.TabIndex = 8;
			this.label4.Text = "x";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// sizeZ
			// 
			this.sizeZ.Location = new System.Drawing.Point(240, 35);
			this.sizeZ.Name = "sizeZ";
			this.sizeZ.Size = new System.Drawing.Size(56, 20);
			this.sizeZ.TabIndex = 5;
			this.sizeZ.Text = "6";
			this.sizeZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.sizeZ.Validating += new System.ComponentModel.CancelEventHandler(this.validateNumber);
			// 
			// NewWorldDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(306, 102);
			this.Controls.Add(this.sizeZ);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.sizeY);
			this.Controls.Add(this.sizeX);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.name);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewWorldDialog";
			this.Text = "Create a new game";
			//! this.Text = "新しいゲームの作成";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void validateNumber(object sender, CancelEventArgs e) {
			TextBox tb = (TextBox)sender;
			try {
				// check if the value is a positive integer.
				if( int.Parse( tb.Text )>0 )
					return;
			} catch( Exception ) {}

			// if not, refuse to move the focus.
			e.Cancel = true;
			tb.Select();
		}

		
		/// <summary>
		/// Creates a new empty world as specified by the user.
		/// </summary>
		public World createWorld() {
			int x = int.Parse(sizeX.Text);
			int y = int.Parse(sizeY.Text);
			int z = int.Parse(sizeZ.Text);
			World w = new World( new Distance(x,y+z*2,z),  z/4 );
			w.name = this.name.Text;
			if(w.name==null || w.name.Length==0)
				w.name = "Terra Incognita";
				//! w.name = "ななしさん";
			return w;
			// TODO: Z dimension
		}
	}
}
