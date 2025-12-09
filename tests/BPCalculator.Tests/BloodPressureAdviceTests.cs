using BPCalculator;

namespace BPCalculator.Tests;

public class BloodPressureAdviceTests
{
    [Theory]
    [InlineData(BPCategory.High, "High BP: seek medical review soon.")]
    [InlineData(BPCategory.PreHigh, "Pre-high: adjust diet/exercise and recheck.")]
    [InlineData(BPCategory.Ideal, "Ideal: keep current habits.")]
    [InlineData(BPCategory.Low, "Low: hydrate and rest if dizzy.")]
    public void Advice_Matches_Category(BPCategory category, string expected)
    {
        var bp = new BloodPressure { Systolic = 100, Diastolic = 70 };
        bp = category switch
        {
            BPCategory.High => new BloodPressure { Systolic = 150, Diastolic = 95 },
            BPCategory.PreHigh => new BloodPressure { Systolic = 125, Diastolic = 82 },
            BPCategory.Ideal => new BloodPressure { Systolic = 110, Diastolic = 70 },
            BPCategory.Low => new BloodPressure { Systolic = 85, Diastolic = 55 },
            _ => bp
        };

        Assert.Equal(expected, bp.Advice);
    }
}

