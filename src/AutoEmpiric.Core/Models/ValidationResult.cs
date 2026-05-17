using System;
using System.Collections.Generic;

namespace AutoEmpiric.Core.Models
{
    public class ValidationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorDetails { get; set; } = string.Empty;
        public Dictionary<string, double> Metrics { get; set; } = new Dictionary<string, double>();
        public DateTime ExecutionTime { get; set; } = DateTime.UtcNow;

        public static ValidationResult CreateSuccess(string message)
        {
            return new ValidationResult
            {
                IsSuccess = true,
                Message = message
            };
        }

        public static ValidationResult CreateFailure(string errorMessage, string details = "")
        {
            return new ValidationResult
            {
                IsSuccess = false,
                Message = errorMessage,
                ErrorDetails = details
            };
        }

        public void AddMetric(string name, double value)
        {
            if (Metrics.ContainsKey(name))
            {
                Metrics[name] = value;
            }
            else
            {
                Metrics.Add(name, value);
            }
        }
    }
}
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.