using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdjusterOptimizerAPI.Attributes;

namespace AdjusterOptimizerAPI.Controllers
{
    [RoleAuthorize("Admin", "Supervisor")]
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceHistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PerformanceHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var records = await _context.PerformanceHistory
                .Include(p => p.Adjuster)
                .Include(p => p.Claim)
                .ToListAsync();

            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _context.PerformanceHistory
                .Include(p => p.Adjuster)
                .Include(p => p.Claim)
                .FirstOrDefaultAsync(p => p.RecordId == id);

            if (record == null)
                return NotFound("Performance record not found.");

            return Ok(record);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PerformanceHistory model)
        {
            _context.PerformanceHistory.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = model.RecordId }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PerformanceHistory model)
        {
            if (id != model.RecordId)
                return BadRequest("Record ID mismatch.");

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

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
