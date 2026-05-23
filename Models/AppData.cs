using System.Collections.Generic;

namespace TouchTypingTutor.Models;

/// <summary>
/// Усі дані додатку в одному об'єкті — для зручного збереження в JSON.
/// </summary>
public class AppData
{
    public List<Lesson> Lessons { get; set; } = new();
    public List<TypingResult> Results { get; set; } = new();
    public AppSettings Settings { get; set; } = new();
}
