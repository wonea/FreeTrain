using System;
using System.Runtime.Serialization;
using System.Windows.Forms;
using freetrain.contributions.sound;
using freetrain.framework.plugin;
using org.kohsuke.directaudio;

namespace freetrain.framework
{
	/// <summary>
	/// Manages the selection and control of BGM
	/// </summary>
	public class BGMManager
	{
		/// <summary> BGM player. </summary>
		private readonly BGM bgm = new BGM();

		/// <summary> Reference to the "music" menu item. </summary>
		private readonly MenuItem musicMenu;

		internal BGMManager( MenuItem musicMenu ) {
			this.musicMenu = musicMenu;

			// "silent"
			MusicMenuItem silence = new MusicMenuItem(this,null);
			musicMenu.MenuItems.Add( silence );
			musicMenu.Popup += new EventHandler(silence.update);

			// populate BGM contributions
			foreach( BGMContribution contrib in Core.plugins.bgms ) {
				MusicMenuItem child = new MusicMenuItem(this,contrib);
				musicMenu.MenuItems.Add( child );
				musicMenu.Popup += new EventHandler(child.update);
			}

			musicMenu.MenuItems.Add( new MenuItem("-") );

			// "select from disk"
			SelectMenuItem miSelect = new SelectMenuItem(this);
			musicMenu.MenuItems.Add( miSelect );
			musicMenu.Popup += new EventHandler(miSelect.update);
		}

		private BGMContribution current = null;

		/// <summary>
		/// Sets or gets the current BGM.
		/// Set null for silence.
		/// </summary>
		public BGMContribution currentBGM {
			get {
				return current;
			}
			set {
				current = value;
				bgm.stop();
				if( current!=null ) {
					try {
						bgm.fileName = current.fileName;
						bgm.run();
					} catch( Exception e ) {
						MessageBox.Show( MainWindow.mainWindow,
							"Can not play back\n"+e.StackTrace, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error );
							//! "再生できません¥n"+e.StackTrace, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				}
			}
		}



		/// <summary>
		/// MenuItem that selects a BGM from BGMContribution.
		/// </summary>
		internal class MusicMenuItem : MenuItem
		{
			private readonly BGMContribution contrib;
			private readonly BGMManager owner;

			public MusicMenuItem() : base() {}	// for some reason Windows Forms need this constructor
			internal MusicMenuItem( BGMManager owner, BGMContribution contrib ) {
				this.contrib = contrib;
				this.owner = owner;
				if( contrib==null)
					this.Text = "Silence";
					//! this.Text = "なし";
				else
					this.Text = contrib.name;
			}

			protected override void OnClick(EventArgs e) {
				owner.currentBGM = contrib;
			}
			internal void update( object sender, EventArgs e ) {
				this.Checked = (owner.currentBGM == contrib);
			}
		}

		/// <summary>
		/// MenuItem that selects a BGM from a file.
		/// </summary>
		internal class SelectMenuItem : MenuItem
		{
			private readonly BGMManager owner;

			public SelectMenuItem() : base() {}	// for some reason Windows Forms need this constructor
			internal SelectMenuItem( BGMManager owner ) {
				this.owner = owner;
				this.Text = "&Select From File...";
				//! this.Text = "ファイルから選択(&S)...";
			}

			protected override void OnClick(EventArgs e) {
				using( OpenFileDialog ofd = new OpenFileDialog() ) {
					if( ofd.ShowDialog(MainWindow.mainWindow)==DialogResult.OK )
						owner.currentBGM = new TempBGMContribution(ofd.FileName);
				}
			}
			internal void update( object sender, EventArgs e ) {
				this.Checked = (owner.currentBGM is TempBGMContribution);
			}
		}

		/// <summary>
		/// Temporary BGM contribution created from a music in a file system.
		/// </summary>
		[Serializable]
		internal class TempBGMContribution : BGMContribution {
			internal TempBGMContribution( string fileName ) : base("temp",fileName,Guid.NewGuid().ToString()) {
			}
			// serialize this object by reference
			public override void GetObjectData( SerializationInfo info, StreamingContext context) {
				info.SetType(typeof(ReferenceImpl));
				info.AddValue("fileName",fileName);
			}
			[Serializable]
			internal new sealed class ReferenceImpl : IObjectReference {
				private string fileName=null;
				public object GetRealObject(StreamingContext context) {
					return new TempBGMContribution(fileName);
				}
			}
		}

	}
}
