using System.Diagnostics;

namespace BPCalculator.E2E;

/// <summary>

/// </summary>
public class WebServerFixture : IDisposable
{
    private readonly Process? _process;
    private const int ServerStartupDelayMs = 5000;
    private const string DefaultUrl = "http://localhost:5000";

    public WebServerFixture()
    {
=        var externalUrl = Environment.GetEnvironmentVariable("BP_E2E_URL");
        if (!string.IsNullOrEmpty(externalUrl))
        {
            Console.WriteLine($"Using external URL: {externalUrl}. Skipping local server startup.");
            return;
        }

        try
        {
=            var solutionDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
            var projectPath = Path.Combine(solutionDir, "BPCalculator/BPCalculator.csproj");

            if (!File.Exists(projectPath))
            {
                throw new FileNotFoundException($"Could not find BPCalculator.csproj at {projectPath}");
            }

            Console.WriteLine($"Starting ASP.NET Core app from: {projectPath}");

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projectPath}\" --urls {DefaultUrl} --no-build",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = solutionDir
            };

            _process = Process.Start(startInfo);

            if (_process == null)
            {
                throw new InvalidOperationException("Failed to start the web server process.");
            }

=            _process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine($"[Server] {e.Data}");
                }
            };
            _process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.Error.WriteLine($"[Server Error] {e.Data}");
                }
            };

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            Console.WriteLine($"Waiting {ServerStartupDelayMs}ms for server to start...");
            Thread.Sleep(ServerStartupDelayMs);

            if (_process.HasExited)
            {
                throw new InvalidOperationException($"Server process exited prematurely with code {_process.ExitCode}");
            }

            Console.WriteLine($"Server started successfully at {DefaultUrl}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to start web server: {ex.Message}");
            Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        if (_process != null && !_process.HasExited)
        {
            Console.WriteLine("Stopping web server...");
            try
            {
                _process.Kill(entireProcessTree: true);
                _process.WaitForExit(5000);
                Console.WriteLine("Web server stopped.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error stopping web server: {ex.Message}");
            }
        }

        _process?.Dispose();
    }
}
