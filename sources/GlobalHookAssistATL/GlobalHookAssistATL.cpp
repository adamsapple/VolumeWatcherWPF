// GlobalHookAssistATL.cpp : DLL エクスポートの実装です。


#include "stdafx.h"
#include "resource.h"
#include "GlobalHookAssistATL_i.h"
#include "dllmain.h"


using namespace ATL;

extern HINSTANCE g_hDLLMod;

// DLL を OLE によってアンロードできるようにするかどうかを指定します。
STDAPI DllCanUnloadNow(void)
{
			return _AtlModule.DllCanUnloadNow();
	}

// 要求された型のオブジェクトを作成するクラス ファクトリを返します。
_Check_return_
STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _Outptr_ LPVOID* ppv)
{
		return _AtlModule.DllGetClassObject(rclsid, riid, ppv);
}

// DllRegisterServer - エントリをシステム レジストリに追加します。
STDAPI DllRegisterServer(void)
{
	// オブジェクト、タイプ ライブラリおよびタイプ ライブラリ内のすべてのインターフェイスを登録します
	HRESULT hr = _AtlModule.DllRegisterServer();
		return hr;
}

// DllUnregisterServer - エントリをシステム レジストリから削除します。
STDAPI DllUnregisterServer(void)
{
	HRESULT hr = _AtlModule.DllUnregisterServer();
		return hr;
}

// DllInstall - ユーザーおよびコンピューターごとのシステム レジストリ エントリを追加または削除します。
STDAPI DllInstall(BOOL bInstall, _In_opt_  LPCWSTR pszCmdLine)
{
	HRESULT hr = E_FAIL;
	static const wchar_t szUserSwitch[] = L"user";

	if (pszCmdLine != NULL)
	{
		if (_wcsnicmp(pszCmdLine, szUserSwitch, _countof(szUserSwitch)) == 0)
		{
			ATL::AtlSetPerUserRegistration(true);
		}
	}

	if (bInstall)
	{	
		hr = DllRegisterServer();
		if (FAILED(hr))
		{
			DllUnregisterServer();
		}
	}
	else
	{
		hr = DllUnregisterServer();
	}

	return hr;
}


//フック開始 
EXPORT HHOOK BeginHook(int idHook, HOOKPROC HookProc)
{
	return SetWindowsHookEx(
		idHook,					// フックのタイプ 
		(HOOKPROC)HookProc,		// callback 
		g_hDLLMod,				// これのインスタンスハンドル
		0);						// 関連付けるスレッドの識別子(0 : 全部) 
}

//フック終了 
EXPORT BOOL EndHook(HHOOK hHook)
{
	return UnhookWindowsHookEx(hHook);
}
