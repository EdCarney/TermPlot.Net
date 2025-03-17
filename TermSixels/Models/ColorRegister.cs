using System.Drawing;

namespace TermSixels.Models;

public struct ColorRegister
{
    private static int _totalRegisterCount = 0;

    public Color Color { get; private set; }

    public ColorSpace ColorSpace { get; set; }

    public int Red { get; private set; }
    public int Green { get; private set; }
    public int Blue { get; private set; }
    public (int, int, int) RgbValues => (Red, Green, Blue);

    public int Hue { get; private set; }
    public int Lightness { get; private set; }
    public int Saturation { get; private set; }
    public (int, int, int) HlsValues => (Hue, Lightness, Saturation);

    public int RegisterNumber { get; }

    public ColorRegister(Color color) : this(color, ColorSpace.RGB) { }

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

    public string ToSixelSetString()
        => $"#{RegisterNumber};{(int)ColorSpace};{Red};{Green};{Blue}";

    public string ToSixelUseString()
        => $"#{RegisterNumber}";
}
