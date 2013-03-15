#include "lib\streams.h"
#include <initguid.h>
#include <comdef.h>
#include <crtdbg.h>
#import "lib/BitmapWriter.TypeLib/BitmapWriter.tlb" raw_interfaces_only

using namespace BitmapWriterTypeLib;

// {E31244AA-0971-4992-A76B-957999AA5C9E}
DEFINE_GUID(CLSID_PushSourceDesktop, 
0xe31244aa, 0x971, 0x4992, 0xa7, 0x6b, 0x95, 0x79, 0x99, 0xaa, 0x5c, 0x9e);

_COM_SMARTPTR_TYPEDEF( IMediaSample, __uuidof(IMediaSample) );