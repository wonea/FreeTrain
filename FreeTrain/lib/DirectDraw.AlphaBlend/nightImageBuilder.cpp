#include "stdafx.h"
#include "AlphaBlender.h"


template < WORD mask, WORD color1, WORD light1, WORD color2, WORD light2, WORD color3, WORD light3 >
inline void darken16( int iWidth, int iHeight, DWORD lPitch, WORD* lpSurface ) {
	
	const DWORD pad = (lPitch - ( iWidth * 2 ))/2;

	for( ; iHeight>0; iHeight-- ) {
		for( int i=iWidth; i>0; i-- ) {
			// Read in a pixel
			WORD pix = *lpSurface;
			
			if( pix==color1 )	pix=light1;
			else
			if( pix==color2 )	pix=light2;
			else
			if( pix==color3 )	pix=light3;
			else
				pix = (pix&mask)>>2;		// pix /= 4

			*lpSurface = pix;

			lpSurface++;
		}

		// Proceed to the next line.
		lpSurface += pad;
	}
}


// color mapping
//Color.FromArgb(8,0,0),		Color.FromArgb(255,  8,  8),
//Color.FromArgb(0,8,0),		Color.FromArgb(252,243,148),
//Color.FromArgb(0,0,8),		Color.FromArgb(255,227, 99)



__declspec(dllexport)
HRESULT buildNightImage( DirectDrawSurface7* lpSurface ) {

	DxVBLib::DDSURFACEDESC2	ddsd;

	if( dwMode==RGBMODE_UNCHECKED )
		init(lpSurface);

	lpSurface->Lock( NULL, &ddsd, /*CONST_DDLOCKFLAGS::*/DxVBLib::DDLOCK_WAIT, NULL );  

	switch ( dwMode )
	{
	case RGBMODE_555:
		darken16<0x739C,
			RGB555(8,0,0),	RGB555(255,  8,  8),
			RGB555(0,8,0),	RGB555(252,243,148),
			RGB555(0,0,8),	RGB555(255,227, 99)
		>( ddsd.lWidth, ddsd.lHeight, ddsd.lPitch, (WORD*)(ddsd.lpSurface) );
		break;
	case RGBMODE_565:
		darken16<0xE79C,
			RGB565(8,0,0),	RGB565(255,  8,  8),
			RGB565(0,8,0),	RGB565(252,243,148),
			RGB565(0,0,8),	RGB565(255,227, 99)
		>( ddsd.lWidth, ddsd.lHeight, ddsd.lPitch, (WORD*)(ddsd.lpSurface) );
		break;
	case RGBMODE_16:
		// TODO
		return E_NOTIMPL;

	case RGBMODE_24:
	case RGBMODE_32:
		const int offset = (dwMode==RGBMODE_24)?3:4;
		const DWORD pad = ddsd.lPitch - ( ddsd.lWidth * offset );
		LPBYTE lpbTarget = LPBYTE(ddsd.lpSurface);

		for( int iHeight=ddsd.lHeight; iHeight>0; iHeight-- ) {
			for( int i=ddsd.lWidth; i>0; i-- ) {
				// Read in a pixel
				DWORD pix = *reinterpret_cast<LPDWORD>(lpbTarget);
				DWORD lower24 = pix&0x00FFFFFF;

				if( lower24==RGB888(8,0,0) )	pix = (pix&0xFF000000)|RGB888(255,  8,  8);
				else
				if( lower24==RGB888(0,8,0) )	pix = (pix&0xFF000000)|RGB888(252,243,148);
				else
				if( lower24==RGB888(0,0,8) )	pix = (pix&0xFF000000)|RGB888(255,227, 99);
				else
												pix = (pix&0xFF000000)|((pix&0x00FCFCFC)>>2);// pix /= 4

				*reinterpret_cast<LPDWORD>(lpbTarget) = pix;

				lpbTarget+=offset;
			}

			// Proceed to the next line.
			lpbTarget += pad;
		}
		break;
	}

	lpSurface->Unlock( NULL );  

	return S_OK;
}
