// dllmain.cpp : DLL �A�v���P�[�V�����̃G���g�� �|�C���g���`���܂��B
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

	//�t�b�N�J�n 
	EXPORT HHOOK BeginHook(int idHook, HOOKPROC HookProc)
	{
		return SetWindowsHookEx(
			idHook,					// �t�b�N�̃^�C�v 
			(HOOKPROC)HookProc,		// callback 
			g_hDLLMod,				// ����̃C���X�^���X�n���h��
			0);						// �֘A�t����X���b�h�̎��ʎq(0 : �S��) 
	}

	//�t�b�N�I�� 
	EXPORT BOOL EndHook(HHOOK hHook)
	{
		return UnhookWindowsHookEx(hHook);
	}

}