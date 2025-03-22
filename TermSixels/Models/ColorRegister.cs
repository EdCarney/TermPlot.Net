using System.Drawing;

namespace TermSixels.Models;

/// <summary>
/// Color register for use in sixel image sequences.
/// </summary>
public struct ColorRegister
{
    private static int _totalRegisterCount = 0;

    public Color Color { get; private set; }

    public ColorSpace ColorSpace { get; set; }

    /// <summary>
    /// Red value of the color as a percentage from 0 to 100.
    /// </summary>
    public int Red { get; private set; }

    /// <summary>
    /// Green value of the color as a percentage from 0 to 100.
    /// </summary>
    public int Green { get; private set; }

    /// <summary>
    /// Blue value of the color as a percentage from 0 to 100.
    /// </summary>
    public int Blue { get; private set; }

    /// <summary>
    /// RGB values of the color as percentages from 0 to 100.
    /// </summary>
    public (int, int, int) RgbValues => (Red, Green, Blue);

    /// <summary>
    /// Hue value of the color as degrees from 0 to 360.
    /// </summary>
    public int Hue { get; private set; }

    /// <summary>
    /// Lightness value of the color value as a percentage from 0 to 100.
    /// </summary>
    public int Lightness { get; private set; }

    /// <summary>
    /// Saturation value of the color value as a percentage from 0 to 100.
    /// </summary>
    public int Saturation { get; private set; }

    /// <summary>
    /// HLS values of the color as degrees from 0 to 360 and percentages from
    /// 0 to 100.
    /// </summary>
    public (int, int, int) HlsValues => (Hue, Lightness, Saturation);

    /// <summary>
    /// The register number; a value unique to each register used to identify it.
    /// </summary>
    public int RegisterNumber { get; }

    /// <summary>
    /// The sixel string used to define the color register.
    /// </summary>
    public string SixelSetString
        => $"#{RegisterNumber};{(int)ColorSpace};{Red};{Green};{Blue}";

    /// <summary>
    /// The sixel string used to use the color register. Can only be used in a sixel
    /// sequence after the register is defined with the set string.
    /// </summary>
    public string SixelUseString
        => $"#{RegisterNumber}";

    /// <summary>
    /// Create a new register for the color in RGB space.
    /// </summary>
    /// <param name="color">The color for the register.</param>
    public ColorRegister(Color color) : this(color, ColorSpace.RGB) { }

    /// <summary>
    /// Create a new register for the color.
    /// </summary>
    /// <param name="color">The color for the register.</param>
    /// <param name="colorSpace">The color space to use for the register.</param>
    public ColorRegister(Color color, ColorSpace colorSpace)
    {
        SetColor(color);
        ColorSpace = colorSpace;
        RegisterNumber = _totalRegisterCount++;
    }

    private void SetColor(Color color)
    {
        Color = color;

        Red = 100 * Color.R / Byte.MaxValue;
        Green = 100 * Color.G / Byte.MaxValue;
        Blue = 100 * Color.B / Byte.MaxValue;

        Hue = (int)Color.GetHue();
        Lightness = (int)(100 * Color.GetBrightness());
        Saturation = (int)(100 * Color.GetSaturation());
    }
}
