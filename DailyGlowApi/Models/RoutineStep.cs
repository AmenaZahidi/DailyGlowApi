namespace DailyGlowApi.Models;

// One step in a routine: which product to use, and in what order
public class RoutineStep
{
    public int Id { get; set; }

    public int RoutineId { get; set; }
    public Routine Routine { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int StepOrder { get; set; }
    public string? Instructions { get; set; }
}
