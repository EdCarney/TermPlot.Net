using System.Collections;
using System.Drawing;

namespace TermSixels.Models;

public class Sixel
{
    private const int _sixelSize = 6;
    private const int _sixelCharOffset = 63;

    public Color Color { get; }

    public char SixelChar { get; private set; }

    public Sixel() : this(new(_sixelSize)) { }

    public Sixel(BitArray bitMask) : this(bitMask, Color.Black) { }

    public Sixel(BitArray bitMask, Color color)
    {
        Color = color;

        if (bitMask.Length != _sixelSize)
            throw new ArgumentException("Sixel bit array must be exact six bits long.", nameof(bitMask));

        SixelChar = GetSixelCharFromBitMask(bitMask);
    }

    private char GetSixelCharFromBitMask(BitArray bitMask)
    {
        int bitMaskVal = _sixelCharOffset;
        for (int i = 0; i < bitMask.Length; i++)
        {
            bitMaskVal += (bitMask[i] ? 1 : 0) * (int)Math.Pow(2, i);
        }
        return (char)bitMaskVal;
    }
}
