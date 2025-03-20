using System.Drawing;
using TermSixels.Models;

namespace TermSixels.Interfaces;

public interface IPixelGraphSeries : IEnumerable<PixelGraphDataPoint>
{
    Color Color { get; set; }

    int PointSize { get; set; }
}
