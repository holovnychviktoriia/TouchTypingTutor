using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TouchTypingTutor.Models;

/// <summary>
/// Урок — текст для тренування друку.
/// Реалізує INotifyPropertyChanged, щоб UI автоматично оновлювався при зміні властивостей.
/// </summary>
public class Lesson : INotifyPropertyChanged
{
    private int _id;
    private string _title = string.Empty;
    private string _text = string.Empty;
    private string _difficulty = "Легкий";
    private bool _isBuiltIn;

    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }

    public string Text
    {
        get => _text;
        set => SetField(ref _text, value);
    }

    public string Difficulty
    {
        get => _difficulty;
        set => SetField(ref _difficulty, value);
    }

    public bool IsBuiltIn
    {
        get => _isBuiltIn;
        set => SetField(ref _isBuiltIn, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (!Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
