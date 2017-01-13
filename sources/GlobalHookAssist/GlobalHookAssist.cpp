// dllmain.cpp : DLL アプリケーションのエントリ ポイントを定義します。
#include "stdafx.h"
#include <windows.h> 


extern "C" {
	HINSTANCE g_hDLLMod;

	BOOL APIENTRY DllMain(HMODULE hModule,
		DWORD  ul_reason_for_call,
		LPVOID lpReserved
	)
	{
		switch (ul_reason_for_call)
		{
		case DLL_PROCESS_ATTACH:
			g_hDLLMod = (HINSTANCE)hModule;
			break;
		case DLL_THREAD_ATTACH:
		case DLL_THREAD_DETACH:
		case DLL_PROCESS_DETACH:
			break;
		}

		return TRUE;
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

}