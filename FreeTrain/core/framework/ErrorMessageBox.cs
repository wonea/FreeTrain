using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.controls;

namespace freetrain.framework
{
	/// <summary>
	/// Shows an exception to the user
	/// (and ask for the forgiveness :-)
	/// </summary>
	public sealed class ErrorMessageBox : System.Windows.Forms.Form
	{
		/// <summary>
		/// Displays a dialog box and returns after the dialog is closed.
		/// </summary>
		/// <param name="owner">can be null.</param>
		public static void show( IWin32Window owner, string caption, Exception e ) {
			using( Form f = new ErrorMessageBox(caption,e) ) {
				f.ShowDialog(owner);
			}
		}

		private System.Windows.Forms.Label msg;
		private UrlLinkLabel linkLabel1;

		private readonly Exception exception;

		private ErrorMessageBox( string caption, Exception e ) {
			this.exception = e;
			this.Text = caption;

			InitializeComponent();

			base.Icon =  SystemIcons.Error;
			icon.Image = SystemIcons.Error.ToBitmap();

			detail.Text = e.Message + "\n" + e.StackTrace;

			while(true) {
				e = e.InnerException;
				if(e==null)		break;

				detail.Text = detail.Text + "\n" + e.Message + "\n" + e.StackTrace;
			}

			detail.Select(0,0);
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private System.Windows.Forms.PictureBox icon;
		private System.Windows.Forms.TextBox detail;
		private System.Windows.Forms.Button okButton;
		private System.ComponentModel.Container components = null;
		
		private void InitializeComponent()
		{
			this.icon = new System.Windows.Forms.PictureBox();
			this.detail = new System.Windows.Forms.TextBox();
			this.okButton = new System.Windows.Forms.Button();
			this.msg = new System.Windows.Forms.Label();
			this.linkLabel1 = new freetrain.controls.UrlLinkLabel();
			this.SuspendLayout();
			// 
			// icon
			// 
			this.icon.Location = new System.Drawing.Point(16, 0);
			this.icon.Name = "icon";
			this.icon.Size = new System.Drawing.Size(48, 48);
			this.icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.icon.TabIndex = 0;
			this.icon.TabStop = false;
			// 
			// detail
			// 
			this.detail.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.detail.Location = new System.Drawing.Point(16, 48);
			this.detail.Multiline = true;
			this.detail.Name = "detail";
			this.detail.ReadOnly = true;
			this.detail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.detail.Size = new System.Drawing.Size(368, 96);
			this.detail.TabIndex = 2;
			this.detail.Text = "detail";
			// 
			// okButton
			// 
			this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.okButton.Location = new System.Drawing.Point(304, 148);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(80, 24);
			this.okButton.TabIndex = 3;
			this.okButton.Text = "&OK";
			// 
			// msg
			// 
			this.msg.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.msg.Location = new System.Drawing.Point(72, 8);
			this.msg.Name = "msg";
			this.msg.Size = new System.Drawing.Size(312, 16);
			this.msg.TabIndex = 1;
			this.msg.Text = "An error has occurred";
			//! this.msg.Text = "エラーが発生しました";
			// 
			// linkLabel1
			// 
			this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.linkLabel1.Location = new System.Drawing.Point(72, 24);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(312, 16);
			this.linkLabel1.TabIndex = 4;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.TargetUrl = "http://freetrain.sourceforge.net/";
			this.linkLabel1.Text = "Report a bug";
			//! this.linkLabel1.Text = "バグを報告する";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ErrorMessageBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(400, 174);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.linkLabel1,
																		  this.okButton,
																		  this.detail,
																		  this.msg,
																		  this.icon});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ErrorMessageBox";
			this.Text = "Error";
			//! this.Text = "エラー";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
