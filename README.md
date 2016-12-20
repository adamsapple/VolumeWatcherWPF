# VolumeWatcherWPF

## 概要
#### 「PCのオーディオデバイスをちょっと便利に 」
+ Volume、mic-levelをキーボードショートカットで操作
+ volume操作時のゲージの表示(ノートPCででるようなヤツ)
+ マイクテスト機能(listen to device)
+ スタートアップ常駐登録
+ C#-WPFで作成

#### こんなとき
+ curseで相手に声が聞こえてるかどうかのテスト
+ ゲーム中音量が大きいんだけど、フォーカスをゲームからタスクバーに移動するのが手間
+ 今はマイクをmuteしておきたい

## 使い方
タスクトレイに常駐。  
アイコンをWクリックするとオプション画面が表示されるのでそこで設定してね。  
#### 機能
+ オーディオデバイスのボリューム/状態を監視
+ キーボードショートカットでボリューム調整が可能
+ ボリューム表示位置/不透明度指定
+ スタートアップ常駐設定

#### キーボードショートカット
###### ボリューム
[Alt]+[>]  Volume+  
[Alt]+[<]  Volume-  
[Alt]+[M]  Mute On/Off  
###### マイクレベル
[Alt]+[L]  mic-level+  
[Alt]+[K]  mic-level-  
[Alt]+[J]  Mute On/Off  

## Trello
Torello使って開発してみてます。  
https://trello.com/b/qy5aIeN1/volumewatcherwpf
