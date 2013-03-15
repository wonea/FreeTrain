using System;
using System.Collections;

namespace freetrain.util
{
	/// <summary>
	/// Enumerator that works on an array
	/// </summary>
	public class ArrayEnumerator : IEnumerator
	{
		public ArrayEnumerator( Array a ) {
			this.a = a;
			idx=-1;
		}

		private readonly Array a;
		private int idx;

		public object Current { get { return a.GetValue(idx); } }
		public bool MoveNext() {
			idx++;
			return idx<a.Length;
		}
		public void Reset() { idx=-1; }
	}
}
