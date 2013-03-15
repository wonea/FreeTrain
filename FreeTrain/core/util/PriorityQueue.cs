using System;
using System.Collections;

namespace freetrain.util
{
	/// <summary>
	/// Priority queue implementation
	/// </summary>
	[Serializable]
	public class PriorityQueue {
		public PriorityQueue() : this(Comparer.Default) {}
		public PriorityQueue( IComparer comp ) {
			this.comparer = comp;
		}

		/// <summary>
		/// Actual data structure that realizes the priority queue.
		/// </summary>
		private readonly SortedList core = new SortedList();

		private readonly IComparer comparer;

		/// <summary>
		/// Inserts a new object into the queue.
		/// </summary>
		public void insert( object priority, object value ) {
			Entry e = new Entry(priority,value,comparer,idGen++);
			core.Add(e,e);
		}


		public delegate bool ValueComparer( object o1, object o2 );

		private ValueComparer valueComparer;

		public void setValueComparer( ValueComparer vc ) {
			this.valueComparer = valueComparer;
		}

		/// <summary>
		/// Removes all the items that has the given value.
		/// </summary>
		public void remove( object value ) {
			for( int i=core.Count-1; i>=0; i-- ) {
				Entry e = (Entry)core.GetKey(i);
				if( e.value.Equals(value) )
					core.RemoveAt(i);
			}
		}

		/// <summary>
		/// Gets the object with the lowest priority.
		/// </summary>
		public object minValue {
			get {
				return ((Entry)core.GetKey(0)).value;
			}
		}

		/// <summary>
		/// Gets the lowest priority in the queue.
		/// </summary>
		public object minPriority {
			get {
				return ((Entry)core.GetKey(0)).priority;
			}
		}

		public int count { get { return core.Count; }}

		/// <summary>
		/// Removes the object with the lowest priority.
		/// </summary>
		public void removeMin() {
			core.RemoveAt(0);
		}

		/// <summary>Used to generate unique id numbers.</summary>
		private int idGen=0;


		/// <summary>
		/// This object will be stored into the SortedList.
		/// </summary>
		[Serializable]
		protected class Entry : IComparable {
			public Entry( object p, object v, IComparer c, int _id ) {
				this.priority=p; this.value=v;
				this.comparar=c; this.id=_id;
			}
			public readonly object priority;
			public readonly object value;

			/// <summary>
			/// Unique id that is used to introduce the order relationship between
			/// two objects with the same priority.
			/// </summary>
			private readonly int id;

			private readonly IComparer comparar;

			public int CompareTo(object o) {
				Entry rhs = (Entry)o;
				
				int r;
				if(comparar!=null)
					r = comparar.Compare( priority, rhs.priority );
				else
					r = ((IComparable)priority).CompareTo( rhs.priority );

				if(r!=0)	return r;
				
				// if two priorities are the same, use the id number
				// so that no two objects will be considered equal.
				return id - rhs.id;
			}
		}
	}
}
