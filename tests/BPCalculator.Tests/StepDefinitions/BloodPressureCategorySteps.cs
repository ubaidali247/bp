using BPCalculator;
using TechTalk.SpecFlow;

namespace BPCalculator.Tests.StepDefinitions;

[Binding]
public sealed class BloodPressureCategorySteps
{
    private BloodPressure? _reading;
    private BPCategory _result;

    [Given(@"a systolic value of (.*) and diastolic value of (.*)")]
    public void GivenAReading(int systolic, int diastolic)
    {
        _reading = new BloodPressure { Systolic = systolic, Diastolic = diastolic };
    }

    [When(@"I evaluate the blood pressure category")]
    public void WhenIEvaluateTheBloodPressureCategory()
    {
        _result = _reading?.Category ?? throw new InvalidOperationException("Reading was not initialized");
    }

    [Then(@"the result should be (.*)")]
    public void ThenTheResultShouldBe(BPCategory expected)
    {
        Assert.Equal(expected, _result);
    }
}
