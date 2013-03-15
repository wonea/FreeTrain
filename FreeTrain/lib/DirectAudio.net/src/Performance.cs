using System;
using System.Diagnostics;
using System.Windows.Forms;
using DxVBLibA;

namespace org.kohsuke.directaudio
{
	/// <summary>
	/// Performance の概要の説明です。
	/// </summary>
	public class Performance : IDisposable
	{
		internal DirectMusicPerformance8 handle;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner">owner window object</param>
		public Performance( IWin32Window owner ) {
			handle = DirectAudio.dx.DirectMusicPerformanceCreate();
			this.handle = handle;

			DMUS_AUDIOPARAMS param = new DMUS_AUDIOPARAMS();
			DirectSound8 nullDs8 = null;

			// TODO: learn more about this initialization process
			handle.InitAudio( owner.Handle.ToInt32(),
				CONST_DMUS_AUDIO.DMUS_AUDIOF_ALL,  
				ref param,
				ref nullDs8,
				CONST_DMUSIC_STANDARD_AUDIO_PATH.DMUS_APATH_DYNAMIC_STEREO, 16 );
		}

		public void Dispose() {
			if(handle!=null) {
				handle.CloseDown();
				System.Runtime.InteropServices.Marshal.ReleaseComObject(handle);
			}
			handle = null;
		}

		/// <summary>
		/// Plays the given segment exclusively.
		/// </summary>
		public SegmentState playExclusive( Segment seg ) {
			return new SegmentState( this, 
				handle.PlaySegmentEx( seg.handle, 0, 0, null, null ),
				handle.GetMusicTime()+seg.handle.GetLength() );
		}

		/// <summary>
		/// Plays the given segment immediately.
		/// </summary>
		public SegmentState play( Segment seg ) {
			return play(seg,0);
		}

		/// <summary>
		/// Plays the given segment after the specified lead time (in milliseconds)
		/// </summary>
		public SegmentState play( Segment seg, int leadTime ) {
			if( leadTime!=0 )
				leadTime = handle.ClockToMusicTime(leadTime*10*1000 + handle.GetClockTime());
			if( leadTime<0 )
				leadTime = 0;

			return new SegmentState( this, handle.PlaySegmentEx( seg.handle,
				CONST_DMUS_SEGF_FLAGS.DMUS_SEGF_SECONDARY, leadTime, null, null ),
				handle.GetMusicTime()+seg.handle.GetLength()+leadTime );
		}

		/// <summary>
		/// Creates an audio path from the properties of the given segment.
		/// </summary>
		public AudioPath createAudioPath( Segment seg ) {
			return new AudioPath( handle.CreateAudioPath( seg.handle.GetAudioPathConfig(), true ) );
		}
	}
}
