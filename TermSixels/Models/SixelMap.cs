using System.Text;
using TermSixels.Constants;

namespace TermSixels.Models;

/// <summary>
/// A map representation of a sixel image.
/// </summary>
public class SixelMap
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    // sixels must be printed row by row, with all colors on a row printed
    // in sequence before moving on to the next row; so sixel dimensions
    // are row -> color -> col
    private readonly Sixel[][][] _sixels;

    /// <summary>
    /// The unique color registers used in the map.
    /// </summary>
    public ColorRegister[] ColorRegisters { get; }

    public int Height { get; }

    public int Width { get; }

    /// <summary>
    /// Create a sixel map from a pixel map.
    /// </summary>
    public SixelMap(PixelMap pixelMap)
    {
        Height = pixelMap.Height / 6;
        Width = pixelMap.Width;

        ColorRegisters = pixelMap
            .AllUniqueColors
            .Select(color => new ColorRegister(color))
            .ToArray();

        _logger.Debug($"Map dimensions: height = {Height}, colors = {ColorRegisters.Count()}, width = {Width}");

        _sixels = new Sixel[Height][][];

        for (int i = 0; i < Height; i++)
        {
            _sixels[i] = new Sixel[ColorRegisters.Count()][];
            for (int j = 0; j < ColorRegisters.Count(); j++)
            {
                _sixels[i][j] = new Sixel[Width];
                for (int k = 0; k < Width; k++)
                {
                    int rangeStart = i * 6;
                    int rangeEnd = (i + 1) * 6;
                    var currentColor = ColorRegisters[j].Color;
                    var colorMap = pixelMap[rangeStart..rangeEnd, k]
                        .Select(color => color == currentColor)
                        .ToArray();

                    _sixels[i][j][k] = new Sixel(new(colorMap), currentColor);
                }
            }
        }

    }

    /// <summary>
    /// Generate the sixel sequence for this map. Includes the start and termination sequences,
    /// so writing this value will start printing the sixel image.
    /// </summary>
    public string ToSixelSequenceString()
    {
        var sb = new StringBuilder();

        sb.AppendLine();
        foreach (var colorReg in ColorRegisters)
            sb.Append(colorReg.ToSixelSetString());

        sb.AppendLine();
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < ColorRegisters.Count(); j++)
            {
                sb.Append(ColorRegisters[j].ToSixelUseString());

                // iterators for using repeat count
                char prevChar = _sixels[i][j].First().SixelChar;
                int consecutiveCharCount = 1;

                for (int k = 1; k < Width; k++)
                {
                    char currChar = _sixels[i][j][k].SixelChar;
                    if (prevChar != currChar)
                    {
                        sb.Append($"!{consecutiveCharCount}{prevChar}");
                        consecutiveCharCount = 0;
                    }
                    prevChar = currChar;
                    consecutiveCharCount++;
                }

                // print final char
                sb.Append($"!{consecutiveCharCount}{prevChar}");

                sb.Append(SixelControlStatements.CarriageReturn);
                sb.AppendLine();
            }
            sb.Append(SixelControlStatements.NewLine);
            sb.AppendLine();
        }

        _logger.Trace($"Final sixel image representation: \n{sb.ToString()}");

        sb.Insert(0, SixelControlStatements.StartSixelSeq);
        sb.AppendLine(SixelControlStatements.TerminateSixelSeq);

        return sb.ToString();
    }
}
