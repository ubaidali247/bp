using BPCalculator;
using BPCalculator.Pages;
using BPCalculator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;

namespace BPCalculator.Tests;

public class IndexPageModelTests
{
    private readonly Mock<ITelemetryService> _mockTelemetry;
    private readonly BloodPressureModel _pageModel;

    public IndexPageModelTests()
    {
        _mockTelemetry = new Mock<ITelemetryService>();
        _pageModel = new BloodPressureModel(_mockTelemetry.Object)
        {
            PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public void OnGet_Initializes_BloodPressure_With_Default_Values()
    {
        _pageModel.OnGet();

        Assert.NotNull(_pageModel.BP);
        Assert.Equal(100, _pageModel.BP.Systolic);
        Assert.Equal(60, _pageModel.BP.Diastolic);
    }

    [Fact]
    public void OnPost_Returns_Page_When_ModelState_Valid()
    {
        _pageModel.BP = new BloodPressure { Systolic = 120, Diastolic = 80 };

        var result = _pageModel.OnPost();

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public void OnPost_Calls_Telemetry_When_ModelState_Valid()
    {
        _pageModel.BP = new BloodPressure { Systolic = 120, Diastolic = 80 };

        _pageModel.OnPost();

        _mockTelemetry.Verify(t => t.TrackCalculation(It.IsAny<BloodPressure>()), Times.Once);
    }

    [Fact]
    public void OnPost_Does_Not_Call_Telemetry_When_ModelState_Invalid()
    {
        _pageModel.BP = new BloodPressure { Systolic = 120, Diastolic = 80 };
        _pageModel.ModelState.AddModelError("", "Test error");

        _pageModel.OnPost();

        _mockTelemetry.Verify(t => t.TrackCalculation(It.IsAny<BloodPressure>()), Times.Never);
    }

    [Fact]
    public void OnPost_Adds_Error_When_Systolic_Not_Greater_Than_Diastolic()
    {
        _pageModel.BP = new BloodPressure { Systolic = 80, Diastolic = 80 };

        _pageModel.OnPost();

        Assert.False(_pageModel.ModelState.IsValid);
        Assert.Contains(_pageModel.ModelState.Values, 
            v => v.Errors.Any(e => e.ErrorMessage == "Systolic must be greater than Diastolic"));
    }

    [Fact]
    public void OnPost_Adds_Error_When_Systolic_Less_Than_Diastolic()
    {
        _pageModel.BP = new BloodPressure { Systolic = 70, Diastolic = 80 };

        _pageModel.OnPost();

        Assert.False(_pageModel.ModelState.IsValid);
        Assert.Contains(_pageModel.ModelState.Values,
            v => v.Errors.Any(e => e.ErrorMessage == "Systolic must be greater than Diastolic"));
    }

    [Theory]
    [InlineData(120, 80)]
    [InlineData(140, 90)]
    [InlineData(100, 70)]
    [InlineData(90, 60)]
    public void OnPost_Accepts_Valid_Blood_Pressure_Values(int systolic, int diastolic)
    {
        _pageModel.BP = new BloodPressure { Systolic = systolic, Diastolic = diastolic };

        var result = _pageModel.OnPost();

        Assert.IsType<PageResult>(result);
        _mockTelemetry.Verify(t => t.TrackCalculation(It.IsAny<BloodPressure>()), Times.Once);
    }
}
