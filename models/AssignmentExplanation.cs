namespace AdjusterOptimizerAPI.Models
{
    public class AssignmentExplanation
    {
        public int ClaimId { get; set; }
        public int AdjusterId { get; set; }
        public double FinalScore { get; set; }

        public string SkillMatchReason { get; set; }
        public string JurisdictionReason { get; set; }
        public string WorkloadReason { get; set; }
        public string PerformanceReason { get; set; }

        public string Summary { get; set; }
    }
}
