using TermSixels.Extensions;
using TermSixels.Models;

namespace TermSixels.Tests.ExtensionsTests;

[TestClass]
public sealed class ValidatorExtensionsTests
{
    [TestMethod]
    [DataRow(10, ValidatorComparison.EQ, 10)]
    [DataRow(10, ValidatorComparison.GT, 5)]
    [DataRow(5, ValidatorComparison.LT, 10)]
    [DataRow(10, ValidatorComparison.GT_EQ, 5)]
    [DataRow(10, ValidatorComparison.GT_EQ, 10)]
    [DataRow(5, ValidatorComparison.LT_EQ, 10)]
    [DataRow(5, ValidatorComparison.LT_EQ, 5)]
    public void ValidateInt_NonZero_TrueCondition(int val, ValidatorComparison cmpType, int cmp)
    {
        val.Validate(cmpType, cmp);
    }

    [TestMethod]
    [DataRow(0, ValidatorComparison.EQ)]
    [DataRow(5, ValidatorComparison.GT)]
    [DataRow(-5, ValidatorComparison.LT)]
    [DataRow(5, ValidatorComparison.GT_EQ)]
    [DataRow(0, ValidatorComparison.GT_EQ)]
    [DataRow(-5, ValidatorComparison.LT_EQ)]
    [DataRow(0, ValidatorComparison.LT_EQ)]
    public void ValidateInt_Zero_TrueCondition(int val, ValidatorComparison cmpType)
    {
        val.Validate(cmpType);
    }

    [TestMethod]
    [DataRow(5, ValidatorComparison.EQ, 10)]
    [DataRow(5, ValidatorComparison.GT, 10)]
    [DataRow(10, ValidatorComparison.LT, 5)]
    [DataRow(5, ValidatorComparison.GT_EQ, 10)]
    [DataRow(10, ValidatorComparison.LT_EQ, 5)]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateInt_NonZero_FalseCondition(int val, ValidatorComparison cmpType, int cmp)
    {
        val.Validate(cmpType, cmp);
    }

    [TestMethod]
    [DataRow(5, ValidatorComparison.EQ)]
    [DataRow(-5, ValidatorComparison.GT)]
    [DataRow(5, ValidatorComparison.LT)]
    [DataRow(-5, ValidatorComparison.GT_EQ)]
    [DataRow(5, ValidatorComparison.LT_EQ)]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateInt_Zero_FalseCondition(int val, ValidatorComparison cmpType)
    {
        val.Validate(cmpType);
    }
}
