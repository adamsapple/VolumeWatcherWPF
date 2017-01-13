// stdafx.h : �W���̃V�X�e�� �C���N���[�h �t�@�C���̃C���N���[�h �t�@�C���A�܂���
// �Q�Ɖ񐔂������A�����܂�ύX����Ȃ��A�v���W�F�N�g��p�̃C���N���[�h �t�@�C��
// ���L�q���܂��B
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Windows �w�b�_�[����g�p����Ă��Ȃ����������O���܂��B
// Windows �w�b�_�[ �t�@�C��:
#include <windows.h>



// TODO: �v���O�����ɕK�v�Ȓǉ��w�b�_�[�������ŎQ�Ƃ��Ă�������
#define HOOK_EXPORTS
#ifdef HOOK_EXPORTS 
	#define EXPORT __declspec(dllexport) 
#else 
	#define EXPORT __declspec(dllimport) 
#endif

extern "C" {
	EXPORT HHOOK BeginHook(int, HOOKPROC);
	EXPORT BOOL EndHook(HHOOK);
}