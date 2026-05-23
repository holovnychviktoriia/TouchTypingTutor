using Avalonia.Controls;
using Avalonia.Interactivity;
using TouchTypingTutor.Views;

namespace TouchTypingTutor;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ContentArea.Content = new HomePage();
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
