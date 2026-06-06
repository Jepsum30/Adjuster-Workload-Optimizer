using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdjusterOptimizerAPI.Attributes;

namespace AdjusterOptimizerAPI.Controllers
{
    /// <summary>
    /// Handles all operations related to adjusters, including
    /// CRUD actions and future workload/performance updates.
    /// </summary>
    [RoleAuthorize("Admin", "Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdjustersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdjustersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------
        // GET: api/Adjusters
        // Returns all adjusters in the system.
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var adjusters = await _context.Adjusters.ToListAsync();
            return Ok(adjusters);
        }

        // ------------------------------------------------------------
        // GET: api/Adjusters/{id}
        // Returns a single adjuster by ID.
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
        // Creates a new adjuster record.
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
        // Updates an existing adjuster.
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
        // Deletes an adjuster from the system.
        // ------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var adjuster = await _context.Adjusters.FindAsync(id);

            if (adjuster == null)
                return NotFound("Adjuster not found.");

            _context.Adjusters.Remove(adjuster);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
