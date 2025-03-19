using System.Drawing;

namespace TermSixels.Models;

public struct PixelGraphSeries
{
    public List<PixelGraphDataPoint> DataPoints { get; }

    public Color Color { get; set; } = Color.White;

    public int PointSize { get; set; } = 2;

    public PixelGraphSeries(IEnumerable<int> x, IEnumerable<int> y, Color? color = null, int? pointSize = null)
    {
        Color = color ?? Color;
        PointSize = pointSize ?? PointSize;
        DataPoints = x.Zip(y)
            .Select(dataPoint => new PixelGraphDataPoint(dataPoint.First, dataPoint.Second))
            .ToList();
    }

    public PixelGraphSeries(int x, int y, Color? color = null, int? pointSize = null)
    {
        Color = color ?? Color;
        PointSize = pointSize ?? PointSize;
        DataPoints = [new(x, y)];
    }
}
