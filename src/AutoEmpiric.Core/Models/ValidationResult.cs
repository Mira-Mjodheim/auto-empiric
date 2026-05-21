using System;
using System.Collections.Generic;

namespace AutoEmpiric.Core.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public bool IsSuccess
        {
            get => IsValid;
            set => IsValid = value;
        }
        public string Message { get; set; } = string.Empty;
        public string ErrorDetails { get; set; } = string.Empty;
        public Dictionary<string, double> Metrics { get; set; } = new Dictionary<string, double>();
        public ConfidenceScore Confidence { get; set; } = new ConfidenceScore();
        public DateTime ExecutionTime { get; set; } = DateTime.UtcNow;

        public static ValidationResult CreateSuccess(string message)
        {
            return new ValidationResult
            {
                IsValid = true,
                Message = message
            };
        }

        public static ValidationResult CreateFailure(string errorMessage, string details = "")
        {
            return new ValidationResult
            {
                IsValid = false,
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
