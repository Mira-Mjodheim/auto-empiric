using System.Threading.Tasks;
using Xunit;
using AutoEmpiric.Core.Interfaces;
using AutoEmpiric.Core.Models;

namespace AutoEmpiric.Core.Tests
{
    public class ValidationTests
    {
        private class EmpiricalValidator : IValidator
        {
            private readonly ISandboxEngine _sandboxEngine;

            public EmpiricalValidator(ISandboxEngine sandboxEngine)
            {
                _sandboxEngine = sandboxEngine;
            }

            public async Task<ValidationResult> ValidateAsync(string code)
            {
                var executionResult = await _sandboxEngine.ExecuteAsync(code);
                
                var result = new ValidationResult();
                
                if (executionResult != null && executionResult.Contains("EMPIRICAL_PROOF_VERIFIED"))
                {
                    result.IsValid = true;
                }
                else
                {
                    result.IsValid = false;
                }

                return result;
            }
        }

        private class TestSandboxEngine : ISandboxEngine
        {
            private readonly string _simulatedOutput;

            public TestSandboxEngine(string simulatedOutput)
            {
                _simulatedOutput = simulatedOutput;
            }

            public Task<string> ExecuteAsync(string code)
            {
                return Task.FromResult(_simulatedOutput);
            }
        }

        [Fact]
        public async Task ValidateAsync_WhenSandboxOutputContainsEmpiricalProof_ShouldReturnValid()
        {
            var sandbox = new TestSandboxEngine("Execution started\nEMPIRICAL_PROOF_VERIFIED\nExecution completed");
            var validator = new EmpiricalValidator(sandbox);

            var validationResult = await validator.ValidateAsync("test_code_payload");

            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_WhenSandboxOutputLacksEmpiricalProof_ShouldReturnInvalid()
        {
            var sandbox = new TestSandboxEngine("Execution started\nExecution completed");
            var validator = new EmpiricalValidator(sandbox);

            var validationResult = await validator.ValidateAsync("test_code_payload");

            Assert.False(validationResult.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_WhenSandboxOutputContainsError_ShouldReturnInvalid()
        {
            var sandbox = new TestSandboxEngine("Execution started\nERROR: Compilation failed\nExecution completed");
            var validator = new EmpiricalValidator(sandbox);

            var validationResult = await validator.ValidateAsync("test_code_payload");

            Assert.False(validationResult.IsValid);
        }
    }
}
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.