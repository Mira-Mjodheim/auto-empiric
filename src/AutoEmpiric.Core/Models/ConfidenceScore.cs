using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoEmpiric.Core.Models
{
    public class ConfidenceScore
    {
        private readonly Dictionary<string, (double Weight, double Certainty)> _evidence = new();

        public double Score { get; set; }

        [Obsolete("Use Score instead. Will be removed in a future version.")]
        public double Level
        {
            get => Score;
            set => Score = value;
        }

        public double MarginOfError { get; set; }
        public string Justification { get; set; } = string.Empty;

        public IReadOnlyDictionary<string, double> EvidenceWeights
            => _evidence.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Weight * kvp.Value.Certainty);

        public void AdjustConfidence(string evidenceId, double weight, double certainty)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(evidenceId);

            if (weight < 0) throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be non-negative.");
            if (certainty is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(certainty), "Certainty must be between 0 and 1.");

            _evidence[evidenceId] = (weight, certainty);
            Recalculate();
        }

        private void Recalculate()
        {
            if (_evidence.Count == 0)
            {
                Score = 0.0;
                MarginOfError = 1.0;
                return;
            }

            double totalWeight = 0;
            double weightedSum = 0;

            foreach (var (_, (w, c)) in _evidence)
            {
                totalWeight += w;
                weightedSum += w * c;
            }

            Score = totalWeight > 0 ? weightedSum / totalWeight : 0;
            MarginOfError = 1.0 - Score;
        }
    }
}
