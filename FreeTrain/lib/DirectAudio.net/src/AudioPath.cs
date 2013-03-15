using System;
using DxVBLibA;

namespace org.kohsuke.directaudio
{
	/// <summary>
	/// AudioPath の概要の説明です。
	/// </summary>
	public class AudioPath : IDisposable
	{
		internal DirectMusicAudioPath8 handle;

		internal AudioPath( DirectMusicAudioPath8 handle ) {
			this.handle = handle;
		}

		public void Dispose() {
			if(handle!=null) {
				System.Runtime.InteropServices.Marshal.ReleaseComObject(handle);
			}
			handle = null;
		}
	}
}
