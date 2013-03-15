using System;
using System.Collections;

namespace freetrain.util
{
	/// <summary>
	/// java.util.Set
	/// </summary>
	[Serializable]
	public sealed class Set : ICollection {
		private readonly Hashtable core = new Hashtable();

		public bool contains( object o ) { return core.Contains(o); }
		public bool add( object o ) {
			if(!core.ContainsKey(o)) {
				core.Add(o,o);
				return true;
			} else
				return false;
		}
		public void remove( object o ) { core.Remove(o); }
		public void clear() { core.Clear(); }

		public int count { get { return core.Count; } }
		public int Count { get { return core.Count; } }

		public bool IsSynchronized { get { return false; } }
		public object SyncRoot { get { return this; } }
		public bool isEmpty { get { return count==0; } }

		public IEnumerator GetEnumerator() {
			return core.Keys.GetEnumerator();
		}

		public object getOne() {
			IEnumerator e = GetEnumerator();
			e.MoveNext();
			return e.Current;
		}

		public Array toArray(Type type) {
			Array r = Array.CreateInstance(type,count);
			int idx=0;

			foreach( object o in this )
				r.SetValue(o,idx++);
			return r;
		}

		public void CopyTo( Array array, int index ) {
			foreach( object o in this )
				array.SetValue(o, index++);
		}
	}
}
