// AlphaBlender.cpp : CAlphaBlender の実装

#include "stdafx.h"
#include "AlphaBlender.h"


DisplayMode dwMode = RGBMODE_UNCHECKED;

// CAlphaBlender

static DisplayMode getDisplayMode( DirectDrawSurface7* lpDDS )
{
	DxVBLib::DDSURFACEDESC2	ddsd;


	//
	// Initialize the surface description structure.
	//
//	memset( &ddsd, 0, sizeof ddsd );
//	ddsd.dwSize = sizeof ddsd;

	// Get the description of the surface.
	lpDDS->GetSurfaceDesc( &ddsd );

	// If we are in 32 bit mode ...
	if ( ddsd.ddpfPixelFormat.lRGBBitCount == 32 )
	{
		// ... inform the caller.
		return RGBMODE_32;
	}

	// If we are in 24 bit mode ...
	if ( ddsd.ddpfPixelFormat.lRGBBitCount == 24 )
	{
		// ... inform the caller.
		return RGBMODE_24;
	}

	// If we are in 16 bit mode ...
	if ( ddsd.ddpfPixelFormat.lRGBBitCount == 16 )
	{
		// 
		// ... determine the exact mode.
		//

		// If we are in 565 mode ...
		if ( ddsd.ddpfPixelFormat.lRBitMask == ( 31 << 11 ) &&
			 ddsd.ddpfPixelFormat.lGBitMask == ( 63 << 5 ) &&
			 ddsd.ddpfPixelFormat.lBBitMask == 31 )
		{
			// ... inform the caller.
			return RGBMODE_565;
		}

		// If we are in 555 mode ...
		if ( ddsd.ddpfPixelFormat.lRBitMask == ( 31 << 10 ) &&
			 ddsd.ddpfPixelFormat.lGBitMask == ( 31 << 5 ) &&
			 ddsd.ddpfPixelFormat.lBBitMask == 31 )
		{
			// ... inform the caller.
			return RGBMODE_555;
		}

		// We got an unknown 16 bit mode.
		return RGBMODE_16;
	}

	// Any other mode must be a palletized one.
	return (DisplayMode)-1;
}

HRESULT init( DirectDrawSurface7* pSurface ) {
	dwMode = getDisplayMode(pSurface);
	return (dwMode!=-1)?S_OK:E_UNEXPECTED;
}

extern "C" __declspec(dllexport)
LPCSTR GetDisplayModeName() {
	switch( dwMode ) {
	case RGBMODE_555:		return "16bit (555)";
	case RGBMODE_565:		return "16bit (565)";
	case RGBMODE_16:		return "16bit (unknown)";
	case RGBMODE_24:		return "24bit";
	case RGBMODE_32:		return "32bit";
	case RGBMODE_UNCHECKED:	return "Unknown";
	default:				return "ERROR!";
	}
}
