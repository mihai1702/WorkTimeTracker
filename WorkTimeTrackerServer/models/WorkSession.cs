using System;

public class WorkSession
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    
    public DateTime? EndTime { get; set; }
    public DateTime PauseStartTime{get;set;}
    public TimeSpan? Duration { get; set; }
    public bool IsPaused { get; set; } = false;
    public TimeSpan PauseActiveDuration{get;set;}= TimeSpan.Zero;

    public void CalculateDuration()
    {
        if (EndTime.HasValue)
        {
            Duration = EndTime.Value - StartTime - PauseActiveDuration;
        }
    }
}
