//------------------------------------------------------------------------------
// File: PushSource.H
//
// Desc: DirectShow sample code - In-memory push mode source filter
//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------
// UNITS = 10 ^ 7  
// UNITS / 30 = 30 fps;
// UNITS / 20 = 20 fps, etc

// Filter name strings
#define g_wszPushDesktop    L"PushSource Desktop Filter"





class CPushPinDesktop : public CSourceStream, public IBitmapWriter
{
protected:

    int m_FramesWritten;				// To track where we are in the file
    BOOL m_bZeroMemory;                 // Do we need to clear the buffer?
    CRefTime m_rtSampleTime;	        // The time stamp for each sample

    int m_iFrameNumber;
    REFERENCE_TIME m_rtFrameLength;		// UNITS/fps

	SIZE size;							// size of the image

    int m_iRepeatTime;                  // Time in msec between frames
    int m_nCurrentBitDepth;             // Screen bit depth

    CMediaType m_MediaType;
    CCritSec m_cSharedState;            // Protects our internal state
    CImageDisplay m_Display;            // Figures out our media type for us

public:

    CPushPinDesktop(HRESULT *phr, CSource *pFilter);
    ~CPushPinDesktop();

	DECLARE_IUNKNOWN
    STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);

    // Override the version that offers exactly one media type
    HRESULT DecideBufferSize(IMemAllocator *pAlloc, ALLOCATOR_PROPERTIES *pRequest);
	HRESULT FillBuffer(IMediaSample* pSample) {
		_ASSERT(FALSE);
		return E_NOTIMPL;
	}
    HRESULT FillBuffer(HBITMAP hBitmap,IMediaSample* pSample);
    
    // Set the agreed media type and set up the necessary parameters
    HRESULT SetMediaType(const CMediaType *pMediaType);

    // Support multiple display formats
    HRESULT CheckMediaType(const CMediaType *pMediaType);
    HRESULT GetMediaType(int iPosition, CMediaType *pmt);

    // Quality control
	// Not implemented because we aren't going in real time.
	// If the file-writing filter slows the graph down, we just do nothing, which means
	// wait until we're unblocked. No frames are ever dropped.
    STDMETHODIMP Notify(IBaseFilter *pSelf, Quality q) {
        return E_FAIL;
    }

	// ignore created secondary thread. We still need to respond to messages
	// for the compatibility with the base class.
	HRESULT DoBufferProcessingLoop(void);
	
	// should be called first to set the image size
	STDMETHOD(Init)( int cx, int cy, int fps ) {
		size.cx = cx;
		size.cy = cy;
		m_rtFrameLength = UNITS/fps;
		return S_OK;
	}

	// deliver the image through the filter	
	STDMETHOD(WriteBitmap)( DWORD _hBitmap ) {
	    IMediaSamplePtr pSample;
		HBITMAP hBitmap = reinterpret_cast<HBITMAP>(_hBitmap);

	    HRESULT hr = GetDeliveryBuffer(&pSample,NULL,NULL,0);
	    if (FAILED(hr))		return hr;

	    // Virtual function user will override.
	    hr = FillBuffer(hBitmap,pSample);
	    if (hr == S_OK) {
			hr = Deliver(pSample);

            // downstream filter returns S_FALSE if it wants us to
            // stop or an error if it's reporting an error.
            if(hr != S_OK) {
                DbgLog((LOG_TRACE, 2, TEXT("Deliver() returned %08x; stopping"), hr));
                return hr;
            }
		}

		return hr;
	}
};




class CPushSourceDesktop : public CSource, public IBitmapWriter
{

private:
    // Constructor is private because you have to use CreateInstance
    CPushSourceDesktop(IUnknown *pUnk, HRESULT *phr);
    ~CPushSourceDesktop();

    CPushPinDesktop *m_pPin;

public:
    static CUnknown * WINAPI CreateInstance(IUnknown *pUnk, HRESULT *phr);  

public:
	DECLARE_IUNKNOWN
    STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);

	// delegate calls
	STDMETHOD(Init)( int cx, int cy, int fps ) {
		return m_pPin->Init(cx,cy,fps);
	}

	STDMETHOD(WriteBitmap)( DWORD _hBitmap ) {
		return m_pPin->WriteBitmap(_hBitmap);
	}
};


