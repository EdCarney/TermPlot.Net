using System.Drawing;

namespace TermSixels;

public class PixelMap
{
    private readonly Pixel[][] _pixels;

    public int Height { get; }

    public int Width { get; }

    public PixelMap(int size) : this(size, size) { }

    public PixelMap(int height, int width)
    {
        Height = height % 6 == 0 ? height : height + (6 - height % 6);
        Width = width;

        _pixels = new Pixel[Height]
            .Select(row => new Pixel[Width])
            .ToArray();
    }

    public Pixel this[int row, int col] => _pixels[row][col];

    public void SetPixelColor(int row, int col, Color color)
    {
        if (row < 0 || row >= Height)
            throw new ArgumentOutOfRangeException(nameof(row));

        if (col < 0 || col >= Width)
            throw new ArgumentOutOfRangeException(nameof(col));

        _pixels[row][col].UpdatePixelColor(color);
    }

    public void SetPixelColor(Range rowRange, Range colRange, Color color)
    {
        rowRange.ValidateBound(Height);
        colRange.ValidateBound(Width);

        foreach (var row in _pixels[rowRange])
            foreach (var pix in row[colRange])
                pix.UpdatePixelColor(color);
    }
}
