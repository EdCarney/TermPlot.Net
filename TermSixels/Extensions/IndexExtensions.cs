namespace TermSixels.Extensions;

public static class IndexExtensions
{
    public static Index FromStart(this Index index, int maxValue)
    {
        int indexVal = index.IsFromEnd ? maxValue - index.Value : index.Value;
        return new(indexVal, fromEnd: false);
    }
}
