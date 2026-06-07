using System.ComponentModel.DataAnnotations.Schema;

namespace AdjusterOptimizerAPI.Models
{
    [Table("performance_history")]
    public class PerformanceHistory
    {
        [Column("RECORD_ID")]
        public int RecordId { get; set; }

        [Column("ADJUSTER_ID")]
        public int AdjusterId { get; set; }

        [Column("CLAIM_ID")]
        public int ClaimId { get; set; }

        [Column("CYCLE_TIME_DAYS")]
        public int CycleTimeDays { get; set; }

        [Column("INDEMNITY_PAID")]
        public int IndemnityPaid { get; set; }

        [Column("REOPEN_FLAG")]
        public int ReopenFlag { get; set; }

        [Column("LITIGATION_FLAG")]
        public int LitigationFlag { get; set; }

        [Column("CUSTOMER_SATISFACTION")]
        public double CustomerSatisfaction { get; set; }

        public Adjuster? Adjuster { get; set; }
        public Claim? Claim { get; set; }
    }
}
