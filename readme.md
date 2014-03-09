#ncvVJoyInputer
##なにこれ？
指定した文字列の読み上げ時に仮想ゲームパッドへ入力を送信する、[NiconamaCommentViewer](http://www.posite-c.com/application/ncv/)用プラグインです。

##どうつかうの？

* [vJoy 公式サイト](http://vjoystick.sourceforge.net/site/)から最新の vJoy インストーラーをダウンロードしインストール（要再起動）。インストール後、vJoy の設定を行う。また、最新の FeederSDK もダウンロードしておく
* NiconamaCommentViewerのpluginsフォルダに本プラグイン(ncvVJoyInputer.dll)を配置
* 先ほどダウンロードした FeederSDK を展開し、SDK/C#/x86/ から vJoyInterface.dll および vJoyInterfaceWrap.dll をコピーしてきて本プラグインと同じ位置に配置
* NiconamaCommentViewerを起動し本プラグインの設定を行う（正規表現については[正規表現言語要素](http://msdn.microsoft.com/ja-jp/library/az24scfc(v=vs.90).aspx)を参照）


##動作環境

* Microsoft Windows 7 SP1(64bit)
* Microsoft .NET Framework 4.0 以上
* NiconamaCommentViewer(α135)
* vJoy(vJoy_x86x64_I030114 Version 2.0.2)
* FeederSDK(vJoy202SDK-011112 Version 2.0.2)

にて動作を確認しています。


##vJoyの設定についての補足

* ボタン数は1から32個に対応
* POV Hat Switch 4 Directions 1つが十字キーに対応
* Basic Axis X と Basic Axis Y が左スティックのX軸とY軸に対応
* Basic Axis Z と Basic Axis Rz が右スティックのX軸とY軸に対応


##ライセンス
[License.txt](https://github.com/102782/ncvVJoyInputer/blob/master/License.txt) をご参照ください。


##お世話になった方々
ncvSample2 プラグインの雛形第2版作者　うつろ様


##連絡先
[https://twitter.com/102782](https://twitter.com/102782)