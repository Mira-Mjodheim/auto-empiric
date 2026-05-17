using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoEmpiric.Core.Interfaces;
using AutoEmpiric.Core.Models;

namespace AutoEmpiric.Core.Tests
{
    public class OrchestratorTests
    {
        [Fact]
        public async Task ExecuteMissionAsync_ShouldReturnSuccess_WhenValidationPasses()
        {
            var mockAgent = new Mock<IAgent>();
            var mockSandbox = new Mock<ISandboxEngine>();
            var mockValidator = new Mock<IValidator>();

            string missionObjective = "Perform data analysis";
            string agentOutput = "import pandas";
            string executionResult = "Success";

            mockAgent.Setup(a => a.ExecuteAsync(missionObjective))
                .ReturnsAsync(agentOutput);

            mockSandbox.Setup(s => s.ExecuteCodeAsync(agentOutput))
                .ReturnsAsync(executionResult);

            var validationResult = new ValidationResult
            {
                IsValid = true,
                Confidence = new ConfidenceScore { Score = 0.98 }
            };

            mockValidator.Setup(v => v.ValidateAsync(executionResult))
                .ReturnsAsync(validationResult);

            var orchestrator = new Orchestrator(mockAgent.Object, mockSandbox.Object, mockValidator.Object);

            var result = await orchestrator.ExecuteMissionAsync(missionObjective);

            Assert.True(result.IsValid);
            Assert.NotNull(result.Confidence);
            Assert.Equal(0.98, result.Confidence.Score);
            mockAgent.Verify(a => a.ExecuteAsync(missionObjective), Times.Once);
            mockSandbox.Verify(s => s.ExecuteCodeAsync(agentOutput), Times.Once);
            mockValidator.Verify(v => v.ValidateAsync(executionResult), Times.Once);
        }

        [Fact]
        public async Task ExecuteMissionAsync_ShouldThrowException_WhenAgentFails()
        {
            var mockAgent = new Mock<IAgent>();
            var mockSandbox = new Mock<ISandboxEngine>();
            var mockValidator = new Mock<IValidator>();

            string missionObjective = "Impossible task";

            mockAgent.Setup(a => a.ExecuteAsync(missionObjective))
                .ThrowsAsync(new InvalidOperationException("Agent generation failed"));

            var orchestrator = new Orchestrator(mockAgent.Object, mockSandbox.Object, mockValidator.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => orchestrator.ExecuteMissionAsync(missionObjective));
            
            mockSandbox.Verify(s => s.ExecuteCodeAsync(It.IsAny<string>()), Times.Never);
            mockValidator.Verify(v => v.ValidateAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteMissionAsync_ShouldReturnFailure_WhenValidationFails()
        {
            var mockAgent = new Mock<IAgent>();
            var mockSandbox = new Mock<ISandboxEngine>();
            var mockValidator = new Mock<IValidator>();

            string missionObjective = "Failing task";
            string agentOutput = "bad syntax";
            string executionResult = "SyntaxError";

            mockAgent.Setup(a => a.ExecuteAsync(missionObjective))
                .ReturnsAsync(agentOutput);

            mockSandbox.Setup(s => s.ExecuteCodeAsync(agentOutput))
                .ReturnsAsync(executionResult);

            var validationResult = new ValidationResult
            {
                IsValid = false,
                Confidence = new ConfidenceScore { Score = 0.15 }
            };

            mockValidator.Setup(v => v.ValidateAsync(executionResult))
                .ReturnsAsync(validationResult);

            var orchestrator = new Orchestrator(mockAgent.Object, mockSandbox.Object, mockValidator.Object);

            var result = await orchestrator.ExecuteMissionAsync(missionObjective);

            Assert.False(result.IsValid);
            Assert.NotNull(result.Confidence);
            Assert.Equal(0.15, result.Confidence.Score);
            mockAgent.Verify(a => a.ExecuteAsync(missionObjective), Times.Once);
            mockSandbox.Verify(s => s.ExecuteCodeAsync(agentOutput), Times.Once);
            mockValidator.Verify(v => v.ValidateAsync(executionResult), Times.Once);
        }
    }
}
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.