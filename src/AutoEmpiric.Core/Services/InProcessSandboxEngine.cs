using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoEmpiric.Core.Interfaces;

namespace AutoEmpiric.Core.Services
{
    public sealed class InProcessSandboxEngine : ISandboxEngine
    {
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<SandboxExecutionResponse> RunAsync(string generatedArtifact, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(generatedArtifact, cancellationToken);
        }

        public async Task<SandboxExecutionResponse> ExecuteAsync(string command, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(command))
            {
                return new SandboxExecutionResponse
                {
                    ExitCode = 1,
                    StandardOutput = string.Empty,
                    StandardError = "Empty command.",
                    ExecutionDuration = TimeSpan.Zero
                };
            }

            var start = Stopwatch.StartNew();
            var psi = new ProcessStartInfo("sh", $"-c \"{command.Replace("\"", "\\\"")}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };
            process.Start();

            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(60));

            try
            {
                await process.WaitForExitAsync(cts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                process.Kill(entireProcessTree: true);
                return new SandboxExecutionResponse
                {
                    ExitCode = -1,
                    StandardOutput = await stdoutTask.ConfigureAwait(false),
                    StandardError = "Execution timed out after 60 seconds.",
                    ExecutionDuration = start.Elapsed
                };
            }

            var stdout = await stdoutTask.ConfigureAwait(false);
            var stderr = await stderrTask.ConfigureAwait(false);

            return new SandboxExecutionResponse
            {
                ExitCode = process.ExitCode,
                StandardOutput = stdout,
                StandardError = stderr,
                ExecutionDuration = start.Elapsed
            };
        }

        public Task<SandboxExecutionResponse> RunScriptAsync(string scriptContent, string language, CancellationToken cancellationToken = default)
        {
            var interpreter = language?.ToLowerInvariant() switch
            {
                "python" or "py" => "python3",
                "javascript" or "js" or "node" => "node",
                "bash" or "sh" or "shell" => "bash",
                "csharp" or "cs" or "dotnet" => "dotnet script",
                _ => "sh"
            };

            var command = $"{interpreter} -e \"{scriptContent.Replace("\"", "\\\"")}\"";
            return ExecuteAsync(command, cancellationToken);
        }

        public Task TerminateAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
