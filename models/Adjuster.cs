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
        /// <summary>
        /// Primary key for the Adjusters table.
        /// </summary>
        [Column("ADJUSTER_ID")]
        public int AdjusterId { get; set; }

        /// <summary>
        /// Adjuster's first name.
        /// </summary>
        public required string First_Name { get; set; }

        /// <summary>
        /// Adjuster's last name.
        /// </summary>
        public required string Last_Name { get; set; }

        /// <summary>
        /// Skill level rating (1–10).
        /// </summary>
        [Column("SKILL_LEVEL")]
        public int SkillLevel { get; set; }

        /// <summary>
        /// Primary specialty (Auto, Property, Liability, Commercial, etc.).
        /// </summary>
        public required string Specialty { get; set; }

        /// <summary>
        /// Primary skill used by the assignment engine.
        /// This is REQUIRED because AssignmentEngine references it.
        /// </summary>
        public string? PrimarySkill { get; set; }

        /// <summary>
        /// Jurisdiction(s) the adjuster is licensed to handle.
        /// </summary>
        public required string Jurisdiction { get; set; }

        /// <summary>
        /// Number of active claims currently assigned.
        /// </summary>
        public int Workload { get; set; }

        /// <summary>
        /// Performance score (0.0–100.0).
        /// </summary>
        [Column("PERFORMANCE_SCORE")]
        public decimal PerformanceScore { get; set; }

        /// <summary>
        /// Number of years the adjuster has been with the company.
        /// </summary>
        [Column("TENURE_YEARS")]
        public int TenureYears { get; set; }

        /// <summary>
        /// Navigation property for all assignments linked to this adjuster.
        /// </summary>
        public List<Assignment> Assignments { get; set; } = new();

        /// <summary>
        /// Navigation property for performance history records.
        /// </summary>
        public List<PerformanceHistory> PerformanceHistory { get; set; } = new();
    }
}
