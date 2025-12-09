using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BPCalculator.Services
{
    public interface ITelemetryService
    {
        void TrackCalculation(BloodPressure reading);
    }

    public sealed class TelemetryService : ITelemetryService
    {
        private static readonly ActivitySource ActivitySource = new("BPCalculator.Telemetry");
        private readonly ILogger<TelemetryService> _logger;

        public TelemetryService(ILogger<TelemetryService> logger)
        {
            _logger = logger;
        }

        public void TrackCalculation(BloodPressure reading)
        {
            if (reading == null)
            {
                return;
            }

            var category = reading.Category;

            using var activity = ActivitySource.StartActivity("BloodPressureCalculation");
            activity?.SetTag("bp.systolic", reading.Systolic);
            activity?.SetTag("bp.diastolic", reading.Diastolic);
            activity?.SetTag("bp.category", category.ToString());

            _logger.LogInformation("Telemetry: {Systolic}/{Diastolic} => {Category}", reading.Systolic, reading.Diastolic, category);
        }
    }
}

