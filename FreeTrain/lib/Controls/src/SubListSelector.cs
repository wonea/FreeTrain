using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace freetrain.controls
{
	/// <summary>
	/// Select an ordered sub list from another list.
	/// 
	/// Set the availables property of this control to populate the candidates.
	/// This control allows the user to build a sub list of these candidates
	/// in arbitrary order.
	/// 
	/// The constructed sub list can be accessible through the selected property.
	/// </summary>
	public class SubListSelector : System.Windows.Forms.UserControl
	{
		/// <summary> Source item list. </summary>
		private IList _availables;

		public SubListSelector() {
			InitializeComponent();
			// the form editor doesn't roundtrip those strings correctly.
			this.select.Text	= new string((char)0xE8,1);
			this.unselect.Text	= new string((char)0xE7,1);
			this.up.Text		= new string((char)0xE9,1);
			this.down.Text		= new string((char)0xEA,1);
		}

		/// <summary>
		/// Set or get the list of selectable items.
		/// </summary>
		public IList availables {
			get { return _availables; }
			set {
				_availables=value;
				availablesList.DataSource = _availables;
				updateButtons();
			}
		}

		/// <summary>
		/// The sub list created by the user.
		/// </summary>
		public IList selected {
			get {
				// TODO: should return a read-only list.
				return selectedList.Items;
			}
			set {
				selectedList.BeginUpdate();
				selectedList.Items.Clear();
				foreach( object o in value )
					selectedList.Items.Add(o);
				selectedList.EndUpdate();
				updateButtons();
			}
		}

		[
			Description("The caption of the first list box"),
			Category("custom")
		]
		public string title1 {
			get { return titleAvailables.Text; }
			set { titleAvailables.Text=value; }
		}

		[
			Description("The caption of the second list box"),
			Category("custom")
		]
		public string title2 {
			get { return titleSelected.Text; }
			set { titleSelected.Text=value; }
		}


		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		private System.Windows.Forms.Button select;
		private System.Windows.Forms.Button unselect;
		private System.Windows.Forms.Button down;
		private System.Windows.Forms.Button up;
		private System.Windows.Forms.Label titleAvailables;
		private System.Windows.Forms.Label titleSelected;
		private System.Windows.Forms.ListBox availablesList;
		private System.Windows.Forms.ListBox selectedList;
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.availablesList = new System.Windows.Forms.ListBox();
			this.selectedList = new System.Windows.Forms.ListBox();
			this.select = new System.Windows.Forms.Button();
			this.unselect = new System.Windows.Forms.Button();
			this.down = new System.Windows.Forms.Button();
			this.up = new System.Windows.Forms.Button();
			this.titleAvailables = new System.Windows.Forms.Label();
			this.titleSelected = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// availablesList
			// 
			this.availablesList.IntegralHeight = false;
			this.availablesList.ItemHeight = 12;
			this.availablesList.Location = new System.Drawing.Point(0, 16);
			this.availablesList.Name = "availablesList";
			this.availablesList.Size = new System.Drawing.Size(136, 192);
			this.availablesList.TabIndex = 2;
			this.availablesList.SelectedIndexChanged += new System.EventHandler(this.updateButtons);
			// 
			// selectedList
			// 
			this.selectedList.IntegralHeight = false;
			this.selectedList.ItemHeight = 12;
			this.selectedList.Location = new System.Drawing.Point(176, 16);
			this.selectedList.Name = "selectedList";
			this.selectedList.Size = new System.Drawing.Size(136, 192);
			this.selectedList.TabIndex = 6;
			this.selectedList.SelectedIndexChanged += new System.EventHandler(this.updateButtons);
			// 
			// select
			// 
			this.select.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.select.Font = new System.Drawing.Font("Wingdings", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.select.Location = new System.Drawing.Point(144, 80);
			this.select.Name = "select";
			this.select.Size = new System.Drawing.Size(24, 24);
			this.select.TabIndex = 3;
			this.select.Text = "e";
			this.select.Click += new System.EventHandler(this.select_Click);
			// 
			// unselect
			// 
			this.unselect.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.unselect.Font = new System.Drawing.Font("Wingdings", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.unselect.Location = new System.Drawing.Point(144, 128);
			this.unselect.Name = "unselect";
			this.unselect.Size = new System.Drawing.Size(24, 24);
			this.unselect.TabIndex = 4;
			this.unselect.Text = "c";
			this.unselect.Click += new System.EventHandler(this.unselect_Click);
			// 
			// down
			// 
			this.down.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.down.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.down.Font = new System.Drawing.Font("Wingdings", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.down.Location = new System.Drawing.Point(320, 128);
			this.down.Name = "down";
			this.down.Size = new System.Drawing.Size(24, 24);
			this.down.TabIndex = 8;
			this.down.Text = "e";
			this.down.Click += new System.EventHandler(this.down_Click);
			// 
			// up
			// 
			this.up.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.up.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.up.Font = new System.Drawing.Font("Wingdings", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.up.Location = new System.Drawing.Point(320, 80);
			this.up.Name = "up";
			this.up.Size = new System.Drawing.Size(24, 24);
			this.up.TabIndex = 7;
			this.up.Text = "e";
			this.up.Click += new System.EventHandler(this.up_Click);
			// 
			// titleAvailables
			// 
			this.titleAvailables.Name = "titleAvailables";
			this.titleAvailables.Size = new System.Drawing.Size(136, 16);
			this.titleAvailables.TabIndex = 1;
			this.titleAvailables.Text = "Available Items:";
			//! this.titleAvailables.Text = "選択可能項目：";
			// 
			// titleSelected
			// 
			this.titleSelected.Location = new System.Drawing.Point(176, 0);
			this.titleSelected.Name = "titleSelected";
			this.titleSelected.Size = new System.Drawing.Size(136, 16);
			this.titleSelected.TabIndex = 5;
			this.titleSelected.Text = "Selected Items:";
			//! this.titleSelected.Text = "選択項目：";
			// 
			// SubListSelector
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.titleSelected,
																		  this.titleAvailables,
																		  this.down,
																		  this.up,
																		  this.unselect,
																		  this.select,
																		  this.selectedList,
																		  this.availablesList});
			this.Name = "SubListSelector";
			this.Size = new System.Drawing.Size(344, 208);
			this.ResumeLayout(false);

		}
		#endregion

		private void select_Click(object sender, System.EventArgs e) {
			selectedList.Items.Add(availablesList.SelectedItem);
			updateButtons();
		}

		private void unselect_Click(object sender, System.EventArgs e) {
			selectedList.Items.Remove(selectedList.SelectedItem);
			updateButtons();
		}

		private void up_Click(object sender, System.EventArgs e) {
			move(-1);
		}

		private void down_Click(object sender, System.EventArgs e) {
			move(+1);
		}

		private void move( int offset ) {
			object cur = selectedList.SelectedItem;
			int idx = selectedList.SelectedIndex;

			selectedList.BeginUpdate();
			selectedList.Items.RemoveAt(idx);
			selectedList.Items.Insert( idx+offset, cur );
			selectedList.SelectedIndex = idx+offset;
			selectedList.EndUpdate();

			updateButtons();
		}

		/// <summary>
		/// Update enable states of buttons 
		/// </summary>
		private void updateButtons() {
			select.Enabled =
				( availablesList.SelectedItem!=null && !selectedList.Items.Contains(availablesList.SelectedItem) );
			unselect.Enabled =
				( selectedList.SelectedItem!=null );
			up.Enabled =
				( selectedList.SelectedIndex>0 );
			down.Enabled =
				( 0 <= selectedList.SelectedIndex && selectedList.SelectedIndex<selectedList.Items.Count-1 );
		}

		private void updateButtons(object sender, System.EventArgs e) {
			updateButtons();
		}

		protected override void OnResize(System.EventArgs e) {
			base.OnResize(e);
			
			// move the controls appropriately.
			int listWidth = (this.Width-72)/2;	// width of buttons and spaces
			int listHeight = this.Height-16;

			titleAvailables.Size	= new Size( listWidth, 16 );
			titleSelected.Location	= new Point( listWidth+40, 0 );
			titleSelected.Size		= new Size( listWidth, 16 );
			availablesList.Location	= new Point( 0, 16 );
			availablesList.Size		= new Size( listWidth, listHeight );
			select.Location			= new Point( listWidth+8, 16+listHeight/2-36);
			unselect.Location		= new Point( listWidth+8, 16+listHeight/2+12);
			selectedList.Location	= new Point( listWidth+40, 16 );
			selectedList.Size		= new Size( listWidth, listHeight );
			up.Location				= new Point( listWidth*2+48, 16+listHeight/2-36);
			down.Location			= new Point( listWidth*2+48, 16+listHeight/2+12);
		}
	}
}
