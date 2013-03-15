using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using org.kohsuke.directaudio;

namespace freetrain.framework.sound
{
	/// <summary>
	/// Coordinates sound effects.
	/// </summary>
	public sealed class SoundEffectManager
	{
		
		private bool available = false;
		public bool IsAvailable { get { return available; } }

		/// <summary>
		/// A new instance should be created only by the MainWindow class.
		/// </summary>
		public SoundEffectManager( IWin32Window owner ) 
		{
			try {
				this.performance = new Performance(owner);
				available = true;
			} catch( Exception e ) {
				MessageBox.Show( owner, e.StackTrace, "DirectAudio can not be initialized. Sound is disabled.",
				//! MessageBox.Show( owner, e.StackTrace, "DirectAudioが初期化できません。サウンドは無効です。",
					MessageBoxButtons.OK, MessageBoxIcon.Stop );
				available = false;
				Core.options.enableSoundEffect = false;
			}
		}

		private readonly Performance performance;

		/// <summary>
		/// Internal method for the SoundEffect class.
		/// 
		/// Plays a sound segment, if the sound effect is turned on.
		/// </summary>
		/// <returns>non-null if the sound is actually played.</returns>
		internal SegmentState play( Segment seg, int leadTime ) {
			if( !Core.options.enableSoundEffect|| !available )	return null;

			seg.downloadTo(performance);
			return performance.play( seg, leadTime );
		}


		[DllImport("winmm.dll")] 
		private static extern long sndPlaySound(String lpszName, long dwFlags);

		/// <summary>
		/// Plays a wav file in a synchronous way.
		/// 
		/// This method is slow compared to SoundEffect object, but 
		/// it is less resource intensive. Useful for one time sound effect
		/// that can take time.
		/// </summary>
		public static void PlaySynchronousSound( string fileName ) {
			if( Core.options.enableSoundEffect && Core.soundEffectManager.IsAvailable )
				sndPlaySound(fileName,0);
		}
		public static void PlayAsynchronousSound( string fileName ) {
			if( Core.options.enableSoundEffect && Core.soundEffectManager.IsAvailable )
				sndPlaySound(fileName,1);
		}
	}
}
