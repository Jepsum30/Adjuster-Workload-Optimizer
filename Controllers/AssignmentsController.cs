using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class AssignmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AssignmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Assignment>>> GetAll()
    {
        var assignments = await _context.Assignments
            .Include(a => a.Adjuster)
            .Include(a => a.Claim)
            .ToListAsync();

        return Ok(assignments);
    }
}
