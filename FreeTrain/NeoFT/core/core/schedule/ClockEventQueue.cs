using System;
using System.Collections;

namespace nft.core.schedule
{
	/// <summary>
	/// Priority queue implementation
	/// </summary>
	[Serializable]
	public class ClockEventQueue : IClockEventQueue
	{
		protected int arrayCapacity;
		public ClockEventQueue() : this(1){}

		public ClockEventQueue(int initialArrayCapacity) { arrayCapacity = initialArrayCapacity; }

		/// <summary>
		/// Actual data structure that realizes the priority queue.
		/// </summary>
		protected readonly SortedList list = new SortedList();

		protected int prevIndex = 0;

		protected int count = 0;
		public int Count { get { return count; }}

		/// <summary>
		/// Inserts a new object into the queue.
		/// </summary>
		public void Add( Time time, ClockHandler h ) 
		{
			int i = list.IndexOfKey(time);
			object o = new HandlerWrapper(h);
			if( i<0 ) 
			{
				ArrayList a = new ArrayList(arrayCapacity);
				a.Add(o);
				list.Add(time, a);
				i = list.IndexOfKey(time);
			}
			else
				((ArrayList)list.GetByIndex(i)).Add(o);
			if( i<prevIndex )
				prevIndex++;
			count++;
		}

		/// <summary>
		/// Removes specified handler which assigned to specified time.
		/// </summary>
		public void Remove( Time time, ClockHandler h ) 
		{
			int i = list.IndexOfKey(time);
			if( i>=0 )
				Remove(h,i);
		}

		/// <summary>
		/// Removes specified handler which assigned to specified time.
		/// This function search all indices in sequence.
		/// </summary>
		public void Remove( ClockHandler h ) 
		{
			for( int i = 0; i<list.Count; i++ )
				Remove( h, i );
		}

		protected void Remove( ClockHandler h, int index )
		{
			ArrayList a = list.GetByIndex(index) as ArrayList;
			if(a!=null)
			{
				for(int i=0; i<a.Count; i++)
				{
					HandlerWrapper hw = a[i] as HandlerWrapper;
					if(hw.o.Equals(h))
						a.RemoveAt(i);
				}
				if(a.Count==0) 
				{
					list.Remove(index);
					if( index<prevIndex )
						prevIndex--;						
				}				
			}
		}

		public virtual void Dispatch( Time to )
		{
			if(list.Count <= prevIndex ) return;
			while( ((Time)list.GetKey(prevIndex))<= to ) {
				FireEventAt(prevIndex);
				prevIndex++;
				if(prevIndex>=list.Count)
					break;
			}
		}

		public virtual void Reset()
		{
			prevIndex = 0;
		}

		/// <summary>
		/// FireHandler by index
		/// </summary>
		/// <param name="index">the index for 'list' field</param>
		protected void FireEventAt(int index)
		{
			ArrayList a = list.GetByIndex(prevIndex) as ArrayList;
			if( a!=null ) 
				foreach( HandlerWrapper hw in a )
					hw.invoke();
		}

		[Serializable]
		class HandlerWrapper 
		{
			public readonly object o;
			public HandlerWrapper(ClockHandler h)
			{
				o = h;
			}
			public void invoke()
			{
				((ClockHandler)o)();
			}
		}
	}

	[Serializable]
	class OneShotClockEventQueue : ClockEventQueue
	{
		public override void Dispatch(Time to)
		{
			if( list.Count==0 ) return;
			while( ((Time)list.GetKey(0))<= to ) 
			{
				FireEventAt(0);
				list.RemoveAt(0);
			}
		}
	}
}

