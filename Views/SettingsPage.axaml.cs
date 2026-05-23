using Avalonia.Controls;
using TouchTypingTutor.Models;
using TouchTypingTutor.Services;

namespace TouchTypingTutor.Views;

public partial class SettingsPage : UserControl
{
    private readonly DataService _dataService = new();
    private AppData _appData;
    private bool _isLoading = true; // щоб події ComboBox не спрацювали при ініціалізації

    public SettingsPage()
    {
        InitializeComponent();

        _appData = _dataService.Load();

        ApplyTexts();
        Localization.LanguageChanged += ApplyTexts;
        DetachedFromVisualTree += (_, _) => Localization.LanguageChanged -= ApplyTexts;

        // Виставляємо поточні значення в комбобоксах
        LanguageBox.SelectedIndex = _appData.Settings.Language == "en" ? 1 : 0;
        ThemeBox.SelectedIndex = _appData.Settings.Theme == "Dark" ? 1 : 0;

        _isLoading = false;
    }

    // Підставляємо локалізовані тексти
    private void ApplyTexts()
    {
        TitleText.Text = Localization.T("settings.title");
        LanguageLabel.Text = Localization.T("settings.language");
        ThemeLabel.Text = Localization.T("settings.theme");
        LightItem.Content = Localization.T("settings.themeLight");
        DarkItem.Content = Localization.T("settings.themeDark");
    }

    private void OnLanguageChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isLoading) return;

        var tag = (LanguageBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();
        if (tag == null) return;

        Localization.SetLanguage(tag);
        _appData.Settings.Language = tag;
        _dataService.Save(_appData);
    }

    private void OnThemeChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isLoading) return;

        var tag = (ThemeBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();
        if (tag == null) return;

        ThemeManager.SetTheme(tag);
        _appData.Settings.Theme = tag;
        _dataService.Save(_appData);
    }
}
