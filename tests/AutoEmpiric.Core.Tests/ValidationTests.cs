using System.Threading;
using System.Threading.Tasks;
using AutoEmpiric.Core.Interfaces;
using AutoEmpiric.Core.Models;
using Xunit;

namespace AutoEmpiric.Core.Tests
{
    public class ValidationTests
    {
        private class EmpiricalValidator : IValidator
        {
            public string Name => "TestEmpiricalValidator";

            public Task<ValidationResult> AssessAsync(SandboxExecutionResponse executionResult, CancellationToken cancellationToken = default)
            {
                var verified = executionResult.StandardOutput.Contains("EMPIRICAL_PROOF_VERIFIED");
                return Task.FromResult(new ValidationResult
                {
                    IsValid = executionResult.IsSuccess && verified,
                    Message = verified ? "Proof verified." : "Proof marker missing."
                });
            }

            public bool SupportsContext(string contextType) => contextType == "sandbox";
        }

        [Fact]
        public async Task AssessAsync_WhenSandboxOutputContainsEmpiricalProof_ShouldReturnValid()
        {
            var validator = new EmpiricalValidator();
            var response = new SandboxExecutionResponse
            {
                ExitCode = 0,
                StandardOutput = "Execution started\nEMPIRICAL_PROOF_VERIFIED\nExecution completed"
            };

            var validationResult = await validator.AssessAsync(response);

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public async Task AssessAsync_WhenSandboxOutputLacksEmpiricalProof_ShouldReturnInvalid()
        {
            var validator = new EmpiricalValidator();
            var response = new SandboxExecutionResponse
            {
                ExitCode = 0,
                StandardOutput = "Execution started\nExecution completed"
            };

            var validationResult = await validator.AssessAsync(response);

            Assert.False(validationResult.IsValid);
        }

        [Fact]
        public async Task AssessAsync_WhenSandboxOutputContainsError_ShouldReturnInvalid()
        {
            var validator = new EmpiricalValidator();
            var response = new SandboxExecutionResponse
            {
                ExitCode = 1,
                StandardError = "ERROR: Compilation failed"
            };

            var validationResult = await validator.AssessAsync(response);

            Assert.False(validationResult.IsValid);
        }
    }
}
