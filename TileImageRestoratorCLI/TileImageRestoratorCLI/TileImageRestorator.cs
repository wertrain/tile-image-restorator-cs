using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using LibPuzzle;

namespace TileImageRestoratorCLI
{
    /// <summary>
    /// 
    /// </summary>
    class TileImageRestorator
    {
        /// <summary>
        /// 
        /// </summary>
        public class TableData
        {
            public List<Tuple<int, double>> Table { get; set; }

            public int Row { get; set; }
            public int Col { get; set; }

            public int TileWidth { get; set; }
            public int TileHeight { get; set; }

            public TableData()
            {
                Table = new List<Tuple<int, double>>();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private TileImageRestorator()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static TableData LoadTable(string filePath)
        {
            using (var reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                try
                {
                    // ヘッダーチェック rtbl
                    if (!('r' == reader.ReadChar() && 't' == reader.ReadChar() && 'b' == reader.ReadChar() && 'l' == reader.ReadChar()))
                    {
                        return null;
                    }

                    // バージョンチェック
                    if (reader.ReadInt16() != 1)
                    {
                        return null;
                    }

                    var tableData = new TableData();

                    // 縦横サイズの取得
                    tableData.Row = reader.ReadInt16();
                    tableData.Col = reader.ReadInt16();

                    // タイル画像の縦横サイズの取得
                    tableData.TileWidth = reader.ReadInt16();
                    tableData.TileHeight = reader.ReadInt16();

                    // テーブルの読み込み
                    for (int index = 0, max = tableData.Row * tableData.Col; index < max; ++index)
                    {
                        var pair = new Tuple<int, double>(reader.ReadInt32(), reader.ReadDouble());
                        tableData.Table.Add(pair);
                    }
                    return tableData;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="tableData"></param>
        /// <returns></returns>
        public static bool SaveTable(string filePath, TableData tableData)
        {
            using (var writer = new BinaryWriter(new FileStream(filePath, FileMode.OpenOrCreate)))
            {
                try
                {
                    // ヘッダー出力 rtbl
                    writer.Write('r');
                    writer.Write('t');
                    writer.Write('b');
                    writer.Write('l');
                    // バージョン出力
                    writer.Write((Int16)1);
                    // 縦横サイズの出力
                    writer.Write((Int16)tableData.Row);
                    writer.Write((Int16)tableData.Col);
                    // タイル画像の縦横サイズの出力
                    writer.Write((Int16)tableData.TileWidth);
                    writer.Write((Int16)tableData.TileHeight);
                    // テーブルの出力
                    foreach (var pair in tableData.Table)
                    {
                        writer.Write((Int32)pair.Item1);
                        writer.Write(pair.Item2);
                    }
                }
                catch(Exception)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcBitmap"></param>
        /// <param name="exampleBitmap"></param>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        /// <returns></returns>
        public static TableData CreateRestoreTable(Bitmap srcBitmap, Bitmap exampleBitmap, int tileWidth, int tileHeight, int rowCount, int colCount)
        {
            return UpdateRestoreTable(null, srcBitmap, exampleBitmap, tileWidth, tileHeight, rowCount, colCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableData"></param>
        /// <param name="srcBitmap"></param>
        /// <param name="exampleBitmap"></param>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        /// <returns></returns>
        public static TableData UpdateRestoreTable(TableData tableData, Bitmap srcBitmap, Bitmap exampleBitmap, int tileWidth, int tileHeight, int rowCount, int colCount)
        {
            if (tableData == null)
            {
                tableData = new TableData();
                tableData.Row = rowCount;
                tableData.Col = colCount;
                tableData.TileWidth = tileWidth;
                tableData.TileHeight = tileHeight;
            }
            else
            {
                if (!(tableData.Row == rowCount && tableData.Col == colCount) && !(tableData.TileWidth == tileWidth && tableData.TileHeight == tileHeight))
                {
                    return null;
                }
            }

            var srcTiles = createTileImage(srcBitmap, tileWidth, tileHeight);
            var exampleTiles = createTileImage(exampleBitmap, tileWidth, tileHeight);

            var context = new PuzzleContext();

            var restoredTable = new int[rowCount, colCount];
            for (int exampleIndex = 0; exampleIndex < exampleTiles.Count; ++exampleIndex)
            {
                var exampleVec = context.FromImage(exampleTiles[exampleIndex]);
                for (int srcIndex = 0; srcIndex < srcTiles.Count; ++srcIndex)
                {
                    var srcVec = context.FromImage(srcTiles[srcIndex]);
                    double dist = exampleVec.GetDistanceFrom(srcVec);

                    if (tableData.Table.Count <= exampleIndex)
                    {
                        tableData.Table.Add(new Tuple<int, double>(-1, 1000.0));
                    }

                    if (tableData.Table[exampleIndex].Item2 > dist)
                    {
                        tableData.Table[exampleIndex] = new Tuple<int, double>(srcIndex, dist);
                    }
                }
            }

            foreach (var bmp in srcTiles)
            {
                bmp.Dispose();
            }
            foreach (var bmp in exampleTiles)
            {
                bmp.Dispose();
            }

            return tableData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcBitmap"></param>
        /// <param name="tableData"></param>
        /// <param name="outputFilePath"></param>
        /// <returns></returns>
        public static bool Restore(Bitmap srcBitmap, TableData tableData, string outputFilePath)
        {
            var restoredBitmap = new Bitmap(srcBitmap.Width, srcBitmap.Height);
            var graphics = Graphics.FromImage(restoredBitmap);

            if (tableData.Row <= 0 || tableData.Col <= 0)
            {
                return false;
            }

            var tileImage = createTileImage(srcBitmap, tableData.TileWidth, tableData.TileHeight);

            for (int row = 0; row < tableData.Row; ++row)
            {
                for (int col = 0; col < tableData.Col; ++col)
                {
                    int x = col * tableData.TileWidth, y = row * tableData.TileHeight;
                    Rectangle rect = new Rectangle(x, y, tableData.TileWidth, tableData.TileHeight);
                    int index = tableData.Table[row * tableData.Col + col].Item1;
                    graphics.DrawImage(tileImage[index], rect);
                }
            }
            restoredBitmap.Save(outputFilePath);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcBitmap"></param>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        /// <returns></returns>
        public static Bitmap CreateRandomTileImage(Bitmap srcBitmap, int tileWidth, int tileHeight)
        {
            var bitmap = new Bitmap(srcBitmap);
            var graphics = Graphics.FromImage(bitmap);

            var tileImage = createTileImage(srcBitmap, tileWidth, tileHeight);

            int colCount = bitmap.Width / tileWidth;
            int rowCount = bitmap.Height / tileHeight;

            // シャッフルする
            var orderIndices = new int[rowCount * colCount];
            for (int i = 0; i < rowCount * colCount; ++i)
            {
                orderIndices[i] = i;
            }
            int[] randomIndices = orderIndices.OrderBy(i => Guid.NewGuid()).ToArray();

            var tiles = new List<Bitmap>();
            for (int row = 0; row < rowCount; ++row)
            {
                for (int col = 0; col < colCount; ++col)
                {
                    int x = col * tileWidth, y = row * tileHeight;
                    Rectangle rect = new Rectangle(x, y, tileWidth, tileHeight);
                    int index = randomIndices[row * colCount + col];
                    graphics.DrawImage(tileImage[index], rect);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        /// <returns></returns>
        private static List<Bitmap> createTileImage(Bitmap bitmap, int tileWidth, int tileHeight)
        {
            int colCount = bitmap.Width / tileWidth;
            int rowCount = bitmap.Height / tileHeight;

            var tiles = new List<Bitmap>();
            for (int row = 0; row < rowCount; ++row)
            {
                for (int col = 0; col < colCount; ++col)
                {
                    int x = col * tileWidth, y = row * tileHeight;
                    Rectangle rect = new Rectangle(x, y, tileWidth, tileHeight);
                    tiles.Add(bitmap.Clone(rect, bitmap.PixelFormat));
                }
            }
            return tiles;
        }
    }
}
