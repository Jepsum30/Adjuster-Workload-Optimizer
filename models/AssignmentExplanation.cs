namespace AdjusterOptimizerAPI.Models
{
    /// <summary>
    /// Provides a detailed breakdown of how the assignment engine
    /// calculated the final score for a claim-adjuster match.
    /// </summary>
    public class AssignmentExplanation
    {
        public int ClaimId { get; set; }
        public int AdjusterId { get; set; }

        /// <summary>
        /// Final weighted score used to determine the best adjuster.
        /// </summary>
        public double FinalScore { get; set; }

        /// <summary>
        /// Explanation of how well the adjuster's skills matched the claim.
        /// </summary>
        public string SkillMatchReason { get; set; } = string.Empty;

        /// <summary>
        /// Explanation of jurisdiction compatibility.
        /// </summary>
        public string JurisdictionReason { get; set; } = string.Empty;

        /// <summary>
        /// Explanation of workload impact on the score.
        /// </summary>
        public string WorkloadReason { get; set; } = string.Empty;

        /// <summary>
        /// Explanation of performance score influence.
        /// </summary>
        public string PerformanceReason { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable summary combining all factors.
        /// </summary>
        public string Summary { get; set; } = string.Empty;
    }
}
