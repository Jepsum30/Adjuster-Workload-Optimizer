using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;
using AdjusterOptimizerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdjusterOptimizerAPI.Attributes;

namespace AdjusterOptimizerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RoleAuthorize("Admin", "Manager")]   // Claims-based authorization
    public class AdjustersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AssignmentEngine _engine;

        public AdjustersController(ApplicationDbContext context, AssignmentEngine engine)
        {
            _context = context;
            _engine = engine;
        }

        // ------------------------------------------------------------
        // GET: api/Adjusters
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var adjusters = await _context.Adjusters.ToListAsync();
            return Ok(adjusters);
        }

        // ------------------------------------------------------------
        // GET: api/Adjusters/{id}
        // ------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var adjuster = await _context.Adjusters.FindAsync(id);

            if (adjuster == null)
                return NotFound("Adjuster not found.");

            return Ok(adjuster);
        }

        // ------------------------------------------------------------
        // POST: api/Adjusters
        // ------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(Adjuster model)
        {
            _context.Adjusters.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = model.AdjusterId }, model);
        }

        // ------------------------------------------------------------
        // PUT: api/Adjusters/{id}
        // ------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Adjuster model)
        {
            if (id != model.AdjusterId)
                return BadRequest("Adjuster ID mismatch.");

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ------------------------------------------------------------
        // DELETE: api/Adjusters/{id}
        // Admin only
        // ------------------------------------------------------------
        [HttpDelete("{id}")]
        [RoleAuthorize("Admin")]   // Only Admin can delete
        public async Task<IActionResult> DeleteAdjuster(int id)
        {
            var adjuster = await _context.Adjusters.FindAsync(id);
            if (adjuster == null)
                return NotFound("Adjuster not found.");

            // Find claims assigned to this adjuster
            var claims = await _context.Claims
                .Where(c => c.AssignedAdjusterId == id)
                .ToListAsync();

            int reassignedCount = 0;

            foreach (var claim in claims)
            {
                var (newAdj, explanation) =
                    await _engine.RecommendAdjusterForClaimAsync(claim.ClaimId);

                claim.AssignedAdjusterId = newAdj.AdjusterId;

                _context.Assignments.Add(new Assignment
                {
                    ClaimId = claim.ClaimId,
                    AdjusterId = newAdj.AdjusterId
                });

                reassignedCount++;
            }

            _context.Adjusters.Remove(adjuster);
            await _context.SaveChangesAsync();

            return Ok($"Adjuster {id} deleted. {reassignedCount} claims were auto‑reassigned.");
        }
    }
}
