// AlphaBlender.h : CAlphaBlender の宣言

#pragma once
#include "resource.h"       // メイン シンボル

#include "DirectDraw.AlphaBlend.h"


enum DisplayMode
{
	RGBMODE_555,
	RGBMODE_565,
	RGBMODE_16,
	RGBMODE_24,
	RGBMODE_32,

	RGBMODE_UNCHECKED,
};

// detect the display mode and set dwMode
HRESULT init( DirectDrawSurface7* pSurface );

extern DisplayMode dwMode;	// current graphics mode



// color mode mask
#define RGB555(r,g,b)	(((r&0xf8)<<8)|((g&0xf8)<<2)|(b>>3))
#define RGB565(r,g,b)	(((r&0xf8)<<9)|((g&0xfc)<<2)|(b>>3))
#define	RGB888(r,g,b)	(((r   )<<16)|((g   )<<8)|(b   ))
#define RGB555to888(dw)	(((dw&0x1f)<<3)|((dw&0x3e0)<<6)|((dw&0x7c00)<<9))
#define RGB565to888(dw)	(((dw&0x1f)<<3)|((dw&0x7e0)<<5)|((dw&0xf800)<<8))
#define RGB888to555(dw)	(((dw&0xf8)>>3)|((dw&0xf800)>>6)|((dw&0xf80000)>>9))
#define RGB888to565(dw)	(((dw&0xf8)>>3)|((dw&0xfc00)>>5)|((dw&0xf80000)>>8))

// CAlphaBlender
class ATL_NO_VTABLE CAlphaBlender : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CAlphaBlender, &CLSID_AlphaBlender>,
	public IDispatchImpl<IAlphaBlender, &IID_IAlphaBlender, &LIBID_DirectDrawAlphaBlendLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
	CAlphaBlender() {}

DECLARE_REGISTRY_RESOURCEID(IDR_ALPHABLENDER)


BEGIN_COM_MAP(CAlphaBlender)
	COM_INTERFACE_ENTRY(IAlphaBlender)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()


	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct() {
		return S_OK;
	}
	
	void FinalRelease()  {}

private:


public:

	STDMETHOD(bltAlphaFast)(
		DirectDrawSurface7* pDDSDest,
		DirectDrawSurface7* pDDSSource,
		int iDestX,
		int iDestY,
		int sourceX1, int sourceY1, int sourceX2, int sourceY2,
		int colorKey );

	STDMETHOD(bltShape)(
		DirectDrawSurface7* lpDDSDest,
		DirectDrawSurface7* lpDDSSource,
		int iDestX,
		int iDestY,
		int sourceX1, int sourceY1, int sourceX2, int sourceY2,
		int fillColor,
		int colorKey );

	STDMETHOD(bltColorTransform)(
		DirectDrawSurface7* lpDDSDest,
		DirectDrawSurface7* lpDDSSource,
		int iDestX,
		int iDestY,
		int sourceX1, int sourceY1, int sourceX2, int sourceY2,
		//IntArray* srcColors,
		//IntArray* dstColors,
		int* srcColors,
		int* dstColors,
		int colorsLen,
		int colorKey,
		BOOL vflip );

	//STDMETHOD(buildNightImage)(
	//	DirectDrawSurface7* lpSurface,
	//	int x1, int y1, int x2, int y2 );
};

OBJECT_ENTRY_AUTO(__uuidof(AlphaBlender), CAlphaBlender)
