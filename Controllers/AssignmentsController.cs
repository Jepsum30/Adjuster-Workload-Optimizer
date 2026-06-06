using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdjusterOptimizerAPI.Attributes;

namespace AdjusterOptimizerAPI.Controllers
{
    /// <summary>
    /// Handles all operations related to claim-to-adjuster assignments,
    /// including CRUD actions and future integration with the assignment engine.
    /// </summary>
    [RoleAuthorize("Admin")]
    [RoleAuthorize("Supervisor")]
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------
        // GET: api/Assignments
        // Returns all assignments with navigation properties.
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assignments = await _context.Assignments
                .Include(a => a.Adjuster)
                .Include(a => a.Claim)
                .ToListAsync();

            return Ok(assignments);
        }

        // ------------------------------------------------------------
        // GET: api/Assignments/{id}
        // Returns a single assignment by ID with navigation properties.
        // ------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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
        // Creates a new assignment record.
        // ------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(Assignment model)
        {
            _context.Assignments.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = model.AssignmentId }, model);
        }

        // ------------------------------------------------------------
        // PUT: api/Assignments/{id}
        // Updates an existing assignment.
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
        // Deletes an assignment from the system.
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
    }
}
