using System.Drawing;

namespace TermSixels.Models;

public class PixelGraphDrawingConfig
{
    private const int _defaultBuffer = 2;

    #region Legend Buffer

    public int LegendBufferLeft { get; set; } = _defaultBuffer;
    public int LegendBufferRight { get; set; } = _defaultBuffer;
    public int LegendBufferTop { get; set; } = _defaultBuffer;
    public int LegendBufferBottom { get; set; } = _defaultBuffer;

    #endregion

    #region Legend Drawing

    public int LegendThickness { get; set; } = 2;
    public Color LegendColor { get; set; } = Color.White;

    #endregion

    #region Data Area Buffer

    public int DataBufferLeft { get; set; } = _defaultBuffer;
    public int DataBufferRight { get; set; } = _defaultBuffer;
    public int DataBufferTop { get; set; } = _defaultBuffer;
    public int DataBufferBottom { get; set; } = _defaultBuffer;

    #endregion

    #region Legend Limits

    public int? LegendMinX { get; set; }
    public int? LegendMinY { get; set; }
    public int? LegendMaxX { get; set; }
    public int? LegendMaxY { get; set; }

    #endregion

    #region Data Series

    public int DefaultDataSeriesRadius { get; set; } = 1;
    public Color DefaultDataSeriesColor { get; set; } = Color.White;

    #endregion
}
