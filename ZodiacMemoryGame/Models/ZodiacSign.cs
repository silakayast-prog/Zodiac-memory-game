using System.Drawing;

namespace ZodiacMemoryGame.Models;

public enum ZodiacElement
{
    Ates,    // Fire
    Toprak,  // Earth
    Hava,    // Air
    Su       // Water
}

public class ZodiacSign
{
    public int Id { get; }
    public string Name { get; }
    public string LatinName { get; }
    public string Symbol { get; }
    public ZodiacElement Element { get; }
    public string DateRange { get; }
    public string Traits { get; }
    public Color PrimaryColor { get; }
    public Color SecondaryColor { get; }

    public ZodiacSign(int id, string name, string latinName, string symbol, ZodiacElement element, string dateRange, string traits, Color primaryColor, Color secondaryColor)
    {
        Id = id;
        Name = name;
        LatinName = latinName;
        Symbol = symbol;
        Element = element;
        DateRange = dateRange;
        Traits = traits;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
    }

    public string ElementName => Element switch
    {
        ZodiacElement.Ates => "Ateş",
        ZodiacElement.Toprak => "Toprak",
        ZodiacElement.Hava => "Hava",
        ZodiacElement.Su => "Su",
        _ => ""
    };

    public string ElementSymbol => Element switch
    {
        ZodiacElement.Ates => "🔥",
        ZodiacElement.Toprak => "🌱",
        ZodiacElement.Hava => "💨",
        ZodiacElement.Su => "💧",
        _ => ""
    };

    public static IReadOnlyList<ZodiacSign> GetAllSigns() => new List<ZodiacSign>
    {
        new(1,  "Koç",     "Aries",       "♈", ZodiacElement.Ates,   "21 Mar - 19 Nis", "Cesur, Lider, Enerjik",       Color.FromArgb(239, 68, 68),   Color.FromArgb(153, 27, 27)),
        new(2,  "Boğa",    "Taurus",      "♉", ZodiacElement.Toprak, "20 Nis - 20 May", "Sabırlı, Kararlı, Sadık",     Color.FromArgb(16, 185, 129), Color.FromArgb(6, 95, 70)),
        new(3,  "İkizler", "Gemini",      "♊", ZodiacElement.Hava,   "21 May - 20 Haz", "Zeki, Meraklı, İletişimci",   Color.FromArgb(56, 189, 248), Color.FromArgb(14, 116, 144)),
        new(4,  "Yengeç",  "Cancer",      "♋", ZodiacElement.Su,     "21 Haz - 22 Tem", "Duygusal, Şefkatli, Sezgisel",Color.FromArgb(129, 140, 248),Color.FromArgb(67, 56, 202)),
        new(5,  "Aslan",   "Leo",         "♌", ZodiacElement.Ates,   "23 Tem - 22 Ağu", "Cömert, Yaratıcı, Gururlu",   Color.FromArgb(245, 158, 11), Color.FromArgb(180, 83, 9)),
        new(6,  "Başak",   "Virgo",       "♍", ZodiacElement.Toprak, "23 Ağu - 22 Eyl", "Titiz, Analitik, Çalışkan",    Color.FromArgb(34, 197, 94),  Color.FromArgb(20, 83, 45)),
        new(7,  "Terazi",  "Libra",       "♎", ZodiacElement.Hava,   "23 Eyl - 22 Eki", "Uyumlu, Adil, Zarif",         Color.FromArgb(14, 165, 233), Color.FromArgb(3, 105, 161)),
        new(8,  "Akrep",   "Scorpio",     "♏", ZodiacElement.Su,     "23 Eki - 21 Kas", "Tutkulu, Güçlü, Gizemli",     Color.FromArgb(168, 85, 247), Color.FromArgb(107, 33, 168)),
        new(9,  "Yay",     "Sagittarius", "♐", ZodiacElement.Ates,   "22 Kas - 21 Ara", "İyimser, Maceracı, Özgür",     Color.FromArgb(249, 115, 22), Color.FromArgb(154, 52, 18)),
        new(10, "Oğlak",   "Capricorn",   "♑", ZodiacElement.Toprak, "22 Ara - 19 Oca", "Disiplinli, Hırslı, Sadık",    Color.FromArgb(5, 150, 105),  Color.FromArgb(4, 47, 46)),
        new(11, "Kova",    "Aquarius",    "♒", ZodiacElement.Hava,   "20 Oca - 18 Şub", "Yenilikçi, Özgün, İnsancıl",  Color.FromArgb(6, 182, 212),  Color.FromArgb(21, 94, 117)),
        new(12, "Balık",   "Pisces",      "♓", ZodiacElement.Su,     "19 Şub - 20 Mar", "Hayalperest, Duyarlı, Empatik", Color.FromArgb(139, 92, 246), Color.FromArgb(76, 29, 149))
    };
}
