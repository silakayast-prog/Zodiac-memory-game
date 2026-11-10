using ZodiacMemoryGame.Models;
using ZodiacMemoryGame.Services;

namespace ZodiacMemoryGame.Tests;

public class GameEngineTests
{
    [Fact]
    public void ZodiacSigns_ShouldHaveExactly12SignsWithAllProperties()
    {
        var signs = ZodiacSign.GetAllSigns();
        Assert.Equal(12, signs.Count);

        // Verify elements distribution: 3 Fire, 3 Earth, 3 Air, 3 Water
        Assert.Equal(3, signs.Count(s => s.Element == ZodiacElement.Ates));
        Assert.Equal(3, signs.Count(s => s.Element == ZodiacElement.Toprak));
        Assert.Equal(3, signs.Count(s => s.Element == ZodiacElement.Hava));
        Assert.Equal(3, signs.Count(s => s.Element == ZodiacElement.Su));

        foreach (var sign in signs)
        {
            Assert.False(string.IsNullOrWhiteSpace(sign.Name));
            Assert.False(string.IsNullOrWhiteSpace(sign.LatinName));
            Assert.False(string.IsNullOrWhiteSpace(sign.Symbol));
            Assert.False(string.IsNullOrWhiteSpace(sign.DateRange));
            Assert.False(string.IsNullOrWhiteSpace(sign.Traits));
        }
    }

    [Theory]
    [InlineData(DifficultyLevel.Kolay, 6, 3, 4)]
    [InlineData(DifficultyLevel.Orta, 8, 4, 4)]
    [InlineData(DifficultyLevel.Zor, 12, 4, 6)]
    public void DifficultyConfig_ShouldReturnCorrectDimensions(DifficultyLevel level, int expectedPairs, int expectedRows, int expectedCols)
    {
        var (pairs, rows, cols) = DifficultyConfig.GetDimensions(level);
        Assert.Equal(expectedPairs, pairs);
        Assert.Equal(expectedRows, rows);
        Assert.Equal(expectedCols, cols);
    }

    [Fact]
    public void StartNewGame_ShouldInitializePairsCorrectly()
    {
        var highScoreService = new HighScoreService();
        var soundService = new SoundService { IsMuted = true };
        var engine = new GameEngine(highScoreService, soundService);

        engine.StartNewGame(DifficultyLevel.Kolay);

        Assert.Equal(12, engine.Cards.Count);
        Assert.Equal(6, engine.TotalPairs);
        Assert.Equal(0, engine.Moves);
        Assert.Equal(0, engine.Score);
        Assert.Equal(0, engine.Combo);
        Assert.False(engine.IsGameOver);

        // Every card must have exactly one partner
        var grouped = engine.Cards.GroupBy(c => c.Sign.Id).ToList();
        Assert.Equal(6, grouped.Count);
        foreach (var group in grouped)
        {
            Assert.Equal(2, group.Count());
        }
    }

    [Fact]
    public async Task MatchingCards_ShouldIncreaseScoreAndCombo()
    {
        var highScoreService = new HighScoreService();
        var soundService = new SoundService { IsMuted = true };
        var engine = new GameEngine(highScoreService, soundService);

        engine.StartNewGame(DifficultyLevel.Kolay);

        // Find a matching pair
        var first = engine.Cards[0];
        var matchingPartner = engine.Cards.First(c => c != first && c.Sign.Id == first.Sign.Id);

        bool flip1 = await engine.SelectCardAsync(first);
        Assert.True(flip1);
        Assert.Equal(CardState.Flipped, first.State);
        Assert.Equal(0, engine.Moves);

        bool flip2 = await engine.SelectCardAsync(matchingPartner);
        Assert.True(flip2);
        Assert.Equal(CardState.Matched, first.State);
        Assert.Equal(CardState.Matched, matchingPartner.State);
        Assert.Equal(1, engine.Moves);
        Assert.Equal(1, engine.Combo);
        Assert.Equal(1, engine.MatchedPairs);
        Assert.True(engine.Score > 0);
    }

    [Fact]
    public async Task MismatchedCards_ShouldResetComboAndHideCards()
    {
        var highScoreService = new HighScoreService();
        var soundService = new SoundService { IsMuted = true };
        var engine = new GameEngine(highScoreService, soundService);

        engine.StartNewGame(DifficultyLevel.Kolay);

        var first = engine.Cards[0];
        var differentCard = engine.Cards.First(c => c.Sign.Id != first.Sign.Id);

        await engine.SelectCardAsync(first);
        bool matchResult = await engine.SelectCardAsync(differentCard);

        Assert.False(matchResult);
        Assert.Equal(CardState.Hidden, first.State);
        Assert.Equal(CardState.Hidden, differentCard.State);
        Assert.Equal(1, engine.Moves);
        Assert.Equal(0, engine.Combo);
    }

    [Fact]
    public async Task CompletingAllPairs_ShouldTriggerGameWon()
    {
        var highScoreService = new HighScoreService();
        var soundService = new SoundService { IsMuted = true };
        var engine = new GameEngine(highScoreService, soundService);

        engine.StartNewGame(DifficultyLevel.Kolay);

        GameEndEventArgs? endArgs = null;
        engine.GameWon += args => endArgs = args;

        var pairs = engine.Cards.GroupBy(c => c.Sign.Id).ToList();
        foreach (var group in pairs)
        {
            var cardList = group.ToList();
            await engine.SelectCardAsync(cardList[0]);
            await engine.SelectCardAsync(cardList[1]);
        }

        Assert.True(engine.IsGameOver);
        Assert.NotNull(endArgs);
        Assert.Equal(engine.TotalPairs, engine.MatchedPairs);
        Assert.True(endArgs.FinalScore > 0);
        Assert.True(endArgs.Stars >= 1 && endArgs.Stars <= 3);
    }
}
