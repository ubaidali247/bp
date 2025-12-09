using BPCalculator;

namespace BPCalculator.Tests;

public class BloodPressureCategoryTests
{
    [Theory]
    [InlineData(140, 60, BPCategory.High)]
    [InlineData(120, 90, BPCategory.High)]
    [InlineData(139, 89, BPCategory.PreHigh)]
    [InlineData(120, 79, BPCategory.PreHigh)]
    [InlineData(119, 79, BPCategory.Ideal)]
    [InlineData(90, 60, BPCategory.Ideal)]
    [InlineData(89, 59, BPCategory.Low)]
    public void Category_Computes_As_Expected(int sys, int dia, BPCategory expected)
    {
        var bp = new BloodPressure { Systolic = sys, Diastolic = dia };
        Assert.Equal(expected, bp.Category);
    }
}

