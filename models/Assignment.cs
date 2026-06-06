using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdjusterOptimizerAPI.Models
{
    /// <summary>
    /// Represents a claim assignment made by the system or a supervisor.
    /// Includes metadata such as score, explanation, and assignment date.
    /// </summary>
    public class Assignment
    {
        [Column("ASSIGNMENT_ID")]
        public int AssignmentId { get; set; }

        [Column("CLAIM_ID")]
        public int ClaimId { get; set; }

        [Column("ADJUSTER_ID")]
        public int AdjusterId { get; set; }

        [Column("ASSIGNMENT_DATE")]
        public DateTime AssignmentDate { get; set; }

        [Column("SCORE")]
        public decimal Score { get; set; }

        [Column("ASSIGNED_BY")]
        public string AssignedBy { get; set; } = string.Empty;

        [Column("EXPLANATION")]
        public string Explanation { get; set; } = string.Empty;

        // Navigation properties
        public Adjuster Adjuster { get; set; } = null!;
        public Claim Claim { get; set; } = null!;
    }
}
