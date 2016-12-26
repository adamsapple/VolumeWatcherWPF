１．ワードのFONTをAwesomeに設定

２．使いたい絵文字を入力

３．テキストエディタを別途開き、先ほど入力した絵文字をコピペ

４．UTF8で保存。ファイル名:[used_letters.txt]

５．rubyにて[buildscript.rb]を実行
　⇒[select_used.pe]が作られる

６．FontForgeをインストール(今回は[FontForge-2016-10-04-Windows.exe])

７．実行

８．[fontawesome-webfont.ttf]を開く

９．メニュー[ファイル>スプリプトを実行]を選択

１０．[select_used.pe]の中身をペーストし、[FF]を選択し[OK]を押下。

１１．必要なアイコン以外が削除されていることを確認。


１２．メニュー[ファイル>フォントを出力]を選択。
ファイル種は[Truetype]を選択。[生成]を押下。
[非標準のEMサイズ]と言われても「はい」を押下。
[問題が見つかりました]と言われても「生成」を押下。

完成



参考：
http://qiita.com/YosukeM/items/086f902c1f3748fca083
