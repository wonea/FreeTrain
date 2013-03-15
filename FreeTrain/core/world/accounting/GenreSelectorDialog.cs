using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.framework.plugin;

namespace freetrain.world.accounting
{
	/// <summary>
	/// Let the user select a list of account genre.
	/// </summary>
	public class GenreSelectorDialog : System.Windows.Forms.Form
	{
		public GenreSelectorDialog( AccountGenre[] current ) {
			InitializeComponent();
			
			selector.availables = 
				PluginManager.theInstance.listContributions(typeof(AccountGenre));
			selector.selected = current;
		}

		/// <summary>
		/// Obtain the list of selected genres in a modifiable array.
		/// </summary>
		public AccountGenre[] selected {
			get {
				IList l = selector.selected;
				AccountGenre[] r = new AccountGenre[l.Count];
				l.CopyTo( r, 0 );
				return r;
			}
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private freetrain.controls.SubListSelector selector;
		private System.ComponentModel.Container components = null;

		private void InitializeComponent()
		{
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.selector = new freetrain.controls.SubListSelector();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.okButton.Location = new System.Drawing.Point(216, 208);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(96, 24);
			this.okButton.TabIndex = 8;
			this.okButton.Text = "&OK";
			this.okButton.Click += new System.EventHandler(this.onOK);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cancelButton.Location = new System.Drawing.Point(320, 208);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(96, 24);
			this.cancelButton.TabIndex = 9;
			this.cancelButton.Text = "&Cancel";
			//! this.cancelButton.Text = "ｷｬﾝｾﾙ(&C)";
			// 
			// selector
			// 
			this.selector.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.selector.availables = null;
			this.selector.Location = new System.Drawing.Point(8, 8);
			this.selector.Name = "selector";
			this.selector.Size = new System.Drawing.Size(408, 192);
			this.selector.TabIndex = 1;
			this.selector.title1 = "&Available Items:";
			this.selector.title2 = "&Selected Items:";
			//! this.selector.title1 = "選択可能項目(&A)：";
			//! this.selector.title2 = "選択項目(&S)：";
			// 
			// GenreSelectorDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(426, 238);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.selector,
																		  this.cancelButton,
																		  this.okButton});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GenreSelectorDialog";
			this.ShowInTaskbar = false;
			this.Text = "Display Settings";
			//! this.Text = "表示項目の設定";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

		private void onOK(object sender, System.EventArgs e) {
			this.DialogResult = DialogResult.OK;
			Close();
		}

	}
}
