// dllmain.h : モジュール クラスの宣言です。

class CGlobalHookAssistATLModule : public ATL::CAtlDllModuleT< CGlobalHookAssistATLModule >
{
public :
	DECLARE_LIBID(LIBID_GlobalHookAssistATLLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_GLOBALHOOKASSISTATL, "{1D89BDED-30F6-4480-A391-DC2D35973EEF}")
};

extern class CGlobalHookAssistATLModule _AtlModule;

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
