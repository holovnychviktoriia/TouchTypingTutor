namespace TouchTypingTutor.Models;

/// <summary>
/// Налаштування додатку — мова інтерфейсу і тема.
/// </summary>
public class AppSettings
{
    public string Language { get; set; } = "uk"; // "uk" або "en"
    public string Theme { get; set; } = "Light"; // "Light" або "Dark"
}
