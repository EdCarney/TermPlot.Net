﻿using System.Drawing;
using System.Text;
using TermSixels.Models;

namespace TestOutput;

class Program
{
    const string DCS_StartSixelSeq = "\x1bPq";
    const string DCS_TerminateSixelSeq = "\x1b";
    const char SixelControl_CR = '$';
    const char SixelControl_CRLF = '-';

    static void Main(string[] args)
    {

        var testDataSeriesX_0 = Enumerable.Range(10, 50);
        var testDataSeriesY_0 = testDataSeriesX_0.Select(x => x * 10);

        var testDataSeriesX_1 = Enumerable.Range(10, 50);
        var testDataSeriesY_1 = testDataSeriesX_1.Select(x => x * x);

        var testDataSeriesX_2 = Enumerable.Range(10, 50);
        var testDataSeriesY_2 = testDataSeriesX_1.Select(x => x * x * x);

        var testDataSeriesX_3 = Enumerable.Range(10, 50).Select(x => (double)x);
        var testDataSeriesY_3 = testDataSeriesX_2.Select(x => Math.Sin(x));

        var graph = new PixelGraph(500, 500, PixelRatioCorrection.AdjustWidth, pixelRatio: 2)
            .WithConfig(
                    new()
                    {
                        LegendBufferTop = 5,
                        LegendBufferBottom = 5,
                        LegendBufferRight = 5,
                        LegendBufferLeft = 5,
                        LegendThickness = 2
                    })
            .WithSeries<int>(testDataSeriesX_0, testDataSeriesY_0, Color.White)
            .WithSeries<int>(testDataSeriesX_1, testDataSeriesY_1, Color.Green);
        //.WithSeries<int>(testDataSeriesX_2, testDataSeriesY_2, Color.Yellow);
        System.Console.WriteLine(graph.ToBitMapString());

        // TODO: can't quite do this yet; need to figure out how to get current terminal width in pixels
        //
        // term_data.WriteLine("\e[14t");
        //
        //
        // int colCount = Console.WindowWidth;
        // Font font = Console.OutputEncoding.GetString(new byte[] { 0x00 });
        //
        // double charWidthInPixels;
        //
        // using (var g = Graphics.FromImage(new Bitmap(1, 1)))
        // {
        //     charWidthInPixels = g.MeasureString("W", font).Width;
        // }
        //
        // int termWidthInPixels = (int)(colCount * charWidthInPixels);
    }

    static void TestPrintSixelMap()
    {
        var pixelMap = new PixelMap(1000);
        pixelMap.SetPixelColor(10..991, 10, Color.White);
        pixelMap.SetPixelColor(990, 10..991, Color.White);
        pixelMap.SetPixelColor(990, 50..70, Color.Green);
        var sixelMap = new SixelMap(pixelMap);
        Console.WriteLine(sixelMap.ToSixelSequenceString());

        var pixelMap1 = new PixelMap(1000, PixelRatioCorrection.AdjustWidth, pixelRatio: 2);
        pixelMap1.SetPixelColor(10..991, 10, Color.White);
        pixelMap1.SetPixelColor(990, 10..991, Color.White);
        pixelMap1.SetPixelColor(990, 50..70, Color.Green);
        var sixelMap1 = new SixelMap(pixelMap1);
        Console.WriteLine(sixelMap1.ToSixelSequenceString());

        string dirPath = @"/home/edcarney/personal/TermPlot.Net";

        using var sw_sixel = File.CreateText(Path.Join(dirPath, "sixel_img.txt"));
        using var sw_pixel = File.CreateText(Path.Join(dirPath, "pixel_img.txt"));
        using var term_data = File.CreateText(Path.Join(dirPath, "term_data.txt"));

        sw_sixel.WriteLine(sixelMap.ToSixelSequenceString());
        sw_pixel.WriteLine(pixelMap.ToBitMapString());
    }

    static void PrintTestMessage()
    {
        var msgBuilder = new StringBuilder();

        msgBuilder.Append(DCS_StartSixelSeq);

        for (int i = 0; i < 300; i++)
        {
            if (i % 10 == 0)
            {
                msgBuilder.Append(GetSixelChar(0b_110000));
            }
            else
            {
                msgBuilder.Append(GetSixelChar(0b_111111));
            }
        }

        msgBuilder.Append(DCS_TerminateSixelSeq);

        Console.WriteLine(msgBuilder.ToString());
    }

    static StringBuilder GetBitMapSixelStringBuilder(int[,] bitmap)
    {
        // the 0-th dimension is the vertical; so we need to divide this by 6
        // to get the number of sixels needed

        var sixelMap = new int[bitmap.GetLength(0) / 6, bitmap.GetLength(1)];

        for (int i = 0; i < sixelMap.GetLength(0); i++)
        {
            for (int j = 0; j < sixelMap.GetLength(1); j++)
            {
                int sixelVal = 0;
                for (int sixelItr = 0; sixelItr < 6; sixelItr++)
                {
                    sixelVal += bitmap[i * 6 + sixelItr, j] * (int)Math.Pow(2, sixelItr);
                }
                sixelMap[i, j] = sixelVal;
            }
        }

        // Console.WriteLine();
        // for (int i = 0; i < sixelMap.GetLength(0); i++)
        // {
        //     for (int j = 0; j < sixelMap.GetLength(1); j++)
        //     {
        //         Console.Write($"{sixelMap[i, j]}");
        //     }
        //     Console.WriteLine();
        // }

        var sixelSb = new StringBuilder(DCS_StartSixelSeq);

        // to build the string we need to arrange the sixels horizontally, then
        // vertically; so we need to iterate across each full row first

        for (int i = 0; i < sixelMap.GetLength(0); i++)
        {
            for (int j = 0; j < sixelMap.GetLength(1); j++)
            {
                sixelSb.Append(GetSixelChar(sixelMap[i, j]));
            }
            sixelSb.Append(SixelControl_CRLF);
        }

        sixelSb.Append(DCS_TerminateSixelSeq);

        return sixelSb;
    }

    static int[,] GetDataBitmap(IEnumerable<double> x, IEnumerable<double> y)
    {
        // for now we will create a 300 X 300 size image
        // assume that the data provided is also 300 in length exactly


        var bitmap = new int[300, 300];

        int legendBuffer = 5;
        int legendThickness = 5;

        for (int i = 0; i < bitmap.GetLength(0); i++)
        {
            if (i < legendBuffer || i >= 300 - legendBuffer)
            {
                continue;
            }

            for (int j = 0; j < bitmap.GetLength(1); j++)
            {
                if (j < legendBuffer || j >= legendBuffer + legendThickness)
                {
                    continue;
                }

                bitmap[i, j] = 1;
            }
        }

        return bitmap;
    }

    // note that the end of the mask is the 'top' vertically;
    // so smaller value numbers are towards the top
    static char GetSixelChar(int binaryMask)
    {
        if (binaryMask < 0 || binaryMask > 0b_111111)
        {
            throw new ArgumentException("Binary mask must be within valid sixel range", nameof(binaryMask));
        }

        return (char)(63 + binaryMask);
    }
}
