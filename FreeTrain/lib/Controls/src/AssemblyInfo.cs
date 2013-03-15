using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("FreeTrain windows forms controls library")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Kohsuke Kawaguhi (kk@kohsuke.org)")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("(C)Copyright by Kohsuke Kawaguchi, 2002")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// アセンブリのバージョン情報は、以下の 4 つの属性で構成されます :
//
//      メジャー バージョン
//      マイナ バージョン 
//      ビルド番号
//      リビジョン
//
// 下にあるように、'*' を使って、すべての値を指定するか、 
// ビルドおよびリビジョン番号を既定値にすることができます :

[assembly: AssemblyVersion("1.0.2002.1221")]

//
// アセンブリに署名するには、使用するキーを指定しなければなりません。 
// アセンブリ署名に関する詳細については、Microsoft .NET Framework ドキュメントを参照してください。
//
// 下記の属性を使って、署名に使うキーを制御します。 
//
// メモ : 
//   (*) キーが指定されないと、アセンブリは署名されません。
//   (*) KeyName は、コンピュータにインストールされている
//        暗号サービス プロバイダ (CSP) のキーを表します。KeyFile は、
//       キーを含むファイルです。
//   (*) KeyFile および KeyName の値が共に指定されている場合は、 
//       以下の処理が行われます :
//       (1) KeyName が CSP に見つかった場合、そのキーが使われます。
//       (2) KeyName が存在せず、KeyFile が存在する場合、 
//           KeyFile にあるキーが CSP にインストールされ、使われます。
//   (*) KeyFile を作成するには、sn.exe (厳密な名前) ユーティリティを使ってください。
//       KeyFile を指定するとき、KeyFile の場所は、
//       プロジェクト出力 ディレクトリへの相対パスでなければなりません。
//       パスは、%Project Directory%¥obj¥<configuration> です。たとえば、KeyFile がプロジェクト ディレクトリにある場合、
//       AssemblyKeyFile 属性を 
//       [assembly: AssemblyKeyFile("..¥¥..¥¥mykey.snk")] として指定します。
//   (*) 遅延署名は高度なオプションです。
//       詳細については Microsoft .NET Framework ドキュメントを参照してください。
//
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
