namespace TermSixels.Models;

public enum PixelRatioCorrection
{
    /// <summary>
    /// No correction will be done for pixel size discrepencies; drawings may not
    /// appear as expected.
    /// </summary>
    NoCorrection,

    /// <summary>
    /// Pixel map width is multiplied by the pixel ratio on creation; all lines drawn
    /// are multiplied in thickness in the width by the pixel ratio.
    /// </summary>
    AdjustWidth,

    /// <summary>
    /// Pixel map width is multiplied by the pixel ratio on creation; all lines drawn
    /// are multiplied in thickness in the height by the pixel ratio.
    /// </summary>
    AdjustHeight,
}
