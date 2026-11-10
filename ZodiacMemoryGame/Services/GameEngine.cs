using ZodiacMemoryGame.Models;

namespace ZodiacMemoryGame.Services;

public class GameEndEventArgs : EventArgs
{
    public int FinalScore { get; }
    public int Moves { get; }
    public int ElapsedSeconds { get; }
    public int MaxCombo { get; }
    public bool IsNewHighScore { get; }
    public int Stars { get; }

    public GameEndEventArgs(int finalScore, int moves, int elapsedSeconds, int maxCombo, bool isNewHighScore, int stars)
    {
        FinalScore = finalScore;
        Moves = moves;
        ElapsedSeconds = elapsedSeconds;
        MaxCombo = maxCombo;
        IsNewHighScore = isNewHighScore;
        Stars = stars;
    }
}

public class GameEngine
{
    private readonly HighScoreService _highScoreService;
    private readonly SoundService _soundService;
    private List<MemoryCard> _cards = new();

    public IReadOnlyList<MemoryCard> Cards => _cards;
    public DifficultyLevel Difficulty { get; private set; } = DifficultyLevel.Orta;
    public int Moves { get; private set; }
    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int MaxCombo { get; private set; }
    public int MatchedPairs { get; private set; }
    public int TotalPairs { get; private set; }
    public int ElapsedSeconds { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsBusy { get; private set; }

    public MemoryCard? FirstCard { get; private set; }
    public MemoryCard? SecondCard { get; private set; }

    public event Action<MemoryCard>? CardFlipped;
    public event Action<MemoryCard, MemoryCard>? CardsMatched;
    public event Action<MemoryCard, MemoryCard>? CardsMismatched;
    public event Action? StateChanged;
    public event Action<GameEndEventArgs>? GameWon;

    public GameEngine(HighScoreService highScoreService, SoundService soundService)
    {
        _highScoreService = highScoreService;
        _soundService = soundService;
    }

    public void StartNewGame(DifficultyLevel difficulty)
    {
        Difficulty = difficulty;
        var (pairs, _, _) = DifficultyConfig.GetDimensions(difficulty);
        TotalPairs = pairs;
        MatchedPairs = 0;
        Moves = 0;
        Score = 0;
        Combo = 0;
        MaxCombo = 0;
        ElapsedSeconds = 0;
        IsActive = false;
        IsGameOver = false;
        IsBusy = false;
        FirstCard = null;
        SecondCard = null;

        // Select random signs for the pairs
        var allSigns = ZodiacSign.GetAllSigns().OrderBy(_ => Guid.NewGuid()).Take(TotalPairs).ToList();
        var cardList = new List<MemoryCard>();
        int idCounter = 1;

        foreach (var sign in allSigns)
        {
            cardList.Add(new MemoryCard(idCounter++, sign));
            cardList.Add(new MemoryCard(idCounter++, sign));
        }

        // Shuffle cards
        _cards = cardList.OrderBy(_ => Guid.NewGuid()).ToList();
        StateChanged?.Invoke();
    }

    public void TickTimer()
    {
        if (IsActive && !IsGameOver)
        {
            ElapsedSeconds++;
            StateChanged?.Invoke();
        }
    }

    public async Task<bool> SelectCardAsync(MemoryCard card)
    {
        if (IsGameOver || IsBusy || card.State != CardState.Hidden)
            return false;

        if (!IsActive)
            IsActive = true;

        if (FirstCard == null)
        {
            // First card flipped
            FirstCard = card;
            card.State = CardState.Flipped;
            _soundService.PlayFlip();
            CardFlipped?.Invoke(card);
            StateChanged?.Invoke();
            return true;
        }

        if (SecondCard == null && card != FirstCard)
        {
            // Second card flipped
            SecondCard = card;
            card.State = CardState.Flipped;
            _soundService.PlayFlip();
            CardFlipped?.Invoke(card);
            Moves++;

            IsBusy = true;
            StateChanged?.Invoke();

            if (FirstCard.Sign.Id == SecondCard.Sign.Id)
            {
                // MATCH!
                Combo++;
                if (Combo > MaxCombo) MaxCombo = Combo;

                FirstCard.State = CardState.Matched;
                SecondCard.State = CardState.Matched;
                MatchedPairs++;

                int basePoints = 120;
                int comboBonus = (Combo - 1) * 50;
                int speedBonus = Math.Max(0, 60 - ElapsedSeconds / 3);
                int earned = basePoints + comboBonus + speedBonus;
                Score += earned;

                _soundService.PlayMatch();
                CardsMatched?.Invoke(FirstCard, SecondCard);

                FirstCard = null;
                SecondCard = null;
                IsBusy = false;
                StateChanged?.Invoke();

                if (MatchedPairs >= TotalPairs)
                {
                    CompleteGame();
                }

                return true;
            }
            else
            {
                // MISMATCH
                Combo = 0;
                Score = Math.Max(0, Score - 15);
                _soundService.PlayMismatch();
                CardsMismatched?.Invoke(FirstCard, SecondCard);
                StateChanged?.Invoke();

                // Brief pause so player sees the second card
                await Task.Delay(900);

                if (FirstCard != null && SecondCard != null)
                {
                    FirstCard.State = CardState.Hidden;
                    SecondCard.State = CardState.Hidden;
                }

                FirstCard = null;
                SecondCard = null;
                IsBusy = false;
                StateChanged?.Invoke();
                return false;
            }
        }

        return false;
    }

    private void CompleteGame()
    {
        IsActive = false;
        IsGameOver = true;

        // Perfect move threshold: TotalPairs * 1.5
        // Calculate stars (1, 2, or 3)
        int stars = 3;
        if (Moves > TotalPairs * 2.2) stars = 1;
        else if (Moves > TotalPairs * 1.5) stars = 2;

        bool isNewBest = _highScoreService.CheckAndSaveHighScore(Difficulty, Score);

        _soundService.PlayVictory();
        GameWon?.Invoke(new GameEndEventArgs(Score, Moves, ElapsedSeconds, MaxCombo, isNewBest, stars));
    }
}
