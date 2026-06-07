using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdjusterOptimizerAPI.Models
{
    [Table("assignments")]
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

        public Claim? Claim { get; set; }
        public Adjuster? Adjuster { get; set; }
    }
}
