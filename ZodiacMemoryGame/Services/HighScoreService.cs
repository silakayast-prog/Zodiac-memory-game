using System.Text.Json;

namespace ZodiacMemoryGame.Services;

public class HighScoreRecord
{
    public int EasyBest { get; set; } = 0;
    public int MediumBest { get; set; } = 0;
    public int HardBest { get; set; } = 0;
}

public class HighScoreService
{
    private readonly string _filePath;
    private HighScoreRecord _records = new();

    public HighScoreService()
    {
        string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ZodiacMemoryGame");
        Directory.CreateDirectory(folder);
        _filePath = Path.Combine(folder, "highscores.json");
        Load();
    }

    public int GetHighScore(DifficultyLevel level) => level switch
    {
        DifficultyLevel.Kolay => _records.EasyBest,
        DifficultyLevel.Orta => _records.MediumBest,
        DifficultyLevel.Zor => _records.HardBest,
        _ => 0
    };

    public bool CheckAndSaveHighScore(DifficultyLevel level, int newScore)
    {
        int current = GetHighScore(level);
        if (newScore > current)
        {
            switch (level)
            {
                case DifficultyLevel.Kolay: _records.EasyBest = newScore; break;
                case DifficultyLevel.Orta: _records.MediumBest = newScore; break;
                case DifficultyLevel.Zor: _records.HardBest = newScore; break;
            }
            Save();
            return true;
        }
        return false;
    }

    private void Load()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<HighScoreRecord>(json);
                if (loaded != null) _records = loaded;
            }
        }
        catch { /* Default to empty if corrupt */ }
    }

    private void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(_records, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch { /* Ignore write errors */ }
    }
}
