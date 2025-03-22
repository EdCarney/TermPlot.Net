using System.Drawing;
using System.Numerics;
using TermSixels.Interfaces;

namespace TermSixels.Models;

/// <summary>
/// A pixel representation of a graph for plotting 2D data.
/// </summary>
public class PixelGraph : PixelMap
{
    private readonly List<IPixelGraphSeries> _pixelSeries = new();
    private PixelGraphDrawingConfig _drawingConfig = new();
    private int _uncorrectedHeight;
    private int _uncorrectedWidth;

    /// <summary>
    /// Create a new square pixel graph.
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
    public PixelGraph(
            int size,
            PixelRatioCorrection pixelRatioCorrection = PixelRatioCorrection.NoCorrection,
            int pixelRatio = 1)
        : this(size, size, pixelRatioCorrection, pixelRatio) { }

    /// <summary>
    /// Create a new pixel graph of a specified height and width.
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
    public PixelGraph(
            int height,
            int width,
            PixelRatioCorrection pixelRatioCorrection = PixelRatioCorrection.NoCorrection,
            int pixelRatio = 1)
        : base(height, width, pixelRatioCorrection, pixelRatio)
    {
        _uncorrectedHeight = height;
        _uncorrectedWidth = width;
    }

    /// <summary>
    /// Override the default configuration for the graph with custom values.
    /// </summary>
    public PixelGraph WithConfig(PixelGraphDrawingConfig drawingConfig)
    {
        _drawingConfig = drawingConfig;
        return this;
    }

    /// <summary>
    /// Adds a data series to the graph with the default color and point size as set in the graph drawing
    /// configuration.
    /// </summary>
    public PixelGraph WithSeries<T>(IEnumerable<T> x, IEnumerable<T> y) where T : INumber<T>
        => WithSeries(x, y, _drawingConfig.DefaultDataSeriesColor, _drawingConfig.DefaultDataSeriesRadius);

    /// <summary>
    /// Adds a data series to the graph with the specified color and the default point size as set in the
    /// default drawing configuration.
    /// graph drawing configuration.
    /// </summary>
    public PixelGraph WithSeries<T>(IEnumerable<T> x, IEnumerable<T> y, Color color) where T : INumber<T>
        => WithSeries(x, y, color, _drawingConfig.DefaultDataSeriesRadius);

    /// <summary>
    /// Adds a data series to the graph with the specified color and point size.
    /// graph drawing configuration.
    /// </summary>
    public PixelGraph WithSeries<T>(IEnumerable<T> x, IEnumerable<T> y, Color color, int pointRadius) where T : INumber<T>
    {
        if (x.Count() != y.Count())
            throw new ArgumentException($"{nameof(x)} and {nameof(y)} must have the same number of elements.");

        _pixelSeries.Add(new PixelGraphSeries<T>(x, y, color, pointRadius));
        return this;
    }

    /// <summary>
    /// Adds a single point to the graph with the default color and point size as set in the graph drawing
    /// configuration.
    /// </summary>
    public PixelGraph WithSeries<T>(T x, T y) where T : INumber<T>
        => WithSeries(x, y, _drawingConfig.DefaultDataSeriesColor, _drawingConfig.DefaultDataSeriesRadius);

    /// <summary>
    /// Adds a single point to the graph with the specified color and the default point size as set in the
    /// default drawing configuration.
    /// graph drawing configuration.
    /// </summary>
    public PixelGraph WithSeries<T>(T x, T y, Color color) where T : INumber<T>
        => WithSeries(x, y, color, _drawingConfig.DefaultDataSeriesRadius);

    /// <summary>
    /// Adds a single point to the graph with the specified color and point size.
    /// graph drawing configuration.
    /// </summary>
    public PixelGraph WithSeries<T>(T x, T y, Color color, int pointSize) where T : INumber<T>
    {
        _pixelSeries.Add(new PixelGraphSeries<T>(x, y, color, pointSize));
        return this;
    }

