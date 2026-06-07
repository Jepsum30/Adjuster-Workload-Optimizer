using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AdjusterOptimizerAPI.Services
{
    public class AssignmentEngine
    {
        private readonly ApplicationDbContext _context;

        public AssignmentEngine(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(Adjuster bestAdjuster, AssignmentExplanation explanation)>
            RecommendAdjusterForClaimAsync(int claimId)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
                throw new InvalidOperationException("Claim not found.");

            var adjusters = await _context.Adjusters.ToListAsync();
            if (!adjusters.Any())
                throw new InvalidOperationException("No adjusters available.");

            Adjuster? best = null;
            double bestScore = double.MinValue;
            AssignmentExplanation? bestExplanation = null;

            foreach (var adj in adjusters)
            {
                var explanation = new AssignmentExplanation
                {
                    ClaimId = claim.ClaimId,
                    AdjusterId = adj.AdjusterId
                };

                double score = 0;

                // ⭐ SKILL MATCH (RequiredSkill vs PrimarySkill)
                if (!string.IsNullOrWhiteSpace(claim.RequiredSkill) &&
                    string.Equals(adj.PrimarySkill, claim.RequiredSkill, StringComparison.OrdinalIgnoreCase))
                {
                    score += 40;
                    explanation.SkillMatchReason =
                        $"Primary skill matches required skill ({adj.PrimarySkill}).";
                }
                else
                {
                    explanation.SkillMatchReason =
                        $"Primary skill ({adj.PrimarySkill}) does not match required skill ({claim.RequiredSkill}).";
                }

                // ⭐ SKILL LEVEL MATCH (int)
                if (adj.SkillLevel >= claim.SeverityScore)
                {
                    score += 20;
                }
                else
                {
                }

                // ⭐ JURISDICTION MATCH
                if (string.Equals(adj.Jurisdiction, claim.Jurisdiction, StringComparison.OrdinalIgnoreCase))
                {
                    score += 15;
                    explanation.JurisdictionReason =
                        $"Jurisdiction matches ({adj.Jurisdiction}).";
                }
                else
                {
                    explanation.JurisdictionReason =
                        $"Jurisdiction mismatch: adjuster {adj.Jurisdiction}, claim {claim.Jurisdiction}.";
                }

                // ⭐ WORKLOAD
                var activeAssignments = await _context.Assignments
                    .CountAsync(a => a.AdjusterId == adj.AdjusterId);

                if (activeAssignments <= 3)
                {
                    score += 15;
                    explanation.WorkloadReason = $"Low workload ({activeAssignments}).";
                }
                else if (activeAssignments <= 7)
                {
                    score += 5;
                    explanation.WorkloadReason = $"Moderate workload ({activeAssignments}).";
                }
                else
                {
                    explanation.WorkloadReason = $"High workload ({activeAssignments}).";
                }

                // ⭐ PERFORMANCE SCORE (double)
                if (adj.PerformanceScore >= 90)
                {
                    score += 10;
                    explanation.PerformanceReason =
                        $"Excellent performance score ({adj.PerformanceScore}).";
                }
                else if (adj.PerformanceScore >= 75)
                {
                    score += 5;
                    explanation.PerformanceReason =
                        $"Good performance score ({adj.PerformanceScore}).";
                }
                else
                {
                    explanation.PerformanceReason =
                        $"Lower performance score ({adj.PerformanceScore}).";
                }

                // ⭐ COMPLEXITY PENALTY
                score -= claim.ComplexityScore * 0.5;

                explanation.FinalScore = score;
                explanation.Summary =
                    $"Adjuster {adj.AdjusterId} scored {score} based on skill, level, jurisdiction, workload, performance, and complexity.";

                if (score > bestScore)
                {
                    bestScore = score;
                    best = adj;
                    bestExplanation = explanation;
                }
            }

            if (best == null || bestExplanation == null)
                throw new InvalidOperationException("No suitable adjuster found.");

            return (best, bestExplanation);
        }
    }
}
