using Avalonia;
using Avalonia.Styling;

namespace TouchTypingTutor.Services;

/// <summary>
/// Простий менеджер теми — перемикає між світлою і темною.
/// </summary>
public static class ThemeManager
{
    public static string CurrentTheme { get; private set; } = "Light";

    public static void SetTheme(string theme)
    {
        if (theme != "Light" && theme != "Dark") return;

        CurrentTheme = theme;
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant =
                theme == "Dark" ? ThemeVariant.Dark : ThemeVariant.Light;
        }
    }
}
