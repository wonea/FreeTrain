using System;
using System.Runtime.InteropServices;

namespace nft.win32util
{
	/// <summary>
	/// Get global memory informations.
	/// each values are obtained when the object created. 
	/// </summary>
	public class GlobalMemoryInfo
	{

		[DllImport("kernel32.dll")]
		extern static void GlobalMemoryStatus(ref MemoryStatus lpBuffer) ;

		struct MemoryStatus
		{		
			public int dwLength ;
			public int dwMemoryLoad ;
			public int dwTotalPhys ; // physical memory total
			public int dwAvailPhys ; // physical memory available
			public int dwTotalPageFile ; // page files total
			public int dwAvailPageFile ; // page files available
			public int dwTotalVirtual ; // virtual memory total
			public int dwAvailVirtual ; // virtual memory available
		}
		
		private readonly MemoryStatus msinfo;

		public GlobalMemoryInfo()
		{
			msinfo = new MemoryStatus();
			msinfo.dwLength = Marshal.SizeOf(msinfo);			
			GlobalMemoryStatus(ref msinfo);
		}

		public int PhysicalMemTotal	{ get { return msinfo.dwTotalPhys; } }
		public int PhysicalMemAvailable	{ get { return msinfo.dwAvailPhys; } }
		public int PageFileTotal { get { return msinfo.dwTotalPageFile; } }
		public int PageFileAvailable { get { return msinfo.dwAvailPageFile; } }
		public int VirtualMemTotal	{ get { return msinfo.dwTotalVirtual; } }
		public int VirtualMemAvailable	{ get { return msinfo.dwAvailVirtual; } }
	}
}
