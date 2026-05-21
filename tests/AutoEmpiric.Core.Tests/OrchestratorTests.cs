using System;
using System.Threading;
using System.Threading.Tasks;
using AutoEmpiric.Core.Interfaces;
using AutoEmpiric.Core.Models;
using Moq;
using Xunit;

namespace AutoEmpiric.Core.Tests
{
    public class OrchestratorTests
    {
        [Fact]
        public async Task ExecuteEmpiricalCycleAsync_ShouldReturnSuccess_WhenValidationPasses()
        {
            var mockAgent = new Mock<IAgent>();
            var mockSandbox = new Mock<ISandboxEngine>();
            var mockValidator = new Mock<IValidator>();

            const string missionObjective = "Perform data analysis";
            const string agentOutput = "import pandas";
            var executionResult = new SandboxExecutionResponse
            {
                ExitCode = 0,
                StandardOutput = "Success"
            };

            mockAgent.Setup(a => a.GenerateAsync(missionObjective, It.IsAny<CancellationToken>()))
                .ReturnsAsync(agentOutput);

            mockSandbox.Setup(s => s.RunAsync(agentOutput, It.IsAny<CancellationToken>()))
                .ReturnsAsync(executionResult);

            var validationResult = new ValidationResult
            {
                IsValid = true,
                Confidence = new ConfidenceScore { Score = 0.98 }
            };

            mockValidator.Setup(v => v.AssessAsync(executionResult, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var orchestrator = new Orchestrator(mockAgent.Object, mockSandbox.Object, mockValidator.Object);

            var result = await orchestrator.ExecuteEmpiricalCycleAsync(missionObjective);

            Assert.True(result.IsValid);
            Assert.NotNull(result.Confidence);
            Assert.Equal(0.98, result.Confidence.Score);
            mockAgent.Verify(a => a.GenerateAsync(missionObjective, It.IsAny<CancellationToken>()), Times.Once);
            mockSandbox.Verify(s => s.RunAsync(agentOutput, It.IsAny<CancellationToken>()), Times.Once);
            mockValidator.Verify(v => v.AssessAsync(executionResult, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteEmpiricalCycleAsync_ShouldThrowException_WhenAgentFails()
        {
            var mockAgent = new Mock<IAgent>();
            var mockSandbox = new Mock<ISandboxEngine>();
            var mockValidator = new Mock<IValidator>();

            const string missionObjective = "Impossible task";

            mockAgent.Setup(a => a.GenerateAsync(missionObjective, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Agent generation failed"));

            var orchestrator = new Orchestrator(mockAgent.Object, mockSandbox.Object, mockValidator.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => orchestrator.ExecuteEmpiricalCycleAsync(missionObjective));

            mockSandbox.Verify(s => s.RunAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            mockValidator.Verify(v => v.AssessAsync(It.IsAny<SandboxExecutionResponse>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteEmpiricalCycleAsync_ShouldReturnFailure_WhenValidationFails()
        {
            var mockAgent = new Mock<IAgent>();
            var mockSandbox = new Mock<ISandboxEngine>();
            var mockValidator = new Mock<IValidator>();

            const string missionObjective = "Failing task";
            const string agentOutput = "bad syntax";
            var executionResult = new SandboxExecutionResponse
            {
                ExitCode = 1,
                StandardError = "SyntaxError"
            };

            mockAgent.Setup(a => a.GenerateAsync(missionObjective, It.IsAny<CancellationToken>()))
                .ReturnsAsync(agentOutput);

            mockSandbox.Setup(s => s.RunAsync(agentOutput, It.IsAny<CancellationToken>()))
                .ReturnsAsync(executionResult);

            var validationResult = new ValidationResult
            {
                IsValid = false,
                Confidence = new ConfidenceScore { Score = 0.15 }
            };

            mockValidator.Setup(v => v.AssessAsync(executionResult, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var orchestrator = new Orchestrator(mockAgent.Object, mockSandbox.Object, mockValidator.Object);

            var result = await orchestrator.ExecuteEmpiricalCycleAsync(missionObjective);

            Assert.False(result.IsValid);
            Assert.NotNull(result.Confidence);
            Assert.Equal(0.15, result.Confidence.Score);
            mockAgent.Verify(a => a.GenerateAsync(missionObjective, It.IsAny<CancellationToken>()), Times.Once);
            mockSandbox.Verify(s => s.RunAsync(agentOutput, It.IsAny<CancellationToken>()), Times.Once);
            mockValidator.Verify(v => v.AssessAsync(executionResult, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
