using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace nft.controls
{
	/// <summary>
	/// MsgAndIconLabel の概要の説明です。
	/// </summary>
	public class MsgAndIconLabel : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label labelIcon;
		private System.Windows.Forms.Label labelMsg;
		private System.Windows.Forms.Panel mainPanel;
		//private System.ComponentModel.IContainer components;

		public MsgAndIconLabel()
		{
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();

		}

		public ImageList ImageList
		{
			get{ return labelIcon.ImageList; }
			set{ labelIcon.ImageList = value; }
		}

		public int ImageIndex
		{
			get{ return labelIcon.ImageIndex; }
			set{ labelIcon.ImageIndex = value; }
		}

		public override string Text
		{
			get{ return labelMsg.Text; }
			set{ labelMsg.Text = value; }
		}

		public string Message
		{
			get{ return labelMsg.Text; }
			set{ labelMsg.Text = value; }
		}

		public new Font Font
		{
			get{ return labelMsg.Font; }
			set{ labelMsg.Font = value; }
		}

		public BorderStyle BorderStyle
		{
			get{ return mainPanel.BorderStyle; }
			set{ mainPanel.BorderStyle = value; }
		}

		#region コンポーネント デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.labelIcon = new System.Windows.Forms.Label();
			this.labelMsg = new System.Windows.Forms.Label();
			this.mainPanel = new System.Windows.Forms.Panel();
			this.mainPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelIcon
			// 
			this.labelIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.labelIcon.BackColor = System.Drawing.Color.Transparent;
			this.labelIcon.Location = new System.Drawing.Point(3, 0);
			this.labelIcon.Name = "labelIcon";
			this.labelIcon.Size = new System.Drawing.Size(18, 24);
			this.labelIcon.TabIndex = 0;
			// 
			// labelMsg
			// 
			this.labelMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelMsg.BackColor = System.Drawing.Color.Transparent;
			this.labelMsg.Location = new System.Drawing.Point(24, 0);
			this.labelMsg.Name = "labelMsg";
			this.labelMsg.Size = new System.Drawing.Size(192, 24);
			this.labelMsg.TabIndex = 0;
			this.labelMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mainPanel
			// 
			this.mainPanel.Controls.Add(this.labelIcon);
			this.mainPanel.Controls.Add(this.labelMsg);
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainPanel.Location = new System.Drawing.Point(0, 0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(216, 24);
			this.mainPanel.TabIndex = 1;
			// 
			// MsgAndIconLabel
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.mainPanel);
			this.Name = "MsgAndIconLabel";
			this.Size = new System.Drawing.Size(216, 24);
			this.mainPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
//			if( disposing )
//			{
//				if(components != null)
//				{
//					components.Dispose();
//				}
//			}
			base.Dispose( disposing );
		}
		#endregion
	}
}
