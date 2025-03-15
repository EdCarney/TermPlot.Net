namespace TermSixels;

public static class RangeExtensions
{
    public static void ValidateBound(this Range range, int arrayLength)
    {
        int start = GetIndexFromStart(range.Start, arrayLength);
        int end = GetIndexFromStart(range.End, arrayLength);

        ValidateIndex(start, arrayLength);
        ValidateIndex(end, arrayLength);

        if (end < start)
            throw new ArgumentException("Range end is before start");
    }

    private static int GetIndexFromStart(Index index, int arrayLength)
        => index.IsFromEnd ? arrayLength - index.Value : index.Value;

    private static void ValidateIndex(int index, int arrayLength)
    {
        if (index < 0 || index >= arrayLength)
            throw new ArgumentOutOfRangeException(nameof(index));
    }
}
