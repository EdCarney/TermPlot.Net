namespace TermSixels.Extensions;

public static class IEnumerableExtensions
{
    public static void ValidateBound<T>(this IEnumerable<T> enumerable, Range range)
    {
        var (start, end) = enumerable.GetBoundsFromStart(range);
        enumerable.ValidateBound(start);
        enumerable.ValidateBound(end);

        if (end < start)
            throw new ArgumentException("Range end is before start");
    }

    public static void ValidateBound<T>(this IEnumerable<T> enumerable, int index)
    {
        if (index < 0 || index > enumerable.Count())
            throw new ArgumentOutOfRangeException(nameof(index));
    }

    public static (int start, int end) GetBoundsFromStart<T>(this IEnumerable<T> enumerable, Range range)
    {
        int length = enumerable.Count();
        int start = GetIndexFromStart(range.Start, length);
        int end = GetIndexFromStart(range.End, length);
        return (start, end);
    }

    private static int GetIndexFromStart(Index index, int arrayLength)
        => index.IsFromEnd ? arrayLength - index.Value : index.Value;
}
