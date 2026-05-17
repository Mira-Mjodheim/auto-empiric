using System;
using System.Collections.Generic;

namespace AutoEmpiric.Core.Models
{
    public class ConfidenceScore
    {
        public double Level { get; set; }
        public double MarginOfError { get; set; }
        public string Justification { get; set; }
        public Dictionary<string, double> EvidenceWeights { get; set; }

        public ConfidenceScore()
        {
            EvidenceWeights = new Dictionary<string, double>();
            Justification = string.Empty;
        }

        public void AdjustConfidence(string evidenceId, double weight, double certainty)
        {
            if (string.IsNullOrWhiteSpace(evidenceId))
            {
                throw new ArgumentException("Evidence identifier cannot be null or whitespace.", nameof(evidenceId));
            }

            EvidenceWeights[evidenceId] = weight * certainty;
            Recalculate();
        }

        private void Recalculate()
        {
            if (EvidenceWeights.Count == 0)
            {
                Level = 0.0;
                MarginOfError = 1.0;
                return;
            }

            double totalWeight = 0;
            double weightedSum = 0;

            foreach (var kvp in EvidenceWeights)
            {
                totalWeight += 1.0; 
                weightedSum += kvp.Value;
            }

            Level = weightedSum / totalWeight;
            MarginOfError = 1.0 - Level;
        }
    }
}
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.