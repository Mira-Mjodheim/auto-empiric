using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoEmpiric.Core.Interfaces
{
    public enum AgentState
    {
        Created,
        Initializing,
        Ready,
        Running,
        Paused,
        Terminating,
        Terminated,
        Error
    }

    public interface IAgent
    {
        Guid Id { get; }
        string Name { get; }
        AgentState State { get; }

        Task InitializeAsync(CancellationToken cancellationToken = default);
        Task ExecuteAsync(CancellationToken cancellationToken = default);
        Task TerminateAsync(CancellationToken cancellationToken = default);
    }
}
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.