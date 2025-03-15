using System.Drawing;

namespace TermSixels;

public struct Pixel
{
    public Color PixelColor { get; } = Color.Black;

    public int Red { get; }
    public int Green { get; }
    public int Blue { get; }
    public (int, int, int) RgbValues => (Red, Green, Blue);

    public int Hue { get; }
    public int Lightness { get; }
    public int Saturation { get; }
    public (int, int, int) HlsValues => (Hue, Lightness, Saturation);

    public Pixel() { }

    public Pixel(Color color)
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
