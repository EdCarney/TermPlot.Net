namespace TermSixels.Extensions;

public static class IEnumerableExtensions
{
    public static void ValidateBound<T>(this IEnumerable<T> enumerable, Range range)
    {
        var (start, end) = enumerable.GetBoundsFromStart(range);
        int length = enumerable.Count();

        ValidateIndex(start, length);
        ValidateIndex(end, length);

        if (end < start)
            throw new ArgumentException("Range end is before start");
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

    private static void ValidateIndex(int index, int arrayLength)
    {
        if (index < 0 || index > arrayLength)
            throw new ArgumentOutOfRangeException(nameof(index));
    }
}
