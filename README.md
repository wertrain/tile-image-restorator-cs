# tile-image-restorator-cs

ランダムに分割された画像を修復します。  

<img src="https://github.com/wertrain/tile-image-restorator-cs/blob/master/Sample/random_8_8.png?raw=true" width="240" />
<img src="https://github.com/wertrain/tile-image-restorator-cs/blob/master/Sample/restored_8_8.png?raw=true" width="240" />

## 使い方

### テーブルを作成
必要に応じて複数の画像を使用して複数回行います。
~~~
TileImageRestoratorCLI table -i="random.png" -e="example.png" -t="tbl.bin" -r=8 -c=8 --width=60 --height=45
~~~

### 作成したテーブルを使用して画像を修復
~~~
TileImageRestoratorCLI restore -i="random.png" -o="restored.png" -t="tbl.bin"
~~~
### （テスト用の画像を作成）
~~~
TileImageRestoratorCLI puzzle -i="random.png" -r=8 -c=8 --width=60 --height=45
~~~
