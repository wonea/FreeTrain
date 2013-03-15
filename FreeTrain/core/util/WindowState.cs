using System;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace freetrain.util
{
	/// <summary>
	/// Persist window state/location across application sessions.
	/// </summary>
	public class WindowStateTracker
	{
		public readonly Form owner;

		/// <summary> This object receives the window size/position information. </summary>
		public readonly PersistentWindowState state;
		
		public WindowStateTracker( Form _owner, PersistentWindowState _state ) {
			this.owner = _owner;
			this.state = _state;

			// subscribe to parent form's events
			owner.Closing += new System.ComponentModel.CancelEventHandler(OnClosing);
			owner.Resize += new System.EventHandler(OnResize);
			owner.Move += new System.EventHandler(OnMove);
			owner.Load += new System.EventHandler(OnLoad);

			// get initial width and height in case form is never resized
			state.size = owner.Size;
		}

		private void OnResize(object sender, EventArgs e) {
			// save width and height
			if(owner.WindowState == FormWindowState.Normal)
				state.size = owner.Size;
		}

		private void OnMove(object sender, EventArgs e) {
			// save position
			if(owner.WindowState == FormWindowState.Normal) {
				state.left = owner.Left;
				state.top = owner.Top;
			}
			// save state
			state.windowState = owner.WindowState;
		}

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			state.save();
		}

		private void OnLoad(object sender, System.EventArgs e) {
			try {
				state.load();
				// if successfully loaded, set it up
				owner.Location = new Point(state.left, state.top);
				owner.Size = state.size;
				owner.WindowState = state.windowState;
			} catch( Exception ) {
				// ignore this error
			}
		}
	}

	[Serializable]
	public abstract class PersistentWindowState
	{
		public int left, top, height, width;
		public FormWindowState windowState;
		
		internal Size size {
			get { return new Size(width,height); }
			set { width=value.Width; height=value.Height; }
		}

		public void set( PersistentWindowState rhs ) {
			this.left	= rhs.left;
			this.top	= rhs.top;
			this.height	= rhs.height;
			this.width	= rhs.width;
			this.windowState = rhs.windowState;
		}

		internal protected abstract void save();
		internal protected abstract void load();
	}

	/// <summary>
	/// Store window state in an XML file.
	/// </summary>
	public class XmlPersistentWindowState : PersistentWindowState
	{
		private string fileName;
		
		public XmlPersistentWindowState( string _fileName ) {
			this.fileName = _fileName;
		}

		// default constructor required by XML serialization
		public XmlPersistentWindowState() {}

		internal protected override void save() {
			XmlSerializer xs = new XmlSerializer(this.GetType());
			using( FileStream fs = new FileStream( fileName, FileMode.Create ) ) {
				xs.Serialize( fs, this );
			}
		}

		internal protected override void load() {
			XmlSerializer xs = new XmlSerializer(this.GetType());
			using( FileStream fs = new FileStream( fileName, FileMode.Open ) ) {
				set( (PersistentWindowState) xs.Deserialize(fs) );
			}
		}
	}

	/// <summary>
	/// PersistentWindowState that saves the state to a registry.
	/// </summary>
	public class RegistryPersistentWindowState : PersistentWindowState
	{
		private readonly RegistryKey key;

		public RegistryPersistentWindowState( RegistryKey _key ) {
			this.key = _key;
		}

		internal protected override void load() {
			left = (int)key.GetValue("Left");
			top = (int)key.GetValue("Top");
			width = (int)key.GetValue("Width");
			height = (int)key.GetValue("Height");
			windowState = (FormWindowState)key.GetValue("WindowState");
		}

		internal protected override void save() {
			// save position, size and state
			key.SetValue("Left", left);
			key.SetValue("Top", top);
			key.SetValue("Width", width);
			key.SetValue("Height", height);

			if(windowState == FormWindowState.Minimized)
				windowState = FormWindowState.Normal;

			key.SetValue("WindowState", (int)windowState);
		}
	}
	
}
