using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BPCalculator.Services;

// page model

namespace BPCalculator.Pages
{
    public class BloodPressureModel : PageModel
    {
        private readonly ITelemetryService _telemetry;

        public BloodPressureModel(ITelemetryService telemetry)
        {
            _telemetry = telemetry;
        }

        [BindProperty]                              // bound on POST
        public BloodPressure BP { get; set; }

        // setup initial data
        public void OnGet()
        {
            BP = new BloodPressure() { Systolic = 100, Diastolic = 60 };
        }

        // POST, validate
        public IActionResult OnPost()
        {
            // extra validation
            if (!(BP.Systolic > BP.Diastolic))
            {
                ModelState.AddModelError("", "Systolic must be greater than Diastolic");
            }

            if (ModelState.IsValid)
            {
                _telemetry.TrackCalculation(BP);
            }
            return Page();
        }
    }
}