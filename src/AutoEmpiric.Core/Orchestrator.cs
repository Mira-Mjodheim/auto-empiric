using System;
using System.Threading;
using System.Threading.Tasks;
using AutoEmpiric.Core.Interfaces;
using AutoEmpiric.Core.Models;

namespace AutoEmpiric.Core
{
    public class Orchestrator
    {
        private readonly IAgent _agent;
        private readonly ISandboxEngine _sandboxEngine;
        private readonly IValidator _validator;

        public Orchestrator(IAgent agent, ISandboxEngine sandboxEngine, IValidator validator)
        {
            _agent = agent ?? throw new ArgumentNullException(nameof(agent));
            _sandboxEngine = sandboxEngine ?? throw new ArgumentNullException(nameof(sandboxEngine));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<ValidationResult> ExecuteEmpiricalCycleAsync(string taskDefinition, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(taskDefinition))
            {
                throw new ArgumentException("Task definition is required.", nameof(taskDefinition));
            }

            var generatedArtifact = await _agent.GenerateAsync(taskDefinition, cancellationToken).ConfigureAwait(false);
            var executionMetrics = await _sandboxEngine.RunAsync(generatedArtifact, cancellationToken).ConfigureAwait(false);
            var result = await _validator.AssessAsync(executionMetrics, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.