using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using TouchTypingTutor.Models;
using TouchTypingTutor.Services;

namespace TouchTypingTutor.Views;

public partial class TypingPage : UserControl
{
    private readonly DataService _dataService = new();
    private AppData _appData;
    private Lesson? _currentLesson;

    private DateTime _startTime;
    private bool _isStarted;
    private bool _isFinished;

    // Скільки символів вже правильно введено (= позиція в тексті)
    private int _position;

    // Усі натиснення друкованих клавіш (включно з помилковими) — для точності
    private int _totalKeyPresses;
    private int _correctKeyPresses;

    private readonly DispatcherTimer _timer;

    public TypingPage()
    {
        InitializeComponent();

        _appData = _dataService.Load();
        LessonPicker.ItemsSource = new ObservableCollection<Lesson>(_appData.Lessons);

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };
        _timer.Tick += OnTimerTick;

        // Щоб контрол ловив клавіатуру одразу після появи
        AttachedToVisualTree += (_, _) => Focus();
    }

    private void OnLessonPicked(object? sender, SelectionChangedEventArgs e)
    {
        if (LessonPicker.SelectedItem is Lesson lesson)
        {
            LoadLesson(lesson);
        }
    }

    private void LoadLesson(Lesson lesson)
    {
        _currentLesson = lesson;
        ResetState();
        RenderText();
        Focus(); // повертаємо фокус після вибору в ComboBox
    }

    private void ResetState()
    {
        _isStarted = false;
        _isFinished = false;
        _startTime = default;
        _position = 0;
        _totalKeyPresses = 0;
        _correctKeyPresses = 0;
        TimeLabel.Text = "0 с";
        WpmLabel.Text = "0";
        AccuracyLabel.Text = "100%";
        StatusText.Text = string.Empty;
        _timer.Stop();
    }

    // Малюємо текст з підсвічуванням
    private void RenderText()
    {
        TextDisplay.Inlines?.Clear();
        if (_currentLesson == null) return;

        var target = _currentLesson.Text;

        for (int i = 0; i < target.Length; i++)
        {
            var run = new Run { Text = target[i].ToString() };

            if (i < _position)
            {
                // Уже правильно введено — зелений
                run.Foreground = new SolidColorBrush(Color.Parse("#2E7D32"));
            }
            else if (i == _position)
            {
                // Поточна позиція — підсвічена жовтим
                run.Background = new SolidColorBrush(Color.Parse("#FFF3A8"));
                run.Foreground = new SolidColorBrush(Color.Parse("#1A1A2E"));
            }
            else
            {
                // Ще не введено — приглушений
                run.Foreground = new SolidColorBrush(Color.Parse("#A0A0B0"));
            }

            TextDisplay.Inlines?.Add(run);
        }
    }

    // Обробка кожного натиснення клавіші
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_currentLesson == null || _isFinished) return;

        // Беремо тільки друковані символи. KeySymbol дає враховану розкладку
        // (наприклад, кирилицю), на відміну від e.Key, який повертає фізичну клавішу
        var symbol = e.KeySymbol;
        if (string.IsNullOrEmpty(symbol) || symbol.Length != 1)
            return;

        // Ігноруємо керуючі символи (Ctrl, Tab і т.п.)
        char ch = symbol[0];
        if (char.IsControl(ch))
            return;

        e.Handled = true;

        var target = _currentLesson.Text;
        if (_position >= target.Length) return;

        // Запускаємо таймер на першому натисканні
        if (!_isStarted)
        {
            _isStarted = true;
            _startTime = DateTime.Now;
            _timer.Start();
            HintText.Text = "Друкуйте далі. Програма не дасть продовжити, поки не натиснете правильний символ.";
        }

        _totalKeyPresses++;

        if (ch == target[_position])
        {
            // Правильний символ — рухаємось далі
            _correctKeyPresses++;
            _position++;
            RenderText();
            UpdateStats();

            if (_position == target.Length)
            {
                FinishTraining();
            }
        }
        else
        {
            // Неправильний — позиція не рухається, але рахується в точність
            UpdateStats();
            FlashError();
        }
    }

    // Короткий червоний "блимок" на поточній позиції при помилці
    private void FlashError()
    {
        if (_currentLesson == null) return;
        var target = _currentLesson.Text;
        if (_position >= target.Length) return;

        // Підсвітимо поточний символ червоним фоном
        TextDisplay.Inlines?.Clear();
        for (int i = 0; i < target.Length; i++)
        {
            var run = new Run { Text = target[i].ToString() };
            if (i < _position)
            {
                run.Foreground = new SolidColorBrush(Color.Parse("#2E7D32"));
            }
            else if (i == _position)
            {
                run.Background = new SolidColorBrush(Color.Parse("#FFCDD2"));
                run.Foreground = new SolidColorBrush(Color.Parse("#D33B3B"));
                run.FontWeight = FontWeight.Bold;
            }
            else
            {
                run.Foreground = new SolidColorBrush(Color.Parse("#A0A0B0"));
            }
            TextDisplay.Inlines?.Add(run);
        }

        // Через 150 мс повертаємо нормальне підсвічування
        DispatcherTimer.RunOnce(() =>
        {
            if (!_isFinished) RenderText();
        }, TimeSpan.FromMilliseconds(150));
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        if (!_isStarted) return;

        var elapsed = DateTime.Now - _startTime;
        TimeLabel.Text = $"{(int)elapsed.TotalSeconds} с";

        // WPM рахуємо лише за правильно набраними символами
        // 5 символів = умовне "слово"
        var minutes = elapsed.TotalMinutes;
        if (minutes > 0)
        {
            int wpm = (int)(_correctKeyPresses / 5.0 / minutes);
            WpmLabel.Text = wpm.ToString();
        }

        // Точність = правильні / всі натиснення (включаючи помилкові)
        if (_totalKeyPresses > 0)
        {
            double accuracy = (double)_correctKeyPresses / _totalKeyPresses * 100;
            AccuracyLabel.Text = $"{accuracy:F0}%";
        }
    }

    private void FinishTraining()
    {
        _isFinished = true;
        _timer.Stop();

        var elapsed = DateTime.Now - _startTime;
        int wpm = elapsed.TotalMinutes > 0
            ? (int)(_correctKeyPresses / 5.0 / elapsed.TotalMinutes)
            : 0;
        double accuracy = _totalKeyPresses > 0
            ? (double)_correctKeyPresses / _totalKeyPresses * 100
            : 100;

        var result = new TypingResult
        {
            LessonId = _currentLesson!.Id,
            LessonTitle = _currentLesson.Title,
            Date = DateTime.Now,
            Wpm = wpm,
            Accuracy = Math.Round(accuracy, 1),
            TimeSeconds = (int)elapsed.TotalSeconds
        };

        _appData.Results.Add(result);
        _dataService.Save(_appData);

        StatusText.Text = $"✓ Готово! Швидкість: {wpm} WPM, точність: {accuracy:F1}%";
        HintText.Text = "Натисніть «Почати заново» для повторного тренування.";
    }

    private void OnRestartClick(object? sender, RoutedEventArgs e)
    {
        if (_currentLesson != null)
        {
            LoadLesson(_currentLesson);
        }
    }
}
