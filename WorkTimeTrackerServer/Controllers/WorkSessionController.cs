using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // <-- Adăugați acest using pentru a permite metodele asincrone

[ApiController]
[Route("api/[controller]")]
public class WorkSessionController : ControllerBase
{
    private readonly AppDbContext _context;

    public WorkSessionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartSession([FromBody] string username)
    {
        var session = new WorkSession
        {
            Username = username,
            StartTime = DateTime.UtcNow
        };

        _context.WorkSessions.Add(session);
        await _context.SaveChangesAsync();

        return Ok(session);
    }

    [HttpPost("stop")]
    public async Task<IActionResult> StopSession([FromBody] string username)
    {
        var session = await _context.WorkSessions
                                     .Where(s => s.Username == username && s.EndTime == null)
                                     .FirstOrDefaultAsync();

        if (session == null)
        {
            return NotFound("Sesiune activă nu a fost găsită pentru acest utilizator.");
        }

        session.EndTime = DateTime.UtcNow;
        session.CalculateDuration();

        _context.WorkSessions.Update(session);
        await _context.SaveChangesAsync();

        return Ok(session);
    }

    [HttpPost("pause")]
    public async Task<IActionResult> PauseSession([FromBody] string username)
    {
        var session = await _context.WorkSessions
                                 .Where(s => s.Username == username && s.EndTime == null && s.IsPaused == false)
                                 .FirstOrDefaultAsync();
        if (session == null)
        {
            return NotFound("Sesiune activă nu a fost găsită pentru acest utilizator sau sesiunea este deja în pauză.");
        }
        session.PauseStartTime = DateTime.UtcNow;
        session.IsPaused = true;
    
        _context.WorkSessions.Update(session);
        await _context.SaveChangesAsync();

        return Ok(session);
    }

    [HttpPost("resume")]
    public async Task<IActionResult> ResumeSession([FromBody] string username)
    {
        var session = await _context.WorkSessions
                                    .Where(s => s.Username == username && s.EndTime == null && s.IsPaused == true)
                                    .FirstOrDefaultAsync();

        if (session == null)
        {
            return NotFound("Sesiune în pauză nu a fost găsită pentru acest utilizator.");
        }

        var pauseDuration = DateTime.UtcNow - session.PauseStartTime;
        session.PauseActiveDuration += pauseDuration;

        session.IsPaused = false;

        _context.WorkSessions.Update(session);
        await _context.SaveChangesAsync();

        return Ok(session);
    }

}
