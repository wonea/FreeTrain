using System;
using System.Collections;
using System.Runtime.InteropServices;
using DirectShowLib;

namespace freetrain.tools.vcr
{
	/// <summary>
	/// DirectShow utility classes
	/// </summary>
	internal class DirectShowUtil
	{
		[ComImport, Guid("62BE5D10-60EB-11d0-BD3B-00A0C911CE86")]
		private class SystemDeviceEnum {}

		static Guid VideoCompressorCategory = new Guid("33D9A760-90C8-11d0-BD43-00A0C911CE86");

		/// <summary>
		/// Enumerates all video compressors.
		/// </summary>
		public static CompressorMoniker[] EnumCompressors() {
			ArrayList r = new ArrayList();

			ICreateDevEnum devEnum = (ICreateDevEnum)new SystemDeviceEnum();
			IEnumMoniker e;
			devEnum.CreateClassEnumerator( ref VideoCompressorCategory, out e, 0 );
			e.Reset();
			while(true) {
				uint count;
				IMoniker moniker=null;
				e.RemoteNext( 1, out moniker, out count );
				if( moniker==null || count==0)	break;

				r.Add(new CompressorMoniker(moniker));
				Marshal.ReleaseComObject(moniker);
			}
			Marshal.ReleaseComObject(e);
			Marshal.ReleaseComObject(devEnum);

			return (CompressorMoniker[])r.ToArray(typeof(CompressorMoniker));
		}
	}
}
