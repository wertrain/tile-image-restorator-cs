# tile-image-restorator-cs

�����_���ɕ������ꂽ�摜���C�����܂��B  

<img src="https://github.com/wertrain/tile-image-restorator-cs/blob/master/Sample/random_8_8.png?raw=true" width="240" />
<img src="https://github.com/wertrain/tile-image-restorator-cs/blob/master/Sample/restored_8_8.png?raw=true" width="240" />

## �g����

### �e�[�u�����쐬
�K�v�ɉ����ĕ����̉摜���g�p���ĕ�����s���܂��B
~~~
TileImageRestoratorCLI table -i="random.png" -e="example.png" -t="tbl.bin" -r=8 -c=8 --width=60 --height=45
~~~

### �쐬�����e�[�u�����g�p���ĉ摜���C��
~~~
TileImageRestoratorCLI restore -i="random.png" -o="restored.png" -t="tbl.bin"
~~~
### �i�e�X�g�p�̉摜���쐬�j
~~~
TileImageRestoratorCLI puzzle -i="random.png" -r=8 -c=8 --width=60 --height=45
~~~
