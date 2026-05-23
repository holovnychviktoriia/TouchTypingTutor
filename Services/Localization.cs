using System;
using System.Collections.Generic;

namespace TouchTypingTutor.Services;

/// <summary>
/// Простий менеджер локалізації.
/// Зберігає всі рядки інтерфейсу для двох мов і викликає подію при зміні мови.
/// </summary>
public static class Localization
{
    public static string CurrentLanguage { get; private set; } = "uk";

    // Подія, на яку підписуються сторінки, щоб оновити свої тексти
    public static event Action? LanguageChanged;

    // Словники рядків для двох мов
    private static readonly Dictionary<string, Dictionary<string, string>> _strings = new()
    {
        ["uk"] = new Dictionary<string, string>
        {
            // Меню
            ["nav.home"] = "Головна",
            ["nav.lessons"] = "Уроки",
            ["nav.typing"] = "Тренування",
            ["nav.settings"] = "Налаштування",

            // HomePage
            ["home.title"] = "Головна",
            ["home.welcome"] = "Ласкаво просимо до Touch Typing Tutor!",
            ["home.total"] = "Тренувань",
            ["home.avgWpm"] = "Середня швидкість (WPM)",
            ["home.avgAcc"] = "Середня точність",
            ["home.chartTitle"] = "Прогрес за останні 10 тренувань (WPM)",
            ["home.recent"] = "Останні результати",
            ["home.empty"] = "Поки що немає тренувань. Перейдіть на вкладку «Тренування» і почніть!",

            // LessonsPage
            ["lessons.title"] = "Уроки",
            ["lessons.add"] = "+ Додати урок",
            ["lessons.name"] = "Назва",
            ["lessons.namePlaceholder"] = "Наприклад: Тренування цифр",
            ["lessons.difficulty"] = "Складність",
            ["lessons.text"] = "Текст для тренування",
            ["lessons.textPlaceholder"] = "Введіть текст, який треба буде набирати",
            ["lessons.save"] = "Зберегти",
            ["lessons.delete"] = "Видалити",
            ["lessons.errEmptyTitle"] = "Введіть назву уроку.",
            ["lessons.errShortText"] = "Текст уроку має містити щонайменше 5 символів.",
            ["lessons.errDifficulty"] = "Оберіть складність.",
            ["lessons.saved"] = "Збережено ✓",

            // TypingPage
            ["typing.title"] = "Тренування",
            ["typing.pick"] = "Оберіть урок:",
            ["typing.time"] = "Час",
            ["typing.wpm"] = "Швидкість (WPM)",
            ["typing.accuracy"] = "Точність",
            ["typing.hintStart"] = "Оберіть урок і просто почніть друкувати на клавіатурі",
            ["typing.hintDuring"] = "Друкуйте далі. Програма не дасть продовжити, поки не натиснете правильний символ.",
            ["typing.hintDone"] = "Натисніть «Почати заново» для повторного тренування.",
            ["typing.restart"] = "Почати заново",
            ["typing.done"] = "✓ Готово! Швидкість: {0} WPM, точність: {1:F1}%",

            // SettingsPage
            ["settings.title"] = "Налаштування",
            ["settings.language"] = "Мова інтерфейсу",
            ["settings.theme"] = "Тема оформлення",
            ["settings.themeLight"] = "Світла",
            ["settings.themeDark"] = "Темна",
        },

        ["en"] = new Dictionary<string, string>
        {
            // Menu
            ["nav.home"] = "Home",
            ["nav.lessons"] = "Lessons",
            ["nav.typing"] = "Practice",
            ["nav.settings"] = "Settings",

            // HomePage
            ["home.title"] = "Home",
            ["home.welcome"] = "Welcome to Touch Typing Tutor!",
            ["home.total"] = "Sessions",
            ["home.avgWpm"] = "Average speed (WPM)",
            ["home.avgAcc"] = "Average accuracy",
            ["home.chartTitle"] = "Progress over last 10 sessions (WPM)",
            ["home.recent"] = "Recent results",
            ["home.empty"] = "No sessions yet. Go to the Practice tab and start!",

            // LessonsPage
            ["lessons.title"] = "Lessons",
            ["lessons.add"] = "+ Add lesson",
            ["lessons.name"] = "Title",
            ["lessons.namePlaceholder"] = "For example: Numbers practice",
            ["lessons.difficulty"] = "Difficulty",
            ["lessons.text"] = "Practice text",
            ["lessons.textPlaceholder"] = "Enter the text to be typed",
            ["lessons.save"] = "Save",
            ["lessons.delete"] = "Delete",
            ["lessons.errEmptyTitle"] = "Enter a lesson title.",
            ["lessons.errShortText"] = "Lesson text must contain at least 5 characters.",
            ["lessons.errDifficulty"] = "Choose difficulty.",
            ["lessons.saved"] = "Saved ✓",

            // TypingPage
            ["typing.title"] = "Practice",
            ["typing.pick"] = "Choose a lesson:",
            ["typing.time"] = "Time",
            ["typing.wpm"] = "Speed (WPM)",
            ["typing.accuracy"] = "Accuracy",
            ["typing.hintStart"] = "Pick a lesson and just start typing on the keyboard",
            ["typing.hintDuring"] = "Keep typing. The app won't let you proceed until you press the correct key.",
            ["typing.hintDone"] = "Press \"Restart\" to practice again.",
            ["typing.restart"] = "Restart",
            ["typing.done"] = "✓ Done! Speed: {0} WPM, accuracy: {1:F1}%",

            // SettingsPage
            ["settings.title"] = "Settings",
            ["settings.language"] = "Interface language",
            ["settings.theme"] = "Theme",
            ["settings.themeLight"] = "Light",
            ["settings.themeDark"] = "Dark",
        },
    };

    /// <summary>
    /// Отримати локалізований рядок за ключем.
    /// </summary>
    public static string T(string key)
    {
        if (_strings.TryGetValue(CurrentLanguage, out var dict) &&
            dict.TryGetValue(key, out var value))
        {
            return value;
        }
        return key; // fallback — повертаємо ключ, якщо переклад не знайдено
    }

    /// <summary>
    /// Змінити мову інтерфейсу.
    /// </summary>
    public static void SetLanguage(string language)
    {
        if (language != "uk" && language != "en") return;
        if (CurrentLanguage == language) return;

        CurrentLanguage = language;
        LanguageChanged?.Invoke();
    }
}
