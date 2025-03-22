using System.Collections;
using System.Drawing;
using TermSixels.Extensions;

namespace TermSixels.Models;

/// <summary>
/// Representation of an individual sixel (six vertical pixels).
/// </summary>
public class Sixel
{
    private const int _sixelSize = 6;
    private const int _sixelCharOffset = 63;

    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public Color Color { get; }

    public char SixelChar { get; private set; }

    /// <summary>
    /// Create a new black sixel.
    /// </summary>
    public Sixel() : this(new(_sixelSize)) { }

    /// <summary>
    /// Create a new black sixel from a bit mask.
    /// </summary>
    /// <param name="bitMask">Bit representation of six vertical pixels with the first value being the topmost vertical pixel.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if bit mask is not exactly six bits long.
    /// </exception>
    public Sixel(BitArray bitMask) : this(bitMask, Color.Black) { }

    /// <summary>
    /// Create a new colored sixel from a bit mask.
    /// </summary>
    /// <param name="bitMask">Bit representation of six vertical pixels with the first value being the topmost vertical pixel.</param>
    /// <param name="color">Color of the sixel.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if bit mask is not exactly six bits long.
    /// </exception>
    public Sixel(BitArray bitMask, Color color)
    {
        bitMask.Count.Validate(ValidatorComparison.EQ, _sixelSize, nameof(bitMask.Count));

        Color = color;
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
