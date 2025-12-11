using BPCalculator;
using BPCalculator.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BPCalculator.Tests;

public class StartupTests
{
    private readonly IConfiguration _configuration;

    public StartupTests()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"TestKey", "TestValue"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public void Constructor_Initializes_Configuration()
    {
        var startup = new Startup(_configuration);

        Assert.NotNull(startup.Configuration);
        Assert.Equal(_configuration, startup.Configuration);
    }

    [Fact]
    public void ConfigureServices_Registers_TelemetryService()
    {
        var services = new ServiceCollection();
        var startup = new Startup(_configuration);

        startup.ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();
        var telemetryService = serviceProvider.GetService<ITelemetryService>();

        Assert.NotNull(telemetryService);
        Assert.IsType<TelemetryService>(telemetryService);
    }

    [Fact]
    public void ConfigureServices_Registers_TelemetryService_As_Singleton()
    {
        var services = new ServiceCollection();
        var startup = new Startup(_configuration);

        startup.ConfigureServices(services);

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITelemetryService));

        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void ConfigureServices_Registers_RazorPages()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var startup = new Startup(_configuration);

        startup.ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();
        
        Assert.NotEmpty(services.Where(s => s.ServiceType.FullName.Contains("RazorPages")));
    }

    [Fact]
    public void Configure_Configures_Development_Pipeline()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddRazorPages();
        var serviceProvider = services.BuildServiceProvider();

        var startup = new Startup(_configuration);
        var app = new ApplicationBuilder(serviceProvider);
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.EnvironmentName).Returns("Development");

        startup.Configure(app, mockEnv.Object);

        Assert.NotNull(app);
    }

    [Fact]
    public void Configure_Configures_Production_Pipeline()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddRazorPages();
        services.AddRouting();
        var serviceProvider = services.BuildServiceProvider();

        var startup = new Startup(_configuration);
        var app = new ApplicationBuilder(serviceProvider);
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.EnvironmentName).Returns("Production");

        startup.Configure(app, mockEnv.Object);

        Assert.NotNull(app);
    }

    [Fact]
    public void Configuration_Property_Returns_Correct_Value()
    {
        var startup = new Startup(_configuration);

        var value = startup.Configuration["TestKey"];

        Assert.Equal("TestValue", value);
    }
}
