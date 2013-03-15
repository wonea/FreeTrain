using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Data;
using Microsoft.Win32;
using freetrain.contributions.dock;
using freetrain.contributions.common;
using freetrain.contributions.rail;
using freetrain.contributions.road;
using freetrain.contributions.structs;
using freetrain.contributions.others;
using freetrain.controls;
using freetrain.controllers;
using freetrain.controllers.land;
using freetrain.controllers.rail;
using freetrain.controllers.road;
using freetrain.controllers.terrain;
using freetrain.controllers.structs;
using freetrain.framework.plugin;
using freetrain.framework.sound;
using freetrain.framework.graphics;
using freetrain.views;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.accounting;
using freetrain.world.rail;
using freetrain.util;
using freetrain.util.command;
using freetrain.util.docking;
using org.kohsuke.directdraw;
using ICSharpCode.SharpZipLib.BZip2;

namespace freetrain.framework
{
	/// <summary>
	/// MDI Container Window
	/// </summary>
	public class MainWindow : Form
	{
		#region GUI components
		private System.Windows.Forms.MenuItem mruMenuItem;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuQuit;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.MainMenu MainMenu;
		private System.Windows.Forms.MenuItem MenuItem_OpenMap;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem MenuItem_File_Save;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.MenuItem menuItem_RailRoadConstruction;
		private System.Windows.Forms.StatusBarPanel statusBar_Message;
		private System.Windows.Forms.StatusBarPanel statusBar_Controller;
		private System.Windows.Forms.MenuItem menuItem_TrainPlacement;
		private System.Windows.Forms.MenuItem menuItem_ClockStop;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ContextMenu clockMenu;
		private System.Windows.Forms.MenuItem menuItem_File_New;
		private System.Windows.Forms.MenuItem menuItem_ClockGo2;
		private System.Windows.Forms.MenuItem menuItem_ClockGo1;
		private System.Windows.Forms.MenuItem menuItem_SlopeRailRoad;
		private System.Windows.Forms.MenuItem menuItem_Platform;
		private System.Windows.Forms.StatusBarPanel statusBar_Time;
		private System.Windows.Forms.MenuItem menuItem_listPlugins;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem_TrainTrading;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem_struct;
		private System.Windows.Forms.MenuItem menuItem_terrain;
		private System.Windows.Forms.MenuItem menuItem_railStationary;
		public System.Windows.Forms.MenuItem menuItem_rail;
		public System.Windows.Forms.MenuItem menuItem_construction;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem_ClockGo4;
		private System.Windows.Forms.MenuItem menuItem_ClockGo3;
		private System.Windows.Forms.ToolBarButton tbTimer;
		private System.Windows.Forms.ToolBarButton tbRailRoad;
		private System.Windows.Forms.ToolBarButton tbSlope;
		private System.Windows.Forms.ImageList toolBarIcons;
		private System.Windows.Forms.ToolBarButton tbStation;
		private System.Windows.Forms.ToolBarButton tbRRAcc;
		private System.Windows.Forms.ToolBarButton tbTrainPlacement;
		private System.Windows.Forms.ToolBarButton tbTrainTrading;
		private System.Windows.Forms.ToolBarButton tbTrainDiagram;
		private System.Windows.Forms.ToolBarButton tbSeparator;
		private System.Windows.Forms.ToolBarButton tbTerrain;
		private System.Windows.Forms.ToolBarButton tbStruct;
		public System.Windows.Forms.MenuItem menuItem_config;
		private System.Windows.Forms.MenuItem menuItem_music;
		private System.Windows.Forms.MenuItem menuItem_option;
		private System.Windows.Forms.Timer timerStatusBarUpdate;
		private System.Windows.Forms.MenuItem MenuItem_File_Open;
		private System.Windows.Forms.MenuItem menuItem_varHeightBldg;
		private System.Windows.Forms.MenuItem menuItem_enableSoundEffect;
		private System.Windows.Forms.MenuItem menuItem_disableSoundEffect;
		private System.Windows.Forms.MenuItem menuItem_soundEffect;
		private System.Windows.Forms.MenuItem menuItem_stationPassageway;
		private System.Windows.Forms.MenuItem menuItem_land;
		public System.Windows.Forms.MenuItem menuItem_road;
		private System.Windows.Forms.MenuItem menuItem_About;
		private System.Windows.Forms.MenuItem menuItem_onlineHelp;
		public System.Windows.Forms.MenuItem menuItem_view;
		public System.Windows.Forms.MenuItem menuItem_help;
		public System.Windows.Forms.MenuItem menuItem_file;
		private System.Windows.Forms.MenuItem menuItem_landProperty;
		private System.Windows.Forms.MenuItem menuItem_balanceSheet;
		#endregion

		//なぜか知らないが起動直後にセーブデータを読み込むと、
		//この画像ファイルがPictureManagerに登録されていないのでエラーになる。
		//このクラスでは使わないけど、ここで参照すれば、起動時には常に登録される。
		//"RailRoads.bmp"なんかはそんなことないのに不思議。
		private static readonly Picture ugChips = ResourceUtil.loadSystemPicture("ugslope.bmp");

		/// <summary> Maintain command-action association. </summary>
		public readonly CommandManager commands = new CommandManager();
		
		/// <summary> Process file drops to this form. </summary>
		private readonly FileDropHandler fileDropHandler;

		/// <summary> Controls window docking. </summary>
		public readonly DockingManagerEx dockingManager;

		/// <summary>
		/// DirectDraw object for those who needs to create secondary surfaces.
		/// Should be treated as read-only.
		/// </summary>
		public WindowedDirectDraw directDraw;
		private System.Windows.Forms.ToolBarButton tbBulldoze;
		private System.Windows.Forms.ToolBar toolBar2;
		private System.Windows.Forms.ImageList viewButtons;
		private System.Windows.Forms.ToolBarButton viewDayAndNight;
		private System.Windows.Forms.ToolBarButton viewAlwaysDay;
		private System.Windows.Forms.ToolBarButton viewAlwaysNight;
		private System.Windows.Forms.ToolBarButton separator;
        private MenuItem menuItem6;

		private readonly MruHelper mruMenu;

