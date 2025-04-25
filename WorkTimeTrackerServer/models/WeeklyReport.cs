public class WeeklyReport
{
    public required int Id { get; set; }
    public string Username { get; set; }
    public DateTime WeekStartDate { get; set; }
    public double TotalHoursWorked { get; set; }
}