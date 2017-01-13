========================================================================
    ATL (ACTIVE TEMPLATE LIBRARY): GlobalHookAssistATL プロジェクトの概要
========================================================================

この GlobalHookAssistATL プロジェクトは、ユーザーがダイナミック リンク ライブラリ (DLL: Dynamic Link Library) を作成するための開始点として使用するために、AppWizard によって作成されました。

このファイルには、プロジェクトを構成する各ファイルの内容の概要が記述されています。

GlobalHookAssistATL.vcxproj
    これは、アプリケーション ウィザードを使用して生成された VC++ プロジェクトのメイン プロジェクト ファイルです。ファイルを生成した Visual C++ のバージョンに関する情報と、アプリケーション ウィザードで選択されたプラットフォーム、構成、およびプロジェクト機能に関する情報が含まれています。

GlobalHookAssistATL.vcxproj.filters
    これは、アプリケーション ウィザードで生成された VC++ プロジェクトのフィルター ファイルです。このファイルには、プロジェクト内のファイルとフィルターとの間の関連付けに関する情報が含まれています。この関連付けは、特定のノードで同様の拡張子を持つファイルのグループ化を示すために IDE で使用されます (たとえば、".cpp" ファイルは "ソース ファイル" フィルターに関連付けられています)。

GlobalHookAssistATL.idl
    このファイルには、プロジェクトで定義されるタイプ ライブラリの IDL 定義、インターフェイス、およびコクラスが含まれます。
    このファイルは MIDL コンパイラによって処理され、次のものが生成されます。
        C++ インターフェイス定義および GUID 宣言 (GlobalHookAssistATL.h)
        GUID 定義                                (GlobalHookAssistATL_i.c)
        タイプ ライブラリ                                  (GlobalHookAssistATL.tlb)
        マーシャリング コード                                 (GlobalHookAssistATL_p.c および dlldata.c)

GlobalHookAssistATL.h
    このファイルには、C++ のインターフェイス定義および GlobalHookAssistATL.idl で定義される項目の GUID 宣言が含まれます。コンパイル中に MIDL によって再生成されます。

GlobalHookAssistATL.cpp
    このファイルには、オブジェクト マップおよび DLL のエクスポートの実装が含まれます。

GlobalHookAssistATL.rc
    これは、プログラムが使用するすべての Microsoft Windows リソースの一覧です。

GlobalHookAssistATL.def
    このモジュール定義ファイルは、DLL に必要なエクスポートに関する情報をリンカーに提供します。次のエクスポート情報が含まれています。
        DllGetClassObject
        DllCanUnloadNow
        DllRegisterServer
        DllUnregisterServer
        DllInstall

/////////////////////////////////////////////////////////////////////////////
その他の標準ファイル :

StdAfx.h, StdAfx.cpp
    これらのファイルは、GlobalHookAssistATL.pch という名前のプリコンパイル済みヘッダー (PCH) ファイルと、StdAfx.obj という名前のプリコンパイル済みの型ファイルをビルドするために使用されます。

Resource.h
    これは、リソース ID を定義する標準のヘッダー ファイルです。

/////////////////////////////////////////////////////////////////////////////
プロキシ/スタブ DLL プロジェクトおよびモジュール定義ファイル :

GlobalHookAssistATLps.vcxproj
    このファイルは、必要に応じてプロキシ/スタブ DLL を構築するためのプロジェクト ファイルです。
	メイン プロジェクトの IDL ファイルにインターフェイスが少なくとも 1 つ含まれていること、およびプロキシ/スタブ DLL をビルドする前に IDL ファイルをコンパイルすることが必要です。
	このプロセスによって、プロキシ/スタブ DLL のビルドに必要な dlldata.c, GlobalHookAssistATL_i.c および GlobalHookAssistATL_p.c が生成されます。

GlobalHookAssistATLps.vcxproj.filters
    proxy/stub プロジェクトのフィルター ファイルです。このファイルには、プロジェクト内のファイルとフィルターとの間の関連付けに関する情報が含まれています。この関連付けは、特定のノードで同様の拡張子を持つファイルのグループ化を示すために IDE で使用されます (たとえば、".cpp" ファイルは "ソース ファイル" フィルターに関連付けられています)。

GlobalHookAssistATLps.def
    このモジュール定義ファイルは、プロキシ/スタブに必要なエクスポートに関する情報をリンカーに提供します。

/////////////////////////////////////////////////////////////////////////////
