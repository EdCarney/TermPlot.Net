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
                sb.AppendLine();
            }
            sb.Append(SixelControlStatements.NewLine);
            sb.AppendLine();
        }

        sb.AppendLine(SixelControlStatements.TerminateSixelSeq);

        return sb.ToString();
    }
}
