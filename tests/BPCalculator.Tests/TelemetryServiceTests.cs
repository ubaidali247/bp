using BPCalculator;
using BPCalculator.Services;
using Microsoft.Extensions.Logging;

namespace BPCalculator.Tests;

public class TelemetryServiceTests
{
    private static ILogger<TelemetryService> CreateLogger() =>
        LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Information))
                     .CreateLogger<TelemetryService>();

    [Fact]
    public void TrackCalculation_Ignores_Null()
    {
        var telemetry = new TelemetryService(CreateLogger());

        var exception = Record.Exception(() => telemetry.TrackCalculation(null));
        Assert.Null(exception);
    }

    [Fact]
    public void TrackCalculation_Works_For_Valid_Reading()
    {
        var telemetry = new TelemetryService(CreateLogger());
        var reading = new BloodPressure { Systolic = 120, Diastolic = 80 };

        var exception = Record.Exception(() => telemetry.TrackCalculation(reading));
        Assert.Null(exception);
    }
}

