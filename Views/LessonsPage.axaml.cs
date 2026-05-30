using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TouchTypingTutor.Models;
using TouchTypingTutor.Services;

namespace TouchTypingTutor.Views;

public partial class LessonsPage : UserControl
{
    private readonly DataService _dataService = new();
    private AppData _appData;
    private ObservableCollection<Lesson> _lessons;
    private Lesson? _selectedLesson;

    public LessonsPage()
    {
        InitializeComponent();

        ApplyTexts();
        Localization.LanguageChanged += ApplyTexts;
        DetachedFromVisualTree += (_, _) => Localization.LanguageChanged -= ApplyTexts;

        _appData = _dataService.Load();
        _lessons = new ObservableCollection<Lesson>(_appData.Lessons);
        LessonsList.ItemsSource = _lessons;

        ClearForm();
    }

    private void ApplyTexts()
    {
        TitleText.Text = Localization.T("lessons.title");
        AddButton.Content = Localization.T("lessons.add");
        NameLabel.Text = Localization.T("lessons.name");
        TitleBox.PlaceholderText = Localization.T("lessons.namePlaceholder");
        DifficultyLabel.Text = Localization.T("lessons.difficulty");
        TextLabel.Text = Localization.T("lessons.text");
        TextBox.PlaceholderText = Localization.T("lessons.textPlaceholder");
        SaveButton.Content = Localization.T("lessons.save");
        DeleteButton.Content = Localization.T("lessons.delete");
    }

    private void OnLessonSelected(object? sender, SelectionChangedEventArgs e)
    {
        _selectedLesson = LessonsList.SelectedItem as Lesson;

        if (_selectedLesson == null)
        {
            ClearForm();
            return;
        }

        // Заповнюємо форму даними вибраного уроку
        TitleBox.Text = _selectedLesson.Title;
        TextBox.Text = _selectedLesson.Text;
        DifficultyBox.SelectedItem = FindComboItem(_selectedLesson.Difficulty);

        // Вбудовані уроки не можна видаляти
        DeleteButton.IsEnabled = !_selectedLesson.IsBuiltIn;
        ErrorText.Text = string.Empty;
    }

    // Кнопка "Додати"
    private void OnAddClick(object? sender, RoutedEventArgs e)
    {
        LessonsList.SelectedItem = null;
        _selectedLesson = null;
        ClearForm();
        TitleBox.Focus();
    }

    // Кнопка "Зберегти"
    private void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        // Валідація
        var title = TitleBox.Text?.Trim() ?? string.Empty;
        var text = TextBox.Text?.Trim() ?? string.Empty;
        var difficulty = (DifficultyBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

        if (string.IsNullOrWhiteSpace(title))
        {
            ErrorText.Text = Localization.T("lessons.errEmptyTitle");
            return;
        }

        if (string.IsNullOrWhiteSpace(text) || text.Length < 5)
        {
            ErrorText.Text = Localization.T("lessons.errShortText");
            return;
        }

        if (string.IsNullOrWhiteSpace(difficulty))
        {
            ErrorText.Text = Localization.T("lessons.errDifficulty");
            return;
        }

        if (_selectedLesson == null)
        {
            // Створюємо новий урок
            var newLesson = new Lesson
            {
                Id = _lessons.Count == 0 ? 1 : _lessons.Max(l => l.Id) + 1,
                Title = title,
                Text = text,
                Difficulty = difficulty,
                IsBuiltIn = false
            };
            _lessons.Add(newLesson);
            LessonsList.SelectedItem = newLesson;
        }
        else
        {
            // Оновлюємо властивості — список оновиться сам завдяки INotifyPropertyChanged
            _selectedLesson.Title = title;
            _selectedLesson.Text = text;
            _selectedLesson.Difficulty = difficulty;
        }

        SaveAll();
        ErrorText.Text = Localization.T("lessons.saved");
    }

    // Кнопка "Видалити"
    private void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (_selectedLesson == null || _selectedLesson.IsBuiltIn)
            return;

        _lessons.Remove(_selectedLesson);
        _selectedLesson = null;
        ClearForm();
        SaveAll();
    }

    // Збереження всіх даних у JSON
    private void SaveAll()
    {
        _appData.Lessons = _lessons.ToList();
        _dataService.Save(_appData);
    }

    private void ClearForm()
    {
        TitleBox.Text = string.Empty;
        TextBox.Text = string.Empty;
        DifficultyBox.SelectedIndex = 0;
        ErrorText.Text = string.Empty;
        DeleteButton.IsEnabled = false;
    }

    private ComboBoxItem? FindComboItem(string difficulty)
    {
        foreach (var item in DifficultyBox.Items)
        {
            if (item is ComboBoxItem ci && ci.Content?.ToString() == difficulty)
                return ci;
        }
        return null;
    }
}
