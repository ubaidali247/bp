using BPCalculator.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;

namespace BPCalculator.Tests;

public class ErrorPageModelTests
{
    private readonly Mock<ILogger<ErrorModel>> _mockLogger;
    private readonly ErrorModel _pageModel;

    public ErrorPageModelTests()
    {
        _mockLogger = new Mock<ILogger<ErrorModel>>();
        _pageModel = new ErrorModel(_mockLogger.Object)
        {
            PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public void OnGet_Sets_RequestId_From_Activity()
    {
        var activity = new Activity("test");
        activity.Start();

        _pageModel.OnGet();

        Assert.NotNull(_pageModel.RequestId);
        Assert.Equal(Activity.Current.Id, _pageModel.RequestId);
        
        activity.Stop();
    }

    [Fact]
    public void OnGet_Sets_RequestId_From_HttpContext_When_No_Activity()
    {
        Activity.Current?.Stop();

        _pageModel.OnGet();

        Assert.NotNull(_pageModel.RequestId);
        Assert.Equal(_pageModel.HttpContext.TraceIdentifier, _pageModel.RequestId);
    }

    [Fact]
    public void ShowRequestId_Returns_False_When_RequestId_Is_Null()
    {
        _pageModel.RequestId = null;

        Assert.False(_pageModel.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_Returns_False_When_RequestId_Is_Empty()
    {
        _pageModel.RequestId = string.Empty;

        Assert.False(_pageModel.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_Returns_True_When_RequestId_Has_Value()
    {
        _pageModel.RequestId = "test-request-id";

        Assert.True(_pageModel.ShowRequestId);
    }

    [Fact]
    public void OnGet_Logs_Error()
    {
        _pageModel.OnGet();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error page accessed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
