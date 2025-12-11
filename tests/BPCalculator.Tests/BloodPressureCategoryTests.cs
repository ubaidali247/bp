using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BPCalculator;

namespace BPCalculator.Tests;

public class BloodPressureCategoryTests
{
    private static IReadOnlyCollection<ValidationResult> Validate(BloodPressure bp)
    {
        var context = new ValidationContext(bp);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(bp, context, results, validateAllProperties: true);
        return results;
    }

    [Theory]
    [InlineData(140, 60, BPCategory.High)]
    [InlineData(120, 90, BPCategory.High)]
    [InlineData(139, 89, BPCategory.PreHigh)]
    [InlineData(120, 79, BPCategory.PreHigh)]
    [InlineData(110, 85, BPCategory.PreHigh)]
    [InlineData(119, 79, BPCategory.Ideal)]
    [InlineData(90, 60, BPCategory.Ideal)]
    [InlineData(89, 59, BPCategory.Low)]
    public void Category_Computes_As_Expected(int sys, int dia, BPCategory expected)
    {
        var bp = new BloodPressure { Systolic = sys, Diastolic = dia };
        Assert.Equal(expected, bp.Category);
    }

    [Theory]
    [InlineData(BloodPressure.SystolicMax, BloodPressure.DiastolicMin, BPCategory.High)]
    [InlineData(BloodPressure.SystolicMin, BloodPressure.DiastolicMax, BPCategory.High)]
    [InlineData(130, 60, BPCategory.PreHigh)]
    [InlineData(100, 65, BPCategory.Ideal)]
    [InlineData(80, 50, BPCategory.Low)]
    public void Category_Respects_Branches_At_Limits(int systolic, int diastolic, BPCategory expected)
    {
        var bp = new BloodPressure { Systolic = systolic, Diastolic = diastolic };
        Assert.Equal(expected, bp.Category);
    }

    [Theory]
    [InlineData(BloodPressure.SystolicMin - 1, 60, "Invalid Systolic Value")]
    [InlineData(BloodPressure.SystolicMax + 1, 60, "Invalid Systolic Value")]
    [InlineData(100, BloodPressure.DiastolicMin - 1, "Invalid Diastolic Value")]
    [InlineData(100, BloodPressure.DiastolicMax + 1, "Invalid Diastolic Value")]
    public void Validation_Attributes_Reject_Out_Of_Range(int systolic, int diastolic, string expectedMessage)
    {
        var bp = new BloodPressure { Systolic = systolic, Diastolic = diastolic };

        var results = Validate(bp);

        Assert.Contains(results, r => r.ErrorMessage == expectedMessage);
    }

    [Fact]
    public void Validation_Attributes_Accept_Bounds()
    {
        var bp = new BloodPressure { Systolic = BloodPressure.SystolicMin, Diastolic = BloodPressure.DiastolicMin };

        var results = Validate(bp);

        Assert.Empty(results);
    }
}

