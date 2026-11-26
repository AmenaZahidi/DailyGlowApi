namespace DailyGlowApi.Models;

// A person who can log in to the API
public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;

    // Only the hash is saved, never the real password
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