    /// <summary>
    /// Generate the sixel image sequence for this pixel graph.
    /// </summary>
    public override string ToBitMapString()
    {
        AddLegend();
        AddSeriesData();

        var sixelMap = new SixelMap(this);
        return sixelMap.ToSixelSequenceString();
    }

    private void AddLegend()
    {
        // draw y-axis
        var yRowRange = _drawingConfig.LegendBufferTop..^_drawingConfig.LegendBufferBottom;
        var yColRange = _drawingConfig.LegendBufferLeft..(_drawingConfig.LegendBufferLeft + _drawingConfig.LegendThickness);
        SetPixelColor(yRowRange, yColRange, Color.White);

        // draw x-axis
        var xRowRange = ^(_drawingConfig.LegendBufferBottom + _drawingConfig.LegendThickness)..^_drawingConfig.LegendBufferBottom;
        var xColRange = _drawingConfig.LegendBufferLeft..^_drawingConfig.LegendBufferRight;
        SetPixelColor(xRowRange, xColRange, Color.White);
    }

    private void AddSeriesData()
    {
        // no action required if there are no data series
        if (!_pixelSeries.Any())
            return;

        // get bounds
        double minX = _drawingConfig.LegendMinX ??
            _pixelSeries.SelectMany(series => series.Select(p => p.x)).Min();
        double minY = _drawingConfig.LegendMinY ??
            _pixelSeries.SelectMany(series => series.Select(p => p.y)).Min();
        double maxX = _drawingConfig.LegendMaxX ??
            _pixelSeries.SelectMany(series => series.Select(p => p.x)).Max();
        double maxY = _drawingConfig.LegendMaxY ??
            _pixelSeries.SelectMany(series => series.Select(p => p.y)).Max();

        // get data range
        double dataRangeX = maxX - minX;
        double dataRangeY = maxY - minY;

        // get scale factor
        double dataDrawingAreaAjustmentX =
            _drawingConfig.DataBufferLeft + _drawingConfig.DataBufferRight +
            _drawingConfig.LegendBufferLeft + _drawingConfig.LegendBufferRight +
            _drawingConfig.LegendThickness;
        double dataDrawingAreaAjustmentY =
            _drawingConfig.DataBufferTop + _drawingConfig.DataBufferBottom +
            _drawingConfig.LegendBufferTop + _drawingConfig.LegendBufferBottom +
            _drawingConfig.LegendThickness;
        double scaleFactorX = (double)(_uncorrectedWidth - dataDrawingAreaAjustmentX) / dataRangeX;
        double scaleFactorY = (double)(_uncorrectedHeight - dataDrawingAreaAjustmentY) / dataRangeY;

        // create anons for scaling points in x and y
        var scalePointX = (double x) => (x - minX) * scaleFactorX;
        var scalePointY = (double y) => (y - minY) * scaleFactorY;

        // need to shift points to account for drawing area
        double pointShiftX = _drawingConfig.LegendBufferLeft + _drawingConfig.LegendThickness + _drawingConfig.DataBufferLeft;
        double pointShiftY = _drawingConfig.LegendBufferBottom + _drawingConfig.LegendThickness + _drawingConfig.DataBufferBottom;

        // draw each point in each data series; scaling as necessary
        foreach (var series in _pixelSeries)
        {
            foreach (var point in series)
            {
                double scaledX = scalePointX(point.x) + pointShiftX;
                int colStart = (int)scaledX - series.PointSize;
                int colEnd = (int)scaledX + series.PointSize;

                double scaledY = scalePointY(point.y) + pointShiftY;
                int rowStart = (int)scaledY - series.PointSize;
                int rowEnd = (int)scaledY + series.PointSize;

                // need to reverse drawing along y-axis since larger indexes
                // mean further 'down' on the pixel map by default
                SetPixelColor(^rowEnd..^rowStart, colStart..colEnd, series.Color);
            }
        }
    }
}
