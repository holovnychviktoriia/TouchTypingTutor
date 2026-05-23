using Avalonia.Controls;
using Avalonia.Interactivity;
using TouchTypingTutor.Services;
using TouchTypingTutor.Views;

namespace TouchTypingTutor;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ApplyTexts();
        Localization.LanguageChanged += ApplyTexts;

        ContentArea.Content = new HomePage();
    }

    private void ApplyTexts()
    {
        HomeButton.Content = Localization.T("nav.home");
        LessonsButton.Content = Localization.T("nav.lessons");
        TypingButton.Content = Localization.T("nav.typing");
        SettingsButton.Content = Localization.T("nav.settings");
    }

    private void OnHomeClick(object? sender, RoutedEventArgs e)
    {
        ContentArea.Content = new HomePage();
    }

    private void OnLessonsClick(object? sender, RoutedEventArgs e)
    {
        ContentArea.Content = new LessonsPage();
    }

    private void OnTypingClick(object? sender, RoutedEventArgs e)
    {
        ContentArea.Content = new TypingPage();
    }

    private void OnSettingsClick(object? sender, RoutedEventArgs e)
    {
        ContentArea.Content = new SettingsPage();
    }
}
