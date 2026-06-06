using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdjusterOptimizerAPI.Models
{
    /// <summary>
    /// Represents an insurance adjuster, including skill level,
    /// specialty, jurisdiction, workload, and performance metrics.
    /// </summary>
    public class Adjuster
    {
        [Column("ADJUSTER_ID")]
        public int AdjusterId { get; set; }

        [Column("FIRST_NAME")]
        public string First_Name { get; set; } = string.Empty;

        [Column("LAST_NAME")]
        public string Last_Name { get; set; } = string.Empty;

        [Column("SKILL_LEVEL")]
        public int SkillLevel { get; set; }

        [Column("SPECIALTY")]
        public string Specialty { get; set; } = string.Empty;

        // Only add this if PRIMARY_SKILL exists in your MySQL table
        [Column("PRIMARY_SKILL")]
        public string? PrimarySkill { get; set; }

        [Column("JURISDICTION")]
        public string Jurisdiction { get; set; } = string.Empty;

        [Column("WORKLOAD")]
        public int Workload { get; set; }

        [Column("PERFORMANCE_SCORE")]
        public decimal PerformanceScore { get; set; }

        [Column("TENURE_YEARS")]
        public int TenureYears { get; set; }

        public List<Assignment> Assignments { get; set; } = new();
        public List<PerformanceHistory> PerformanceHistory { get; set; } = new();
    }
}
