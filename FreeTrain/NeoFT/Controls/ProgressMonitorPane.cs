using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace nft.controls
{
	/// <summary>
	/// ProgressMonitorPane の概要の説明です。
	/// </summary>
	public class ProgressMonitorPane : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ProgressBar progress_l1;
		private System.Windows.Forms.Label status_l1;
		private System.Windows.Forms.ProgressBar progress_l2;
		private System.Windows.Forms.Label status_l2;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProgressMonitorPane()
		{
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();

			// TODO: InitializeComponent 呼び出しの後に初期化処理を追加します。

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

		public void updateMessage( int level, int percentage, string msg ) 
		{
			if(level==1)
			{
				progress_l1.Value=percentage;
				if(msg!=null)
					status_l1.Text = msg;
			}
			else
			{
				progress_l2.Value=percentage;
				if(msg!=null)
					status_l2.Text = msg;
			}
			Application.DoEvents();
		}

		#region コンポーネント デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.progress_l1 = new System.Windows.Forms.ProgressBar();
			this.status_l1 = new System.Windows.Forms.Label();
			this.progress_l2 = new System.Windows.Forms.ProgressBar();
			this.status_l2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progress_l1
			// 
			this.progress_l1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.progress_l1.Location = new System.Drawing.Point(8, 14);
			this.progress_l1.Name = "progress_l1";
			this.progress_l1.Size = new System.Drawing.Size(216, 10);
			this.progress_l1.TabIndex = 4;
			// 
			// status_l1
			// 
			this.status_l1.AutoSize = true;
			this.status_l1.Location = new System.Drawing.Point(0, -2);
			this.status_l1.Name = "status_l1";
			this.status_l1.Size = new System.Drawing.Size(0, 15);
			this.status_l1.TabIndex = 3;
			this.status_l1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// progress_l2
			// 
			this.progress_l2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.progress_l2.Location = new System.Drawing.Point(8, 42);
			this.progress_l2.Name = "progress_l2";
			this.progress_l2.Size = new System.Drawing.Size(216, 10);
			this.progress_l2.TabIndex = 6;
			// 
			// status_l2
			// 
			this.status_l2.AutoSize = true;
			this.status_l2.Location = new System.Drawing.Point(0, 26);
			this.status_l2.Name = "status_l2";
			this.status_l2.Size = new System.Drawing.Size(0, 15);
			this.status_l2.TabIndex = 5;
			this.status_l2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// ProgressMonitorPane
			// 
			this.Controls.Add(this.progress_l2);
			this.Controls.Add(this.status_l2);
			this.Controls.Add(this.progress_l1);
			this.Controls.Add(this.status_l1);
			this.Name = "ProgressMonitorPane";
			this.Size = new System.Drawing.Size(232, 56);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
