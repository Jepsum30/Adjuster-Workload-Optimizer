using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdjusterOptimizerAPI.Models
{
    [Table("claims")]
    public class Claim
    {
        [Column("CLAIM_ID")]
        public int ClaimId { get; set; }

        [Column("CLAIM_TYPE")]
        public string ClaimType { get; set; } = string.Empty;

        [Column("SEVERITY_SCORE")]
        public double SeverityScore { get; set; }

        [Column("COMPLEXITY_SCORE")]
        public double ComplexityScore { get; set; }

        [Column("JURISDICTION")]
        public string Jurisdiction { get; set; } = string.Empty;

        [Column("REQUIRED_SKILL")]
        public string RequiredSkill { get; set; } = string.Empty;

        [Column("ESTIMATED_HOURS")]
        public int EstimatedHours { get; set; }

        [Column("STATUS")]
        public string Status { get; set; } = string.Empty;

        // This column DOES exist in your DB
        [Column("ASSIGNED_ADJUSTER_ID")]
        public int? AssignedAdjusterId { get; set; }

        // Navigation properties
        public Adjuster? AssignedAdjuster { get; set; }
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
