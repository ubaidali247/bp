using BPCalculator;
using BPCalculator.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        using var factory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
            });

        using var client = factory.CreateClient();
        Assert.NotNull(client);
    }

    [Fact]
    public void Configure_Configures_Production_Pipeline()
    {
        using var factory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Production");
            });

        using var client = factory.CreateClient();
        Assert.NotNull(client);
    }

    [Fact]
    public void Configuration_Property_Returns_Correct_Value()
    {
        var startup = new Startup(_configuration);

        var value = startup.Configuration["TestKey"];

        Assert.Equal("TestValue", value);
    }
}
