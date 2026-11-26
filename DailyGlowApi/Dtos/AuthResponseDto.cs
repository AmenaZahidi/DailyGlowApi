namespace DailyGlowApi.Dtos;

// What the client gets back after a successful login
public record AuthResponseDto(string Token, DateTime ExpiresAtUtc);
