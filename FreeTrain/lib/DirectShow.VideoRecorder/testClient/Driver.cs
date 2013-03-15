using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using BitmapWriterTypeLib;
using QuartzTypeLib;
using DirectShowLib;
using Microsoft.Win32;

namespace TestClient
{
	class Compressor : IDisposable
	{
		internal Compressor( IMoniker m ) {
			object _bag;
			IPropertyBag bag;

			Guid IID_IPropertyBag = new Guid("55272A00-42CB-11CE-8135-00AA004BB851");
			m.RemoteBindToStorage( null, null, ref IID_IPropertyBag, out _bag );
			bag = (IPropertyBag)_bag;

			object o;
			bag.RemoteRead("FriendlyName", out o, null, 0, null );
			name = (string)o;

			Guid IID_IBaseFilter = new Guid("56a86895-0ad4-11ce-b03a-0020af0ba770");
			m.RemoteBindToObject( null, null, ref IID_IBaseFilter, out o );
			compressor = (IBaseFilter)o;
		}

		public readonly string name;
		public readonly IBaseFilter compressor;

		public void Dispose() {
			Marshal.ReleaseComObject(compressor);
		}
	}

	class Driver
	{
		[ComImport, Guid("E31244AA-0971-4992-A76B-957999AA5C9E")] 
		private class CVideoRecorder {}
		
		[ComImport, Guid("BF87B6E1-8C27-11d0-B3F0-00AA003761C5")]
		private class CaptureGraphBuilder2Cls {}

		[ComImport, Guid("62BE5D10-60EB-11d0-BD3B-00A0C911CE86")]
		private class SystemDeviceEnum {}

		static Guid VideoCompressorCategory = new Guid("33D9A760-90C8-11d0-BD43-00A0C911CE86");
		static Guid MEDIASUBTYPE_Avi = new Guid("e436eb88-524f-11ce-9f53-0020af0ba770");

		[DllImport("gdi32.dll")]
		static extern bool DeleteObject(IntPtr hObject);

		/// <summary>
		/// Enum compressor devices
		/// </summary>
		static Compressor[] EnumCompressors() {
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

				r.Add(new Compressor(moniker));
				Marshal.ReleaseComObject(moniker);
			}
			Marshal.ReleaseComObject(e);
			Marshal.ReleaseComObject(devEnum);

			return (Compressor[])r.ToArray(typeof(Compressor));
		}


		[STAThread]
		static void Main(string[] args)
		{
			Compressor[] comp = EnumCompressors();
			foreach( Compressor c in comp )
				Console.WriteLine(c.name);

			Console.WriteLine("---");

			// create BitmapWriter component and configure it
			IBitmapWriter writer = (IBitmapWriter)new CVideoRecorder();
			writer.Init(100,100,5);

			// create DirectShow graph builder
			ICaptureGraphBuilder2 builder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2Cls();

			// create AVIMux and file writer component
			IBaseFilter pMux;
			IFileSinkFilter unused;
			builder.SetOutputFileName( ref MEDIASUBTYPE_Avi, "test.avi", out pMux, out unused );

			IGraphBuilder graph;
			builder.GetFiltergraph(out graph);

			// add BitmapWriter as the source component
			graph.AddFilter( (IBaseFilter)writer, "source" );
			// if the compressor is specified, add it
			IBaseFilter compressor=null;
			if( args.Length!=0 ) {
				compressor = comp[int.Parse(args[0])].compressor;
				graph.AddFilter( compressor, "compressor" );
			}

			// connect components
			builder.RenderStream( IntPtr.Zero, IntPtr.Zero, writer, compressor, pMux );

			// run the graph and send images
			IMediaControl control = (IMediaControl)graph;
			control.Run();
			for( int i=0; i<10; i++ ) {
				Bitmap bmp = new Bitmap(@"..\..\test"+(i%2)+".bmp");
				IntPtr hBmp = bmp.GetHbitmap();	
				writer.WriteBitmap( (uint)hBmp.ToInt32() );
				DeleteObject(hBmp);
				bmp.Dispose();
			}

			// stop the graph and flash any pending data throught the pipeline
			control.Stop();

			Console.WriteLine("done");
		}
	}
}
