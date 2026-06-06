using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdjusterOptimizerAPI.Attributes;

namespace AdjusterOptimizerAPI.Controllers
{
    /// <summary>
    /// Handles all operations related to insurance claims,
    /// including CRUD actions and wildcard search functionality.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClaimsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------
        // GET: api/Claims
        // Admin only: Returns all claims in the system.
        // ------------------------------------------------------------
        [HttpGet]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> GetAll()
        {
            var claims = await _context.Claims.ToListAsync();
            return Ok(claims);
        }

        // ------------------------------------------------------------
        // GET: api/Claims/my
        // Adjuster only: Returns claims assigned to the logged-in adjuster.
        // ------------------------------------------------------------
        [HttpGet("my")]
        [RoleAuthorize("Adjuster")]
        public async Task<IActionResult> GetMyClaims()
        {
            var userId = HttpContext.Session.GetInt32("USER_ID");

            if (userId == null)
                return Unauthorized("Not logged in.");

            var claims = await _context.Claims
                .Where(c => c.AssignedAdjusterId == userId)
                .ToListAsync();

            return Ok(claims);
        }

        // ------------------------------------------------------------
        // GET: api/Claims/{id}
        // Admin: can view any claim
        // Adjuster: can only view their own claim
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

            if (role == "Adjuster" && claim.AssignedAdjusterId == userId)
                return Ok(claim);

            return Forbid();
        }

        // ------------------------------------------------------------
        // POST: api/Claims
        // Admin only: Creates a new claim record.
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
        // PUT: api/Claims/{id}
        // Admin only: Updates an existing claim.
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
        // DELETE: api/Claims/{id}
        // Admin only: Deletes a claim from the system.
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
        // GET: api/Claims/search?query=auto
        // Admin: searches all claims
        // Adjuster: searches only their assigned claims
        // ------------------------------------------------------------
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Claim>>> SearchClaims(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty.");

            var role = HttpContext.Session.GetString("ROLE");
            var userId = HttpContext.Session.GetInt32("USER_ID");

            IQueryable<Claim> baseQuery = _context.Claims;

            if (role == "Adjuster")
            {
                baseQuery = baseQuery.Where(c => c.AssignedAdjusterId == userId);
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
