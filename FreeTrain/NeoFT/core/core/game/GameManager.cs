using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using nft.framework;
using nft.framework.plugin;
using nft.ui.mainframe;
using nft.core.schedule;
using nft.util;

namespace nft.core.game
{
	/// <summary>
	/// GameManager manages current game, game options, and save/load games.
	/// </summary>
	public class GameManager
	{		
		static private GameManager theManager;
		static public GameManager theInstance { get{ return theManager; } }

		static public void initManager( MainFrame mainFrame )
		{
			theManager = new GameManager( mainFrame );
		}

		protected MainFrame theFrame;
		protected IGameMode _curMode;
		protected IGame _curGame;
		/// <summary>
		/// Fired after a new world is loaded/created.
		/// </summary>
		public static event EventHandler OnNewGame;

		protected GameManager(MainFrame mainFrame)
		{
			this.theFrame = mainFrame;
			theFrame.Closing+=new System.ComponentModel.CancelEventHandler(theFrame_Closing);
		}

		public IGameMode CurrentMode { get{ return _curMode; } }
		public IGame CurrentGame { get{ return _curGame; } }
		public void SetGame(IGame newgame, bool prompt)
		{
			if( _curGame==newgame ) return;
			if(_curGame != null)
			{
				if( prompt )
					if( !ConfirmCloseGame() ) return;
				else
					_curGame.Close();
			}
			_curGame = newgame;
			theFrame.SetSubTitle( _curGame.Name );
			_curGame.Start();
		}

		/// <summary>
		/// Starts a new game
		/// </summary>
		protected void newGame() 
		{
			if(!ConfirmCloseGame()) return;

//			NewWorldDialog dialog = new NewWorldDialog();
//
//			if(dialog.ShowDialog(this)==DialogResult.OK) 
//			{
//				IGame game;
//				try
//				{
//					IGame game = dialog.createGame();
//				}
//				catch(Exception e)
//				{
//					string text = Main.resources["game.game_create_error"].stringValue;
//					UIUtil.ShowException(text,e,UIInformLevel.severe );
//				}
//				SetGame(game, false );
//			}
		}

		/// <summary>
		/// prompt before close current game.
		/// if user allows, current game is closed.
		/// </summary>
		/// <returns>true if the current world can be safely destroyed.</returns>
		public bool ConfirmCloseGame() 
		{
			bool b = false;
			if(_curGame!=null && _curGame.Modified) 
			{
				string text = Main.resources["game.close_game_warning"].stringValue;
				b = UIUtil.ConfirmMessage(text,UIMessageType.warning,UIInformLevel.normal);
				if( b )
				{
					_curGame.Close();
					_curGame=null;
				}
			}
			return b;
		}

		private const string filterString = "Game data (*.ftgd)|*.ftgd|Game data (compatibility format) (*.ftgt)|*.ftgt";
		//! private const string filterString = "ゲームデータ (*.ftgd)|*.ftgd|ゲームデータ(互換形式) (*.ftgt)|*.ftgt";
		/// <summary>
		/// Saves the current game.
		/// </summary>
		/// <returns>DialogResult.OK if the game was in fact saved.</returns>
		private bool saveGame() 
		{
			using(SaveFileDialog sd = new SaveFileDialog()) 
			{
//				sd.FileName = World.world.name;
//				sd.Filter = filterString;
//				sd.RestoreDirectory = true;
//
//				DialogResult r=0 = sd.ShowDialog(this);
//
//				if(r==DialogResult.OK) 
//				{
//					Cursor.Current = Cursors.WaitCursor;
//
//					// use the file name to update the name of the world
//					World.world.name = Path.GetFileNameWithoutExtension(sd.FileName);
//					updateCaption();
//
//					// save the game
//					saveGame( new FileInfo(sd.FileName) );
//				}
//				return r;
				return false;
			}
		}

		private void saveGame( FileInfo file ) 
		{
//			Stream stream = file.OpenWrite();
//			stream.WriteByte((byte)'U');
//			stream.WriteByte((byte)'C');
//			//			stream.WriteByte((byte)'B');
//			//			stream.WriteByte((byte)'Z');
//			//			stream = new BZip2OutputStream(stream);
//			//			stream = new GZipOutputStream( stream );
//			World.world.save( getFormatter(file), stream );
//			stream.Close();
//			mruMenu.addFile(file);
		}


		private void loadGame() 
		{
			if(!ConfirmCloseGame()) return;

			using(OpenFileDialog ofd = new OpenFileDialog()) 
			{
//				ofd.Filter = filterString;
//				ofd.RestoreDirectory = true;
//
//				if(ofd.ShowDialog(this)==DialogResult.OK)
//					loadGame( new FileInfo(ofd.FileName) );
			}
		}

		/// <summary>
		/// Loads a game from a file.
		/// </summary>
		internal void loadGame( FileInfo file ) 
		{
//			try
//			{
//				loadGame(file, file.OpenRead() );
//				mruMenu.addFile(file);
//			}
//			catch( Exception e )
//			{
//				string templ = Main.resources["game.game_load_error"].stringValue;
//				UIUtil.ShowException(string.Format(templ,file.Name),e,UIInformLevel.severe );
//			}
		}
		
		private IFormatter getFormatter( char formatByte ) 
		{
//			if( formatByte=='X' )
//				return new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
//			else
				return new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
		}

		/// <summary> Load a game from a stream. </summary>
		private void loadGame( Stream stream ) 
		{
//			IFormatter f;
//			try 
//			{
//				// read the header
//				int b1 = stream.ReadByte();
//				int b2 = stream.ReadByte();
//
//				f = getFormatter(b1); // first byte specifies formatter.
//
//				if( b2=='Z' ) // second byte specifies compress mode.
//				{
//					stream = new BZip2InputStream(stream);
//				} 
//				else 
//				{
//					// uncompressed
//				}
//
//				f.Binder = new PluginSerializationBinder();
//				//Main.bgmManager.currentBGM = ((BGMContribution[])f.Deserialize(stream))[0];
//				_curGame = (IGame)f.Deserialize(stream);
//			} 
//			catch( Exception e ) 
//			{
//				throw e;
//			}
//			finally
//			{
//				stream.Close();
//			}
		}

		private void theFrame_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{			
			e.Cancel = !ConfirmCloseGame();
		}
	}
}
