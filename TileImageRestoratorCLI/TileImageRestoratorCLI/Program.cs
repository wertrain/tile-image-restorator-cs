﻿using System;
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
                var tableOptionImage = command.Option("-i|--image <filepath>",
                    "修復対象の画像パス",
                    CommandOptionType.SingleValue);
                var tableOptionExample = command.Option("-e|--example <filepath>",
                    "修復後のサンプル画像パス",
                    CommandOptionType.SingleValue);
                var tableOptionTable = command.Option("-t|--table <filepath>",
                    "テーブルデータまでのパス",
                    CommandOptionType.SingleValue);
                var tableOptionRow = command.Option("-r|--row <row>",
                    "分割行数",
                    CommandOptionType.SingleValue);
                var tableOptionCol = command.Option("-c|--col <col>",
                    "分割列数",
                    CommandOptionType.SingleValue);
                var tableOptionWidth = command.Option("-w|--width <width>",
                   "分割画像幅",
                   CommandOptionType.SingleValue);
                var tableOptionHeight = command.Option("-g|--height <height>",
                    "分割画像高さ",
                    CommandOptionType.SingleValue);

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

                    int tileWidth = 0;
                    foreach (var value in tableOptionWidth.Values)
                    {
                        tileWidth = int.Parse(value);
                    }

                    int tileHeight = 0;
                    foreach (var value in tableOptionHeight.Values)
                    {
                        tileHeight = int.Parse(value);
                    }

                    using (var image = new Bitmap(inputPath))
                    using (var example = new Bitmap(examplePath))
                    {
                        if (File.Exists(tablePath))
                        {
                            var tableData = TileImageRestorator.LoadTable(tablePath);
                            TileImageRestorator.UpdateRestoreTable(tableData, image, example, tileWidth, tileHeight, rowCount, colCount);
                        }
                        else
                        {
                            var tableData = TileImageRestorator.CreateRestoreTable(image, example, tileWidth, tileHeight, rowCount, colCount);
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
                var restoreOptionInput = command.Option("-i|--input <filepath>",
                    "リストア対象の画像パス",
                    CommandOptionType.SingleValue);
                var restoreOptionOutput = command.Option("-o|--output <filepath>",
                    "リストア後の出力画像パス",
                    CommandOptionType.SingleValue);
                var restoreOptionTable = command.Option("-t|--table <filepath>",
                    "テーブルデータまでのパス",
                    CommandOptionType.SingleValue);

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
                var puzzleOptionImage = command.Option("-i|--image <filepath>",
                    "対象の画像パス",
                    CommandOptionType.SingleValue);
                var puzzleOptionOutput = command.Option("-o|--output <filepath>",
                    "分割後の出力画像パス",
                    CommandOptionType.SingleValue);
                var puzzleOptionWidth = command.Option("-w|--width <width>",
                    "分割画像幅",
                    CommandOptionType.SingleValue);
                var puzzleOptionHeight = command.Option("-g|--height <height>",
                    "分割画像高さ",
                    CommandOptionType.SingleValue);

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

                    int tileWidth = 0;
                    foreach (var value in puzzleOptionWidth.Values)
                    {
                        tileWidth = int.Parse(value);
                    }

                    int tileHeight = 0;
                    foreach (var value in puzzleOptionHeight.Values)
                    {
                        tileHeight = int.Parse(value);
                    }

                    using (var bitmap = new Bitmap(inputPath))
                    {
                        var tileImage = TileImageRestorator.CreateRandomTileImage(bitmap, tileWidth, tileHeight);
                        tileImage.Save(outputPath);
                    }
                    return 0;
                });
            });

            app.Execute(args);
        }
    }
}
