# Blood Pressure Calculator (BP Calculator)

A comprehensive ASP.NET Core Razor Pages web application for calculating and categorizing blood pressure readings with advanced testing and monitoring capabilities.

## Project Overview

The BP Calculator is a full-stack web application that:
- Validates user blood pressure inputs (Systolic/Diastolic)
- Categorizes readings into four categories: Low, Ideal, Pre-High, and High
- Provides personalized health recommendations based on BP category
- Tracks calculations using OpenTelemetry for observability
- Includes comprehensive test coverage (unit, E2E, and performance tests)

## Technology Stack

- **Framework**: ASP.NET Core (net8.0)
- **UI**: Razor Pages
- **Testing**: 
  - xUnit (unit tests)
  - Microsoft Playwright (end-to-end tests)
  - k6 (performance/load tests)
- **Monitoring**: OpenTelemetry with Activity Source
- **Build**: MSBuild / .NET CLI

## Project Structure

```
BPCalculator/
├── Program.cs              # Application entry point
├── Startup.cs              # Dependency injection & pipeline configuration
├── BloodPressure.cs        # Domain model with BP categorization logic
├── Pages/
│   ├── Index.cshtml.cs     # Calculator page model
│   ├── Privacy.cshtml.cs
│   └── Error.cshtml.cs
├── Services/
│   └── TelemetryService.cs # OpenTelemetry tracking
└── wwwroot/                # Static assets (CSS, JavaScript)

tests/
├── BPCalculator.Tests/     # Unit tests
├── BPCalculator.E2E/       # End-to-end browser tests
└── perf/                   # Performance/load tests
```

## Core Components

### BloodPressure Model
- **Properties**: Systolic (70-190), Diastolic (40-100) with validation
- **Categories**: 
  - High: Systolic ≥ 140 OR Diastolic ≥ 90
  - Pre-High: Systolic ≥ 120 OR Diastolic ≥ 80
  - Ideal: Systolic 90-119 AND Diastolic 60-79
  - Low: Below 90/60
- **Advice**: Contextual health recommendations per category

### TelemetryService
- Tracks all BP calculations using OpenTelemetry
- Creates activity spans with tags (systolic, diastolic, category)
- Provides structured logging for monitoring and analysis

### Index Page Model
- Initializes form with default values (100/60)
- Validates user input with custom business logic
- Ensures Systolic > Diastolic
- Tracks valid calculations

## Testing Strategy

### Unit Tests (xUnit)
- **BloodPressureCategoryTests.cs**: 40+ parameterized tests covering:
  - Boundary conditions for all BP categories
  - Input validation constraints
  - Edge cases at min/max values

### End-to-End Tests (Playwright)
- **CalculatorUiTests.cs**: Browser automation tests
- Validates complete user workflows
- Tests form submission and result display
- Configurable via `BP_E2E_URL` environment variable

### Performance Tests (k6)
- **bp-load-test.js**: Load testing and SLA validation
- Default configuration: 5 concurrent users, 30 seconds duration
- SLA thresholds:
  - 95th percentile response time < 500ms
  - Failure rate < 1%
- Environment variables:
  - `BP_BASE_URL`: Target URL (default: http://localhost:5000)
  - `BP_VUS`: Virtual users count
  - `BP_DURATION`: Test duration

## Deployment Method: Blue/Green

This project uses **Blue/Green Deployment** strategy to ensure zero-downtime updates and rapid rollback capability.

### Blue/Green Deployment Overview

The application maintains two identical production environments:

- **Blue Environment**: Current production environment serving live traffic
- **Green Environment**: Staging environment with new version deployed
- **Load Balancer**: Routes traffic between environments

### Deployment Process

1. **Prepare Green Environment**
   - Deploy latest application build to Green environment
   - Run full test suite (unit, E2E, performance)
   - Validate SLA thresholds (response time, error rates)

2. **Pre-Deployment Validation**
   - Execute E2E tests against Green environment
   - Run k6 load tests to ensure performance meets SLAs
   - Verify telemetry and logging systems functional

3. **Traffic Switch**
   - Once Green environment is validated, switch load balancer to route traffic to Green
   - Blue environment remains running with previous version
   - Minimal to zero downtime during switch

4. **Post-Deployment Monitoring**
   - Monitor application metrics and telemetry
   - Track error rates and response times
   - Verify health checks passing

5. **Rollback (if needed)**
   - If issues detected, quickly switch load balancer back to Blue
   - Maintain previous stable version while investigating issues
   - Keep Green environment for troubleshooting

### Blue/Green Advantages for This Project

- **Zero Downtime**: Users experience no service interruption during deployment
- **Easy Rollback**: Immediate switch back to previous version if issues arise
- **Full Testing**: New version tested in production-like environment before traffic switch
- **Telemetry Validation**: Monitor both environments to compare performance
- **Load Testing**: k6 tests validate performance under expected load before switch

## Running the Application

### Development
```bash
dotnet build
dotnet run --project BPCalculator/BPCalculator.csproj
```

### Testing

**Unit Tests**:
```bash
dotnet test tests/BPCalculator.Tests/
```

**End-to-End Tests** (requires running application):
```bash
BP_E2E_URL=http://localhost:5000 dotnet test tests/BPCalculator.E2E/
```

**Performance Tests** (requires k6):
```bash
BP_BASE_URL=http://localhost:5000 k6 run tests/perf/bp-load-test.js
```

## Deployment Workflow

1. Create feature branch and implement changes
2. Run all tests locally
3. Deploy to Green environment
4. Execute E2E and performance tests against Green
5. If successful, switch load balancer to route traffic to Green
6. Monitor application health and metrics
7. Keep Blue as rollback target for 24+ hours
8. Once stable, prepare Blue for next deployment

## Configuration

Environment variables:
- `BP_E2E_URL`: E2E test target (default: http://localhost:5000)
- `BP_BASE_URL`: Performance test target
- `BP_VUS`: Virtual users for load testing (default: 5)
- `BP_DURATION`: Load test duration (default: 30s)

## Monitoring & Observability

- **OpenTelemetry Activities**: Track BP calculations with detailed tags
- **Structured Logging**: ILogger integration for events
- **Performance Metrics**: k6 captures response times and success rates
- **Health Checks**: Application health validation during deployments
