#if use_removed
using System;
using System.Runtime.InteropServices;
using MsHtmlHost;

namespace freetrain.controls
{
	/// <summary>
	/// IDocHostUIHandler implementation.
	/// </summary>
	public class DocHostUIHandlerImpl : IDocHostUIHandler
	{
		private readonly object externalObject;

		/// <param name="_externalObject">
		/// This object will be accessible from within HTML as "window.external"
		/// </param>
		public DocHostUIHandlerImpl( object _externalObject ) {
			this.externalObject = _externalObject;
		}

        void IDocHostUIHandler.EnableModeless(int fEnable) {
        }
        
        void IDocHostUIHandler.FilterDataObject(IDataObject pDO, out IDataObject ppDORet) {
            ppDORet = null;
        }
        
        void IDocHostUIHandler.GetDropTarget(IDropTarget pDropTarget, out IDropTarget ppDropTarget) {
            ppDropTarget = null;
        }
        
        void IDocHostUIHandler.GetExternal(out object ppDispatch) {
            ppDispatch = externalObject;
        }
        
        void IDocHostUIHandler.GetHostInfo(ref _DOCHOSTUIINFO pInfo) {
        }
        
        void IDocHostUIHandler.GetOptionKeyPath(out string pchKey, uint dw) {
            pchKey = null;
        }
        
        void IDocHostUIHandler.HideUI() {
        }
        
        void IDocHostUIHandler.OnDocWindowActivate(int fActivate) {
        }
        
        void IDocHostUIHandler.OnFrameWindowActivate(int fActivate) {
        }
        
        void IDocHostUIHandler.ResizeBorder(ref tagRECT prcBorder, IOleInPlaceUIWindow pUIWindow, int fRameWindow) {
        }
        
        void IDocHostUIHandler.ShowContextMenu(uint dwID, ref tagPOINT ppt, object pcmdtReserved, object pdispReserved) {
        }
        
        void IDocHostUIHandler.ShowUI(uint dwID, IOleInPlaceActiveObject pActiveObject, IOleCommandTarget pCommandTarget, IOleInPlaceFrame pFrame, IOleInPlaceUIWindow pDoc) {
        }
        
        void IDocHostUIHandler.TranslateAccelerator(ref tagMSG lpmsg, ref Guid pguidCmdGroup, uint nCmdID) {
			throw new COMException("", 1);	// return 1
        }
        
        void IDocHostUIHandler.TranslateUrl(uint dwTranslate, ref ushort pchURLIn, IntPtr ppchURLOut) {
        }
        
        void IDocHostUIHandler.UpdateUI() {
        }
	}
}
#endif
