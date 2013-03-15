using System;
using System.Collections;

namespace nft.core.schedule
{
	/// <summary>
	/// Priority queue implementation
	/// </summary>
	public interface IClockEventQueue 
	{
		/// <summary>
		/// The count of queue items
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Inserts a new object into the queue.
		/// </summary>
		void Add( Time time, ClockHandler h );

		/// <summary>
		/// Removes specified handler which assigned to specified time.
		/// </summary>
		void Remove( Time time, ClockHandler h );

		/// <summary>
		/// Removes specified handler which assigned to specified time.
		/// This function search all indices in sequence.
		/// </summary>
		void Remove( ClockHandler h );

		/// <summary>
		/// Dispatch events earlier than the specified Time 'To'
		/// </summary>
		/// <param name="to"></param>
		void Dispatch( Time to );

		/// <summary>
		/// 
		/// </summary>
		void Reset();

	}

}

