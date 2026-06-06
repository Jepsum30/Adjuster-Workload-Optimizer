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
    public string? ClaimType { get; set; }

    [Column("SEVERITY_SCORE")]
    public double? SeverityScore { get; set; }

    [Column("COMPLEXITY_SCORE")]
    public double? ComplexityScore { get; set; }

    [Column("JURISDICTION")]
    public string? Jurisdiction { get; set; }

    [Column("REQUIRED_SKILL")]
    public string? RequiredSkill { get; set; }

    [Column("ESTIMATED_HOURS")]
    public int? EstimatedHours { get; set; }

    [Column("STATUS")]
    public string? Status { get; set; }

    [Column("ASSIGNED_ADJUSTER_ID")]
    public int? AssignedAdjusterId { get; set; }

    public Adjuster? AssignedAdjuster { get; set; }

    // ⭐ REQUIRED for EF Core relationship mapping
    public List<PerformanceHistory> PerformanceHistory { get; set; } = new();
    public List<Assignment> Assignments { get; set; } = new();
}
}
