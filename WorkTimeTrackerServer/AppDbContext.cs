using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<WorkSession> WorkSessions { get; set; }
    public DbSet<WeeklyReport> WeeklyReports { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
}