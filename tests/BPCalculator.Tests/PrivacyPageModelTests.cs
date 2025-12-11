using BPCalculator.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;

namespace BPCalculator.Tests;

public class PrivacyPageModelTests
{
    private readonly Mock<ILogger<PrivacyModel>> _mockLogger;
    private readonly PrivacyModel _pageModel;

    public PrivacyPageModelTests()
    {
        _mockLogger = new Mock<ILogger<PrivacyModel>>();
        _pageModel = new PrivacyModel(_mockLogger.Object)
        {
            PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public void OnGet_Executes_Successfully()
    {
        var exception = Record.Exception(() => _pageModel.OnGet());

        Assert.Null(exception);
    }

    [Fact]
    public void OnGet_Logs_Information()
    {
        _pageModel.OnGet();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Privacy page accessed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_Initializes_Logger()
    {
        var logger = Mock.Of<ILogger<PrivacyModel>>();
        var model = new PrivacyModel(logger);

        Assert.NotNull(model);
    }
}
