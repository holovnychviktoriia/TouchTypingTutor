namespace TouchTypingTutor.Models;

/// <summary>
/// Допоміжний клас для відображення результату в списку на головній.
/// </summary>
public class ResultRow
{
    public string LessonTitle { get; set; } = string.Empty;
    public string DateText { get; set; } = string.Empty;
    public string WpmText { get; set; } = string.Empty;
    public string AccText { get; set; } = string.Empty;
}
