using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdjusterOptimizerAPI.Models
{
    [Table("adjusters")]
    public class Adjuster
    {
        [Column("ADJUSTER_ID")]
        public int AdjusterId { get; set; }

        [Column("FIRST_NAME")]
        public string FirstName { get; set; } = string.Empty;

        [Column("LAST_NAME")]
        public string LastName { get; set; } = string.Empty;

        // SKILL_LEVEL is INT in DB
        [Column("SKILL_LEVEL")]
        public int SkillLevel { get; set; }

        [Column("SPECIALTY")]
        public string Specialty { get; set; } = string.Empty;

        [Column("JURISDICTION")]
        public string Jurisdiction { get; set; } = string.Empty;

        [Column("WORKLOAD")]
        public int Workload { get; set; }

        // PERFORMANCE_SCORE is DOUBLE in DB → use double
        [Column("PERFORMANCE_SCORE")]
        public double PerformanceScore { get; set; }

        [Column("TENURE_YEARS")]
        public int TenureYears { get; set; }

        // PRIMARY_SKILL exists in DB but was missing in your model
        [Column("PRIMARY_SKILL")]
        public string? PrimarySkill { get; set; }

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<PerformanceHistory> PerformanceHistory { get; set; } = new List<PerformanceHistory>();
    }
}
