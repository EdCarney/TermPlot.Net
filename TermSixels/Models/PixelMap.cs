using System.Drawing;
using System.Text;
using TermSixels.Extensions;

namespace TermSixels.Models;

public class PixelMap
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly Color[][] _pixels;

    public int Height { get; }

    public int Width { get; }

    /// <summary>
    /// Determines how the pixel ratio correction should be applied. Since the pixel ratio must
    /// be equal to or greater than one (1), this is used to ensure that the correct dimension is
    /// adjusted.
    /// </summary>
    public PixelRatioCorrection PixelRatioCorrection { get; }

    /// <summary>
    /// The ratio between pixel dimensions. Must be a number equal to or greater than one (1), so
    /// this should be computed as [larger pixel dimension] / [smaller pixel dimension].
    /// </summary>
    public int PixelRatio { get; }

    /// <summary>
    /// The set of all unique colors that currently exist in the pixel map.
    /// </summary>
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

    /// <summary>
    /// Create a new square pixel map.
    /// </summary>
    /// <param name="size">The height and wide of the map.</param>
    /// <param name="pixelRatioCorrection">
    /// Optional pixel ratio correction type. This is used to correct drawing size if pixel representation
    /// in the terminal is not square.
    /// </param>
    /// <param name="pixelRatio">
    /// The ratio between pixel height and width. Note this must be a number greater than or equal to one.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the specified size or pixel ratio are invalid.
    /// </exception>
    public PixelMap(
            int size,
            PixelRatioCorrection pixelRatioCorrection = PixelRatioCorrection.NoCorrection,
            int pixelRatio = 1)
        : this(size, size, pixelRatioCorrection, pixelRatio) { }

    /// <summary>
    /// Create a new pixel map of a specified height and width.
    /// </summary>
    /// <param name="height">The height the map.</param>
    /// <param name="width">The width the map.</param>
    /// <param name="pixelRatioCorrection">
    /// Optional pixel ratio correction type. This is used to correct drawing size if pixel representation
    /// in the terminal is not square.
    /// </param>
    /// <param name="pixelRatio">
    /// The ratio between pixel height and width. Note this must be a number greater than or equal to one.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the specified size or pixel ratio are invalid.
    /// </exception>
    public PixelMap(
            int height,
            int width,
            PixelRatioCorrection pixelRatioCorrection = PixelRatioCorrection.NoCorrection,
            int pixelRatio = 1)
    {
        height.Validate(ValidatorComparison.GT, nameof(height));
        width.Validate(ValidatorComparison.GT, nameof(width));
        pixelRatio.Validate(ValidatorComparison.GT_EQ, 1, nameof(pixelRatio));

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
        => this[row..(row + 1), col..(col + 1)].First();

    public Color[] this[Range rowRange, int col]
        => this[rowRange, col..(col + 1)];

    public Color[] this[int row, Range colRange]
        => this[row..(row + 1), colRange];

    public Color[] this[Range rowRange, Range colRange]
    {
        get
        {
            _pixels.ValidateBound(rowRange);
            _pixels.First().ValidateBound(colRange);

            var (rowStart, rowEnd) = _pixels.GetBoundsFromStart(rowRange);
            var (colStart, colEnd) = _pixels.First().GetBoundsFromStart(colRange);

            int numRows = rowEnd - rowStart;
            int numCols = colEnd - colStart;
            int numPixs = numRows * numCols;

            int index = 0;
            var pixs = new Color[numRows * numCols];
            for (int i = rowStart; i < rowEnd; i++)
                for (int j = colStart; j < colEnd; j++)
                    pixs[index++] = _pixels[i][j];

            return pixs;
        }
    }

    /// <summary>
    /// Set the color of a specific pixel.
    /// </summary>
    /// <param name="row">Pixel row.</param>
    /// <param name="col">Pixel column.</param>
    /// <param name="color">Color to set pixel to.</param>
    public void SetPixelColor(int row, int col, Color color)
        => SetPixelColor(row..(row + 1), col..(col + 1), color);

    /// <summary>
    /// Set the color of a pixel range
    /// </summary>
    /// <param name="row">Pixel row.</param>
    /// <param name="colRange">Pixel column range.</param>
    /// <param name="color">Color to set pixels to.</param>
    public void SetPixelColor(int row, Range colRange, Color color)
        => SetPixelColor(row..(row + 1), colRange, color);

    /// <summary>
    /// Set the color of a pixel range
    /// </summary>
    /// <param name="rowRange">Pixel row range.</param>
    /// <param name="col">Pixel column.</param>
    /// <param name="color">Color to set pixels to.</param>
    public void SetPixelColor(Range rowRange, int col, Color color)
        => SetPixelColor(rowRange, col..(col + 1), color);

    /// <summary>
    /// Set the color of a pixel range
    /// </summary>
    /// <param name="rowRange">Pixel row range.</param>
    /// <param name="colRange">Pixel column range.</param>
    /// <param name="color">Color to set pixels to.</param>
    public void SetPixelColor(Range rowRange, Range colRange, Color color)
    {
        _logger.Trace($"Setting pixel range ({rowRange.ToRangeString()}) ({colRange.ToRangeString()}) to {color}");

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
        _logger.Trace($"Original pixel coordinate: ({height}, {width})");

        switch (PixelRatioCorrection)
        {
            case PixelRatioCorrection.AdjustWidth:
                width *= PixelRatio;
                break;
            case PixelRatioCorrection.AdjustHeight:
                height *= PixelRatio;
                break;
            default:
                break;
        }

        _logger.Trace($"Corrected pixel coordinate: ({height}, {width})");

        return (height, width);
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
        _logger.Trace($"Original pixel range: height = ({heightRange.ToRangeString()}), width = ({widthRange.ToRangeString()})");

        var widthRangeFromStart = widthRange.FromStart(maxWidth);
        var heightRangeFromStart = heightRange.FromStart(maxHeight);

        switch (PixelRatioCorrection)
        {
            case PixelRatioCorrection.AdjustWidth:
                int wStart = widthRangeFromStart.Start.Value * (widthRange.Start.IsFromEnd ? 1 : PixelRatio);
                int wEnd = widthRangeFromStart.End.Value * (widthRange.End.IsFromEnd ? 1 : PixelRatio);
                widthRangeFromStart = wStart..wEnd;
                break;
            case PixelRatioCorrection.AdjustHeight:
                int hStart = heightRangeFromStart.Start.Value * (heightRange.Start.IsFromEnd ? 1 : PixelRatio);
                int hEnd = heightRangeFromStart.End.Value * (heightRange.End.IsFromEnd ? 1 : PixelRatio);
                heightRangeFromStart = hStart..hEnd;
                break;
            default:
                break;
        }

        _logger.Trace($"Corrected pixel range: height = ({heightRangeFromStart.ToRangeString()}), width = ({widthRangeFromStart.ToRangeString()})");

        return (heightRangeFromStart, widthRangeFromStart);
    }

    /// <summary>
    /// Get a bit map representation of the pixel map. Ones (1) are used to represent any non-black
    /// pixel. Black pixels are represented with zeros (0).
    /// </summary>
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
