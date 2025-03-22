using System.Numerics;
using TermSixels.Models;

namespace TermSixels.Extensions;

public static class ValidatorExtensions
{

    /// <summary>
    /// Compares this value to zero and throws an exception if the comparison is not true.
    /// </summary>
    /// <param name="cmpType">The comparison type to use.</param>
    /// <param name="paramName">Optional param name to include in thrown argument.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the comparison is not true.
    /// </exception>
    public static void Validate<T>(this T val, ValidatorComparison cmpType, string? paramName = default) where T : INumber<T>
        => val.Validate(cmpType, T.Zero, paramName);

    /// <summary>
    /// Compares this value to another value and throws an exception if the comparison is not true.
    /// </summary>
    /// <param name="cmpType">The comparison type to use.</param>
    /// <param name="cmp">Value to compare against.</param>
    /// <param name="paramName">Optional param name to include in thrown argument.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the comparison is not true.
    /// </exception>
    public static void Validate<T>(this T val, ValidatorComparison cmpType, T cmp, string? paramName = default) where T : INumber<T>
    {
        (bool condition, string expectation) = cmpType switch
        {
            ValidatorComparison.EQ => (val == cmp, "equal to"),
            ValidatorComparison.GT => (val > cmp, "greater than"),
            ValidatorComparison.GT_EQ => (val >= cmp, "greater than or equal to"),
            ValidatorComparison.LT => (val < cmp, "less than"),
            ValidatorComparison.LT_EQ => (val <= cmp, "less than or equal to"),
            _ => throw new ArgumentException($"Unknown comparison type {cmpType}", nameof(cmpType))
        };

        if (!condition)
            throw paramName is null
                ? new ArgumentException($"Expected {val} to be {expectation} {cmp}")
                : new ArgumentException($"Expected {val} to be {expectation} {cmp}", paramName);
    }
}