		public MainWindow( string[] args, bool constructionMode ) {
//			this.additionalPluginDirectories = args;

			// set the singleton reference
			Debug.Assert(mainWindow==null);
			mainWindow = this;	// set the instance to this field

			// persist window state
			new WindowStateTracker( this,
				new RegistryPersistentWindowState( Core.userRegistry.CreateSubKey("mainWindowState") ) );
			
			// spawn file drop handler
			fileDropHandler = new FileDropHandler(this,new FileDropEventHandler(onFileDropped));

			// initialize the form
			InitializeComponent();

			this.IsMdiContainer = true;

			// set up docking manager
			this.dockingManager = new DockingManagerEx(this);
			this.dockingManager.OuterControl = statusBar;



			timer.Tick += new EventHandler(timerHandler);
			clockMenu.Popup += new EventHandler(clockMenuUpdater);

			// set toolbar bitmap
			Bitmap bmp = ResourceUtil.loadSystemBitmap("Toolbar.bmp");
			toolBarIcons.TransparentColor = bmp.GetPixel(0,0);
			toolBarIcons.Images.AddStrip(bmp);

			Bitmap bmp2 = ResourceUtil.loadSystemBitmap("DayNight.bmp");
			viewButtons.TransparentColor = bmp2.GetPixel(0,0);
			viewButtons.Images.AddStrip(bmp2);

			errorIcon = new Icon(ResourceUtil.findSystemResource("error.ico"));

			//
			// register command handlers
			//
			#region command handler registration

			// file
			new Command( commands )
				.addDialogExecuteHandler( typeof(plugin.PluginListDialog), this )
				.commandInstances.AddAll( menuItem_listPlugins );
			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(this.Close) )
				.commandInstances.AddAll( menuQuit );
			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(saveGame2) )
				.commandInstances.AddAll( MenuItem_File_Save );
			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(loadGame) )
				.commandInstances.AddAll( MenuItem_File_Open);
			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(newGame) )
				.commandInstances.AddAll( menuItem_File_New );

			// view
			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(BalanceSheetForm.create) )
				.commandInstances.Add( menuItem_balanceSheet );


			// train
			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(RailRoadController.create) )
				.commandInstances.AddAll( menuItem_RailRoadConstruction, tbRailRoad );

			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(SlopeRailRoadController.create) )
				.commandInstances.AddAll( menuItem_SlopeRailRoad, tbSlope );

			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(PlatformController.create) )
				.commandInstances.AddAll( menuItem_Platform, tbStation );

			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(StationPassagewayController.create) )
				.commandInstances.AddAll( menuItem_stationPassageway );

			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(StationaryStructPlacementController.create) )
				.commandInstances.AddAll( menuItem_railStationary, tbRRAcc );
			
			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(TrainPlacementController.create) )
				.commandInstances.AddAll( menuItem_TrainPlacement, tbTrainPlacement );

			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(showTrainControllerDialog) )
				.commandInstances.AddAll( tbTrainDiagram );

			new Command( commands )
				.addDialogExecuteHandler( typeof(TrainTradingDialog), this )
				.commandInstances.AddAll( menuItem_TrainTrading, tbTrainTrading );
			
			// road

			// other
			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(CommercialStructPlacementController.create) )
				.commandInstances.AddAll( menuItem_struct/*, tbStruct*/ );

			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(MountainController.create) )
				.commandInstances.AddAll( menuItem_terrain, tbTerrain );

			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(BulldozeController.create) )
				.commandInstances.AddAll( tbBulldoze );

			new Command( commands )
				.addExecuteHandler( new CommandHandlerNoArg(VarHeightBuildingController.create) )
				.commandInstances.AddAll( menuItem_varHeightBldg );

			new Command( commands )
				.addExecuteHandler(new CommandHandlerNoArg(LandController.create))
				.commandInstances.AddAll( menuItem_land );

			new Command( commands )
				.addExecuteHandler(new CommandHandlerNoArg(LandPropertyController.create))
				.commandInstances.AddAll( menuItem_landProperty );



			new Command( commands )
				.addDialogExecuteHandler( typeof(ConfigDialog), this )
				.commandInstances.AddAll( menuItem_option );

			new Command( commands )
				.addExecuteHandler(new CommandHandler(enableSoundEffect))
				.addUpdateHandler(new CommandHandler(updateEnableSoundEffect))
				.commandInstances.AddAll( menuItem_enableSoundEffect );

			new Command( commands )
				.addExecuteHandler(new CommandHandler(disableSoundEffect))
				.addUpdateHandler(new CommandHandler(updateDisableSoundEffect))
				.commandInstances.AddAll( menuItem_disableSoundEffect );


			// help
			new Command( commands )
				.addExecuteHandler(new CommandHandlerNoArg(AboutDialog.show))
				.commandInstances.AddAll( menuItem_About );
			new Command( commands )
				.addExecuteHandler(new CommandHandlerNoArg(showOnlineHelp))
				.commandInstances.AddAll( menuItem_onlineHelp );


			// tool bar
			new Command( commands )
				.addExecuteHandler(new CommandHandler(toggleClock))
				.addUpdateHandler(new CommandHandler(updateClock))
				.commandInstances.AddAll( tbTimer );

			// view options tool bar
			new Command( commands )
				.addExecuteHandler(new CommandHandler(viewOptionDayNightChanged))
				.addUpdateHandler(new CommandHandler(viewOptionDayNightUpdate))
				.commandInstances.Add( viewDayAndNight );
			new Command( commands )
				.addExecuteHandler(new CommandHandler(viewOptionDayNightChanged))
				.addUpdateHandler(new CommandHandler(viewOptionDayNightUpdate))
				.commandInstances.Add( viewAlwaysDay );
			new Command( commands )
				.addExecuteHandler(new CommandHandler(viewOptionDayNightChanged))
				.addUpdateHandler(new CommandHandler(viewOptionDayNightUpdate))
				.commandInstances.Add( viewAlwaysNight );

			// status bar
			new Command( commands )
				.addUpdateHandler(new CommandHandler(updateDisplayedCurrentTime))
				.commandInstances.AddAll( statusBar_Time );
			#endregion
			
			// initialize the FreeTrain framework
			using(Splash s = new Splash()) {
				s.Show();
				Application.DoEvents();

				Core.init(args,this,menuItem_music,new ProgressHandler(s.updateMessage),constructionMode);
			}

			mruMenu = new MruHelper(this,mruMenuItem);
		}

		internal MainWindow(string[] args, ProgressHandler handler){
			Debug.Assert(mainWindow==null);
			mainWindow = this;	// set the instance to this field		

			// initialize the form
			InitializeComponent();

			this.IsMdiContainer = true;

			// set up docking manager
			this.dockingManager = new DockingManagerEx(this);
			this.dockingManager.OuterControl = statusBar;

			timer.Tick += new EventHandler(timerHandler);

			// set toolbar bitmap
			Bitmap bmp = ResourceUtil.loadSystemBitmap("Toolbar.bmp");
			toolBarIcons.TransparentColor = bmp.GetPixel(0,0);
			toolBarIcons.Images.AddStrip(bmp);

			Bitmap bmp2 = ResourceUtil.loadSystemBitmap("DayNight.bmp");
			viewButtons.TransparentColor = bmp2.GetPixel(0,0);
			viewButtons.Images.AddStrip(bmp2);

			errorIcon = new Icon(ResourceUtil.findSystemResource("error.ico"));
			
			Core.init(args,this,menuItem_music,handler,false);

			//mruMenu = new MruHelper(this,mruMenuItem);
		}

		public void SetToolBarButtonHandler(string barName,int index,CommandHandlerNoArg handler)
		{
			foreach( Control c in this.Controls )
			{
				ToolBar bar = c as ToolBar;
				if(bar!=null && bar.Name.Equals(barName))
				{
						if( bar.Buttons.Count>index )
						{
							new Command( commands )
								.addExecuteHandler( handler)
								.commandInstances.AddAll( bar.Buttons[index] );
						}
				}
			}
		}

		public void setWorld( World w ) {
			// close all the views attached to the previous world
			// closing a view will modify the views set, so copy to an array first.
			IView[] vs = (IView[])views.toArray(typeof(IView));
			foreach( IView e in vs )
				e.close();
			detachController();
			Debug.Assert(views.isEmpty);

			World.setWorld(w);
			updateCaption();
			viewOptionDayNightUpdate(null);


			// open a new map view
			addView(new MapView());
		}



		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			dockingManager.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusBar_Message = new System.Windows.Forms.StatusBarPanel();
            this.statusBar_Controller = new System.Windows.Forms.StatusBarPanel();
            this.statusBar_Time = new System.Windows.Forms.StatusBarPanel();
            this.MainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem_file = new System.Windows.Forms.MenuItem();
            this.menuItem_File_New = new System.Windows.Forms.MenuItem();
            this.MenuItem_File_Open = new System.Windows.Forms.MenuItem();
            this.MenuItem_File_Save = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem_listPlugins = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.mruMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuQuit = new System.Windows.Forms.MenuItem();
            this.menuItem_view = new System.Windows.Forms.MenuItem();
            this.MenuItem_OpenMap = new System.Windows.Forms.MenuItem();
            this.menuItem_balanceSheet = new System.Windows.Forms.MenuItem();
            this.menuItem_rail = new System.Windows.Forms.MenuItem();
            this.menuItem_RailRoadConstruction = new System.Windows.Forms.MenuItem();
            this.menuItem_SlopeRailRoad = new System.Windows.Forms.MenuItem();
            this.menuItem_Platform = new System.Windows.Forms.MenuItem();
            this.menuItem_stationPassageway = new System.Windows.Forms.MenuItem();
            this.menuItem_railStationary = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem_TrainPlacement = new System.Windows.Forms.MenuItem();
            this.menuItem_TrainTrading = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem_road = new System.Windows.Forms.MenuItem();
            this.menuItem_construction = new System.Windows.Forms.MenuItem();
            this.menuItem_struct = new System.Windows.Forms.MenuItem();
            this.menuItem_terrain = new System.Windows.Forms.MenuItem();
            this.menuItem_varHeightBldg = new System.Windows.Forms.MenuItem();
            this.menuItem_land = new System.Windows.Forms.MenuItem();
            this.menuItem_landProperty = new System.Windows.Forms.MenuItem();
            this.menuItem_config = new System.Windows.Forms.MenuItem();
            this.menuItem_music = new System.Windows.Forms.MenuItem();
            this.menuItem_soundEffect = new System.Windows.Forms.MenuItem();
            this.menuItem_enableSoundEffect = new System.Windows.Forms.MenuItem();
            this.menuItem_disableSoundEffect = new System.Windows.Forms.MenuItem();
            this.menuItem_option = new System.Windows.Forms.MenuItem();
            this.menuItem_help = new System.Windows.Forms.MenuItem();
            this.menuItem_onlineHelp = new System.Windows.Forms.MenuItem();
            this.menuItem_About = new System.Windows.Forms.MenuItem();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.tbTimer = new System.Windows.Forms.ToolBarButton();
            this.clockMenu = new System.Windows.Forms.ContextMenu();
            this.menuItem_ClockStop = new System.Windows.Forms.MenuItem();
            this.menuItem_ClockGo1 = new System.Windows.Forms.MenuItem();
            this.menuItem_ClockGo2 = new System.Windows.Forms.MenuItem();
            this.menuItem_ClockGo3 = new System.Windows.Forms.MenuItem();
            this.menuItem_ClockGo4 = new System.Windows.Forms.MenuItem();
            this.tbRailRoad = new System.Windows.Forms.ToolBarButton();
            this.tbSlope = new System.Windows.Forms.ToolBarButton();
            this.tbStation = new System.Windows.Forms.ToolBarButton();
            this.tbRRAcc = new System.Windows.Forms.ToolBarButton();
            this.tbTrainPlacement = new System.Windows.Forms.ToolBarButton();
            this.tbTrainTrading = new System.Windows.Forms.ToolBarButton();
            this.tbTrainDiagram = new System.Windows.Forms.ToolBarButton();
            this.tbSeparator = new System.Windows.Forms.ToolBarButton();
            this.tbTerrain = new System.Windows.Forms.ToolBarButton();
            this.tbStruct = new System.Windows.Forms.ToolBarButton();
            this.tbBulldoze = new System.Windows.Forms.ToolBarButton();
            this.toolBarIcons = new System.Windows.Forms.ImageList(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.timerStatusBarUpdate = new System.Windows.Forms.Timer(this.components);
            this.toolBar2 = new System.Windows.Forms.ToolBar();
            this.viewDayAndNight = new System.Windows.Forms.ToolBarButton();
            this.viewAlwaysDay = new System.Windows.Forms.ToolBarButton();
            this.viewAlwaysNight = new System.Windows.Forms.ToolBarButton();
            this.separator = new System.Windows.Forms.ToolBarButton();
            this.viewButtons = new System.Windows.Forms.ImageList(this.components);
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.statusBar_Message)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBar_Controller)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBar_Time)).BeginInit();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 270);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBar_Message,
            this.statusBar_Controller,
            this.statusBar_Time});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(544, 16);
            this.statusBar.TabIndex = 0;
            // 
            // statusBar_Message
            // 
            this.statusBar_Message.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusBar_Message.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
            this.statusBar_Message.MinWidth = 40;
            this.statusBar_Message.Name = "statusBar_Message";
            this.statusBar_Message.Width = 347;
            // 
            // statusBar_Controller
            // 
            this.statusBar_Controller.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusBar_Controller.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBar_Controller.MinWidth = 0;
            this.statusBar_Controller.Name = "statusBar_Controller";
            this.statusBar_Controller.Text = "-";
            this.statusBar_Controller.ToolTipText = "Current mode";
			//! this.statusBar_Controller.ToolTipText = "現在のモード";
            this.statusBar_Controller.Width = 21;
            // 
            // statusBar_Time
            // 
            this.statusBar_Time.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusBar_Time.Name = "statusBar_Time";
            this.statusBar_Time.Text = "date/time";
            this.statusBar_Time.ToolTipText = "Current time";
			//! this.statusBar_Time.ToolTipText = "現在の時刻";
            this.statusBar_Time.Width = 160;
            // 
            // MainMenu
            // 
            this.MainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_file,
            this.menuItem_view,
            this.menuItem_rail,
            this.menuItem_road,
            this.menuItem_construction,
            this.menuItem_config,
            this.menuItem_help});
            // 
            // menuItem_file
            // 
            this.menuItem_file.Index = 0;
            this.menuItem_file.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_File_New,
            this.MenuItem_File_Open,
            this.MenuItem_File_Save,
            this.menuItem2,
            this.menuItem_listPlugins,
            this.menuItem3,
            this.mruMenuItem,
            this.menuItem1,
            this.menuQuit});
            this.menuItem_file.Text = "&File";
			//! this.menuItem_file.Text = "ファイル(&F)";
            // 
            // menuItem_File_New
            // 
            this.menuItem_File_New.Index = 0;
            this.menuItem_File_New.Text = "&New Game...";
			//! this.menuItem_File_New.Text = "新規作成&(N)...";
            // 
            // MenuItem_File_Open
            // 
            this.MenuItem_File_Open.Index = 1;
            this.MenuItem_File_Open.Text = "&Open...";
			//! this.MenuItem_File_Open.Text = "開く(&O)...";
            // 
            // MenuItem_File_Save
            // 
            this.MenuItem_File_Save.Index = 2;
            this.MenuItem_File_Save.Text = "&Save...";
			//! this.MenuItem_File_Save.Text = "保存(&S)...";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 3;
            this.menuItem2.Text = "-";
            // 
            // menuItem_listPlugins
            // 
            this.menuItem_listPlugins.Index = 4;
            this.menuItem_listPlugins.Text = "&Plugin List...";
			//! this.menuItem_listPlugins.Text = "プラグイン一覧(&P)...";
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 5;
            this.menuItem3.Text = "-";
            // 
            // mruMenuItem
            // 
            this.mruMenuItem.Index = 6;
            this.mruMenuItem.Text = "(Recently Used Files)";
			//! this.mruMenuItem.Text = "(最近使われたファイル)";
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 7;
            this.menuItem1.Text = "-";
            // 
            // menuQuit
            // 
            this.menuQuit.Index = 8;
            this.menuQuit.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
            this.menuQuit.Text = "&Quit";
			//! this.menuQuit.Text = "終了(&Q)";
            // 
            // menuItem_view
            // 
            this.menuItem_view.Index = 1;
            this.menuItem_view.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MenuItem_OpenMap,
            this.menuItem_balanceSheet});
            this.menuItem_view.MergeOrder = 1;
            this.menuItem_view.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
            this.menuItem_view.Text = "&View";
			//! this.menuItem_view.Text = "表示(&V)";
            // 
            // MenuItem_OpenMap
            // 
            this.MenuItem_OpenMap.Index = 0;
            this.MenuItem_OpenMap.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
            this.MenuItem_OpenMap.Text = "&Map";
			//! this.MenuItem_OpenMap.Text = "マップ(&M)";
            this.MenuItem_OpenMap.Click += new System.EventHandler(this.MenuItem_OpenMap_Click);
            // 
            // menuItem_balanceSheet
            // 
            this.menuItem_balanceSheet.Index = 1;
            this.menuItem_balanceSheet.Text = "&Balance Sheet";
			//! this.menuItem_balanceSheet.Text = "バランスシート(&B)";
            // 
            // menuItem_rail
            // 
            this.menuItem_rail.Index = 2;
            this.menuItem_rail.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_RailRoadConstruction,
            this.menuItem_SlopeRailRoad,
            this.menuItem_Platform,
            this.menuItem_stationPassageway,
            this.menuItem_railStationary,
            this.menuItem5,
            this.menuItem_TrainPlacement,
            this.menuItem_TrainTrading,
            this.menuItem4});
            this.menuItem_rail.MergeOrder = 2;
            this.menuItem_rail.Text = "&Rail";
			//! this.menuItem_rail.Text = "鉄道(&R)";
            // 
            // menuItem_RailRoadConstruction
            // 
            this.menuItem_RailRoadConstruction.Index = 0;
            this.menuItem_RailRoadConstruction.Text = "Lay &Rail...";
			//! this.menuItem_RailRoadConstruction.Text = "線路工事(&R)...";
            // 
            // menuItem_SlopeRailRoad
            // 
            this.menuItem_SlopeRailRoad.Index = 1;
            this.menuItem_SlopeRailRoad.Text = "Lay &Slope...";
			//! this.menuItem_SlopeRailRoad.Text = "勾配工事(&S)...";
            // 
            // menuItem_Platform
            // 
            this.menuItem_Platform.Index = 2;
            this.menuItem_Platform.Text = "Build St&ation...";
			//! this.menuItem_Platform.Text = "駅工事(&A)...";
            // 
            // menuItem_stationPassageway
            // 
            this.menuItem_stationPassageway.Index = 3;
            this.menuItem_stationPassageway.Text = "Over&pass...";
			//! this.menuItem_stationPassageway.Text = "跨線橋(&P)...";
            // 
            // menuItem_railStationary
            // 
            this.menuItem_railStationary.Index = 4;
            this.menuItem_railStationary.Text = "A&ccessories...";
			//! this.menuItem_railStationary.Text = "アクセサリ(&C)...";
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 5;
            this.menuItem5.Text = "-";
            // 
            // menuItem_TrainPlacement
            // 
            this.menuItem_TrainPlacement.Index = 6;
            this.menuItem_TrainPlacement.Text = "Place &Train...";
			//! this.menuItem_TrainPlacement.Text = "車両配置(&T)...";
            // 
            // menuItem_TrainTrading
            // 
            this.menuItem_TrainTrading.Index = 7;
            this.menuItem_TrainTrading.Text = "&Buy Trains...";
			//! this.menuItem_TrainTrading.Text = "車両購入(&B)...";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 8;
            this.menuItem4.Text = "-";
            // 
            // menuItem_road
            // 
            this.menuItem_road.Index = 3;
            this.menuItem_road.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem6});
            this.menuItem_road.MergeOrder = 3;
            this.menuItem_road.Text = "R&oad";
			//! this.menuItem_road.Text = "道路(&O)";
            // 
            // menuItem_construction
            // 
            this.menuItem_construction.Index = 4;
            this.menuItem_construction.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_struct,
            this.menuItem_terrain,
            this.menuItem_varHeightBldg,
            this.menuItem_land,
            this.menuItem_landProperty});
            this.menuItem_construction.MergeOrder = 4;
            this.menuItem_construction.Text = "&Construction";
			//! this.menuItem_construction.Text = "工事(&C)";
            // 
            // menuItem_struct
            // 
            this.menuItem_struct.Index = 0;
            this.menuItem_struct.Text = "Building &Construction...";
			//! this.menuItem_struct.Text = "建物の工事(仮)...";
            // 
            // menuItem_terrain
            // 
            this.menuItem_terrain.Index = 1;
            this.menuItem_terrain.Text = "&Modify Terrain...";
			//! this.menuItem_terrain.Text = "整地(仮)(&M)...";
            // 
            // menuItem_varHeightBldg
            // 
            this.menuItem_varHeightBldg.Index = 2;
            this.menuItem_varHeightBldg.Text = "Rental &Buildings...";
			//! this.menuItem_varHeightBldg.Text = "貸しビル(&B)...";
            // 
            // menuItem_land
            // 
            this.menuItem_land.Index = 3;
            this.menuItem_land.Text = "Terrain Object&s...";
			//! this.menuItem_land.Text = "地表(&S)...";
            // 
            // menuItem_landProperty
            // 
            this.menuItem_landProperty.Index = 4;
            this.menuItem_landProperty.Text = "Trade &Land...";
			//! this.menuItem_landProperty.Text = "土地売買(&L)...";
            // 
            // menuItem_config
            // 
            this.menuItem_config.Index = 5;
            this.menuItem_config.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_music,
            this.menuItem_soundEffect,
            this.menuItem_option});
            this.menuItem_config.MergeOrder = 5;
            this.menuItem_config.Text = "Co&nfigure";
			//! this.menuItem_config.Text = "設定(&C)";
            // 
            // menuItem_music
            // 
            this.menuItem_music.Index = 0;
            this.menuItem_music.Text = "&Music";
			//! this.menuItem_music.Text = "音楽(&M)";
            // 
            // menuItem_soundEffect
            // 
            this.menuItem_soundEffect.Index = 1;
            this.menuItem_soundEffect.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_enableSoundEffect,
            this.menuItem_disableSoundEffect});
            this.menuItem_soundEffect.Text = "&Sound Effects";
			//! this.menuItem_soundEffect.Text = "効果音(&S)";
            this.menuItem_soundEffect.Popup += new System.EventHandler(this.onMenuPopup);
            // 
            // menuItem_enableSoundEffect
            // 
            this.menuItem_enableSoundEffect.Index = 0;
            this.menuItem_enableSoundEffect.Text = "O&n";
			//! this.menuItem_enableSoundEffect.Text = "あり(&N)";
            // 
            // menuItem_disableSoundEffect
            // 
            this.menuItem_disableSoundEffect.Index = 1;
            this.menuItem_disableSoundEffect.Text = "O&ff";
			//! this.menuItem_disableSoundEffect.Text = "なし(&F)";
            // 
            // menuItem_option
            // 
            this.menuItem_option.Index = 2;
            this.menuItem_option.Text = "&Options...";
			//! this.menuItem_option.Text = "オプション(&O)...";
            // 
            // menuItem_help
            // 
            this.menuItem_help.Index = 6;
            this.menuItem_help.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_onlineHelp,
            this.menuItem_About});
            this.menuItem_help.MergeOrder = 6;
            this.menuItem_help.Text = "&Help";
			//! this.menuItem_help.Text = "ヘルプ(&H)";
            // 
            // menuItem_onlineHelp
            // 
            this.menuItem_onlineHelp.Index = 0;
            this.menuItem_onlineHelp.Text = "&Online Help";
			//! this.menuItem_onlineHelp.Text = "オンラインヘルプ(&O)";
            // 
            // menuItem_About
            // 
            this.menuItem_About.Index = 1;
            this.menuItem_About.Text = "&About FreeTrain...";
			//! this.menuItem_About.Text = "FreeTrainExについて(&A)...";
            // 
            // toolBar1
            // 
            this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbTimer,
            this.tbRailRoad,
            this.tbSlope,
            this.tbStation,
            this.tbRRAcc,
            this.tbTrainPlacement,
            this.tbTrainTrading,
            this.tbTrainDiagram,
            this.tbSeparator,
            this.tbTerrain,
            this.tbStruct,
            this.tbBulldoze,
            this.separator,
            this.viewDayAndNight,
            this.viewAlwaysDay,
            this.viewAlwaysNight});
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.ImageList = this.toolBarIcons;
            this.toolBar1.Location = new System.Drawing.Point(0, 0);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(544, 27);
            this.toolBar1.TabIndex = 1;
            this.toolBar1.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
            // 
            // tbTimer
            // 
            this.tbTimer.DropDownMenu = this.clockMenu;
            this.tbTimer.ImageIndex = 12;
            this.tbTimer.Name = "tbTimer";
            this.tbTimer.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
            this.tbTimer.Text = "Timer";
			//! this.tbTimer.Text = "時間";
            // 
            // clockMenu
            // 
            this.clockMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_ClockStop,
            this.menuItem_ClockGo1,
            this.menuItem_ClockGo2,
            this.menuItem_ClockGo3,
            this.menuItem_ClockGo4});
            // 
            // menuItem_ClockStop
            // 
            this.menuItem_ClockStop.Index = 0;
            this.menuItem_ClockStop.Text = "&Pause";
			//! this.menuItem_ClockStop.Text = "一時停止(&S)";
            this.menuItem_ClockStop.Click += new System.EventHandler(this.menuItem_ClockStop_Click);
            // 
            // menuItem_ClockGo1
            // 
            this.menuItem_ClockGo1.Index = 1;
            this.menuItem_ClockGo1.Text = "&Slow";
			//! this.menuItem_ClockGo1.Text = "ゆっくり(&S)";
            this.menuItem_ClockGo1.Click += new System.EventHandler(this.menuItem_ClockGo1_Click);
            // 
            // menuItem_ClockGo2
            // 
            this.menuItem_ClockGo2.Index = 2;
            this.menuItem_ClockGo2.Text = "&Normal";
			//! this.menuItem_ClockGo2.Text = "普通(&N)";
            this.menuItem_ClockGo2.Click += new System.EventHandler(this.menuItem_ClockGo2_Click);
            // 
            // menuItem_ClockGo3
            // 
            this.menuItem_ClockGo3.Index = 3;
            this.menuItem_ClockGo3.Text = "&Fast";
			//! this.menuItem_ClockGo3.Text = "高速(&F)";
            this.menuItem_ClockGo3.Click += new System.EventHandler(this.menuItem_ClockGo3_Click);
            // 
            // menuItem_ClockGo4
            // 
            this.menuItem_ClockGo4.Index = 4;
            this.menuItem_ClockGo4.Text = "&Ultra";
			//! this.menuItem_ClockGo4.Text = "最高速(&U)";
            this.menuItem_ClockGo4.Click += new System.EventHandler(this.menuItem_ClockGo4_Click);
            // 
            // tbRailRoad
            // 
            this.tbRailRoad.ImageIndex = 0;
            this.tbRailRoad.Name = "tbRailRoad";
            this.tbRailRoad.ToolTipText = "Lay rail";
			//! this.tbRailRoad.ToolTipText = "線路敷設";
            // 
            // tbSlope
            // 
            this.tbSlope.ImageIndex = 1;
            this.tbSlope.Name = "tbSlope";
            this.tbSlope.ToolTipText = "Lay slope";
			//! this.tbSlope.ToolTipText = "勾配線路敷設";
            // 
            // tbStation
            // 
            this.tbStation.ImageIndex = 2;
            this.tbStation.Name = "tbStation";
            this.tbStation.ToolTipText = "Build station";
			//! this.tbStation.ToolTipText = "駅建設";
            // 
            // tbRRAcc
            // 
            this.tbRRAcc.ImageIndex = 3;
            this.tbRRAcc.Name = "tbRRAcc";
            this.tbRRAcc.ToolTipText = "Build rail accessories";
			//! this.tbRRAcc.ToolTipText = "鉄道アクセサリ設置";
            // 
            // tbTrainPlacement
            // 
            this.tbTrainPlacement.ImageIndex = 4;
            this.tbTrainPlacement.Name = "tbTrainPlacement";
            this.tbTrainPlacement.ToolTipText = "Place trains";
			//! this.tbTrainPlacement.ToolTipText = "車両設置";
            // 
            // tbTrainTrading
            // 
            this.tbTrainTrading.ImageIndex = 5;
            this.tbTrainTrading.Name = "tbTrainTrading";
            this.tbTrainTrading.ToolTipText = "Buy or sell trains";
			//! this.tbTrainTrading.ToolTipText = "車両売買";
            // 
            // tbTrainDiagram
            // 
            this.tbTrainDiagram.ImageIndex = 6;
            this.tbTrainDiagram.Name = "tbTrainDiagram";
            this.tbTrainDiagram.ToolTipText = "Diagram settings";
			//! this.tbTrainDiagram.ToolTipText = "ダイアグラム設定";
            // 
            // tbSeparator
            // 
            this.tbSeparator.Name = "tbSeparator";
            this.tbSeparator.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // tbTerrain
            // 
            this.tbTerrain.ImageIndex = 7;
            this.tbTerrain.Name = "tbTerrain";
            this.tbTerrain.ToolTipText = "Raise and lower terrain";
			//! this.tbTerrain.ToolTipText = "土地の上下";
            // 
            // tbStruct
            // 
            this.tbStruct.ImageIndex = 8;
            this.tbStruct.Name = "tbStruct";
            this.tbStruct.ToolTipText = "Building construction";
			//! this.tbStruct.ToolTipText = "建物の設置";
            // 
            // tbBulldoze
            // 
            this.tbBulldoze.ImageIndex = 9;
            this.tbBulldoze.Name = "tbBulldoze";
            this.tbBulldoze.ToolTipText = "Bulldozer";
			//! this.tbBulldoze.ToolTipText = "ブルドーザー";
            // 
            // toolBarIcons
            // 
            this.toolBarIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.toolBarIcons.ImageSize = new System.Drawing.Size(16, 15);
            this.toolBarIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // timerStatusBarUpdate
            // 
            this.timerStatusBarUpdate.Enabled = true;
            this.timerStatusBarUpdate.Interval = 500;
            this.timerStatusBarUpdate.Tick += new System.EventHandler(this.updateStatusBar);
            // 
            // toolBar2
            // 
            /*this.toolBar2.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBar2.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.viewDayAndNight,
            this.viewAlwaysDay,
            this.viewAlwaysNight,
            this.separator});
            this.toolBar2.ButtonSize = new System.Drawing.Size(16, 15);
            this.toolBar2.DropDownArrows = true;
            this.toolBar2.ImageList = this.viewButtons;
            this.toolBar2.Location = new System.Drawing.Point(0, 27);
            this.toolBar2.Name = "toolBar2";
            this.toolBar2.ShowToolTips = true;
            this.toolBar2.Size = new System.Drawing.Size(544, 27);
            this.toolBar2.TabIndex = 2;*/
            // 
            // viewDayAndNight
            // 
            this.viewDayAndNight.ImageIndex = 15;
            this.viewDayAndNight.Name = "viewDayAndNight";
            this.viewDayAndNight.Pushed = true;
            this.viewDayAndNight.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.viewDayAndNight.Tag = freetrain.views.NightSpriteMode.AlignClock;
            this.viewDayAndNight.ToolTipText = "Day and night";
			//! this.viewDayAndNight.ToolTipText = "昼と夜";
            // 
            // viewAlwaysDay
            // 
            this.viewAlwaysDay.ImageIndex = 16;
            this.viewAlwaysDay.Name = "viewAlwaysDay";
            this.viewAlwaysDay.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.viewAlwaysDay.Tag = freetrain.views.NightSpriteMode.AlwaysDay;
            this.viewAlwaysDay.ToolTipText = "Always day";
			//! this.viewAlwaysDay.ToolTipText = "常に昼";
            // 
            // viewAlwaysNight
            // 
            this.viewAlwaysNight.ImageIndex = 17;
            this.viewAlwaysNight.Name = "viewAlwaysNight";
            this.viewAlwaysNight.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.viewAlwaysNight.Tag = freetrain.views.NightSpriteMode.AlwaysNight;
            this.viewAlwaysNight.ToolTipText = "Always night";
			//! this.viewAlwaysNight.ToolTipText = "常に夜";
            // 
            // separator
            // 
            this.separator.Name = "separator";
            this.separator.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // viewButtons
            // 
            this.viewButtons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.viewButtons.ImageSize = new System.Drawing.Size(16, 15);
            this.viewButtons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 0;
            this.menuItem6.Text = "-";
            // 
            // MainWindow
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            this.ClientSize = new System.Drawing.Size(544, 286);
            //this.Controls.Add(this.toolBar2);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.toolBar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.MainMenu;
            this.Name = "MainWindow";
            this.Text = "FreeTrain";
            ((System.ComponentModel.ISupportInitialize)(this.statusBar_Message)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBar_Controller)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBar_Time)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(mainWindow);
		}

		/// <summary> Reference to the single instance of the main window. </summary>
		public static MainWindow mainWindow;
		


		private static MapView _primaryMapView;

		/// <summary>
		/// Gets the "primary" map view, which will be controlled by other views.
		/// </summary>
		public static MapView primaryMapView {
			get {
				if( _primaryMapView.IsDisposed )
					_primaryMapView = null;
				return _primaryMapView;
			}
			set {
				_primaryMapView = value;
			}
		}

		#region error message
		private readonly Icon errorIcon;

		/// <summary>
		/// Sets the status bar message.
		/// 
		/// The message will disappear after the certain amount of time.
		/// </summary>
		public string statusText {
			set {
				statusBar_Message.Text = value;
				statusBar_Message.Icon = errorIcon;
				statusBarTime = DateTime.Now + new System.TimeSpan(0,0,Core.options.messageDisplayTime);
			}
		}

		private void updateStatusBar(object sender, System.EventArgs e) {
			if( statusBarTime < DateTime.Now ) {
				statusBar_Message.Text = "";
				statusBar_Message.Icon = null;
				statusBarTime = DateTime.MaxValue;
			}
		}

		[DllImport("user32.dll")]
		public static extern bool MessageBeep(uint soundtype);

		/// <summary>
		/// Reports an error.
		/// Depending on the configuration, this will either pop up a message box
		/// or just send the message to the status bar
		/// </summary>
		public static void showError( string msg ) {
			if(Core.options.showErrorMessageBox) {
				MessageBox.Show(mainWindow,msg,Application.ProductName,
					MessageBoxButtons.OK, MessageBoxIcon.Stop);
			} else {
				mainWindow.statusText = msg;
				MessageBeep(0x10);
			}
		}
