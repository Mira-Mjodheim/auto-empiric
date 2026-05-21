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
        Task<string> GenerateAsync(string taskDefinition, CancellationToken cancellationToken = default);
        Task TerminateAsync(CancellationToken cancellationToken = default);
    }
}
