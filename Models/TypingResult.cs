using System;

namespace TouchTypingTutor.Models;

/// <summary>
/// Результат одного завершеного тренування.
/// </summary>
public class TypingResult
{
    public int LessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Wpm { get; set; }           // швидкість, слів за хвилину
    public double Accuracy { get; set; }   // точність, %
    public int TimeSeconds { get; set; }
}