//#define MB_ICONHAND                 0x00000010L
//#define MB_ICONQUESTION             0x00000020L
//#define MB_ICONEXCLAMATION          0x00000030L
//#define MB_ICONASTERISK             0x00000040L

		/// <summary>
		/// The time when the current status message should be cleared.
		/// </summary>
		private DateTime statusBarTime = DateTime.MaxValue;
		#endregion

		#region Controller management
		private ModalController controller;

		/// <summary>
		/// Currently activated controller, if any. Or null.
		/// </summary>
		public ModalController currentController { get { return controller; } }

		/// <summary>
		/// Activates a new ModalController.
		/// </summary>
		public void attachController( ModalController newHandler ) {
			if(controller==newHandler)
				return;	// already activated
			if(controller!=null)
				detachController();	// deactive the current handler first

			controller = newHandler;
			controller.onAttached();
			statusBar_Controller.Text = controller.name;

			// update all the views
			// TODO: update voxels correctly
			World.world.onAllVoxelUpdated();
		}

		/// <summary>
		/// Deactivates the current ModalController, if any.
		/// </summary>
		public void detachController() {
			if(controller==null)	return;

			controller.onDetached();
			controller=null;
			statusBar_Controller.Text = null;

			// update all the views
			// TODO: update voxels correctly
			World.world.onVoxelUpdated(world.Location.UNPLACED);
		}
		#endregion

		#region View management
		/// <summary>
		/// Set of currently registered views.
		/// </summary>
		private readonly Set views = new Set();

		public IView[] getAllViews() { return (IView[])views.toArray(typeof(IView)); }

		/// <summary>
		/// Put a new view under the control of the container.
		/// A view object shouldn't attempt to open the window by itself,
		/// but rather call this method and let the container call
		/// the show method.
		/// </summary>
		public void addView( IView newView ) {
			views.add(newView);
			newView.show(this);
		}

		/// <summary>
		/// Internal method for IView. Removes the given view.
		/// 
		/// If an user closes a view, a view should call this method
		/// to notify the MainWindow object. Note that calling this method
		/// won't close a view. To programatically close a view, call the
		/// close method of IView.
		/// </summary>
		public void removeView( IView view ) {
			views.remove(view);
		}
		#endregion

		#region save/load
		// FIXME: the save/load architecture is too ugly.

		/// <summary>
		/// Starts a new game
		/// </summary>
		private void newGame() {
			if(!saveIfNecessary()) return;

			NewWorldDialog dialog = new NewWorldDialog();
			if(dialog.ShowDialog(this)==DialogResult.OK) {
				World w = dialog.createWorld();
				if(w!=null)	setWorld(w);
			}
		}

		/// <summary>
		/// Saves the game if necessary.
		/// </summary>
		/// <returns>true if the current world can be safely destroyed.</returns>
		public bool saveIfNecessary() {
			if(World.world!=null) {
				switch(MessageBox.Show(this,"Do you want to save the current game?","FreeTrain",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question )) {
				//! switch(MessageBox.Show(this,"現在のゲームを保存しますか？","FreeTrain",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question )) {
				case DialogResult.Yes:
					if(saveGame()!=DialogResult.OK)
						return false;	// if the user didn't save the game, abort.
					break;
				case DialogResult.No:
					break;
				default:
					return false;	// in all other cases, don't start a new game
				}
			}

			return true;	// OK to proceed
		}

		private const string filterString = "Game data (*.ftgd)|*.ftgd|Game data (compatibility format) (*.ftgt)|*.ftgt";
		//! private const string filterString = "ゲームデータ (*.ftgd)|*.ftgd|ゲームデータ(互換形式) (*.ftgt)|*.ftgt";
		/// <summary>
		/// Saves the current game.
		/// </summary>
		/// <returns>DialogResult.OK if the game was in fact saved.</returns>
		private DialogResult saveGame() {
			using(SaveFileDialog sd = new SaveFileDialog()) {
				sd.FileName = World.world.name;
				sd.Filter = filterString;
				sd.RestoreDirectory = true;

				DialogResult r = sd.ShowDialog(this);

				if(r==DialogResult.OK) {
					System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

					// use the file name to update the name of the world
					World.world.name = Path.GetFileNameWithoutExtension(sd.FileName);
					updateCaption();

					// save the game
					saveGame( new FileInfo(sd.FileName) );
				}
				return r;
			}
		}

		private void saveGame( FileInfo file ) {
			Stream stream = file.OpenWrite();
			stream.WriteByte((byte)'U');
			stream.WriteByte((byte)'C');
//			stream.WriteByte((byte)'B');
//			stream.WriteByte((byte)'Z');
//			stream = new BZip2OutputStream(stream);
//			stream = new GZipOutputStream( stream );
			World.world.save( getFormatter(file), stream );
			stream.Close();
			mruMenu.addFile(file);
		}

		private void saveGame2() { saveGame(); }	// change the return type

		private void loadGame() {
			if(!saveIfNecessary()) return;

			using(OpenFileDialog ofd = new OpenFileDialog()) {
				ofd.Filter = filterString;
				ofd.RestoreDirectory = true;

				if(ofd.ShowDialog(this)==DialogResult.OK)
					loadGame( new FileInfo(ofd.FileName) );
			}
		}

		/// <summary>
		/// Loads a game from a file.
		/// </summary>
		internal void loadGame( FileInfo file ) {
			loadGame( getFormatter(file), file.OpenRead() );
			mruMenu.addFile(file);
		}
		
		private IFormatter getFormatter( FileInfo fi ) {
			return getFormatter(fi.Extension.Equals(".ftgd"));
		}
		private IFormatter getFormatter( bool binary ) {
			if( binary )
				return new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			else
				return new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
		}

		/// <summary> Load a game from a stream. </summary>
		private void loadGame( IFormatter f, Stream stream ) {
			try {
				// read the header
				int b1 = stream.ReadByte();
				int b2 = stream.ReadByte();

				if( b1=='B' && b2=='Z' ) {
					stream = new BZip2InputStream(stream);
				} else {
					// uncompressed
				}

				setWorld( World.load(f,stream) );
				stream.Close();
			} catch( Exception e ) {
				ErrorMessageBox.show(this,"Loading error",e);
				//! ErrorMessageBox.show(this,"ロードエラー",e);
			}
			stream.Close();
		}
		#endregion

		private void MenuItem_OpenMap_Click(object sender, System.EventArgs e) {
			addView(new MapView());
		}
		
		private void updateCaption() {
			this.Text = "FreeTrain - "+World.world.name;
		}


		private bool inTimerProcessing;

		private void timerHandler(object sender, EventArgs e) {

			if(inTimerProcessing)
				// avoid reentrance
				return;
			
			inTimerProcessing = true;
			// if this window is disabled, freeze the clock.
			// this would stop the game from going when an user
			// is working with a modal dialog.
			//
			// for some reson, the Enabled property doesn't work but this one does.
			if( timer.Interval==1 ) {
				for( int i=0; timer.Interval==1 && this.CanFocus && timer.Enabled; i++ ) {
					// DoEvents might launch a new clock object. So don't cache a reference
					Application.DoEvents();
					Clock clock = World.world.clock;
					clock.tick();
					clock.tick();
					if((i%60*24)==0)
						statusBar_Time.Text = clock.displayString;
				}
			} else {
				if(this.CanFocus) {
					Clock clock = World.world.clock;
					clock.tick();
					clock.tick();
				}
			}

			inTimerProcessing = false;
		}

		private void updateDisplayedCurrentTime( Command cmd ) {
			if( World.world!=null )
				cmd.Text = World.world.clock.displayString;
		}

		// update the clockMenu
		private void clockMenuUpdater( object sender, EventArgs e ) {
			menuItem_ClockStop.Checked = !timer.Enabled;
			menuItem_ClockGo1.Checked = timer.Enabled && timer.Interval==200;
			menuItem_ClockGo2.Checked = timer.Enabled && timer.Interval==100;
			menuItem_ClockGo3.Checked = timer.Enabled && timer.Interval==50;
			menuItem_ClockGo4.Checked = timer.Enabled && timer.Interval==1;
			updateTimerImage();
		}

		private void menuItem_ClockStop_Click( object sender, EventArgs e ) {
			timer.Interval=9999;	// set to some value other than those values used for other speeds
			timer.Stop();
			updateTimerImage();
		}

		private void menuItem_ClockGo1_Click(object sender, System.EventArgs e) {
			timer.Interval=200;
			timer.Start();
			updateTimerImage();
		}

		private void menuItem_ClockGo2_Click( object sender, EventArgs e ) {
			timer.Interval=100;
			timer.Start();
			updateTimerImage();
		}

		private void menuItem_ClockGo3_Click(object sender, System.EventArgs e) {
			timer.Interval=50;
			timer.Start();
			updateTimerImage();
		}

		private void menuItem_ClockGo4_Click(object sender, System.EventArgs e) {
			timer.Interval=1;
			timer.Start();
			updateTimerImage();
		}

		private void updateTimerImage()
		{
			if (timer.Enabled)
			{
				switch (timer.Interval)
				{
					case 200: this.tbTimer.ImageIndex = 11; break;
					case 100: this.tbTimer.ImageIndex = 12; break;
					case 50: this.tbTimer.ImageIndex = 13; break;
					case 1: this.tbTimer.ImageIndex = 14; break;
				}
			}
			else tbTimer.ImageIndex = 10;
		}

		private void toggleClock( Command c ) {
			timer.Enabled = !timer.Enabled;
			updateTimerImage();
		}

		private void updateClock( Command c ) {
//			c.Checked = !timer.Enabled;
			tbTimer.Pushed = !timer.Enabled;
			updateTimerImage();
		}

		private void viewOptionDayNightChanged( Command c ) 
		{
			ToolBarButton tb = (ToolBarButton)c.commandInstances[0];
			foreach(ToolBarButton tbb in toolBar1.Buttons)
				if (tbb.Tag != null)
						tbb.Pushed = (tbb==tb);
			
			World.world.viewOptions.nightSpriteMode = (NightSpriteMode)tb.Tag;
		}

		private void viewOptionDayNightUpdate( Command c ) 
		{
			try
			{
				NightSpriteMode curMode = World.world.viewOptions.nightSpriteMode;
				foreach(ToolBarButton tbb in toolBar1.Buttons)
				if (tbb.Tag != null)
						tbb.Pushed = ( curMode == (NightSpriteMode)tbb.Tag);			
			}
			catch{}
		}

		protected override void OnLoad(System.EventArgs e) {
			base.OnLoad(e);
			
			directDraw = new WindowedDirectDraw(this);

			// merge menu contributions
			foreach( MenuContribution contrib in Core.plugins.menus )
				contrib.mergeMenu(this.MainMenu);

			// add special rail contributions
			foreach( SpecialRailContribution contrib in Core.plugins.specialRails ) {
				SpecialRailPlacementHandler handler = new SpecialRailPlacementHandler(contrib);
				menuItem_rail.MenuItems.Add( handler.createMenuItem() );
			}

			// add special structure contributions
			foreach( SpecialStructureContribution contrib in Core.plugins.specialStructures ) {
				SpecialStructureHandler handler = new SpecialStructureHandler(contrib);
				menuItem_construction.MenuItems.Add( handler.createMenuItem() );
			}

			// add road contributions
			if(Core.plugins.roads.Length>0)
			{
				RoadPlacementHandler rphandler = new RoadPlacementHandler();
				menuItem_road.MenuItems.Add(0,rphandler.createMenuItem());
			}
//			int idx=0;
//			foreach( RoadContribution contrib in Core.plugins.roads ) {
//				RoadPlacementHandler handler = new RoadPlacementHandler(contrib);
//				menuItem_road.MenuItems.Add(idx++,handler.createMenuItem());
//			}
//			if(idx!=0)
//				// insert a separator
//				menuItem_road.MenuItems.Add( idx, new MenuItem("-") );

			// start a new game by default 
			setWorld(new World(new Distance(127, 127, 12), 2));

			// load the screen layout
			try 
			{
				dockingManager.LoadConfigFromFile("layout.config");
			} catch( Exception ex ) {
				// exception will be thrown if the file doesn't exist.
				Debug.WriteLine(ex.Message);
				Debug.WriteLine(ex.StackTrace);
			}

			timer.Start();
		}


		private void enableSoundEffect( Command c ) {
			Core.options.enableSoundEffect = true;
			Core.options.save();
		}
		private void disableSoundEffect( Command c ) {
			Core.options.enableSoundEffect = false;
			Core.options.save();
		}
		private void updateEnableSoundEffect( Command c ) {
			c.Enabled = Core.soundEffectManager.IsAvailable;
			c.Checked = Core.options.enableSoundEffect;
		}
		private void updateDisableSoundEffect( Command c ) {
			c.Enabled = Core.soundEffectManager.IsAvailable;
			c.Checked = !Core.options.enableSoundEffect;
		}
		private void showTrainControllerDialog() {
			DockingContribution dc = (DockingContribution)
				PluginManager.theInstance.getContribution("{CBB96A74-1201-4A26-82ED-B7A8C71EC5AD}");
			if(dc!=null)	dc.show();
		}

		private void onMenuPopup(object sender, System.EventArgs e) {
			commands.updateAll();
		}

		private void showOnlineHelp() {
			UrlInvoker.openUrl("http://freetrain.sourceforge.net/");
		}

		/// <summary> Called when a new file is dropped on this form. </summary>
		private void onFileDropped( string fileName ) {
			if(!saveIfNecessary()) return;
			
			loadGame(new FileInfo(fileName));
		}
		
		protected override void OnClosing( CancelEventArgs e ) {
			base.OnClosing(e);
			Core.options.save();
			dockingManager.SaveConfigToFile("layout.config");

			// confirm the user before we actually close the window
			// but if the game is very young, don't bother.
			// the user is not likely to have anything serious.

			if( World.world.clock.totalMinutes < 60*24*3 ) {
				e.Cancel = false;
				return;
			}

			DialogResult res = MessageBox.Show(this,"Do you want to quit without saving?",
			//! DialogResult res = MessageBox.Show(this,"ゲームを保存しないで終了しますか？",
						Application.ProductName,
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question,
						MessageBoxDefaultButton.Button2);
			e.Cancel = (res != DialogResult.Yes );
		}


		/// <summary>
		/// A class that encapsulates an instance of special purpose rail controller
		/// </summary>
		private class SpecialRailPlacementHandler {
			internal SpecialRailPlacementHandler( SpecialRailContribution _type ) {
				this.type = _type;
			}
			private readonly SpecialRailContribution type;
			private SpecialPurposeRailController controller;

			internal MenuItem createMenuItem() {
				MenuItem mi = new MenuItem();
				mi.Text = type.name;
				mi.Click += new EventHandler(handle);
				mi.Select += new EventHandler(select);
				return mi;
			}

			private void handle( object sender, EventArgs e ) {
				if( controller==null || controller.IsDisposed )
					controller = new SpecialPurposeRailController(type);
				controller.Show();
			}

			private void select( object sender, EventArgs e ) {
				MainWindow.mainWindow.statusBar.Text = type.oneLineDescription;
			}
		}

	

		/// <summary>
		/// A class that encapsulates an instance of road controller
		/// </summary>
		private class RoadPlacementHandler {
			internal RoadPlacementHandler() {
			}
			private readonly RoadContribution[] contribs;
			private RoadController controller;

			internal MenuItem createMenuItem() {
				MenuItem mi = new MenuItem();
				mi.Text = "Road Construction";
				//! mi.Text = "道路工事";
				mi.Click += new EventHandler(handle);
				mi.Select += new EventHandler(select);
				return mi;
			}

			private void handle( object sender, EventArgs e ) {
				if( controller==null || controller.IsDisposed )
					controller = new RoadController();
				controller.Show();
			}

			private void select( object sender, EventArgs e ) {
				MainWindow.mainWindow.statusBar.Text = "Lay roads.";
				//! MainWindow.mainWindow.statusBar.Text = "道路を敷設します.";
			}
		}
	
	
		/// <summary>
		/// A class that encapsulates an instance of special purpose structure controller
		/// </summary>
		private class SpecialStructureHandler {
			internal SpecialStructureHandler( SpecialStructureContribution _contrib ) {
				this.contrib = _contrib;
			}
			private readonly SpecialStructureContribution contrib;

			internal MenuItem createMenuItem() {
				MenuItem mi = new MenuItem();
				mi.Text = contrib.name;
				mi.Click += new EventHandler(handle);
				mi.Select += new EventHandler(select);
				return mi;
			}

			private void handle( object sender, EventArgs e ) {
				contrib.showDialog();
			}

			private void select( object sender, EventArgs e ) {
				MainWindow.mainWindow.statusBar.Text = contrib.oneLineDescription;
			}
		}

		private static void run( string[] args ) {
			// start the game
			Application.Run(new MainWindow(args,false));
		}
	}
}
