using System;
using freetrain.world;

namespace freetrain.framework.sound
{
	/// <summary>
	/// Sound Effect
	/// </summary>
	public interface SoundEffect
	{
		/// <summary>
		/// Requests to play this sound effect, which is conceptually
		/// generated from the specified location.
		/// 
		/// The location may or may not be used to determine whether
		/// a sound should be actually played.
		/// 
		/// A sound effect may or may not be played immediately.
		/// For example, a sound effect can accumulate requests
		/// and play it later.
		/// </summary>
		void play( Location src );
	}
}
