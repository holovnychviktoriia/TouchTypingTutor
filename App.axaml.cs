using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TouchTypingTutor.Services;

namespace TouchTypingTutor;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Підвантажуємо збережені налаштування (мова + тема)
        var data = new DataService().Load();
        Localization.SetLanguage(data.Settings.Language);
        ThemeManager.SetTheme(data.Settings.Theme);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
