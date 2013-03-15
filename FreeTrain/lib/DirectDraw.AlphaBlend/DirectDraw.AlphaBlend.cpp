// DirectDraw.AlphaBlend.cpp : DLL エクスポートの実装です。

#include "stdafx.h"
#include "resource.h"
#include "DirectDraw.AlphaBlend.h"

class CDirectDrawAlphaBlendModule : public CAtlDllModuleT< CDirectDrawAlphaBlendModule >
{
public :
	DECLARE_LIBID(LIBID_DirectDrawAlphaBlendLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_DIRECTDRAWALPHABLEND, "{A622BFB1-EAFB-4471-8523-313435ED01D6}")
};

CDirectDrawAlphaBlendModule _AtlModule;


// DLL エントリ ポイント
extern "C" BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved)
{
	hInstance;
    return _AtlModule.DllMain(dwReason, lpReserved); 
}


// DLL を OLE によってアンロードできるようにするかどうかを指定します。
STDAPI DllCanUnloadNow(void)
{
    return _AtlModule.DllCanUnloadNow();
}


// 要求された型のオブジェクトを作成するクラス ファクトリを返します。
STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
    return _AtlModule.DllGetClassObject(rclsid, riid, ppv);
}


// DllRegisterServer - エントリをシステム レジストリに追加します。
STDAPI DllRegisterServer(void)
{
    // オブジェクト、タイプ ライブラリおよびタイプ ライブラリ内の全てのインターフェイスを登録します
    HRESULT hr = _AtlModule.DllRegisterServer();
	return hr;
}


// DllUnregisterServer - エントリをレジストリから削除します。
//
STDAPI DllUnregisterServer(void)
{
	HRESULT hr = _AtlModule.DllUnregisterServer();
	return hr;
}
