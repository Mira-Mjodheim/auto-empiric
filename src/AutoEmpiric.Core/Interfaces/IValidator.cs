using System.Threading;
using System.Threading.Tasks;
using AutoEmpiric.Core.Models;

namespace AutoEmpiric.Core.Interfaces
{
    public interface IValidator
    {
        string Name { get; }

        Task<ValidationResult> AssessAsync(SandboxExecutionResponse executionResult, CancellationToken cancellationToken = default);

        bool SupportsContext(string contextType);
    }
}
