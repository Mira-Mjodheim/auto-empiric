using System;
using System.Threading;
using System.Threading.Tasks;
using AutoEmpiric.Core.Interfaces;

namespace AutoEmpiric.Core.Services
{
    public sealed class BasicAgent : IAgent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => "BasicAgent";
        public AgentState State { get; private set; } = AgentState.Created;

        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            State = AgentState.Ready;
            return Task.CompletedTask;
        }

        public Task<string> GenerateAsync(string taskDefinition, CancellationToken cancellationToken = default)
        {
            State = AgentState.Running;
            return Task.FromResult(taskDefinition);
        }

        public Task TerminateAsync(CancellationToken cancellationToken = default)
        {
            State = AgentState.Terminated;
            return Task.CompletedTask;
        }
    }
}
