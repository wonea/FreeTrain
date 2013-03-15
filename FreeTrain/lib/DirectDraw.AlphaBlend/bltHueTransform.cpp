#include "stdafx.h"
#include "AlphaBlender.h"


// translate a color
static inline DWORD translateColor( DWORD color, int* srcColors, int* dstColors, int colorsLen ) {
	for( int i=colorsLen-1; i>=0; i-- )
		if( srcColors[i]==color )
			return (DWORD)dstColors[i];
	return color;
}

static inline DWORD rgb555( int r, int g, int b ) {
	return RGB555(r,g,b);
}

static inline DWORD rgb565( int r, int g, int b ) {
	return RGB565(r,g,b);
}

static inline DWORD rgb888( int r, int g, int b ) {
	return RGB888(r,g,b);
}

static inline DWORD rgb555to888( DWORD v ) {
	return RGB555to888(v);
}

static inline DWORD rgb565to888( DWORD v ) {
	return RGB565to888(v);
}

static inline DWORD rgb888to555( DWORD v ) {
	return RGB888to555(v);
}

static inline DWORD rgb888to565( DWORD v ) {
	return RGB888to565(v);
}

typedef DWORD (*MaskFunction)(int,int,int);

typedef DWORD (*ColorFmtConvertor)(DWORD);

struct MaskTest {
	DWORD mask;
	DWORD test;

	DWORD brightnessMask;
	int  bitShift;
	int   brightnessBits;

	inline MaskTest() {}
	inline MaskTest( DWORD _mask, DWORD _test, DWORD _b, int _s, int _bb ) :
		mask(_mask), test(_test), brightnessMask(_b), bitShift(_s), brightnessBits(_bb) {}

	// test a match to this mask
	template <typename T>
	inline bool match( T t ) {
		return (t&mask)==test;
	}

	template <typename T>
	inline T brightness( T t ) {
		return ((t&brightnessMask)<<(8-brightnessBits)) >>bitShift;
	}
};

struct HueTransformer {
	DWORD rgb[3];

	inline HueTransformer( DWORD Red_dst, DWORD Green_dst, DWORD Blue_dst ){
		rgb[0] = Red_dst; rgb[1] = Green_dst; rgb[2] = Blue_dst;
		for(int i=0; i<3; i++)
			if((rgb[i]&0xff000000)==0)
				rgb[i]=0;
	}

	// test a match to this mask
	template <typename T>
	inline T convert( T t ) {
		DWORD dest = t;
		if(t!=0xffffff&&t!=0){
			dest ^= procChannel(0,0xff0000,16,t);
			dest ^= procChannel(1,0x00ff00,8,t);
			dest ^= procChannel(2,0x0000ff,0,t);
		}
		return dest;
	}

	inline private DWORD procChannel(int c, DWORD mask, int shift, DWORD test){
		DWORD others = test & (~mask);
		if(rgb[c]==0 || others!=0) return 0;
		DWORD dst = rgb[c];
		DWORD bright = (test&mask)>>shift;
		DWORD r = dst&0xff0000;
		r = (r*bright+255)&0xff000000;
		DWORD g = dst&0x00ff00;
		g = (g*bright+255)&0xff0000;
		DWORD b = dst&0x0000ff;
		b = (b*bright+255)&0xff00;
		return test^(r|g|b)>>8;
	}
};

template < MaskFunction mask, int b1, int b2, int b3 >
inline MaskTest buildMask( int keyR, int keyG, int keyB ) {
	if( keyR==-1 )		return MaskTest( mask(0,255,255), mask(0,keyG,keyB), mask(255,0,0), b2+b3, b1 );
	if( keyG==-1 )		return MaskTest( mask(255,0,255), mask(keyR,0,keyB), mask(0,255,0),    b3, b2 );
						return MaskTest( mask(255,255,0), mask(keyR,keyG,0), mask(0,0,255),     0, b3 );
}

inline HueTransformer buildTransformer( DWORD R_dst, DWORD G_dst, DWORD B_dst ) {
	return HueTransformer( R_dst, G_dst, B_dst );
}

#define computeColor( word, R,G,B, rgbBuilder )	\
	(brightness=maskAndTest.brightness(word), \
	rgbBuilder( \
		(R*brightness)>>8, \
		(G*brightness)>>8, \
		(B*brightness)>>8))



