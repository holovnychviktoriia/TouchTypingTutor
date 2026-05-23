using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TouchTypingTutor.Models;

namespace TouchTypingTutor.Services;

/// <summary>
/// Сервіс для збереження і завантаження даних додатку в JSON-файл.
/// Файл лежить у папці користувача (~/Library/Application Support/TouchTypingTutor на Mac).
/// </summary>
public class DataService
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public DataService()
    {
        // Стандартна папка для даних додатку, незалежно від ОС
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TouchTypingTutor");

        Directory.CreateDirectory(folder);
        _filePath = Path.Combine(folder, "data.json");
    }

    /// <summary>
    /// Завантажує всі дані з файлу. Якщо файлу немає — повертає дані за замовчуванням.
    /// </summary>
    public AppData Load()
    {
        if (!File.Exists(_filePath))
        {
            return CreateDefaultData();
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            var data = JsonSerializer.Deserialize<AppData>(json);
            return data ?? CreateDefaultData();
        }
        catch
        {
            // Якщо файл пошкоджений — повертаємо дефолт
            return CreateDefaultData();
        }
    }

    /// <summary>
    /// Зберігає всі дані у файл.
    /// </summary>
    public void Save(AppData data)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        File.WriteAllText(_filePath, json);
    }

    /// <summary>
    /// Початкові дані з вбудованими уроками.
    /// </summary>
    private AppData CreateDefaultData()
    {
        return new AppData
        {
            Lessons = new List<Lesson>
            {
                new()
                {
                    Id = 1,
                    Title = "Базові літери (ASDF JKL;)",
                    Text = "asdf jkl; asdf jkl; fjfj dkdk slsl a;a; fdsa jkl; asdf",
                    Difficulty = "Легкий",
                    IsBuiltIn = true
                },
                new()
                {
                    Id = 2,
                    Title = "Прості слова",
                    Text = "сила вода мова рука нога день ніч рік дім ліс поле небо",
                    Difficulty = "Легкий",
                    IsBuiltIn = true
                },
                new()
                {
                    Id = 3,
                    Title = "Речення українською",
                    Text = "Сонце світить яскраво. Птахи співають весело. Діти граються у дворі.",
                    Difficulty = "Середній",
                    IsBuiltIn = true
                },
                new()
                {
                    Id = 4,
                    Title = "English practice",
                    Text = "The quick brown fox jumps over the lazy dog. Practice makes perfect.",
                    Difficulty = "Середній",
                    IsBuiltIn = true
                },
                new()
                {
                    Id = 5,
                    Title = "Складний текст із пунктуацією",
                    Text = "Програмування - це мистецтво розв'язування задач! Чи готовий ти до виклику?",
                    Difficulty = "Складний",
                    IsBuiltIn = true
                }
            }
        };
    }
}
