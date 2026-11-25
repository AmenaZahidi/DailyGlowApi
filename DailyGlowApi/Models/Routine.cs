namespace DailyGlowApi.Models;

// A morning or evening routine. It is made of steps.
public class Routine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeOfDay TimeOfDay { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // One routine has many steps
    public List<RoutineStep> Steps { get; set; } = new();
}
