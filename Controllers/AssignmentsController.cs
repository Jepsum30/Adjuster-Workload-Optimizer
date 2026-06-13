using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;

namespace AdjusterOptimizerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------
        // GET: api/Assignments
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAll()
        {
            var assignments = await _context.Assignments
                .Include(a => a.Adjuster)
                .Include(a => a.Claim)
                .ToListAsync();

            return Ok(assignments);
        }

        // ------------------------------------------------------------
        // GET: api/Assignments/{id}
        // ------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<Assignment>> GetById(int id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Adjuster)
                .Include(a => a.Claim)
                .FirstOrDefaultAsync(a => a.AssignmentId == id);

            if (assignment == null)
                return NotFound("Assignment not found.");

            return Ok(assignment);
        }

        // ------------------------------------------------------------
        // POST: api/Assignments
        // ------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Assignment>> Create(Assignment model)
        {
            _context.Assignments.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = model.AssignmentId }, model);
        }

        // ------------------------------------------------------------
        // PUT: api/Assignments/{id}
        // ------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Assignment model)
        {
            if (id != model.AssignmentId)
                return BadRequest("Assignment ID mismatch.");

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ------------------------------------------------------------
        // DELETE: api/Assignments/{id}
        // ------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);

            if (assignment == null)
                return NotFound("Assignment not found.");

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ------------------------------------------------------------
        // NEW: GET api/Assignments/recommend/{claimId}
        // ------------------------------------------------------------
        [HttpGet("recommend/{claimId}")]
        public async Task<IActionResult> Recommend(int claimId)
        {
            // Placeholder logic — replace with your real scoring engine
            var adjuster = await _context.Adjusters.FirstOrDefaultAsync();
            if (adjuster == null)
                return NotFound("No adjusters available.");

            var response = new
            {
                adjuster = new
                {
                    adjuster.AdjusterId,
                    // adjuster.Name removed: use explicit properties available on Adjuster model
                    adjuster.PrimarySkill,
                    adjuster.Jurisdiction,
                    adjuster.PerformanceScore,
                    adjuster.Workload
                },
                explanation = new
                {
                    skillMatchReason = "Skill matches claim type.",
                    jurisdictionReason = "Adjuster works in this jurisdiction.",
                    workloadReason = "Workload is acceptable.",
                    performanceReason = "Strong performance score.",
                    finalScore = 92,
                    summary = "This adjuster is the best match for this claim."
                }
            };

            return Ok(response);
        }

        // ------------------------------------------------------------
        // NEW: POST api/Assignments/auto-assign/{claimId}
        // ------------------------------------------------------------
        [HttpPost("auto-assign/{claimId}")]
        public async Task<IActionResult> AutoAssign(int claimId)
        {
            var adjuster = await _context.Adjusters.FirstOrDefaultAsync();
            if (adjuster == null)
                return NotFound("No adjusters available.");

            var assignment = new Assignment
            {
                ClaimId = claimId,
                AdjusterId = adjuster.AdjusterId,
                // AssignedDate property removed from Assignment model; omit setting it here
            };

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Adjuster auto‑assigned successfully.",
                assignmentId = assignment.AssignmentId
            });
        }
    }
}
