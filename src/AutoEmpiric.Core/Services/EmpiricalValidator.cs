using System.Threading;
using System.Threading.Tasks;
using AutoEmpiric.Core.Interfaces;
using AutoEmpiric.Core.Models;

namespace AutoEmpiric.Core.Services
{
    public sealed class EmpiricalValidator : IValidator
    {
        public string Name => "EmpiricalValidator";

        public Task<ValidationResult> AssessAsync(SandboxExecutionResponse executionResult, CancellationToken cancellationToken = default)
        {
            var isValid = executionResult.IsSuccess && !string.IsNullOrWhiteSpace(executionResult.StandardOutput);
            var result = new ValidationResult
            {
                IsValid = isValid,
                Message = isValid ? "Execution completed successfully." : "Execution failed validation.",
                ErrorDetails = executionResult.StandardError,
                Confidence = new ConfidenceScore
                {
                    Score = isValid ? 0.9 : 0.1,
                    Justification = isValid ? "Sandbox returned a successful execution." : "Sandbox returned an error or empty output."
                }
            };

            result.AddMetric("exit_code", executionResult.ExitCode);
            result.AddMetric("duration_ms", executionResult.ExecutionDuration.TotalMilliseconds);

            return Task.FromResult(result);
        }

        public bool SupportsContext(string contextType)
        {
            return string.IsNullOrWhiteSpace(contextType) || contextType == "sandbox";
        }
    }
}
