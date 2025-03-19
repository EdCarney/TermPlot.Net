using System.Drawing;

namespace TermSixels.Models;

public class PixelGraph : PixelMap
{
    private readonly List<PixelGraphSeries> _pixelSeries = new();
    private PixelGraphDrawingConfig _drawingConfig = new();
    private int _uncorrectedHeight;
    private int _uncorrectedWidth;


    public PixelGraph(
            int size,
            PixelRatioCorrection pixelRatioCorrection = PixelRatioCorrection.NoCorrection,
            int pixelRatio = 1)
        : this(size, size, pixelRatioCorrection, pixelRatio) { }

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

    public PixelGraph WithConfig(PixelGraphDrawingConfig drawingConfig)
    {
        _drawingConfig = drawingConfig;
        return this;
    }

    public PixelGraph WithSeries(IEnumerable<int> x, IEnumerable<int> y, Color? color = null)
    {
        _pixelSeries.Add(new(x, y, color ?? Color.White));
        return this;
    }

    public PixelGraph WithSeries(int x, int y, Color? color = null, int? pointSize = null)
    {
        _pixelSeries.Add(new(x, y, color, pointSize));
        return this;
    }

    public override string ToBitMapString()
    {
        AddLegend();
        // AddSeriesData();

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

        xRowRange.PrintRange();
        xColRange.PrintRange();
        SetPixelColor(xRowRange, xColRange, Color.White);
    }

    private void AddSeriesData()
    {
        // get bounds
        int legendMinX = _drawingConfig.LegendMinX ??
            _pixelSeries.SelectMany(series => series.DataPoints.Select(p => p.x)).Min();
        int legendMinY = _drawingConfig.LegendMinY ??
            _pixelSeries.SelectMany(series => series.DataPoints.Select(p => p.y)).Min();
        int legendMaxX = _drawingConfig.LegendMaxX ??
            _pixelSeries.SelectMany(series => series.DataPoints.Select(p => p.x)).Max();
        int legendMaxY = _drawingConfig.LegendMaxY ??
            _pixelSeries.SelectMany(series => series.DataPoints.Select(p => p.y)).Max();
    }
}
