using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;
using ClaimModel = AdjusterOptimizerAPI.Models.Claim;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdjusterOptimizerAPI.Attributes;
using System.Security.Claims;

namespace AdjusterOptimizerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClaimsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------
        // GET ALL CLAIMS (Admin only)
        // ------------------------------------------------------------
        [HttpGet]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> GetAll()
        {
            var claims = await _context.Claims.ToListAsync();
            return Ok(claims);
        }

        // ------------------------------------------------------------
        // GET CLAIMS ASSIGNED TO LOGGED-IN ADJUSTER
        // ------------------------------------------------------------
        [HttpGet("my")]
        [RoleAuthorize("Adjuster")]
        public async Task<IActionResult> GetMyClaims()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("Not logged in.");

            int adjusterId = int.Parse(userId);

            var claims = await _context.Assignments
                .Where(a => a.AdjusterId == adjusterId)
                .Include(a => a.Claim)
                .Select(a => a.Claim)
                .ToListAsync();

            return Ok(claims);
        }

        // ------------------------------------------------------------
        // GET CLAIM BY ID (Admin OR assigned Adjuster)
        // ------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
                return NotFound("Claim not found.");

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (role == "Admin")
                return Ok(claim);

            if (role == "Adjuster" && userId != null)
            {
                int adjusterId = int.Parse(userId);

                bool assigned = await _context.Assignments
                    .AnyAsync(a => a.ClaimId == id && a.AdjusterId == adjusterId);

                if (assigned)
                    return Ok(claim);
            }

            return Forbid();
        }

        // ------------------------------------------------------------
        // CREATE CLAIM (Admin only)
        // ------------------------------------------------------------
        [HttpPost]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Create(ClaimModel model)
        {
            _context.Claims.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = model.ClaimId }, model);
        }

        // ------------------------------------------------------------
        // UPDATE CLAIM (Admin only)
        // ------------------------------------------------------------
        [HttpPut("{id}")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Update(int id, ClaimModel model)
        {
            if (id != model.ClaimId)
                return BadRequest("Claim ID mismatch.");

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ------------------------------------------------------------
        // DELETE CLAIM (Admin only)
        // ------------------------------------------------------------
        [HttpDelete("{id}")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
                return NotFound("Claim not found.");

            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ------------------------------------------------------------
        // SEARCH CLAIMS (Admin OR assigned Adjuster)
        // ------------------------------------------------------------
        [HttpGet("search")]
        public async Task<IActionResult> SearchClaims(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty.");

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            IQueryable<ClaimModel> baseQuery = _context.Claims;

            if (role == "Adjuster" && userId != null)
            {
                int adjusterId = int.Parse(userId);

                baseQuery = _context.Assignments
                    .Where(a => a.AdjusterId == adjusterId)
                    .Include(a => a.Claim)
                    .Select(a => a.Claim!)
                    .Where(c => c != null)!;
            }
            else if (role != "Admin")
            {
                return Forbid();
            }

            var results = await baseQuery
                .Where(c =>
                    EF.Functions.Like(c.ClaimType, $"%{query}%") ||
                    EF.Functions.Like(c.Status, $"%{query}%") ||
                    EF.Functions.Like(c.Jurisdiction, $"%{query}%"))
                .ToListAsync();

            return Ok(results);
        }
    }
}
