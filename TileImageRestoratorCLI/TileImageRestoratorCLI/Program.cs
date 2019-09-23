using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace TileImageRestoratorCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                // アプリケーション名（ヘルプの出力で使用される）
                Name = "TileImageRestoratorCLI",
            };

            // ヘルプ出力のトリガーとなるオプションを指定
            app.HelpOption("-?|-h|--help");

            app.OnExecute(() =>
            {
#if false
                //var src = Bitmap.FromFile(@"D:\Downloads\sakura1\235.jpg");
                var example = Bitmap.FromFile(@"C:\Users\Owner\Pictures\example1.PNG");

                var tileImageRestorator = new TileImageRestorator();
                tileImageRestorator.IsAbleRestore(new Bitmap(src), new Bitmap(example), 38, 27);
                var table = tileImageRestorator.Load("tbl");
                //table.Row = 38;
                //table.Col = 27;
                tileImageRestorator.Restore(new Bitmap(src), table, "test.bmp");
                //tileImageRestorator.Save("tbl", table);
                //tileImageRestorator.CreateRandomTileImage(new Bitmap(example), 2, 2).Save("r.bmp");
#else

                //TileImageRestorator.CreateRestoreTable();
                //var src = Bitmap.FromFile(@"D:\Develop\C#\tile-image-restorator-cs\TileImageRestoratorCLI\TileImageRestoratorCLI\bin\Debug\random.png");
                //var example = Bitmap.FromFile(@"C:\Users\Owner\Pictures\origin.png");

                //var tileImageRestorator = new TileImageRestorator();
                //tileImageRestorator.IsAbleRestore(new Bitmap(src), new Bitmap(example), 8, 8);
                //var table = tileImageRestorator.Load("tbl");
                //table.Row = 38;
                //table.Col = 27;
                //tileImageRestorator.Restore(new Bitmap(src), table, "new.bmp");

                //var example = Bitmap.FromFile(@"C:\Users\Owner\Pictures\origin.png");
                //var tileImageRestorator = new TileImageRestorator();
                //tileImageRestorator.CreateRandomTileImage(new Bitmap(example), 8,8).Save("test.png");


                /*var src = Bitmap.FromFile(@"r.bmp");
                var example = Bitmap.FromFile(@"C:\Users\Owner\Pictures\example1.PNG");

                //var tileImageRestorator = new TileImageRestorator();
                tileImageRestorator.IsAbleRestore(new Bitmap(src), new Bitmap(example), 2, 2);
                var table = tileImageRestorator.Load("tbl");
                tileImageRestorator.Restore(new Bitmap(src), table, "test.bmp");*/
#endif
                return 0;
            });

            app.Command("table", (command) =>
            {
                // 説明（ヘルプの出力で使用される）
                command.Description = "リストア用のテーブルを作成します.";

                // コマンドについてのヘルプ出力のトリガーとなるオプションを指定
                command.HelpOption("-?|-h|--help");

                // コマンドの引数（名前と説明を引数で渡しているが、これはヘルプ出力で使用される）
                var restoreArgs = command.Argument("table", "リストア用のテーブルを作成します.");

                // オプションの設定
                var tableOptionImage = command.Option("-i|--image <opitons>",
                    "修復対象の画像パス",
                    CommandOptionType.MultipleValue);
                var tableOptionExample = command.Option("-e|--example <opitons>",
                    "修復後のサンプル画像パス",
                    CommandOptionType.MultipleValue);
                var tableOptionTable = command.Option("-t|--table <opitons>",
                    "テーブルデータまでのパス",
                    CommandOptionType.MultipleValue);
                var tableOptionRow = command.Option("-r|--row <opitons>",
                    "分割行数",
                    CommandOptionType.MultipleValue);
                var tableOptionCol = command.Option("-c|--col <opitons>",
                    "分割列数",
                    CommandOptionType.MultipleValue);

                // 実行
                command.OnExecute(() =>
                {
                    var inputPath = string.Empty;
                    foreach (var value in tableOptionImage.Values)
                    {
                        inputPath = value;
                    }

                    var examplePath = string.Empty;
                    foreach (var value in tableOptionExample.Values)
                    {
                        examplePath = value;
                    }

                    var tablePath = string.Empty;
                    foreach (var value in tableOptionTable.Values)
                    {
                        tablePath = value;
                    }

                    int rowCount = 0;
                    foreach (var value in tableOptionRow.Values)
                    {
                        rowCount = int.Parse(value);
                    }

                    int colCount = 0;
                    foreach (var value in tableOptionCol.Values)
                    {
                        colCount = int.Parse(value);
                    }

                    using (var image = new Bitmap(inputPath))
                    using (var example = new Bitmap(examplePath))
                    {
                        if (File.Exists(tablePath))
                        {
                            var tableData = TileImageRestorator.LoadTable(tablePath);
                            TileImageRestorator.UpdateRestoreTable(tableData, image, example, rowCount, colCount);
                        }
                        else
                        {
                            var tableData = TileImageRestorator.CreateRestoreTable(image, example, rowCount, colCount);
                            TileImageRestorator.SaveTable(tablePath, tableData);
                        }
                    }
                    return 0;
                });
            });

            app.Command("restore", (command) =>
            {
                // 説明（ヘルプの出力で使用される）
                command.Description = "画像をリストアします.";

                // コマンドについてのヘルプ出力のトリガーとなるオプションを指定
                command.HelpOption("-?|-h|--help");

                // コマンドの引数（名前と説明を引数で渡しているが、これはヘルプ出力で使用される）
                var restoreArgs = command.Argument("restore", "画像をリストアします.");

                // オプションの設定
                var restoreOptionInput = command.Option("-i|--input <opitons>",
                    "リストア対象の画像パス",
                    CommandOptionType.MultipleValue);
                var restoreOptionOutput = command.Option("-o|--output <opitons>",
                    "リストア後の出力画像パス",
                    CommandOptionType.MultipleValue);
                var restoreOptionTable = command.Option("-t|--table <opitons>",
                    "テーブルデータまでのパス",
                    CommandOptionType.MultipleValue);

                // 実行
                command.OnExecute(() =>
                {
                    var inputPath = string.Empty;
                    foreach (var value in restoreOptionInput.Values)
                    {
                        inputPath = value;
                    }

                    var outputPath = string.Empty;
                    foreach (var value in restoreOptionOutput.Values)
                    {
                        outputPath = value;
                    }

                    var tablePath = string.Empty;
                    foreach (var value in restoreOptionTable.Values)
                    {
                        tablePath = value;
                    }

                    using (var bitmap = new Bitmap(inputPath))
                    {
                        var tableData = TileImageRestorator.LoadTable(tablePath);
                        TileImageRestorator.Restore(bitmap, tableData, outputPath);
                    }

                    return 0;
                });
            });

            app.Command("puzzle", (command) =>
            {
                // 説明（ヘルプの出力で使用される）
                command.Description = "分割された画像を生成します.";

                // コマンドについてのヘルプ出力のトリガーとなるオプションを指定
                command.HelpOption("-?|-h|--help");

                // コマンドの引数（名前と説明を引数で渡しているが、これはヘルプ出力で使用される）
                var restoreArgs = command.Argument("puzzle", "分割された画像を生成します.");

                // オプションの設定
                var puzzleOptionImage = command.Option("-i|--image <opitons>",
                    "対象の画像パス",
                    CommandOptionType.MultipleValue);
                var puzzleOptionOutput = command.Option("-o|--output <opitons>",
                    "分割後の出力画像パス",
                    CommandOptionType.MultipleValue);
                var puzzleOptionRow = command.Option("-r|--row <opitons>",
                    "分割行数",
                    CommandOptionType.MultipleValue);
                var puzzleOptionCol = command.Option("-c|--col <opitons>",
                    "分割列数",
                    CommandOptionType.MultipleValue);

                // 実行
                command.OnExecute(() =>
                {
                    var inputPath = string.Empty;
                    foreach (var value in puzzleOptionImage.Values)
                    {
                        inputPath = value;
                    }

                    var outputPath = string.Empty;
                    foreach (var value in puzzleOptionOutput.Values)
                    {
                        outputPath = value;
                    }

                    int rowCount = 0;
                    foreach (var value in puzzleOptionRow.Values)
                    {
                        rowCount = int.Parse(value);
                    }

                    int colCount = 0;
                    foreach (var value in puzzleOptionCol.Values)
                    {
                        colCount = int.Parse(value);
                    }

                    using (var bitmap = new Bitmap(inputPath))
                    {
                        var tileImage = TileImageRestorator.CreateRandomTileImage(bitmap, rowCount, colCount);
                        tileImage.Save(outputPath);
                    }
                    return 0;
                });
            });

            app.Execute(args);
        }
    }
}
