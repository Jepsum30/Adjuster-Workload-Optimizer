using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdjusterOptimizerAPI.Attributes;

namespace AdjusterOptimizerAPI.Controllers
{
    /// <summary>
    /// Manages performance history records for adjusters,
    /// including cycle time, indemnity, litigation, and satisfaction data.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [RoleAuthorize("Admin", "Supervisor")]   // Claims-based authorization
    public class PerformanceHistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PerformanceHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------
        // GET ALL PERFORMANCE RECORDS
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var records = await _context.PerformanceHistory
                .Include(p => p.Adjuster)
                .ToListAsync();

            return Ok(records);
        }

        // ------------------------------------------------------------
        // GET PERFORMANCE RECORD BY ID
        // ------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _context.PerformanceHistory
                .Include(p => p.Adjuster)
                .FirstOrDefaultAsync(p => p.RecordId == id);

            if (record == null)
                return NotFound("Performance record not found.");

            return Ok(record);
        }

        // ------------------------------------------------------------
        // CREATE PERFORMANCE RECORD
        // ------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(PerformanceHistory model)
        {
            _context.PerformanceHistory.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = model.RecordId }, model);
        }

        // ------------------------------------------------------------
        // UPDATE PERFORMANCE RECORD
        // ------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PerformanceHistory model)
        {
            if (id != model.RecordId)
                return BadRequest("Record ID mismatch.");

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ------------------------------------------------------------
        // DELETE PERFORMANCE RECORD
        // ------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _context.PerformanceHistory.FindAsync(id);

            if (record == null)
                return NotFound("Performance record not found.");

            _context.PerformanceHistory.Remove(record);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
