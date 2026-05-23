using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using TouchTypingTutor.Models;
using TouchTypingTutor.Services;

namespace TouchTypingTutor.Views;

public partial class HomePage : UserControl
{
    private readonly DataService _dataService = new();

    public HomePage()
    {
        InitializeComponent();

        ApplyTexts();
        Localization.LanguageChanged += ApplyTexts;
        DetachedFromVisualTree += (_, _) => Localization.LanguageChanged -= ApplyTexts;

        LoadStats();
        ChartCanvas.SizeChanged += (_, _) => DrawChart();
    }

    private void ApplyTexts()
    {
        TitleText.Text = Localization.T("home.title");
        WelcomeText.Text = Localization.T("home.welcome");
        TotalCaption.Text = Localization.T("home.total");
        AvgWpmCaption.Text = Localization.T("home.avgWpm");
        AvgAccCaption.Text = Localization.T("home.avgAcc");
        ChartTitleText.Text = Localization.T("home.chartTitle");
        RecentTitleText.Text = Localization.T("home.recent");
        EmptyText.Text = Localization.T("home.empty");
    }

    private void LoadStats()
    {
        var data = _dataService.Load();
        var results = data.Results;

        if (results.Count == 0)
        {
            EmptyText.IsVisible = true;
            return;
        }

        // Загальна статистика
        TotalLabel.Text = results.Count.ToString();
        AvgWpmLabel.Text = ((int)results.Average(r => r.Wpm)).ToString();
        AvgAccLabel.Text = $"{results.Average(r => r.Accuracy):F0}%";

        // Останні 5 результатів — у списку
        var recent = results
            .OrderByDescending(r => r.Date)
            .Take(5)
            .Select(r => new ResultRow
            {
                LessonTitle = r.LessonTitle,
                DateText = r.Date.ToString("dd.MM HH:mm"),
                WpmText = $"{r.Wpm} WPM",
                AccText = $"{r.Accuracy:F0}%"
            })
            .ToList();

        RecentList.ItemsSource = recent;
    }

    // Малюємо графік WPM по останніх 10 тренуваннях
    private void DrawChart()
    {
        ChartCanvas.Children.Clear();

        var data = _dataService.Load();
        var last10 = data.Results
            .OrderByDescending(r => r.Date)
            .Take(10)
            .Reverse()
            .Select(r => r.Wpm)
            .ToList();

        if (last10.Count == 0) return;

        double w = ChartCanvas.Bounds.Width;
        double h = ChartCanvas.Bounds.Height;
        if (w < 10 || h < 10) return;

        const double padding = 20;
        double chartW = w - padding * 2;
        double chartH = h - padding * 2;

        int maxWpm = Math.Max(last10.Max(), 30);

        // Горизонтальна вісь — нижня лінія
        var axis = new Line
        {
            StartPoint = new Point(padding, h - padding),
            EndPoint = new Point(w - padding, h - padding),
            Stroke = new SolidColorBrush(Color.Parse("#D0D0D8")),
            StrokeThickness = 1
        };
        ChartCanvas.Children.Add(axis);

        // Точки і лінії між ними
        var brush = new SolidColorBrush(Color.Parse("#5B6CFF"));
        Point? prev = null;

        for (int i = 0; i < last10.Count; i++)
        {
            // Координати точки
            double x = last10.Count == 1
                ? padding + chartW / 2
                : padding + i * chartW / (last10.Count - 1);
            double y = h - padding - (last10[i] / (double)maxWpm) * chartH;

            // Лінія до попередньої точки
            if (prev.HasValue)
            {
                var line = new Line
                {
                    StartPoint = prev.Value,
                    EndPoint = new Point(x, y),
                    Stroke = brush,
                    StrokeThickness = 2
                };
                ChartCanvas.Children.Add(line);
            }

            // Сама точка
            var dot = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = brush
            };
            Canvas.SetLeft(dot, x - 4);
            Canvas.SetTop(dot, y - 4);
            ChartCanvas.Children.Add(dot);

            // Підпис значення над точкою
            var label = new TextBlock
            {
                Text = last10[i].ToString(),
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.Parse("#5B6CFF"))
            };
            Canvas.SetLeft(label, x - 8);
            Canvas.SetTop(label, y - 20);
            ChartCanvas.Children.Add(label);

            prev = new Point(x, y);
        }
    }
}
