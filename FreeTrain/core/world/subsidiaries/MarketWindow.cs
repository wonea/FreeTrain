using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.contributions.dock;
using freetrain.framework;
using freetrain.util.docking;
using freetrain.util.command;

namespace freetrain.world.subsidiaries
{
	/// <summary>
	/// Controller of SubsidiaryMarket.
	/// 
	/// Uses the event notification to update the list in real time.
	/// </summary>
	public class MarketWindow : Form //, IDockingWindow
	{
		/// <summary>
		/// map from SubsidiaryCompany to ListViewItem.
		/// </summary>
		private readonly IDictionary companies = new Hashtable();

		public MarketWindow() {
			InitializeComponent();
			Bitmap bmp = ResourceUtil.loadSystemBitmap("SubsidiaryMarketWindowToolButton.bmp");
			imageList.TransparentColor = bmp.GetPixel(0,0);
			imageList.Images.AddStrip(bmp);
			
			World.onNewWorld += new EventHandler(reset);

			SubsidiaryMarket.BUY .volatileEvents.onAdded += new SubsidiaryMarketListener(onAdded);
			SubsidiaryMarket.SELL.volatileEvents.onAdded += new SubsidiaryMarketListener(onAdded);
			SubsidiaryMarket.BUY .volatileEvents.onRemoved += new SubsidiaryMarketListener(onRemoved);
			SubsidiaryMarket.SELL.volatileEvents.onRemoved += new SubsidiaryMarketListener(onRemoved);

			reset(null,null);

			freetrain.util.controls.ToolBarCustomizerUI.attach(toolBar);
			tbModeSell.Pushed = true;

			// associate UI with code
			CommandManager commands = new CommandManager();
			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(onGoToClicked) )
				.addUpdateHandler( new CommandHandler(updateGoToButton) )
				.commandInstances.AddAll( tbGo );
			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(onOKClicked) )
				.addUpdateHandler( new CommandHandler(updateOKButton) )
				.commandInstances.AddAll( tbOK );
		}

		private bool isBuying { get { return tbModeBuy.Pushed; } }

		/// <summary>
		/// Current market we are looking in.
		/// </summary>
		private SubsidiaryMarket market {
			get {
				if( isBuying )	return SubsidiaryMarket.BUY;
				else			return SubsidiaryMarket.SELL;
			}
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
			
			World.onNewWorld -= new EventHandler(reset);
			SubsidiaryMarket.BUY .volatileEvents.onAdded -= new SubsidiaryMarketListener(onAdded);
			SubsidiaryMarket.SELL.volatileEvents.onAdded -= new SubsidiaryMarketListener(onAdded);
			SubsidiaryMarket.BUY .volatileEvents.onRemoved -= new SubsidiaryMarketListener(onRemoved);
			SubsidiaryMarket.SELL.volatileEvents.onRemoved -= new SubsidiaryMarketListener(onRemoved);
		}

		/// <summary>
		/// Returns the currently selected company or null if none is selected.
		/// </summary>
		private SubsidiaryCompany selectedProperty {
			get {
				if( list.SelectedItems.Count==0 )	return null;
				else	return (SubsidiaryCompany)list.SelectedItems[0].Tag;
			}
		}
		

		private void onAdded( SubsidiaryMarket m, SubsidiaryCompany company ) {
			if( m!=market )		return;	// ignore

			ListViewItem item = new ListViewItemEx(company);
			list.Items.Insert( 0, item );
			companies.Add( company, item );
		}

		private void onRemoved( SubsidiaryMarket m, SubsidiaryCompany company ) {
			if( m!=market )		return;	// ignore
			
			ListViewItemEx item = (ListViewItemEx)companies[company];
			companies.Remove(company);
			list.Items.Remove(item);
		}
		
		private void reset( object sender, EventArgs e ) {
			reset();
		}

		/// <summary>
		/// Reset the list view.
		/// </summary>
		private void reset() {
			list.BeginUpdate();
			list.Items.Clear();
			companies.Clear();
			foreach( SubsidiaryCompany c in market.companiesOnSale )
				onAdded(market,c);
			list.EndUpdate();
		}
	
