namespace TermSixels.Models;

public enum PixelRatioCorrection
{
    // no correction will be done for pixel size discrepencies; drawings
    // may not appear as expected
    NoCorrection,

    // pixel map width is multiplied by the pixel ratio on creation;
    // all lines drawn are multiplied in thickness in the width by the pixel
    // ratio
    AdjustWidth,

    // pixel map height is multiplied by the pixel ratio on creation;
    // all lines drawn are multiplied in thickness in the height by the pixel
    // ratio
    AdjustHeight,
}
