using System.Drawing;
using TermSixels.Extensions;

namespace TermSixels.Models;

public class PixelMap
{
    private readonly Color[][] _pixels;

    public int Height { get; }

    public int Width { get; }

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

    public PixelMap(int size) : this(size, size) { }

    public PixelMap(int height, int width)
    {
        Height = height % 6 == 0 ? height : height + (6 - height % 6);
        Width = width;

        _pixels = new Color[Height]
            .Select(row => new Color[Width])
            .ToArray();
    }

    public Color this[int row, int col] => _pixels[row][col];

    public Color[] this[Range rowRange, int col]
    {
        get
        {
            _pixels.ValidateBound(rowRange);
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
            _pixels.First().ValidateBound(colRange);
            return _pixels[row][colRange];
        }
    }

    public Color[][] this[Range rowRange, Range colRange]
    {
        get
        {
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
        _pixels.ValidateBound(row);
        _pixels.First().ValidateBound(col);
        _pixels[row][col] = color;
    }

    public void SetPixelColor(int row, Range colRange, Color color)
    {
        _pixels.ValidateBound(row);
        _pixels.First().ValidateBound(colRange);

        var (colStart, colEnd) = _pixels.First().GetBoundsFromStart(colRange);

        for (int j = colStart; j < colEnd; j++)
            _pixels[row][j] = color;
    }

    public void SetPixelColor(Range rowRange, int col, Color color)
    {
        _pixels.ValidateBound(rowRange);
        _pixels.First().ValidateBound(col);

        var (rowStart, rowEnd) = _pixels.GetBoundsFromStart(rowRange);

        for (int i = rowStart; i < rowEnd; i++)
            _pixels[i][col] = color;
    }

    public void SetPixelColor(Range rowRange, Range colRange, Color color)
    {
        _pixels.ValidateBound(rowRange);
        _pixels.ValidateBound(colRange);

        var (rowStart, rowEnd) = _pixels.GetBoundsFromStart(rowRange);
        var (colStart, colEnd) = _pixels.First().GetBoundsFromStart(colRange);

        for (int i = rowStart; i < rowEnd; i++)
            for (int j = colStart; j < colEnd; j++)
                _pixels[i][j] = color;
    }
}
