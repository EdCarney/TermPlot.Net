using System.Drawing;

namespace TermSixels.Models;

public class Pixel
{
    public Color Color { get; private set; }

    public int Red { get; private set; }
    public int Green { get; private set; }
    public int Blue { get; private set; }
    public (int, int, int) RgbValues => (Red, Green, Blue);

    public int Hue { get; private set; }
    public int Lightness { get; private set; }
    public int Saturation { get; private set; }
    public (int, int, int) HlsValues => (Hue, Lightness, Saturation);

    public Pixel() : this(Color.Black) { }

    public Pixel(Color color)
    {
        UpdateColor(color);
    }

    public void UpdateColor(Color color)
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
