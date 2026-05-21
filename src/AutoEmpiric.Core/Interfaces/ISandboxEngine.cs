using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoEmpiric.Core.Interfaces
{
    public interface ISandboxEngine : IAsyncDisposable, IDisposable
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
        Task<SandboxExecutionResponse> RunAsync(string generatedArtifact, CancellationToken cancellationToken = default);
        Task<SandboxExecutionResponse> ExecuteAsync(string command, CancellationToken cancellationToken = default);
        Task<SandboxExecutionResponse> RunScriptAsync(string scriptContent, string language, CancellationToken cancellationToken = default);
        Task TerminateAsync(CancellationToken cancellationToken = default);
    }

    public class SandboxExecutionResponse
    {
        public int ExitCode { get; set; }
        public string StandardOutput { get; set; } = string.Empty;
        public string StandardError { get; set; } = string.Empty;
        public TimeSpan ExecutionDuration { get; set; }
        public bool IsSuccess => ExitCode == 0;
    }
}
