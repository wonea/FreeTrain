using System;
using System.Windows.Forms;

namespace freetrain.controls
{
	/// <summary> Delegate that received dropped files. </summary>
	public delegate void FileDropEventHandler( string fileName );


	/// <summary>
	/// Encapsulates Drag&Drop logic for Windows Forms.
	/// </summary>
	public class FileDropHandler
	{
		private FileDropEventHandler handler;

		public FileDropHandler( Control parent, FileDropEventHandler handler ) {
			this.handler = handler;

			parent.AllowDrop = true;
			parent.DragEnter += new DragEventHandler(onDragEnter);
			parent.DragDrop  += new DragEventHandler(onDragDrop);
		}

		private void onDragEnter( object sender, DragEventArgs args ) {
			// make sure they're actually dropping files (not text or anything else)
			if( args.Data.GetDataPresent(DataFormats.FileDrop, false) == true )
			// allow them to continue
			// (without this, the cursor stays a "NO" symbol
			args.Effect = DragDropEffects.All;
		}

		private void onDragDrop( object sender, DragEventArgs args ) {
			string[] files = (string[])args.Data.GetData(DataFormats.FileDrop);

			// report dropped files
			foreach( string file in files )
				onFileDropped(file);
		}

		/// <summary>
		/// Overridable. This method is called for each dropped file.
		/// </summary>
		/// <param name="fileName"></param>
		protected virtual void onFileDropped( string fileName ) {
			handler(fileName);
		}
	}
}
