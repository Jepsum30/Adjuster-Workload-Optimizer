using System.ComponentModel.DataAnnotations.Schema;

namespace AdjusterOptimizerAPI.Models
{
    /// <summary>
    /// Historical performance data for an adjuster on a specific claim.
    /// Includes cycle time, indemnity paid, litigation/reopen flags,
    /// and customer satisfaction metrics.
    /// </summary>
    [Table("performance_history")]
    public class PerformanceHistory
    {
        [Column("RECORD_ID")]
        public int RecordId { get; set; }

        [Column("ADJUSTER_ID")]
        public int AdjusterId { get; set; }

        [Column("CLAIM_ID")]
        public int ClaimId { get; set; }

        /// <summary>
        /// Number of days it took to close the claim.
        /// </summary>
        [Column("CYCLE_TIME_DAYS")]
        public int CycleTimeDays { get; set; }

        /// <summary>
        /// Total indemnity paid on the claim.
        /// Stored as DECIMAL in MySQL.
        /// </summary>
        [Column("INDEMNITY_PAID")]
        public decimal IndemnityPaid { get; set; }

        /// <summary>
        /// Indicates whether the claim was reopened (0 = No, 1 = Yes).
        /// </summary>
        [Column("REOPEN_FLAG")]
        public int ReopenFlag { get; set; }

        /// <summary>
        /// Indicates whether the claim went into litigation (0 = No, 1 = Yes).
        /// </summary>
        [Column("LITIGATION_FLAG")]
        public int LitigationFlag { get; set; }

        /// <summary>
        /// Customer satisfaction score (0.0–10.0).
        /// Stored as DECIMAL in MySQL.
        /// </summary>
        [Column("CUSTOMER_SATISFACTION")]
        public decimal CustomerSatisfaction { get; set; }

        /// <summary>
        /// Navigation property for the adjuster.
        /// </summary>
        public Adjuster Adjuster { get; set; }

        /// <summary>
        /// Navigation property for the claim.
        /// </summary>
        public Claim Claim { get; set; }
    }
}
