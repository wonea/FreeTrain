using System;
using System.Collections;
using org.kohsuke.directaudio;
using freetrain.world;
using freetrain.views.map;

namespace freetrain.framework.sound
{
	/// <summary>
	/// SoundEffect object that handles multiple
	/// simultaneous requests in a smart way.
	/// 
	/// A sound that a train moves for example doesn't simply
	/// play five sounds simultaneously when five trains are moving.
	/// Instead, it plays just two sounds but with a short interval.
	/// 
	/// This implementation handles this kind of behavior.
	/// </summary>
	public class RepeatableSoundEffectImpl : SoundEffect
	{
		/// <summary>
		/// </summary>
		/// <param name="seg">Sound-effect object</param>
		/// <param name="concurrentPlaybackMax">Number of maximum concurrent playback.</param>
		/// <param name="intervalTime">Interval between two successive playbacks</param>
		public RepeatableSoundEffectImpl( Segment seg, int concurrentPlaybackMax, int intervalTime ) {
			this.segment = seg;
			this.concurrentPlaybackMax = concurrentPlaybackMax;
			this.intervalTime = intervalTime;
		}

		public RepeatableSoundEffectImpl( Segment seg ) : this(seg,3,200) {}

		private readonly Segment segment;

		/// <summary>
		/// SegmentState objects that represent the state
		/// of segments being played.
		/// </summary>
		private readonly ArrayList states = new ArrayList();

		/// <summary>
		/// Don't schedule more than this number of concurrent playback.
		/// </summary>
		private readonly int concurrentPlaybackMax;

		private readonly int intervalTime;


		/// <summary>
		/// Number of scheduled playbacks.
		/// </summary>
		private int queue = 0;

		/// <summary> Count the number of simltaneously played sound. </summary>
		private int countOverlap() {
			while( states.Count!=0 && !((SegmentState)states[0]).isPlaying )
				states.RemoveAt(0);
			return states.Count;
		}

		public void play( Location loc ) {
			if( !MapView.isVisibleInAny(loc) )
				return;

			if( countOverlap()+queue < concurrentPlaybackMax )
				if( queue++ == 0 )
					World.world.clock.endOfTurnHandlers += new EventHandler(onTurnEnd);
		}

		// called at the end of turn
		private void onTurnEnd( object sender, EventArgs a ) {
			int ms=0;
			for( ; queue>0; queue--,ms+=intervalTime ) {
				SegmentState st = Core.soundEffectManager.play(segment,ms);
				if(st!=null)	states.Add(st);
			}
		}
	}
}
