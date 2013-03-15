========================================================================
    アクティブ テンプレート ライブラリ : DirectDraw.AlphaBlend プロジェクトの概要
========================================================================

AppWizard が作成したこの DirectDraw.AlphaBlend アプリケーションには、
ダイナミック リンク ライブラリ (DLL) の基本的な使い方が示されています。アプリケーション作成のひな型としてお使いください。

このファイルにはプロジェクトを構成している各ファイルの
概要説明が含まれています。

DirectDraw.AlphaBlend.vcproj
    これはアプリケーション ウィザードで生成された VC++ プロジェクトのメイン プロジェクト ファイルです。
    ファイルが生成された Visual C++ のバージョン情報が含まれています。 
    また、アプリケーション ウィザードで選択したプラットフォーム、構成およびプロジェクト機能に関する
    情報も含まれています。

DirectDrawAlphaBlend.idl
    このファイルはタイプ ライブラリの IDL 定義、プロジェクトで定義されたインターフェイス、
    およびコクラスを含んでいます。
    このファイルは MIDL コンパイラによって処理され、以下のファイルを生成します :
        C++ インターフェイス定義および GUID 宣言 (DirectDraw.AlphaBlend.h)
        GUID 宣言                                (DirectDraw.AlphaBlend_i.c)
        タイプ ライブラリ                                  (DirectDrawAlphaBlend.tlb)
        マーシャリング コード                                 (DirectDraw.AlphaBlend_p.c and dlldata.c)

DirectDraw.AlphaBlend.h
    このファイルは DirectDrawAlphaBlend.idl で定義された項目の C++ インターフェイス定義および GUID 宣言
    を含んでいます。このファイルは MIDL によってコンパイラ時に再生成されます。
DirectDraw.AlphaBlend.cpp
    このファイルはオブジェクト マップおよび DLL エクスポートの実装を含んでいます。
DirectDraw.AlphaBlend.rc
    これはプログラムが使用する Microsoft Windows のリソースの
    一覧ファイルです。

DirectDraw.AlphaBlend.def
    このモジュール定義ファイルは、DLL で必要なエクスポートに関する情報へのリンカを提供し、
    次のエクスポート情報を含んでいます :
        DllGetClassObject  
        DllCanUnloadNow    
        GetProxyDllInfo    
        DllRegisterServer	
        DllUnregisterServer

/////////////////////////////////////////////////////////////////////////////
その他の標準ファイル :

StdAfx.h, StdAfx.cpp
    これらのファイルはプリコンパイル済み (PCH) ヘッダー ファイル DirectDraw.AlphaBlend.pch、
    およびプリコンパイルされた型のファイル StdAfx.obj をビルドするために使われます。

Resource.h
    このファイルはリソース ID を定義する標準ヘッダー ファイルです。

/////////////////////////////////////////////////////////////////////////////
プロキシ/スタブ DLL プロジェクトおよびモジュール定義ファイル :

DirectDraw.AlphaBlendps.vcproj
    このファイルは必要に応じてプロキシ/スタブのビルドに使用されるプロジェクト ファイルです。
	主なプロジェクトの IDL ファイルには少なくともインターフェイスを 1 つ含み、
	プロキシ/スタブ DLL をビルドする前に IDL ファイルをコンパイルする必要があります。	この過程で
\プロキシ/スタブ DLL をビルドするのに必要な tdlldata.c、DirectDraw.AlphaBlend_i.c および DirectDraw.AlphaBlend_p.c が
	生成されます。

DirectDraw.AlphaBlendps.def
    このモジュール定義ファイルは、プロキシ/スタブで必要なエクスポートに関する
    情報へのリンカを提供します。
/////////////////////////////////////////////////////////////////////////////
