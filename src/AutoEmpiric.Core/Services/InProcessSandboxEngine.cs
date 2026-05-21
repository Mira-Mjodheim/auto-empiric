using System;
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

        public Task<SandboxExecutionResponse> ExecuteAsync(string command, CancellationToken cancellationToken = default)
        {
            var response = new SandboxExecutionResponse
            {
                ExitCode = string.IsNullOrWhiteSpace(command) ? 1 : 0,
                StandardOutput = command ?? string.Empty,
                StandardError = string.IsNullOrWhiteSpace(command) ? "Empty command." : string.Empty,
                ExecutionDuration = TimeSpan.Zero
            };

            return Task.FromResult(response);
        }

        public Task<SandboxExecutionResponse> RunScriptAsync(string scriptContent, string language, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(scriptContent, cancellationToken);
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
