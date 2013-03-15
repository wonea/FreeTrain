using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using DirectShowLib;
using QuartzTypeLib;
using BitmapWriterTypeLib;
using freetrain.views;
using freetrain.world;
using freetrain.framework;

namespace freetrain.tools.vcr
{
	/// <summary>
	/// Records a part of the world map.
	/// 
	/// 1. call the start method to start recording.
	/// 2. call the stop method to stop recording
	/// 3. then dispose it.
	/// 
	/// The pause method can be called to pause the recording
	/// while the object is in the recording state. Call the
	/// record method again to resume recording.
	/// </summary>
	public class Recorder : IDisposable
	{
		public enum State {
			Recording,
			Pausing,
			Stopping
		}

		private State _currentState = State.Stopping;

		private readonly IMediaControl mediaControl;
		private readonly IBitmapWriter writer;
		private readonly IGraphBuilder graph;
		
		/// <summary>
		/// Used to draw the map.
		/// </summary>
		private readonly QuarterViewDrawer drawer;
		
		

		/// <summary>
		/// Create a new recorder with the given setting.
		/// </summary>
		public Recorder( Rectangle rect, VCROptions config ) {
			drawer = new QuarterViewDrawer( World.world, MainWindow.mainWindow.directDraw, rect );

			// register timer
			World.world.clock.registerRepeated( new ClockHandler(capture), TimeLength.fromMinutes(config.period) );

			// create BitmapWriter component and configure it
			writer = (IBitmapWriter)new CVideoRecorder();
			writer.Init( rect.Width, rect.Height, config.fps );

			// create DirectShow graph builder
			ICaptureGraphBuilder2 builder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2Cls();

			// create AVIMux and file writer component
			IBaseFilter pMux;
			IFileSinkFilter unused;
			builder.SetOutputFileName( ref MEDIASUBTYPE_Avi, "test.avi", out pMux, out unused );

			builder.GetFiltergraph(out graph);

			// add BitmapWriter as the source component
			graph.AddFilter( (IBaseFilter)writer, "source" );
			// if the compressor is specified, add it
			IBaseFilter compressor = config.compressor.bind();
			graph.AddFilter( compressor, "compressor" );

			// connect components
			builder.RenderStream( IntPtr.Zero, IntPtr.Zero, writer, compressor, pMux );

			mediaControl = (IMediaControl)graph;

			// explicitly release local objects
//			Marshal.ReleaseComObject(builder);
//			Marshal.ReleaseComObject(pMux);
//			Marshal.ReleaseComObject(unused);
//			Marshal.ReleaseComObject(compressor);
		}

		public State currentState { get { return _currentState; } }


		public void Dispose() {
			stop();		// stop recording just in case the client forgets
			Marshal.ReleaseComObject(writer);
			Marshal.ReleaseComObject(mediaControl);
			Marshal.ReleaseComObject(graph);
		}

		/// <summary>
		/// Captures the current frame.
		/// </summary>
		private void capture() {
			if( currentState!=State.Recording )
				return;		// don't do this unless we are actually recording.

			Debug.WriteLine("capturing video frame");

			using( Bitmap bmp = drawer.createBitmap() ) {
				IntPtr hBmp = bmp.GetHbitmap();	
				writer.WriteBitmap( (uint)hBmp.ToInt32() );
				DeleteObject(hBmp);
			}
		}

		public void start() {
			try {
				if( currentState==State.Stopping )
					mediaControl.Run();	// start the process
				_currentState = State.Recording;
			} catch( Exception e ) {
				Debug.WriteLine(e);
				throw e;
			}
		}

		public void pause() {
			if( currentState==State.Pausing )
				throw new Exception("bad state");
			_currentState = State.Pausing;
		}

		public void stop() {
			if( currentState!=State.Stopping )
				mediaControl.Stop();	// stop the recording
			_currentState = State.Stopping;
		}



		// InterOp stuff

		[ComImport, Guid("E31244AA-0971-4992-A76B-957999AA5C9E")] 
		private class CVideoRecorder {}

		[ComImport, Guid("BF87B6E1-8C27-11d0-B3F0-00AA003761C5")]
		private class CaptureGraphBuilder2Cls {}

		private static Guid MEDIASUBTYPE_Avi = new Guid("e436eb88-524f-11ce-9f53-0020af0ba770");

		[DllImport("gdi32.dll")]
		static extern bool DeleteObject(IntPtr hObject);
	}
}
