using System.Collections;
using System.Drawing;

namespace TermSixels.Models;

public class Sixel : Pixel
{
    private const int _sixelCharOffset = 63;

    public char SixelChar { get; private set; }

    public Sixel(BitArray bitMask) : this(bitMask, Color.Black) { }

    public Sixel(BitArray bitMask, Color color) : base(color)
    {
        if (bitMask.Length != 6)
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
