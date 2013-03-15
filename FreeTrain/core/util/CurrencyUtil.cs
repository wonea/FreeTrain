using System;
using System.Text;

namespace freetrain.util
{
	/// <summary>
	/// Currency converter.
	/// </summary>
	public class CurrencyUtil
	{
		/// <summary>
		/// Format to a string
		/// </summary>
		public static string format( long v ) {
			string r="";
			while(v>=1000) {
				r = ',' + (v%1000).ToString("000") + r;
				v /= 1000;
			}
			r = v.ToString() + r;

			return r;
		}
	}
}
