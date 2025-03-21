namespace TermSixels.Extensions;

public static class RangeExtensions
{
    public static void PrintRange(this Range range)
    {
        System.Console.WriteLine(range.ToRangeString());
    }

    public static string ToRangeString(this Range range)
    {
        return $"Start: {range.Start}, End: {range.End}";
    }

    public static Range FromStart(this Range range, int rangeMaxValue)
    {
        return new(range.Start.FromStart(rangeMaxValue), range.End.FromStart(rangeMaxValue));
    }
}
