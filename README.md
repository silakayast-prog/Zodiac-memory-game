# ✦ Burçlar Diyarı: C# Windows Forms Hafıza Kartı Oyunu ✦

Astrolojik burç sembolleri ve element temalarıyla tasarlanmış, modern ve titreşimsiz (DoubleBuffered) GDI+ grafik motoruna sahip bir C# Windows Forms hafıza kartı oyunu.

---

## 🌟 Oyun Özellikleri

1. **12 Burç ve Özellikleri**:
   - **Ateş**: ♈ Koç, ♌ Aslan, ♐ Yay
   - **Toprak**: ♉ Boğa, ♍ Başak, ♑ Oğlak
   - **Hava**: ♊ İkizler, ♎ Terazi, ♒ Kova
   - **Su**: ♋ Yengeç, ♏ Akrep, ♓ Balık
   - Her kartta burcun simgesi, Türkçe adı, tarih aralığı ve elementine özel renk teması yer alır.

2. **3 Farklı Zorluk Seviyesi**:
   - **Kolay**: 6 çift (12 kart - 3x4 ızgara)
   - **Orta**: 8 çift (16 kart - 4x4 ızgara)
   - **Zor (Klasik)**: 12 çift (24 kart - 4x6 ızgara / Tüm burçlar)

3. **Gelişmiş Puanlama ve Kombo Sistemi**:
   - Ardışık başarılı eşleşmelerde katlanarak artan **Kombo Çarpanı** (🔥 2x, 3x, 4x...).
   - Hızlı eşleşmelerde süre bonusu.
   - Hatalı tahminlerde küçük puan kesintisi (puan 0'ın altına düşmez).

4. **Kalıcı En Yüksek Skor (High Score)**:
   - Zorluk seviyesine göre en yüksek skorlar yerel olarak `%LOCALAPPDATA%\ZodiacMemoryGame\highscores.json` dosyasına kaydedilir.

5. **Görsel ve İşitsel Tasarım**:
   - GDI+ ile vektörel çizilen mistik gece gökyüzü kart arkası (altın yıldızlar ve kozmik desenler).
   - Çevirme, eşleşme, hata ve zafer ses efektleri (İsteğe bağlı olarak tek tıkla sessize alınabilir).
   - Zafer ekranında yıldız derecelendirmesi (★★★) ve detaylı oyun sonu istatistikleri.
   - "Burç Rehberi" penceresi ile 12 burcun tarihleri, elementleri ve karakteristik özellikleri incelenebilir.

---

## 🚀 Nasıl Çalıştırılır?

### Komut Satırından Çalıştırma:
```bash
cd C:\Users\LENOVO\.gemini\antigravity\scratch\ZodiacMemoryGame
dotnet run
```

### Testleri Çalıştırma:
```bash
cd C:\Users\LENOVO\.gemini\antigravity\scratch\ZodiacMemoryGame.Tests
dotnet test
```

### Visual Studio / VS Code ile Açma:
- `ZodiacMemoryGame.slnx` çözüm dosyasını Visual Studio veya C# Dev Kit yüklü VS Code ile açıp `F5` tuşuna basarak doğrudan hata ayıklama modunda başlatabilirsiniz.
