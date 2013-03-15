using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace freetrain.finance.stock
{
	public delegate void NumberChangeListener();
	/// <summary>
	/// NumberEditEx の概要の説明です。
	/// </summary>
	public class NumberEditEx : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox textbox;
		private System.Windows.Forms.Button plus1;
		private System.Windows.Forms.Button minus1;
		private System.Windows.Forms.Button plus10;
		private System.Windows.Forms.Button minus10;
		private System.Windows.Forms.Button minus100;
		private System.Windows.Forms.Button plus100;
		private System.Windows.Forms.Button plus1000;
		private System.Windows.Forms.Button minus1000;
		private System.Windows.Forms.Button plus10000;
		private System.Windows.Forms.Button minus10000;
		private System.Windows.Forms.Button btn_max;
		private System.Windows.Forms.Button btn_zero;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Label lblAdd;
		private Label lblSubtract;
		public NumberChangeListener onNumberChanged;

		public NumberEditEx()
		{
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();

			// TODO: InitForm を呼び出しの後に初期化処理を追加します。

		}

		// the number linked to the edit box
		public int number {
			get { return int.Parse(textbox.Text); }
			set { textbox.Text = value.ToString(); }
		}

		public int numberMax = 100000;

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

		#region Component Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.textbox = new System.Windows.Forms.TextBox();
			this.plus1 = new System.Windows.Forms.Button();
			this.minus1 = new System.Windows.Forms.Button();
			this.plus10 = new System.Windows.Forms.Button();
			this.minus10 = new System.Windows.Forms.Button();
			this.minus100 = new System.Windows.Forms.Button();
			this.plus100 = new System.Windows.Forms.Button();
			this.plus1000 = new System.Windows.Forms.Button();
			this.minus1000 = new System.Windows.Forms.Button();
			this.plus10000 = new System.Windows.Forms.Button();
			this.minus10000 = new System.Windows.Forms.Button();
			this.btn_max = new System.Windows.Forms.Button();
			this.btn_zero = new System.Windows.Forms.Button();
			this.lblAdd = new System.Windows.Forms.Label();
			this.lblSubtract = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textbox
			// 
			this.textbox.Location = new System.Drawing.Point(119, 23);
			this.textbox.Name = "textbox";
			this.textbox.Size = new System.Drawing.Size(102, 20);
			this.textbox.TabIndex = 18;
			this.textbox.Text = "0";
			this.textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textbox_KeyPress);
			this.textbox.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			// 
			// plus1
			// 
			this.plus1.Location = new System.Drawing.Point(203, 1);
			this.plus1.Name = "plus1";
			this.plus1.Size = new System.Drawing.Size(19, 22);
			this.plus1.TabIndex = 14;
			this.plus1.TabStop = false;
			//this.plus1.Text = "＋";
			this.plus1.Text = "1";
			this.plus1.Click += new System.EventHandler(this.plus_Click);
			// 
			// minus1
			// 
			this.minus1.Location = new System.Drawing.Point(203, 43);
			this.minus1.Name = "minus1";
			this.minus1.Size = new System.Drawing.Size(19, 22);
			this.minus1.TabIndex = 13;
			this.minus1.TabStop = false;
			//this.minus1.Text = "−";
			this.minus1.Text = "1";
			this.minus1.Click += new System.EventHandler(this.minus_Click);
			// 
			// plus10
			// 
			this.plus10.Location = new System.Drawing.Point(177, 1);
			this.plus10.Name = "plus10";
			this.plus10.Size = new System.Drawing.Size(27, 22);
			this.plus10.TabIndex = 15;
			this.plus10.TabStop = false;
			//this.plus10.Text = "＋";
			this.plus10.Text = "10";
			this.plus10.Click += new System.EventHandler(this.plus_Click);
			// 
			// minus10
			// 
			this.minus10.Location = new System.Drawing.Point(177, 43);
			this.minus10.Name = "minus10";
			this.minus10.Size = new System.Drawing.Size(27, 22);
			this.minus10.TabIndex = 17;
			this.minus10.TabStop = false;
			//this.minus10.Text = "−";
			this.minus10.Text = "10";
			this.minus10.Click += new System.EventHandler(this.minus_Click);
			// 
			// minus100
			// 
			this.minus100.Location = new System.Drawing.Point(145, 43);
			this.minus100.Name = "minus100";
			this.minus100.Size = new System.Drawing.Size(33, 22);
			this.minus100.TabIndex = 16;
			this.minus100.TabStop = false;
			//this.minus100.Text = "−";
			this.minus100.Text = "100";
			this.minus100.Click += new System.EventHandler(this.minus_Click);
			// 
			// plus100
			// 
			this.plus100.Location = new System.Drawing.Point(145, 1);
			this.plus100.Name = "plus100";
			this.plus100.Size = new System.Drawing.Size(33, 22);
			this.plus100.TabIndex = 12;
			this.plus100.TabStop = false;
			//this.plus100.Text = "＋";
			this.plus100.Text = "100";
			this.plus100.Click += new System.EventHandler(this.plus_Click);
			// 
			// plus1000
			// 
			this.plus1000.Location = new System.Drawing.Point(119, 1);
			this.plus1000.Name = "plus1000";
			this.plus1000.Size = new System.Drawing.Size(27, 22);
			this.plus1000.TabIndex = 8;
			this.plus1000.TabStop = false;
			//this.plus1000.Text = "＋";
			this.plus1000.Text = "1k";
			this.plus1000.Click += new System.EventHandler(this.plus_Click);
			// 
			// minus1000
			// 
			this.minus1000.Location = new System.Drawing.Point(119, 43);
			this.minus1000.Name = "minus1000";
			this.minus1000.Size = new System.Drawing.Size(27, 22);
			this.minus1000.TabIndex = 7;
			this.minus1000.TabStop = false;
			//this.minus1000.Text = "−";
			this.minus1000.Text = "1k";
			this.minus1000.Click += new System.EventHandler(this.minus_Click);
			// 
			// plus10000
			// 
			this.plus10000.Location = new System.Drawing.Point(87, 1);
			this.plus10000.Margin = new System.Windows.Forms.Padding(0);
			this.plus10000.Name = "plus10000";
			this.plus10000.Size = new System.Drawing.Size(33, 22);
			this.plus10000.TabIndex = 6;
			this.plus10000.TabStop = false;
			//this.plus10000.Text = "＋";
			this.plus10000.Text = "10k";
			this.plus10000.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.plus10000.Click += new System.EventHandler(this.plus_Click);
			// 
			// minus10000
			// 
			this.minus10000.Location = new System.Drawing.Point(87, 43);
			this.minus10000.Name = "minus10000";
			this.minus10000.Size = new System.Drawing.Size(33, 22);
			this.minus10000.TabIndex = 11;
			this.minus10000.TabStop = false;
			//this.minus10000.Text = "−";
			this.minus10000.Text = "10k";
			this.minus10000.Click += new System.EventHandler(this.minus_Click);
			// 
			// btn_max
			// 
			this.btn_max.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btn_max.Location = new System.Drawing.Point(0, 1);
			this.btn_max.Margin = new System.Windows.Forms.Padding(1);
			this.btn_max.Name = "btn_max";
			this.btn_max.Size = new System.Drawing.Size(36, 22);
			this.btn_max.TabIndex = 19;
			this.btn_max.TabStop = false;
			this.btn_max.Text = "Max";
			this.btn_max.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.btn_max.Click += new System.EventHandler(this.btn_max_Click);
			// 
			// btn_zero
			// 
			this.btn_zero.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btn_zero.Location = new System.Drawing.Point(0, 43);
			this.btn_zero.Name = "btn_zero";
			this.btn_zero.Size = new System.Drawing.Size(36, 22);
			this.btn_zero.TabIndex = 19;
			this.btn_zero.TabStop = false;
			this.btn_zero.Text = "0";
			this.btn_zero.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.btn_zero.Click += new System.EventHandler(this.btn_zero_Click);
			// 
			// lblAdd
			// 
			this.lblAdd.AutoSize = true;
			this.lblAdd.Location = new System.Drawing.Point(56, 5);
			this.lblAdd.Name = "lblAdd";
			this.lblAdd.Size = new System.Drawing.Size(29, 13);
			this.lblAdd.TabIndex = 20;
			this.lblAdd.Text = "Add:";
			// 
			// lblSubtract
			// 
			this.lblSubtract.AutoSize = true;
			this.lblSubtract.Location = new System.Drawing.Point(35, 48);
			this.lblSubtract.Name = "lblSubtract";
			this.lblSubtract.Size = new System.Drawing.Size(50, 13);
			this.lblSubtract.TabIndex = 21;
			this.lblSubtract.Text = "Subtract:";
			// 
			// NumberEditEx
			// 
			this.Controls.Add(this.lblSubtract);
			this.Controls.Add(this.lblAdd);
			this.Controls.Add(this.textbox);
			this.Controls.Add(this.plus1);
			this.Controls.Add(this.minus1);
			this.Controls.Add(this.plus10);
			this.Controls.Add(this.minus10);
			this.Controls.Add(this.minus100);
			this.Controls.Add(this.plus100);
			this.Controls.Add(this.plus1000);
			this.Controls.Add(this.minus1000);
			this.Controls.Add(this.plus10000);
			this.Controls.Add(this.minus10000);
			this.Controls.Add(this.btn_max);
			this.Controls.Add(this.btn_zero);
			this.Name = "NumberEditEx";
			this.Size = new System.Drawing.Size(225, 72);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void textbox_KeyPress(object sender, KeyPressEventArgs e)
		{
			char key = e.KeyChar;
			if( '0' <= key && '9' >= key ) return;
			if( '\b' == key ) return;
			e.Handled = true;		
		}

		private void plus_Click(object sender, System.EventArgs e) {
			Button b = (Button)sender;
			int v = int.Parse(b.Name.Substring(4));
			number = number+v;
			if( number > numberMax ) number = numberMax;
		}

		private void minus_Click(object sender, System.EventArgs e)	{
			Button b = (Button)sender;
			int v = int.Parse(b.Name.Substring(5));
			number = number-v;		
			if( number < 0 ) number = 0;
		}

		private void btn_max_Click(object sender, System.EventArgs e) {
			number = numberMax;
		}

		private void btn_zero_Click(object sender, System.EventArgs e) {
			number = 0;
		}

		private void textbox_TextChanged(object sender, System.EventArgs e)	{
			if(onNumberChanged != null )
				onNumberChanged();
		}
	}
}
