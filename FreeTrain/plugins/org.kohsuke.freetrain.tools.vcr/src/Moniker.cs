using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using DirectShowLib;

namespace freetrain.tools.vcr
{
	/// <summary>
	/// Wrapper of IMoniker
	/// </summary>
	public class Moniker
	{
		/// <summary>
		/// Wrapped object.
		/// </summary>
		protected IMoniker core;

		public Moniker() {}
		public Moniker( IMoniker mon ) {
			this.core = mon;
		}


		/// <summary>
		/// For XML serialization use only.
		/// </summary>
		public string CLSID {
			get {
				Guid guid;
				core.GetClassID(out guid);
				return guid.ToString();
			}
			set {
				// create a new moniker instance
				Guid clsid = new Guid(value);
				Guid iif = IID_IMoniker;
				int hr = CoCreateInstance( ref clsid, IntPtr.Zero, 7/*CLSCTX_ALL*/,
					ref iif, out core );
				if(hr!=0 || core==null)		throw new COMException("unable to create a moniker",hr);
			}
		}

		/// <summary>
		/// For XML serialization use only
		/// </summary>
		public byte[] encodedState {
			get {
				IStream stream;
				CreateStreamOnHGlobal(IntPtr.Zero,true,out stream);
				core.Save(stream,-1);

				_ULARGE_INTEGER size;
				_LARGE_INTEGER zero;
				zero.QuadPart=0;
				stream.RemoteSeek( zero, 2/*END*/, out size );
				byte[] r = new byte[size.QuadPart];
				stream.RemoteSeek( zero, 0/*BGN*/, out size );
				((UCOMIStream)stream).Read( r, r.Length, IntPtr.Zero);
				return r;
			}
			set {
				IStream stream;
				CreateStreamOnHGlobal(IntPtr.Zero,true,out stream);
				((UCOMIStream)stream).Write( value, value.Length, IntPtr.Zero );
				((UCOMIStream)stream).Seek ( 0, 0, IntPtr.Zero );

				core.Load(stream);
			}
		}



		/// <summary>
		/// Binds to an object
		/// </summary>
		protected object bindToObject( Guid iid ) {
			object o;
			core.RemoteBindToObject( null, null, ref iid, out o );
			return o;
		}

		/// <summary>
		/// Binds to a storage.
		/// </summary>
		protected object bindToStorage( Guid iid ) {
			object storage;
			core.RemoteBindToStorage( null, null, ref iid, out storage );
			return storage;
		}



		[DllImport("ole32.dll")]
		private static extern int CoCreateInstance(
			ref Guid clsid, IntPtr outerUnknown, int context, ref Guid iid, out IMoniker ptr );

		[DllImport("ole32.dll")]
		private static extern uint CreateStreamOnHGlobal(
			IntPtr mem, bool deleteOnFree, out IStream stream );

		private static readonly Guid IID_IMoniker = new Guid("{0000000f-0000-0000-C000-000000000046}");

//		/// <summary>
//		/// IStream implementation backed by a .NET Stream interface.
//		/// </summary>
//		private class StreamAdaptor : IStream
//		{
//			private readonly Stream core;
//
//			public StreamAdaptor( Stream _core ) {
//				this.core = _core;
//			}
//
//			public void Clone(out DirectShowLib.IStream ppstm) {
//				ppstm = new StreamAdaptor(core);
//			}
//
//			public void Commit(uint grfCommitFlags) {
//				core.Flush();
//			}
//
//			public void LockRegion(DirectShowLib._ULARGE_INTEGER libOffset, DirectShowLib._ULARGE_INTEGER cb, uint dwLockType) {
//				throw new NotImplementedException();
//			}
//
//			public void UnlockRegion(DirectShowLib._ULARGE_INTEGER libOffset, DirectShowLib._ULARGE_INTEGER cb, uint dwLockType) {
//				throw new NotImplementedException();
//			}
//
//			public void RemoteCopyTo(DirectShowLib.IStream pstm, DirectShowLib._ULARGE_INTEGER cb, out DirectShowLib._ULARGE_INTEGER pcbRead, out DirectShowLib._ULARGE_INTEGER pcbWritten) {
//				throw new NotImplementedException();
//			}
//
//			public void Revert() {
//				throw new NotImplementedException();
//			}
//
//			public void RemoteSeek(_LARGE_INTEGER dlibMove, uint dwOrigin, out _ULARGE_INTEGER newPos) {
//				switch(dwOrigin) {
//				case 0:	// SEEK_SET
//					newPos.QuadPart = core.Seek( dlibMove.QuadPart, SeekOrigin.Begin );
//					return;
//				case 1: // SEEK_CUR
//					newPos.QuadPart = core.Seek( dlibMove.QuadPart, SeekOrigin.Current );
//					return;
//				case 2: // SEEK_END
//					newPos.QuadPart = core.Seek( dlibMove.QuadPart, SeekOrigin.End );
//					return;
//				default:
//					throw new NotImplementedException();
//				}
//			}
//
//			public void SetSize(_ULARGE_INTEGER newSize) {
//				core.SetLength(newSize.QuadPart);
//			}
//
//			public void Stat(out DirectShowLib.tagSTATSTG pstatstg, uint grfStatFlag) {
//				throw new NotImplementedException();
//			}
//
//
//		}
	}


	/// <summary>
	/// Moniker to DirectShow Video Compressor
	/// </summary>
	public class CompressorMoniker : Moniker
	{
		public CompressorMoniker() {}
		public CompressorMoniker( IMoniker mon ) : base(mon) {
			Console.WriteLine("name: "+name);
		}

		/// <summary>
		/// Get the user-friendly name.
		/// </summary>
		public string name {
			get {
				object storage;
				Guid guid = IID_IPropertyBag;
				core.RemoteBindToStorage( null, null, ref guid, out storage );
				Console.WriteLine(storage);
				IPropertyBag bag = (IPropertyBag)storage;

				object o;
				bag.RemoteRead("FriendlyName", out o, null, 0, null );
				return (string)o;
			}
		}

		/// <summary>
		/// Create a compressor instance
		/// </summary>
		/// <returns></returns>
		public IBaseFilter bind() {
			return (IBaseFilter)bindToObject(IID_IBaseFilter);
		}

		public override string ToString() {
			return name;
		}

		private static readonly Guid IID_IBaseFilter = new Guid("56a86895-0ad4-11ce-b03a-0020af0ba770");
		private static readonly Guid IID_IPropertyBag = new Guid("55272A00-42CB-11CE-8135-00AA004BB851");
		
	}
}
