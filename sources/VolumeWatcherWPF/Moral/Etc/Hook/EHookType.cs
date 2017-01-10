// http://hp.vector.co.jp/authors/VA016117/hook.html
// http://mrxray.on.coocan.jp/Delphi/Others/A_KindOfHook.htm
// http://qiita.com/rbtnn/items/74d0b1438776cca1c1ea

namespace Moral.Etc.Hook
{
    /// <summary>
    /// WindowsでのHook種別
    /// </summary>
    public enum EHookType : int
    {
        WH_NOTHOOK         = -1,
        WH_JOURNALRECORD   = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD        = 2,
        WH_GETMESSAGE      = 3,
        WH_CALLWNDPROC     = 4,
        WH_CBT             = 5,
        WH_SYSMSGFILTER    = 6,
        WH_MOUSE           = 7,
        WH_HARDWARE        = 8,
        WH_DEBUG           = 9,
        WH_SHELL           = 10,
        WH_FOREGROUNDIDLE  = 11,
        WH_CALLWNDPROCRET  = 12,
        WH_KEYBOARD_LL     = 13,
        WH_MOUSE_LL        = 14
    }
}
