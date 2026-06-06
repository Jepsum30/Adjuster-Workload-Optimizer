using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdjusterOptimizerAPI.Models
{
    /// <summary>
    /// Represents an insurance claim, including type, severity,
    /// complexity, jurisdiction, required skills, and status.
    /// </summary>
    public class Claim
    {
        [Column("CLAIM_ID")]
        public int ClaimId { get; set; }

        [Column("CLAIM_TYPE")]
        public required string ClaimType { get; set; }

        [Column("SEVERITY_SCORE")]
        public decimal SeverityScore { get; set; }

        [Column("COMPLEXITY_SCORE")]
        public decimal ComplexityScore { get; set; }

        public required string Jurisdiction { get; set; }

        [Column("REQUIRED_SKILL")]
        public required string RequiredSkill { get; set; }

        [Column("ESTIMATED_HOURS")]
        public int EstimatedHours { get; set; }

        public required string Status { get; set; }

        // ⭐ REQUIRED FOR ASSIGNMENT ENGINE + CONTROLLERS
        public int? AssignedAdjusterId { get; set; }
        public Adjuster? AssignedAdjuster { get; set; }

        public List<Assignment> Assignments { get; set; } = new();
        public List<PerformanceHistory> PerformanceHistory { get; set; } = new();
    }
}
