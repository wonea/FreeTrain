using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.contributions.others;
using freetrain.framework.plugin;
using freetrain.world;

namespace freetrain.framework
{
	/// <summary>
	/// Let the user create a new world.
	/// </summary>
	public class NewWorldDialog : System.Windows.Forms.Form
	{
		public NewWorldDialog() {
			InitializeComponent();

			NewGameContribution[] contribs = (NewGameContribution[])
				PluginManager.theInstance.listContributions(typeof(NewGameContribution));
			
			// list view doesn't support databinding. We have to live with
			// listbox for now.
			list.DataSource = contribs;
			list.DisplayMember = "name";
			author.DataBindings.Add("Text",contribs,"author");
			description.DataBindings.Add("Text",contribs,"description");
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		public World createWorld() {
			NewGameContribution contrib = (NewGameContribution)list.SelectedItem;
			return contrib.createNewGame();
		}

		#region Windows Form Designer generated code

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label author;
		private System.Windows.Forms.TextBox description;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private GroupBox groupBox1;
		private ListBox list;
		private Label label3;
		private System.ComponentModel.Container components = null;

		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.author = new System.Windows.Forms.Label();
			this.description = new System.Windows.Forms.TextBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.list = new System.Windows.Forms.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 104);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Author:";
			//! this.label1.Text = "作者：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 128);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Description:";
			//! this.label2.Text = "解説：";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// author
			// 
			this.author.Location = new System.Drawing.Point(84, 104);
			this.author.Name = "author";
			this.author.Size = new System.Drawing.Size(234, 16);
			this.author.TabIndex = 3;
			this.author.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// description
			// 
			this.description.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.description.Location = new System.Drawing.Point(84, 128);
			this.description.Multiline = true;
			this.description.Name = "description";
			this.description.ReadOnly = true;
			this.description.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.description.Size = new System.Drawing.Size(234, 64);
			this.description.TabIndex = 4;
			this.description.Text = "";
			// 
			// okButton
			// 
			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.okButton.Location = new System.Drawing.Point(160, 217);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(88, 26);
			this.okButton.TabIndex = 5;
			this.okButton.Text = "&OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cancelButton.Location = new System.Drawing.Point(256, 217);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(88, 26);
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "&Cancel";
			//! this.cancelButton.Text = "ｷｬﾝｾﾙ(&C)";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.list);
			this.groupBox1.Controls.Add(this.description);
			this.groupBox1.Controls.Add(this.author);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(330, 199);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "New Game Plug-Ins";
			// 
			// list
			// 
			this.list.Location = new System.Drawing.Point(84, 19);
			this.list.Name = "list";
			this.list.Size = new System.Drawing.Size(234, 69);
			this.list.TabIndex = 12;
			// 
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 19);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(69, 13);
			this.label3.TabIndex = 14;
			this.label3.Text = "Name:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// NewWorldDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(354, 247);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewWorldDialog";
			this.ShowInTaskbar = false;
			this.Text = "New Game";
			//! this.Text = "新しいゲーム";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private void okButton_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
