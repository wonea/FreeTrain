using System;
using System.Diagnostics;
using System.Net;
using System.IO;
using nft.util;

namespace nft.framework
{
	public delegate void DownloadMonitor( long readedBytes, long totalBytes );
	public delegate void DownloadCompleteHandler( FileInfo info );
	/// <summary>
	/// WebLoader の概要の説明です。
	/// </summary>
	public class WebLoader
	{
		protected string url;
		protected RequestState state;
		public DownloadMonitor DownloadProgress;
		public DownloadCompleteHandler DownloadCompleted;

		public WebLoader(string url)
		{
			this.url = url;
		}

		/// <summary>
		/// Synchronous download method.
		/// </summary>
		/// <returns></returns>
		public FileInfo Load(string destDir)
		{
			
			try
			{
				using(WebClient wc = new WebClient())
				{
					string filename = Path.Combine(	destDir , Path.GetFileName(url) );
					wc.DownloadFile(url,filename);
					return new FileInfo(filename);
				}
			}
			catch(WebException e)
			{
				Debug.WriteLine(e);
				string templ = Core.resources["webloader.web_exception"].stringValue;
				string msg = string.Format(templ,url,e.Message,e.Status);
				UIUtil.Message(msg,UIMessageType.warning,UIInformLevel.normal);
			} 
			catch(Exception e)
			{
				Debug.WriteLine(e);
				string templ = Core.resources["webloader.other_exception"].stringValue;
				UIUtil.ShowException(string.Format(templ,url),e,UIInformLevel.normal);
			}
			return null;
		}

		public void StartAsyncLoad(string destDir)
		{
			try
			{			
				lock(this)
				{
					if( state!=null ) return;
					state = new RequestState(url,destDir);
				}
				IAsyncResult async_res = state.request.BeginGetResponse(new AsyncCallback(ResponseCallback),state);				
			}
			catch(WebException e)
			{
				Debug.WriteLine(e);
				string templ = Core.resources["webloader.web_exception"].stringValue;
				string msg = string.Format(templ,url,e.Message,e.Status);
				UIUtil.Message(msg,UIMessageType.warning,UIInformLevel.normal);
			} 
			catch(Exception e)
			{
				Debug.WriteLine(e);
				string templ = Core.resources["webloader.other_exception"].stringValue;
				UIUtil.ShowException(string.Format(templ,url),e,UIInformLevel.normal);
			}
			
		}

		public void AbortAsyncLoad()
		{
			lock(this)
			{
				if( state==null ) return;
			}
			try
			{
				state.request.Abort();
			}
			catch(WebException e)
			{
				Debug.WriteLine(e);
				string templ = Core.resources["webloader.web_exception"].stringValue;
				string msg = string.Format(templ,url,e.Message,e.Status);
				UIUtil.Message(msg,UIMessageType.warning,UIInformLevel.normal);
			} 
			catch(Exception e)
			{
				Debug.WriteLine(e);
				string templ = Core.resources["webloader.other_exception"].stringValue;
				UIUtil.ShowException(string.Format(templ,url),e,UIInformLevel.normal);
			}
			finally
			{
				state.Release();
			}
		}

		//called when request end
		protected void ResponseCallback(IAsyncResult ar)
		{
			try
			{
				RequestState state = (RequestState)ar.AsyncState;

				//end request
				HttpWebResponse webres =
					(HttpWebResponse)state.request.EndGetResponse(ar);

				state.stream = webres.GetResponseStream();
				state.length = webres.ContentLength;

				//start downloading
				IAsyncResult result = state.stream.BeginRead( state.bufferData, 0, state.bufferData.Length, 
					new AsyncCallback(DownloadCallback), state);
			}
			catch(WebException e)
			{
				Debug.WriteLine(e);
				string templ = Core.resources["webloader.web_exception"].stringValue;
				string msg = string.Format(templ,url,e.Message,e.Status);
				UIUtil.Message(msg,UIMessageType.warning,UIInformLevel.normal);
			} 
			catch(Exception e)
			{
				Debug.WriteLine(e);
				string templ = Core.resources["webloader.other_exception"].stringValue;
				UIUtil.ShowException(string.Format(templ,url),e,UIInformLevel.normal);
			}
		}

		//called when async download completed;
		private void DownloadCallback(IAsyncResult ar)
		{
			try
			{
				RequestState state = (RequestState)ar.AsyncState;

				// wait async load
				int readSize = state.stream.EndRead(ar);

				if (readSize > 0)
				{
					//new data arrived.
					state.file.Write(state.bufferData, 0, readSize);

					if(	DownloadProgress!=null )
						DownloadProgress(state.ReadedBytes,state.TotalBytes);
					//set callback for further data
					IAsyncResult result = state.stream.BeginRead( state.bufferData, 0, state.bufferData.Length, 
						new AsyncCallback(DownloadCallback), state);
				}
				else
				{
					//end of stream
					state.Release();
					if(	DownloadCompleted!=null )
						DownloadCompleted(new FileInfo(state.filename));
				}
			}
			catch(WebException e)
			{
				string templ = Core.resources["webloader.web_exception"].stringValue;
				string msg = string.Format(templ,url,e.Message,e.Status);
				UIUtil.Message(msg,UIMessageType.warning,UIInformLevel.normal);
			} 
			catch(Exception e)
			{
				Debug.WriteLine(e);
				string templ = Core.resources["webloader.other_exception"].stringValue;
				UIUtil.ShowException(string.Format(templ,url),e,UIInformLevel.normal);
			}
		}

		internal protected class RequestState
		{
			public const int DefaultBufferSize = 1024;
			public readonly string url;
			public HttpWebRequest request;
			// This class stores the state of the request.
			public WebResponse response;
			// destination file;
			public readonly string filename;
			public FileStream file;

			// temporary data store buffer;
			public byte[] bufferData;
			// source stream;
			public Stream stream;

			public long ReadedBytes{ get{ return file.Length; } }
			internal long length = 0;
			public long TotalBytes{ get{ return length; } }

			public RequestState( string url, string destDir, int bufferSize)
			{
				try
				{
					this.url = url;
					filename = Path.Combine(destDir,Path.GetFileName(url));
					request = (HttpWebRequest)WebRequest.Create(url);
					response = null;
					bufferData = new byte[bufferSize];
					file = new FileStream(filename,FileMode.Create,FileAccess.Write);
				}
				catch(WebException e)
				{
					Debug.WriteLine(e);
					Release();
					string templ = Core.resources["webloader.web_exception"].stringValue;
					string msg = string.Format(templ,url,e.Message,e.Status);
					UIUtil.Message(msg,UIMessageType.warning,UIInformLevel.normal);
				} 
				catch(Exception e)
				{
					Debug.WriteLine(e);
					Release();
					string templ = Core.resources["webloader.other_exception"].stringValue;
					UIUtil.ShowException(string.Format(templ,url),e,UIInformLevel.normal);
				}
			}
			
			public RequestState(string url, string destDir) : this(url, destDir, DefaultBufferSize)
			{
			}

			public void Release()
			{
				try
				{
					if(response!=null)
					{
						response.Close();
					}
					if(file!=null)
					{
						file.Close();
						file = null;
					}
					if(stream!=null)
					{
						stream.Close();
						stream = null;
					}
					if(bufferData!=null)
						bufferData = null;
				}
				catch(Exception e)
				{
					Debug.WriteLine(e);
				}
			}
		}

	}
}
