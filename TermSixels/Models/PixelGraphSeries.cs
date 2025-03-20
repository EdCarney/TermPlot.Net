using System.Collections;
using System.Drawing;
using System.Numerics;
using TermSixels.Interfaces;

namespace TermSixels.Models;

public struct PixelGraphSeries<T> : IPixelGraphSeries where T : INumber<T>
{
    public List<PixelGraphDataPoint> DataPoints { get; }

    public Color Color { get; set; }

    public int PointSize { get; set; }

    public PixelGraphSeries(IEnumerable<T> x, IEnumerable<T> y, Color color, int pointSize)
    {
        Color = color;
        PointSize = pointSize;

        // create data points, converting INumber<T> to double

        DataPoints = x.Select(xVal => double.CreateChecked(xVal))
            .Zip(y.Select(yVal => double.CreateChecked(yVal)))
            .Select(dataPoint => new PixelGraphDataPoint(dataPoint.First, dataPoint.Second))
            .ToList();
    }

    public PixelGraphSeries(T x, T y, Color color, int pointSize)
    {
        Color = color;
        PointSize = pointSize;
        DataPoints = [new(double.CreateChecked(x), double.CreateChecked(y))];
    }

    public IEnumerator<PixelGraphDataPoint> GetEnumerator()
    {
        return DataPoints.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
