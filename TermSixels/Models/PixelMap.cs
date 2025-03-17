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
            return _pixels[rowRange][col];
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
            return _pixels[rowRange][colRange];
        }
    }

    public void SetPixelColor(int row, int col, Color color)
    {
        if (row < 0 || row >= Height)
            throw new ArgumentOutOfRangeException(nameof(row));

        if (col < 0 || col >= Width)
            throw new ArgumentOutOfRangeException(nameof(col));

        _pixels[row][col] = color;
    }

    public void SetPixelColor(Range rowRange, Range colRange, Color color)
    {
        _pixels.ValidateBound(rowRange);
        _pixels.First().ValidateBound(colRange);

        var (rowStart, rowEnd) = _pixels.GetBoundsFromStart(rowRange);
        var (colStart, colEnd) = _pixels.First().GetBoundsFromStart(colRange);

        for (int i = rowStart; i < rowEnd; i++)
            for (int j = colStart; j < colEnd; j++)
                _pixels[i][j] = color;
    }
}
