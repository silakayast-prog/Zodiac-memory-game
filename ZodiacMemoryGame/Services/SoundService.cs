using System.Media;

namespace ZodiacMemoryGame.Services;

public class SoundService
{
    public bool IsMuted { get; set; } = false;

    public void PlayFlip()
    {
        if (IsMuted) return;
        try
        {
            // Subtle click/tap sound
            SystemSounds.Asterisk.Play();
        }
        catch { /* Ignore audio subsystem errors */ }
    }

    public void PlayMatch()
    {
        if (IsMuted) return;
        try
        {
            // Uplifting match sound
            SystemSounds.Hand.Play();
        }
        catch { /* Ignore */ }
    }

    public void PlayMismatch()
    {
        if (IsMuted) return;
        try
        {
            SystemSounds.Beep.Play();
        }
        catch { /* Ignore */ }
    }

    public void PlayVictory()
    {
        if (IsMuted) return;
        try
        {
            SystemSounds.Exclamation.Play();
        }
        catch { /* Ignore */ }
    }
}
