using System;
using DxVBLibA;

namespace org.kohsuke.directaudio
{
	/// <summary>
	/// Music clip
	/// </summary>
	public class Segment : IDisposable
	{
		internal DirectMusicSegment8 handle;


		private Segment( DirectMusicSegment8 handle ) {
			this.handle = handle;
		}
		
		public static Segment fromFile( string fileName ) {
			try {
				return new Segment( DirectAudio.loader.LoadSegment(fileName) );
			} catch( Exception e ) {
				throw new Exception("unable to load music file: "+fileName,e);
			}
		}

		public static Segment fromMidiFile( string fileName ) {
			Segment seg = fromFile(fileName);
			seg.handle.SetStandardMidiFile();
			return seg;
		}


		public void Dispose() {
			if(handle!=null) {
				System.Runtime.InteropServices.Marshal.ReleaseComObject(handle);
			}
			handle = null;
		}



		/// <summary>
		/// Prepares this sound object for the play by the performance object.
		/// </summary>
		public void downloadTo( Performance p ) {
			handle.Download( p.handle.GetDefaultAudioPath() );
		}

		/// <summary>
		/// Reverses the effect of the downloadTo method.
		/// </summary>
		public void unloadFrom( Performance p ) {
			handle.Unload( p.handle.GetDefaultAudioPath() );
		}

		public Segment clone() {
			return new Segment( handle.Clone(0,0) );
		}

		public int repeats {
			get {
				return handle.GetRepeats();
			}
			set {
				handle.SetRepeats(value);
			}
		}

	}
}
