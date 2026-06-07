using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdjusterOptimizerAPI.Attributes;

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
            var userId = HttpContext.Session.GetInt32("USER_ID");

            if (userId == null)
                return Unauthorized("Not logged in.");

            // Claims assigned via assignments table
            var claims = await _context.Assignments
                .Where(a => a.AdjusterId == userId)
                .Include(a => a.Claim)
                .Select(a => a.Claim)
                .ToListAsync();

            return Ok(claims);
        }

        // ------------------------------------------------------------
        // GET CLAIM BY ID
        // ------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
                return NotFound("Claim not found.");

            var role = HttpContext.Session.GetString("ROLE");
            var userId = HttpContext.Session.GetInt32("USER_ID");

            if (role == "Admin")
                return Ok(claim);

            if (role == "Adjuster")
            {
                bool assignedToAdjuster = await _context.Assignments
                    .AnyAsync(a => a.ClaimId == id && a.AdjusterId == userId);

                if (assignedToAdjuster)
                    return Ok(claim);
            }

            return Forbid();
        }

        // ------------------------------------------------------------
        // CREATE CLAIM (Admin only)
        // ------------------------------------------------------------
        [HttpPost]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Create(Claim model)
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
        public async Task<IActionResult> Update(int id, Claim model)
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
        // SEARCH CLAIMS
        // ------------------------------------------------------------
        [HttpGet("search")]
        public async Task<IActionResult> SearchClaims(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty.");

            var role = HttpContext.Session.GetString("ROLE");
            var userId = HttpContext.Session.GetInt32("USER_ID");

            IQueryable<Claim> baseQuery = _context.Claims;

            if (role == "Adjuster")
            {
                baseQuery = _context.Assignments
                    .Where(a => a.AdjusterId == userId)
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
