using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AutoEmpiric.Core.Interfaces
{
    public interface IValidator
    {
        string Name { get; }

        Task<ValidationResult> ValidateAsync(string executionContext, string generatedOutput, CancellationToken cancellationToken = default);

        bool SupportsContext(string contextType);
    }

    public class ValidationResult
    {
        public bool IsValid { get; }
        public string Feedback { get; }
        public IReadOnlyDictionary<string, string> Metrics { get; }

        public ValidationResult(bool isValid, string feedback, Dictionary<string, string> metrics = null)
        {
            IsValid = isValid;
            Feedback = feedback ?? string.Empty;
            Metrics = metrics ?? new Dictionary<string, string>();
        }

        public static ValidationResult Success(string feedback = "", Dictionary<string, string> metrics = null)
        {
            return new ValidationResult(true, feedback, metrics);
        }

        public static ValidationResult Failure(string feedback, Dictionary<string, string> metrics = null)
        {
            return new ValidationResult(false, feedback, metrics);
        }
    }
}
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.