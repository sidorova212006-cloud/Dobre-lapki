namespace DobreLapki.Domain.Models;

/// <summary>Данные регистрации пользователя.</summary>
public sealed record UserRegistration(string Name, string Email, string? Phone, string Password);

/// <summary>Данные входа пользователя.</summary>
public sealed record UserLogin(string Email, string Password);

/// <summary>Данные обновления профиля.</summary>
public sealed record UserUpdateDraft(string Name, string Email, string? Phone);
