namespace TermSixels.Models;

public class PixelGraphDrawingConfig
{
    private const int _defaultBuffer = 2;

    // buffer area from legend ends to end of pixel map

    public int LegendBufferLeft { get; set; } = _defaultBuffer;
    public int LegendBufferRight { get; set; } = _defaultBuffer;
    public int LegendBufferTop { get; set; } = _defaultBuffer;
    public int LegendBufferBottom { get; set; } = _defaultBuffer;

    // legend drawing config

    public int LegendThickness { get; set; } = 2;

    // buffer area for the data within the legend

    public int DataBufferLeft { get; set; } = _defaultBuffer;
    public int DataBufferRight { get; set; } = _defaultBuffer;
    public int DataBufferTop { get; set; } = _defaultBuffer;
    public int DataBufferBottom { get; set; } = _defaultBuffer;

    // legend limits

    public int? LegendMinX { get; set; }
    public int? LegendMinY { get; set; }
    public int? LegendMaxX { get; set; }
    public int? LegendMaxY { get; set; }

}
