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
}
