using System.Collections;
using System.Drawing;

namespace TermSixels.Models;

public class Sixel
{
    private const int _sixelSize = 6;
    private const int _sixelCharOffset = 63;

    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public Color Color { get; }

    public char SixelChar { get; private set; }

    public Sixel() : this(new(_sixelSize)) { }

    public Sixel(BitArray bitMask) : this(bitMask, Color.Black) { }

    public Sixel(BitArray bitMask, Color color)
    {
        Color = color;

        if (bitMask.Length != _sixelSize)
            throw new ArgumentException($"Sixel bit array must be exactly six bits long, was instead {bitMask.Length} bits long.", nameof(bitMask));

        SixelChar = GetSixelCharFromBitMask(bitMask);
    }

    private char GetSixelCharFromBitMask(BitArray bitMask)
    {
        int bitMaskVal = 0;
        for (int i = 0; i < bitMask.Length; i++)
        {
            bitMaskVal += (bitMask[i] ? 1 : 0) * (int)Math.Pow(2, i);
        }

        int finalBitMaskVal = bitMaskVal + _sixelCharOffset;

        _logger.Trace($"Converted bit mask value {Convert.ToString(bitMaskVal, 2).PadLeft(6, '0')}: ({(char)bitMaskVal}) => ({(char)finalBitMaskVal})");

        return (char)finalBitMaskVal;
    }
}
