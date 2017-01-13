// dllmain.cpp : DllMain の実装です。

#include "stdafx.h"
#include "resource.h"
#include "GlobalHookAssistATL_i.h"
#include "dllmain.h"

CGlobalHookAssistATLModule _AtlModule;
HINSTANCE g_hDLLMod;

// DLL エントリ ポイント
extern "C" {
	BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved)
	{
		g_hDLLMod = (HINSTANCE)hInstance;
		return _AtlModule.DllMain(dwReason, lpReserved);
	}

}
