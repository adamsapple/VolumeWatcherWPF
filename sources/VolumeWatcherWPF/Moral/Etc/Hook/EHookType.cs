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
        /// <summary>入力イベントを監視､ 記録する。通常､ アプリケーションは､ このフックを使ってマウス イベントやキーボード イベントのシーケンスを記録し､ 後でWH_JOURNALPLAYBACKフックを使って再生する</summary>
        WH_JOURNALRECORD = 0,
        /// <summary>システム メッセージ キューにメッセージを挿入する。アプリケーションは､ WH_JOURNALRECORDフックで記録しておいた一連のマウス イベントやキーボード イベントを､ このフックを使って再生できる。WH_JOURNALPLAYBACKフックがインストールされているときは､ 通常のマウス入力やキーボード入力は使用不能になる。</summary>
        WH_JOURNALPLAYBACK = 1,
        /// <summary>GetMessage関数やPeekMessage関数が返そうとしているWM_KEYDOWNメッセージやWM_KEYUPメッセージの流れを監視</summary>
        WH_KEYBOARD = 2,
        /// <summary>GetMessage関数やPeekMessage関数が返そうとしているメッセージを監視</summary>
        WH_GETMESSAGE = 3,
        /// <summary>SendMessage関数でウィンドウ プロシージャに送られるメッセージを監視する。Windowsは､ 送り先のウィンドウ プロシージャにメッセージを渡す前</summary>
        WH_CALLWNDPROC = 4,
        /// <summary>Windowsは､ 次に示す処理を行う前に､ WH_CBTフック プロシージャを呼び出す。
        ///・ ウィンドウのアクティブ化､ 作成､ 破棄､ 最大化､ アイコン化､ 移動､ サイズ変更
        ///・ システム メッセージ キューからのマウス イベントやキーボード イベントの削除
        ///・ 入力フォーカスの設定
        ///・ システム メッセージ キューの同期化
        ///</summary>
        WH_CBT = 5,
        /// <summary>メニューやスクロール バー､ メッセージ ボックス､ ダイアログ ボックスが処理しようとしているメッセージを監視したり､ ユーザーがAlt+TabキーやAlt+Escキーを押したときに別のウィンドウがアクティブ化されようとするのを検出できる。WH_MSGFILTERフックは､ フック プロシージャをインストールしたアプリケーションが作成したメニューやスクロール バー､ メッセージ ボックス､ ダイアログ ボックスに渡されるメッセージしか監視できない。WH_SYSMSGFILTERフックは､ すべてのアプリケーションについてこのようなメッセージを監視できる。</summary>
        WH_SYSMSGFILTER = 6,
        /// <summary>GetMessage関数やPeekMessage関数が返そうとしているマウス メッセージを監視</summary>
        WH_MOUSE = 7,
        /// <summary></summary>
        WH_HARDWARE = 8,
        /// <summary>システムのほかのフックに関連付けられているフック プロシージャを呼び出す前</summary>
        WH_DEBUG = 9,
        /// <summary>Windowsシェル アプリケーションが､ 重要な通知を取得するときに使う。Windowsは､ シェル アプリケーションがアクティブ化されようとしているときや､ トップ レベル ウィンドウが作成または破棄されるときに､ WH_SHELLフック プロシージャを呼び出す。</summary>
        WH_SHELL = 10,
        /// <summary>フォアグラウンドスレッドがアイドル状態になったとき</summary>
        WH_FOREGROUNDIDLE = 11,
        /// <summary>SendMessage関数でウィンドウ プロシージャに送られるメッセージを監視する。Windowsは､ 送り先のウィンドウ プロシージャがメッセージを処理した後</summary>
        WH_CALLWNDPROCRET = 12,
        /// <summary></summary>
        WH_KEYBOARD_LL = 13,
        /// <summary></summary>
        WH_MOUSE_LL = 14
    }
}