template < ColorFmtConvertor encoder, ColorFmtConvertor decoder >
static inline void process16(
	int iDestX,
	int iDestY,
	DxVBLib::DDSURFACEDESC2&	ddsdSource,
	DxVBLib::DDSURFACEDESC2&	ddsdTarget,
	int sourceX1, int sourceY1, int sourceX2, int sourceY2,
	int R_dst, int G_dst, int B_dst,

	int iWidth,
	int iHeight,
	int colorKey
	) {

	bool			gOddWidth;
	int				i;
	DWORD			brightness;
	DWORD			dwTargetTemp;
	DWORD			dwSourceTemp;
	DWORD doubleColorKey = (colorKey<<16)|colorKey;

	//MaskTest maskAndTest = buildMask<rgbBuilder, R,G,B>(keyR,keyG,keyB);
	HueTransformer transformer = buildTransformer(R_dst,G_dst,B_dst);

	ddsdTarget.lpSurface += ddsdTarget.lPitch*iDestY + iDestX*2;
	int dwTargetPad = ddsdTarget.lPitch - ( iWidth * 2 );
	
	ddsdSource.lpSurface += ddsdSource.lPitch*sourceY1 + sourceX1*2;
	int dwSourcePad = ddsdSource.lPitch - ( iWidth * 2 );
	
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

	// Get the address of the target.
	BYTE* lpbTarget = ( BYTE* ) ddsdTarget.lpSurface;

	// Get the address of the source.
	BYTE* lpbSource = ( BYTE* ) ddsdSource.lpSurface;

	do
	{
		// Reset the width.
		i = iWidth;

		// 
		// process two pixels at once.
		//
		while ( i-- > 0 )
		{
			// Read in two source pixels.
			dwSourceTemp = *( ( DWORD* ) lpbSource );

			// If the two source pixels are not both black ...
			if ( dwSourceTemp != doubleColorKey )
			{
				// ... read in two target pixels.
				dwTargetTemp = *( ( DWORD* ) lpbTarget );

				// If the first source is not the key color
				DWORD firstWord = dwSourceTemp>>16;
				if ( firstWord != colorKey ) {
					dwTargetTemp &= 0x0000FFFF;
					dwTargetTemp |= encoder(transformer.convert(decoder(firstWord)))<<16;
				}

				// If the second source is not the key color
				DWORD secondWord = dwSourceTemp & 0x0000FFFF;
				if ( secondWord != colorKey ) {
					dwTargetTemp &= 0xFFFF0000;
					dwTargetTemp |=  encoder(transformer.convert(decoder(secondWord)));
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
			if ( dwSourceTemp != colorKey ) {
				*( (WORD*)lpbTarget )
					= encoder(transformer.convert(decoder(dwSourceTemp)));
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
}





extern "C" __declspec(dllexport)
HRESULT bltHueTransform(
	DirectDrawSurface7* lpDDSDest,
	DirectDrawSurface7* lpDDSSource,
	int iDestX,
	int iDestY,
	int sourceX1, int sourceY1, int sourceX2, int sourceY2,
	DWORD R_dst, DWORD G_dst, DWORD B_dst, int colorKey ) {

	DxVBLib::DDSURFACEDESC2	ddsdSource;
	DxVBLib::DDSURFACEDESC2	ddsdTarget;
	int			dwTargetPad;
	int			dwSourcePad;
	DWORD			dwTargetTemp;
	DWORD			dwSourceTemp;
	BYTE*			lpbTarget;
	BYTE*			lpbSource;
	int				iWidth;
	int				iHeight;
	int				iRet = 0;
	int				i;
	DWORD			brightness;

	MaskTest maskAndTest;

	if( dwMode==RGBMODE_UNCHECKED )
		init(lpDDSDest);

//	DWORD doubleColorKey = (colorKey<<16)+colorKey;
	

	//
	// Get the width and height from the passed rectangle.
	//
	iWidth =  sourceX2 -  sourceX1;
	iHeight = sourceY2 - sourceY1; 

	int originalBottom = sourceY2;


	// Lock down the destination surface.
	lpDDSDest->Lock( NULL, &ddsdTarget, /*CONST_DDLOCKFLAGS::*/DxVBLib::DDLOCK_WAIT, NULL );  

	// Lock down the source surface.
	lpDDSSource->Lock( NULL, &ddsdSource, /*CONST_DDLOCKFLAGS::*/DxVBLib::DDLOCK_WAIT, NULL );


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
		process16<rgb888to555, rgb555to888>(iDestX,iDestY,ddsdSource,ddsdTarget,
			sourceX1, sourceY1, sourceX2, sourceY2,
			R_dst, G_dst, B_dst,
			iWidth, iHeight, colorKey );
		break;

	case RGBMODE_565:
		process16<rgb888to565, rgb565to888>(iDestX,iDestY,ddsdSource,ddsdTarget,
			sourceX1, sourceY1, sourceX2, sourceY2,
			R_dst, G_dst, B_dst,
			iWidth, iHeight, colorKey );
		break;

	case RGBMODE_16:
		return E_NOTIMPL;



	/* 24 bit mode. */
	case RGBMODE_24:
		{
		//maskAndTest = buildMask<rgb888, 8,8,8>(keyR,keyG,keyB);
		HueTransformer transformer = buildTransformer( R_dst, G_dst, B_dst );
		//
		// Determine the padding bytes for the target and the source.
		//
		ddsdTarget.lpSurface += ddsdTarget.lPitch*iDestY + iDestX*3;
		dwTargetPad = ddsdTarget.lPitch - ( iWidth * 3 );

		ddsdSource.lpSurface += ddsdSource.lPitch*sourceY1 + sourceX1*3;
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
				dwSourceTemp = *( ( DWORD* ) lpbSource ) & 0x00FFFFFF;
				if ( dwSourceTemp!=colorKey )
				{
					dwTargetTemp = transformer.convert(dwSourceTemp);

					//
					// Write the destination pixel.
					//
					*( ( WORD* ) lpbTarget ) = ( WORD ) dwTargetTemp;
					lpbTarget += 2;
					*lpbTarget = ( BYTE ) ( dwTargetTemp >> 16 );
					lpbTarget++;
				}
				else
					lpbTarget+=3;
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
		}
	/* 32 bit mode. */
	case RGBMODE_32:
		{
		//maskAndTest = buildMask<rgb888, 8,8,8>(keyR,keyG,keyB);
		HueTransformer transformer = buildTransformer( R_dst, G_dst, B_dst );
		//
		// Determine the padding bytes for the target and the source.
		//
		ddsdTarget.lpSurface += ddsdTarget.lPitch*iDestY + iDestX*4;
		dwTargetPad = ddsdTarget.lPitch - ( iWidth * 4 );

		ddsdSource.lpSurface += ddsdSource.lPitch*sourceY1 + sourceX1*4;
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
				dwSourceTemp &= 0x00FFFFFF;
				if ( dwSourceTemp!=colorKey )
					*( ( DWORD* ) lpbTarget ) = transformer.convert(dwSourceTemp);
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
		}
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
