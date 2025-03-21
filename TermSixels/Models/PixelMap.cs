﻿using System.Drawing;
using System.Text;
using TermSixels.Extensions;

namespace TermSixels.Models;

public class PixelMap
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly Color[][] _pixels;

    public int Height { get; }

    public int Width { get; }

    public PixelRatioCorrection PixelRatioCorrection { get; }

    public int PixelRatio { get; }

    public HashSet<Color> AllUniqueColors
    {
        get
        {
            var colorSet = new HashSet<Color>();
            foreach (var row in _pixels)
                foreach (var pixColor in row)
                    colorSet.Add(pixColor);
            return colorSet;
        }
    }

    public PixelMap(
            int size,
            PixelRatioCorrection pixelRatioCorrection = PixelRatioCorrection.NoCorrection,
            int pixelRatio = 1)
        : this(size, size, pixelRatioCorrection, pixelRatio) { }

    public PixelMap(
            int height,
            int width,
            PixelRatioCorrection pixelRatioCorrection = PixelRatioCorrection.NoCorrection,
            int pixelRatio = 1)
    {
        _logger.Debug($"Requested dimensions: {nameof(height)} = {height}, {nameof(width)} = {width}");

        PixelRatioCorrection = pixelRatioCorrection;
        PixelRatio = pixelRatio;

        var (correctedH, correctedW) = CorrectHeightWidth(height, width);
        Height = correctedH % 6 == 0 ? correctedH : correctedH + 6 - correctedH % 6;
        Width = correctedW;

        _logger.Debug($"Corrected dimensions: {nameof(height)} = {Height}, {nameof(width)} = {Width}");

        _pixels = new Color[Height]
            .Select(row => Enumerable.Repeat(Color.Black, Width).ToArray())
            .ToArray();
    }

    public Color this[int row, int col]
    {
        get
        {
            (row, col) = CorrectHeightWidth(row, col);
            _pixels.ValidateBound(row);
            _pixels.First().ValidateBound(col);

            return _pixels[row][col];
        }
    }

    public Color[] this[Range rowRange, int col]
    {
        get
        {
            _pixels.ValidateBound(rowRange);
            _pixels.First().ValidateBound(col);

            var (start, end) = _pixels.GetBoundsFromStart(rowRange);
            var pixs = new Color[end - start];
            for (int i = start; i < end; i++)
                pixs[i - start] = _pixels[i][col];

            return pixs;
        }
    }

    public Color[] this[int row, Range colRange]
    {
        get
        {
            (row, colRange) = CorrectHeightWidth(row, colRange);
            _pixels.ValidateBound(row);
            _pixels.First().ValidateBound(colRange);

            return _pixels[row][colRange];
        }
    }

    public Color[][] this[Range rowRange, Range colRange]
    {
        get
        {
            (rowRange, colRange) = CorrectHeightWidth(rowRange, colRange);
            _pixels.ValidateBound(rowRange);
            _pixels.First().ValidateBound(colRange);

            var (rowStart, rowEnd) = _pixels.GetBoundsFromStart(rowRange);
            var (colStart, colEnd) = _pixels.First().GetBoundsFromStart(colRange);

            var pixs = new Color[rowEnd - rowStart]
                .Select(row => new Color[colEnd - colStart])
                .ToArray();
            for (int i = rowStart; i < rowEnd; i++)
                for (int j = colStart; j < colEnd; j++)
                    pixs[i - rowStart][j - colStart] = _pixels[i][j];

            return pixs;
        }
    }

    public void SetPixelColor(int row, int col, Color color)
    {
        (row, col) = CorrectHeightWidth(row, col);
        _pixels.ValidateBound(row);
        _pixels.First().ValidateBound(col);

        _pixels[row][col] = color;
    }

    public void SetPixelColor(int row, Range colRange, Color color)
    {
        (row, colRange) = CorrectHeightWidth(row, colRange);
        _pixels.ValidateBound(row);
        _pixels.First().ValidateBound(colRange);

        var (colStart, colEnd) = _pixels.First().GetBoundsFromStart(colRange);

        for (int j = colStart; j < colEnd; j++)
            _pixels[row][j] = color;
    }

    public void SetPixelColor(Range rowRange, int col, Color color)
    {
        (rowRange, col) = CorrectHeightWidth(rowRange, col);
        _pixels.ValidateBound(rowRange);
        _pixels.First().ValidateBound(col);

        var (rowStart, rowEnd) = _pixels.GetBoundsFromStart(rowRange);

        for (int i = rowStart; i < rowEnd; i++)
            _pixels[i][col] = color;
    }

    public void SetPixelColor(Range rowRange, Range colRange, Color color)
    {
        (rowRange, colRange) = CorrectHeightWidth(rowRange, colRange);

        _pixels.ValidateBound(rowRange);
        _pixels.First().ValidateBound(colRange);

        var (rowStart, rowEnd) = _pixels.GetBoundsFromStart(rowRange);
        var (colStart, colEnd) = _pixels.First().GetBoundsFromStart(colRange);

        for (int i = rowStart; i < rowEnd; i++)
            for (int j = colStart; j < colEnd; j++)
                _pixels[i][j] = color;
    }

    private (int height, int width) CorrectHeightWidth(int height, int width)
    {
        switch (PixelRatioCorrection)
        {
            case PixelRatioCorrection.AdjustWidth:
                return (height, width * PixelRatio);
            case PixelRatioCorrection.AdjustHeight:
                return (height * PixelRatio, width);
            default:
                return (height, width);
        }
    }

    private (Range heightRange, int width) CorrectHeightWidth(Range heightRange, int width)
    {
        var (newHeightRange, newWidthRange) = CorrectHeightWidth(heightRange, width..(width + 1));
        return (newHeightRange, newWidthRange.Start.Value);
    }

    private (int height, Range widthRange) CorrectHeightWidth(int height, Range widthRange)
    {
        var (newHeightRange, newWidthRange) = CorrectHeightWidth(height..(height + 1), widthRange);
        return (newHeightRange.Start.Value, newWidthRange);
    }

    private (Range heightRange, Range widthRange) CorrectHeightWidth(Range heightRange, Range widthRange)
        => CorrectHeightWidth(heightRange, widthRange, _pixels.Count(), _pixels.First().Count());

    private (Range heightRange, Range widthRange) CorrectHeightWidth(Range heightRange, Range widthRange, int maxHeight, int maxWidth)
    {
        var widthRangeFromStart = widthRange.FromStart(maxWidth);
        var heightRangeFromStart = heightRange.FromStart(maxHeight);

        switch (PixelRatioCorrection)
        {
            case PixelRatioCorrection.AdjustWidth:
                int wStart = widthRangeFromStart.Start.Value * (widthRange.Start.IsFromEnd ? 1 : PixelRatio);
                int wEnd = widthRangeFromStart.End.Value * (widthRange.End.IsFromEnd ? 1 : PixelRatio);
                return (heightRangeFromStart, wStart..wEnd);
            case PixelRatioCorrection.AdjustHeight:
                int hStart = heightRangeFromStart.Start.Value * (heightRange.Start.IsFromEnd ? 1 : PixelRatio);
                int hEnd = heightRangeFromStart.End.Value * (heightRange.End.IsFromEnd ? 1 : PixelRatio);
                return (hStart..hEnd, widthRangeFromStart);
            default:
                return (heightRange, widthRange);
        }
    }

    public virtual string ToBitMapString()
    {
        var sb = new StringBuilder();

        foreach (var row in _pixels)
        {
            foreach (var pix in row)
            {
                sb.Append(pix == Color.Black ? "0" : "1");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
