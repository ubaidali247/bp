using Microsoft.Playwright;
using Xunit;

namespace BPCalculator.E2E;

public class CalculatorUiTests : IClassFixture<WebServerFixture>, IAsyncLifetime
{
    private static readonly string BaseUrl = Environment.GetEnvironmentVariable("BP_E2E_URL") ?? "http://localhost:5000";
    private readonly WebServerFixture _serverFixture;

    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;

    public CalculatorUiTests(WebServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        try
        {
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        }
        catch (PlaywrightException ex) when (ex.Message.Contains("executable", StringComparison.OrdinalIgnoreCase))
        {
            throw new SkipException("Playwright browsers missing. Run `playwright install` then re-run tests.");
        }

        var context = await _browser.NewContextAsync(new BrowserNewContextOptions { BaseURL = BaseUrl });
        _page = await context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        if (_page != null)
        {
            await _page.Context.CloseAsync();
        }

        if (_browser != null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
    }

    [SkippableTheory]
    [InlineData("150", "95", "High Blood Pressure")]
    [InlineData("125", "82", "Pre-High Blood Pressure")]
    [InlineData("100", "70", "Ideal Blood Pressure")]
    [InlineData("80", "50", "Low Blood Pressure")]
    public async Task SubmitReading_Shows_Computed_Category(string systolic, string diastolic, string expectedCategory)
    {
        if (_page == null)
        {
            throw new InvalidOperationException("Page failed to initialize");
        }

        var response = await _page.GotoAsync("/");
        if (response is null || !response.Ok)
        {
            throw new SkipException($"App not reachable at {BaseUrl}. Start the site or set BP_E2E_URL.");
        }

        await _page.FillAsync("input[name=\"BP.Systolic\"]", systolic);
        await _page.FillAsync("input[name=\"BP.Diastolic\"]", diastolic);
        await _page.ClickAsync("input[type=\"submit\"]");

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        var categoryElement = await _page.WaitForSelectorAsync($"text={expectedCategory}", new PageWaitForSelectorOptions { Timeout = 5000 });
        Assert.NotNull(categoryElement);
    }
}
