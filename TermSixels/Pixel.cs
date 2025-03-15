using System.Drawing;

namespace TermSixels;

public struct Pixel
{
    public Color PixelColor { get; private set; }

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
        UpdatePixelColor(color);
    }

    public void UpdatePixelColor(Color color)
    {
        PixelColor = color;

        Red = 100 * PixelColor.R / Byte.MaxValue;
        Green = 100 * PixelColor.G / Byte.MaxValue;
        Blue = 100 * PixelColor.B / Byte.MaxValue;

        Hue = (int)PixelColor.GetHue();
        Lightness = (int)(100 * PixelColor.GetBrightness());
        Saturation = (int)(100 * PixelColor.GetSaturation());
    }
}
