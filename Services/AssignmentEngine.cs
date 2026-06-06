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

                // Skill match
                if (string.Equals(adj.PrimarySkill, claim.ClaimType, StringComparison.OrdinalIgnoreCase))
                {
                    score += 40;
                    explanation.SkillMatchReason = $"Primary skill matches claim type ({adj.PrimarySkill}).";
                }
                else
                {
                    explanation.SkillMatchReason = $"Primary skill ({adj.PrimarySkill}) differs from claim type ({claim.ClaimType}).";
                }

                // Jurisdiction match
                if (string.Equals(adj.Jurisdiction, claim.Jurisdiction, StringComparison.OrdinalIgnoreCase))
                {
                    score += 25;
                    explanation.JurisdictionReason = $"Jurisdiction matches ({adj.Jurisdiction}).";
                }
                else
                {
                    explanation.JurisdictionReason = $"Jurisdiction mismatch: adjuster {adj.Jurisdiction}, claim {claim.Jurisdiction}.";
                }

                // Workload (fewer active assignments = better)
                var activeAssignments = _context.Assignments
                    .Count(a => a.AdjusterId == adj.AdjusterId);

                if (activeAssignments <= 3)
                {
                    score += 20;
                    explanation.WorkloadReason = $"Low workload ({activeAssignments} active assignments).";
                }
                else if (activeAssignments <= 7)
                {
                    score += 10;
                    explanation.WorkloadReason = $"Moderate workload ({activeAssignments} active assignments).";
                }
                else
                {
                    explanation.WorkloadReason = $"High workload ({activeAssignments} active assignments).";
                }

                // Performance score (assuming 0–100)
                if (adj.PerformanceScore >= 90)
                {
                    score += 15;
                    explanation.PerformanceReason = $"Excellent performance score ({adj.PerformanceScore}).";
                }
                else if (adj.PerformanceScore >= 75)
                {
                    score += 8;
                    explanation.PerformanceReason = $"Good performance score ({adj.PerformanceScore}).";
                }
                else
                {
                    explanation.PerformanceReason = $"Lower performance score ({adj.PerformanceScore}).";
                }

                explanation.FinalScore = score;
                explanation.Summary =
                    $"Adjuster {adj.AdjusterId} scored {score} based on skill, jurisdiction, workload, and performance.";

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
