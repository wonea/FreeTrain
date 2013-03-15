#include "stdafx.h"
#include "AlphaBlender.h"


STDMETHODIMP CAlphaBlender::bltShape(
	DirectDrawSurface7* lpDDSDest,
	DirectDrawSurface7* lpDDSSource,
	int iDestX,
	int iDestY,
	int sourceX1, int sourceY1, int sourceX2, int sourceY2,
	int fillColor,
	int colorKey ) {

	DxVBLib::DDSURFACEDESC2	ddsdSource;
	DxVBLib::DDSURFACEDESC2	ddsdTarget;
	DWORD			dwTargetPad;
	DWORD			dwSourcePad;
	DWORD			dwTargetTemp;
	DWORD			dwSourceTemp;
	WORD			wMask;
	DWORD			dwDoubleMask;
	BYTE*			lpbTarget;
	BYTE*			lpbSource;
	int				iWidth;
	int				iHeight;
	bool			gOddWidth;
	int				iRet = 0;
	int				i;


	if( dwMode==RGBMODE_UNCHECKED )
		init(lpDDSDest);

	//
	// Determine the dimensions of the source surface.
	//
//	if ( lprcSource )
	{
		//
		// Get the width and height from the passed rectangle.
		//
		iWidth =  sourceX2 -  sourceX1;
		iHeight = sourceY2 - sourceY1; 
	}
/*	else
	{
		//
		// Get the with and height from the surface description.
		//
///		memset( &ddsdSource, 0, sizeof ddsdSource );
///		ddsdSource.lSize = sizeof ddsdSource;
///		ddsdSource.dwFlags = DDSD_WIDTH | DDSD_HEIGHT;
/// ? couldn't figure out how to set this flag
		lpDDSSource->GetSurfaceDesc( &ddsdSource );

		//
		// Remember the dimensions.
		//
		iWidth = ddsdSource.lWidth;
		iHeight = ddsdSource.lHeight;
	}
*/


	//
	// Lock down the destination surface.
	//
///	memset( &ddsdTarget, 0, sizeof ddsdTarget );
///	ddsdTarget.dwSize = sizeof ddsdTarget;
	lpDDSDest->Lock( NULL, &ddsdTarget, /*CONST_DDLOCKFLAGS::*/DxVBLib::DDLOCK_WAIT, NULL );  

	//
	// Lock down the source surface.
	//
///	memset( &ddsdSource, 0, sizeof ddsdSource );
///	ddsdSource.dwSize = sizeof ddsdSource;
	lpDDSSource->Lock( NULL, &ddsdSource, /*CONST_DDLOCKFLAGS::*/DxVBLib::DDLOCK_WAIT, NULL );


/// Now this might be my problem, but ddsdTarget.lpSurface
/// doesn't seem to correctly reflect the lock region.
/// so I modified the code to lock the entire region and adjust lpSurface afterward.

	
	// clipping
	if(iDestX<0) {
		sourceX1 -= iDestX;
		iWidth += iDestX;
		iDestX=0;
	}
	if(iDestY<0) {
		sourceY1 -= iDestY;
		iHeight += iDestY;
		iDestY=0;
	}
	int extra;
	extra = (iDestX+iWidth)-ddsdTarget.lWidth;
	if(extra>0) {
		sourceX2 -= extra;
		iWidth -= extra;
	}
	extra = (iDestY+iHeight)-ddsdTarget.lHeight;
	if(extra>0) {
		sourceY2 -= extra;
		iHeight -= extra;
	}
	if( iWidth<=0 || iHeight<=0 ) {
		lpDDSDest->Unlock( NULL );
		lpDDSSource->Unlock( NULL );
		return true;	// no region to draw
	}

	//
	// Perform the blit operation.
	//
	switch ( dwMode )
	{
	case RGBMODE_555:
	case RGBMODE_565:
	case RGBMODE_16:
		ddsdTarget.lpSurface += ddsdTarget.lPitch*iDestY + iDestX*2;
		ddsdSource.lpSurface += ddsdSource.lPitch*sourceY1 + sourceX1*2;
		//
		// Determine the padding bytes for the target and the source.
		//
		dwTargetPad = ddsdTarget.lPitch - ( iWidth * 2 );
		dwSourcePad = ddsdSource.lPitch - ( iWidth * 2 );

		// If the width is odd ...
		if ( iWidth & 0x01 )
		{
			// ... set the flag ...
			gOddWidth = true;

			// ... and calculate the width.
			iWidth = ( iWidth - 1 ) / 2;
		}
		// If the width is even ...
		else
		{
			// ... clear the flag ...
			gOddWidth = false;

			// ... and calculate the width.
			iWidth /= 2;
		}

		// Create the bit mask used to clear the lowest bit of each color channel´s mask.
		wMask = ( WORD ) ( ( ddsdTarget.ddpfPixelFormat.lRBitMask & 
						   ( ddsdTarget.ddpfPixelFormat.lRBitMask << 1 ) ) | 
						   ( ddsdTarget.ddpfPixelFormat.lGBitMask & 
						   ( ddsdTarget.ddpfPixelFormat.lGBitMask << 1 ) ) | 
						   ( ddsdTarget.ddpfPixelFormat.lBBitMask & 
						   ( ddsdTarget.ddpfPixelFormat.lBBitMask << 1 ) ) );

		// Create a double bit mask.
		dwDoubleMask = wMask | ( wMask << 16 );

		// Get the address of the target.
		lpbTarget = ( BYTE* ) ddsdTarget.lpSurface;

		// Get the address of the source.
		lpbSource = ( BYTE* ) ddsdSource.lpSurface;

		do
		{
			// Reset the width.
			i = iWidth;

			// 
			// Alpha-blend two pixels at once.
			//
			while ( i-- > 0 )
			{
				// Read in two source pixels.
				dwSourceTemp = *( ( DWORD* ) lpbSource );

				// If the two source pixels are not both black ...
				if ( dwSourceTemp != colorKey )
				{
					// ... read in two target pixels.
					dwTargetTemp = *( ( DWORD* ) lpbTarget );

					// If the first source is not black ...
					if ( ( dwSourceTemp >> 16 ) != colorKey )
					{
						dwTargetTemp = (fillColor<<16) | (dwTargetTemp&0x0000FFFF);
					}

					// If the second source is not black ...
					if ( ( dwSourceTemp & 0xffff ) != colorKey )
					{
						// ... make sure the second target pixel won´t change.
						dwTargetTemp = (dwTargetTemp & 0xFFFF0000)|fillColor;
					}

					// Write the destination pixels.
					*( ( DWORD* ) lpbTarget ) = dwTargetTemp;
				}

				//
				// Proceed to the next two pixels.
				//
				lpbTarget += 4;
				lpbSource += 4;
			}

			//
			// Handle an odd width.
			//
			if ( gOddWidth )
			{
				// Read in one source pixel.
				dwSourceTemp = *( ( WORD* ) lpbSource );

				// If this is not the color key ...
				if ( dwSourceTemp != colorKey )
				{
					*( ( WORD* ) lpbTarget ) = fillColor;
				}

				// 
				// Proceed to next pixel.
				//
				lpbTarget += 2;
				lpbSource += 2;
			}

			//
			// Proceed to the next line.
			//
			lpbTarget += dwTargetPad;
			lpbSource += dwSourcePad;
		} 
		while ( --iHeight > 0 );

		break;

	/* 24 bit mode. */
	case RGBMODE_24:
		ddsdTarget.lpSurface += ddsdTarget.lPitch*iDestY + iDestX*3;
		ddsdSource.lpSurface += ddsdSource.lPitch*sourceY1 + sourceX1*3;
		
		//
		// Determine the padding bytes for the target and the source.
		//
		dwTargetPad = ddsdTarget.lPitch - ( iWidth * 3 );
		dwSourcePad = ddsdSource.lPitch - ( iWidth * 3 );

		// Get the address of the target.
		lpbTarget = ( BYTE* ) ddsdTarget.lpSurface;

		// Get the address of the source.
		lpbSource = ( BYTE* ) ddsdSource.lpSurface;

		do
		{
			// Reset the width.
			i = iWidth;

			// 
			// Alpha-blend the pixels in the current row.
			//
			while ( i-- > 0 )
			{
				// Read in the next source pixel.
				dwSourceTemp = *( ( DWORD* ) lpbSource );	

				// If the source pixel is not black ...
				if ( ( dwSourceTemp & 0x00ffffff ) != colorKey )
				{
					dwTargetTemp = fillColor;

					//
					// Write the destination pixel.
					//
					*( ( WORD* ) lpbTarget ) = ( WORD ) dwTargetTemp;
					lpbTarget += 2;
					*lpbTarget = ( BYTE ) ( dwTargetTemp >> 16 );
					lpbTarget++;
				}
				// If the source pixel is our color key ...
				else
				{
					// ... advance the target pointer.
					lpbTarget += 3;
				}

				// Proceed to the next source pixel.
				lpbSource += 3;
			}

			//
			// Proceed to the next line.
			//
			lpbTarget += dwTargetPad;
			lpbSource += dwSourcePad;
		}
		while  ( --iHeight > 0 );

		break;

	/* 32 bit mode. */
	case RGBMODE_32:
		ddsdTarget.lpSurface += ddsdTarget.lPitch*iDestY + iDestX*4;
		ddsdSource.lpSurface += ddsdSource.lPitch*sourceY1 + sourceX1*4;

		//
		// Determine the padding bytes for the target and the source.
		//
		dwTargetPad = ddsdTarget.lPitch - ( iWidth * 4 );
		dwSourcePad = ddsdSource.lPitch - ( iWidth * 4 );

		// Get the address of the target.
		lpbTarget = ( BYTE* ) ddsdTarget.lpSurface;

		// Get the address of the source.
		lpbSource = ( BYTE* ) ddsdSource.lpSurface;

		do
		{
			// Reset the width.
			i = iWidth;

			// 
			// Alpha-blend the pixels in the current row.
			//
			while ( i-- > 0 )
			{
				// Read in the next source pixel.
				dwSourceTemp = *( ( DWORD* ) lpbSource );	

				// If the source pixel is not black ...
				if ( ( dwSourceTemp & 0xffffff ) != colorKey )
				{
					// Write the destination pixel.
					*( ( DWORD* ) lpbTarget ) = fillColor;
				}

				//
				// Proceed to the next pixel.
				//
				lpbTarget += 4;
				lpbSource += 4;
			}

			//
			// Proceed to the next line.
			//
			lpbTarget += dwTargetPad;
			lpbSource += dwSourcePad;
		}
		while  ( --iHeight > 0 );

		break;

	/* Invalid mode. */
	default:
		iRet = -1;
	}

	// Unlock the target surface.
	lpDDSDest->Unlock( NULL );

	// Unlock the source surface.
	lpDDSSource->Unlock( NULL );

	// Return the result.
	return iRet==0?S_OK:E_FAIL;
}
