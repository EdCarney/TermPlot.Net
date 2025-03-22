using System.Collections;
using System.Drawing;
using System.Numerics;
using TermSixels.Extensions;
using TermSixels.Interfaces;

namespace TermSixels.Models;

public struct PixelGraphSeries<T> : IPixelGraphSeries where T : INumber<T>
{
    public List<PixelGraphDataPoint> DataPoints { get; }

    public Color Color { get; set; }

    public int PointSize { get; set; }

    /// <summary>
    /// Creates a new 2D graph series using the provided x and y coordinates.
    /// </summary>
    /// <param name="x">Values of data along the x-axis.</param>
    /// <param name="y">Values of data along the y-axis.</param>
    /// <param name="color">Color to plot the series.</param>
    /// <param name="pointSize">The point size as a radius around the data point.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the lengths of the x and y data are not the same, or if the point size
    /// is an invalid number.
    /// </exception>
    public PixelGraphSeries(IEnumerable<T> x, IEnumerable<T> y, Color color, int pointSize)
    {
        x.Count().Validate(ValidatorComparison.EQ, y.Count(), "x and y");
        pointSize.Validate(ValidatorComparison.GT, nameof(pointSize));

        Color = color;
        PointSize = pointSize;

        // create data points, converting INumber<T> to double

        DataPoints = x.Select(xVal => double.CreateChecked(xVal))
            .Zip(y.Select(yVal => double.CreateChecked(yVal)))
            .Select(dataPoint => new PixelGraphDataPoint(dataPoint.First, dataPoint.Second))
            .ToList();
    }

    /// <summary>
    /// Creates a new 2D graph series using the provided x and y coordinate.
    /// </summary>
    /// <param name="x">Value of datum point along the x-axis.</param>
    /// <param name="y">Value of datum point along the y-axis.</param>
    /// <param name="color">Color to plot the series.</param>
    /// <param name="pointSize">The point size as a radius around the data point.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the point size is an invalid number.
    /// </exception>
    public PixelGraphSeries(T x, T y, Color color, int pointSize)
    {
        pointSize.Validate(ValidatorComparison.GT, nameof(pointSize));

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
