using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// http://hp.vector.co.jp/authors/VA016117/hook.html
// http://mrxray.on.coocan.jp/Delphi/Others/A_KindOfHook.htm
namespace Moral.Etc
{
    public enum EHookType
    {
        WH_CALLWNDPROC,
        WH_CALLWNDPROCRET,
        WH_CBT,
        WH_DEBUG,
        WH_FOREGROUNDIDLE,
        WH_GETMESSAGE,
        WH_JOURNALRECORD,
        WH_JOURNALPLAYBACK,
        WH_KEYBOARD_LL,
        WH_KEYBOARD,
        WH_MOUSE_LL,
        WH_MOUSE,
        WH_MSGFILTER,
        WH_SYSMSGFILTER,
        WH_SHELL,
    }
}
