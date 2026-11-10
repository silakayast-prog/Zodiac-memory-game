namespace ZodiacMemoryGame.Services;

public enum DifficultyLevel
{
    Kolay,   // 6 pairs = 12 cards (3x4)
    Orta,    // 8 pairs = 16 cards (4x4)
    Zor      // 12 pairs = 24 cards (4x6)
}

public static class DifficultyConfig
{
    public static (int Pairs, int Rows, int Cols) GetDimensions(DifficultyLevel level) => level switch
    {
        DifficultyLevel.Kolay => (6, 3, 4),
        DifficultyLevel.Orta => (8, 4, 4),
        DifficultyLevel.Zor => (12, 4, 6),
        _ => (8, 4, 4)
    };
}