//		public void setSite( ContentEx site ) {
//			site.SaveCustomConfig += new Crownwood.Magic.Docking.SaveCustomConfigHandler(saveConfig);
//			site.SaveCustomConfig += new Crownwood.Magic.Docking.LoadCustomConfigHandler(loadConfig);
//		}

		#region Windows Form Designer generated code
		private System.Windows.Forms.ToolBarButton tbModeBuy;
		private System.Windows.Forms.ToolBarButton tbModeSell;
		private System.Windows.Forms.ToolBarButton tbGo;
		private System.Windows.Forms.ToolBar toolBar;
		private System.Windows.Forms.ListView list;
		private System.Windows.Forms.ToolBarButton tbSeparator;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ToolBarButton tbOK;
		private System.Windows.Forms.ColumnHeader columnHeader5;


		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.toolBar = new System.Windows.Forms.ToolBar();
			this.tbModeSell = new System.Windows.Forms.ToolBarButton();
			this.tbModeBuy = new System.Windows.Forms.ToolBarButton();
			this.tbSeparator = new System.Windows.Forms.ToolBarButton();
			this.tbGo = new System.Windows.Forms.ToolBarButton();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.list = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.tbOK = new System.Windows.Forms.ToolBarButton();
			this.SuspendLayout();
			// 
			// toolBar
			// 
			this.toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.tbModeSell,
																					   this.tbModeBuy,
																					   this.tbSeparator,
																					   this.tbGo,
																					   this.tbOK});
			this.toolBar.Dock = System.Windows.Forms.DockStyle.Left;
			this.toolBar.DropDownArrows = true;
			this.toolBar.ImageList = this.imageList;
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(57, 174);
			this.toolBar.TabIndex = 0;
			this.toolBar.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.onButtonClick);
			// 
			// tbModeSell
			// 
			this.tbModeSell.ImageIndex = 0;
			this.tbModeSell.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbModeSell.Text = "My Company";
			//! this.tbModeSell.Text = "自社";
			// 
			// tbModeBuy
			// 
			this.tbModeBuy.ImageIndex = 0;
			this.tbModeBuy.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbModeBuy.Text = "Other Companies";
			//! this.tbModeBuy.Text = "他社";
			// 
			// tbSeparator
			// 
			this.tbSeparator.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbGo
			// 
			this.tbGo.ImageIndex = 2;
			this.tbGo.Text = "Go";
			//! this.tbGo.Text = "移動";
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// list
			// 
			this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																				   this.columnHeader1,
																				   this.columnHeader2,
																				   this.columnHeader3,
																				   this.columnHeader4,
																				   this.columnHeader5});
			this.list.Dock = System.Windows.Forms.DockStyle.Fill;
			this.list.FullRowSelect = true;
			this.list.Location = new System.Drawing.Point(57, 0);
			this.list.Name = "list";
			this.list.Size = new System.Drawing.Size(351, 174);
			this.list.TabIndex = 1;
			this.list.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			//! this.columnHeader1.Text = "名前";
			this.columnHeader1.Width = 200;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Value";
			//! this.columnHeader2.Text = "評価額";
			this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.columnHeader2.Width = 100;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Balance";
			//! this.columnHeader3.Text = "収支";
			this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Sales";
			//! this.columnHeader4.Text = "売上";
			this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Expenses";
			//! this.columnHeader5.Text = "経費";
			this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// tbOK
			// 
			this.tbOK.ImageIndex = 1;
			this.tbOK.Text = "Buy";
			//! this.tbOK.Text = "購入";
			// 
			// MarketWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(408, 174);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.list,
																		  this.toolBar});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "MarketWindow";
			this.Text = "Subsidiaries";
			//! this.Text = "子会社取引";
			this.ResumeLayout(false);

		}
		#endregion
		
		private void onGoToClicked() {
			MainWindow.primaryMapView.moveTo( selectedProperty.owner.locationClue );
		}
		private void updateGoToButton( Command cmd ) {
			cmd.Enabled = (selectedProperty!=null);
		}

		private void onOKClicked() {
			if( isBuying )		selectedProperty.buy();
			else				selectedProperty.sell();

		}
		private void updateOKButton( Command cmd ) {
			cmd.Enabled = (selectedProperty!=null);
			cmd.Text = isBuying?"Buy":"Sell";
			//! cmd.Text = isBuying?"購入":"売却";
		}

		private void onButtonClick(object sender, ToolBarButtonClickEventArgs e) {
			if( e.Button==tbModeBuy ) {
				tbModeBuy.Pushed = true;
				tbModeSell.Pushed = false;
				reset();
			}
			if( e.Button==tbModeSell ) {
				tbModeBuy.Pushed = false;
				tbModeSell.Pushed = true;
				reset();
			}
		}

		
		/// <summary>
		/// ListViewItem implementation that hooks itself up to the associated
		/// SubsidiaryCompany.
		/// 
		/// It will dispose itself when it find that it's no longer in use.
		/// </summary>
		private class ListViewItemEx : ListViewItem
		{
			public SubsidiaryCompany company { get { return (SubsidiaryCompany)Tag; } }

			internal ListViewItemEx( SubsidiaryCompany company ) {
				base.Tag = company;
				company.onPriceChangedV += new EventHandler(onChanged);
				updateData();
			}

			private void updateData() {
				SubsidiaryCompany company = this.company;

				base.SubItems.Clear();
				base.Text = company.owner.name;
				base.SubItems.Add(util.CurrencyUtil.format(company.currentMarketPrice));
				base.SubItems.Add(util.CurrencyUtil.format(company.profit));
				base.SubItems.Add(util.CurrencyUtil.format(company.sales));
				base.SubItems.Add(util.CurrencyUtil.format(company.cost));
			}

			internal void onDeleted() {
				company.onPriceChangedV -= new EventHandler(onChanged);
			}

			private void onChanged( object sender, EventArgs e ) {
				if( this.ListView==null )
					// if this list view item is orphaned, discontinue the event handling.
					onDeleted();
				else
					updateData();
			}
		}
	}
}
