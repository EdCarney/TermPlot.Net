using System.Text;
using TermSixels.Constants;

namespace TermSixels.Models;

public class SixelMap
{
    // sixels must be printed row by row, with all colors on a row printed
    // in sequence before moving on to the next row; so sixel dimensions
    // are row -> color -> col
    private readonly Sixel[][][] _sixels;

    public ColorRegister[] ColorRegisters { get; }

    public int Height { get; }

    public int Width { get; }

    public SixelMap(PixelMap pixelMap)
    {
        Height = pixelMap.Height / 6;
        Width = pixelMap.Width;

        ColorRegisters = pixelMap
            .AllUniqueColors
            .Select(color => new ColorRegister(color))
            .ToArray();

        _sixels = new Sixel[Height]
            .Select(row => new Sixel[ColorRegisters.Count()]
                .Select(col => new Sixel[Width])
                .ToArray())
            .ToArray();

        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < ColorRegisters.Count(); j++)
            {
                for (int k = 0; k < Width; k++)
                {
                    int rangeStart = i * 6;
                    int rangeEnd = (i + 1) * 6;
                    var colorMap = pixelMap[rangeStart..rangeEnd, k]
                        .Select(color => color == ColorRegisters[j].Color)
                        .ToArray();

                    _sixels[i][j][k] = new Sixel(new(colorMap));
                }
            }
        }

    }

    public string ToSixelSequenceString()
    {
        var sb = new StringBuilder(SixelControlStatements.StartSixelSeq);

        sb.AppendLine();
        foreach (var colorReg in ColorRegisters)
            sb.Append(colorReg.ToSixelSetString());

        sb.AppendLine();
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < ColorRegisters.Count(); j++)
            {
                sb.Append(ColorRegisters[j].ToSixelUseString());
                for (int k = 0; k < Width; k++)
                {
                    sb.Append(_sixels[i][j][k].SixelChar);
                }
                sb.Append(SixelControlStatements.CarriageReturn);
            }
            sb.Append(SixelControlStatements.NewLine);
            sb.AppendLine();
        }

        sb.AppendLine(SixelControlStatements.TerminateSixelSeq);

        return sb.ToString();
    }
}
