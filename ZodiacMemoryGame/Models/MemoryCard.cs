namespace ZodiacMemoryGame.Models;

public enum CardState
{
    Hidden,
    Flipped,
    Matched
}

public class MemoryCard
{
    public int UniqueId { get; }
    public ZodiacSign Sign { get; }
    public CardState State { get; set; }

    public MemoryCard(int uniqueId, ZodiacSign sign)
    {
        UniqueId = uniqueId;
        Sign = sign;
        State = CardState.Hidden;
    }

    public bool IsRevealed => State == CardState.Flipped || State == CardState.Matched;
}
