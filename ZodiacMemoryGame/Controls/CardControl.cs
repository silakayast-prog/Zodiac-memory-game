using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using ZodiacMemoryGame.Models;

namespace ZodiacMemoryGame.Controls;

public class CardControl : Control
{
    private MemoryCard? _card;
    private bool _isHovered;

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public MemoryCard? Card
    {
        get => _card;
        set
        {
            _card = value;
            Invalidate();
        }
    }

    public CardControl()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw, true);

        Cursor = Cursors.Hand;
        Size = new Size(110, 150);
        Margin = new Padding(6);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        if (_card != null && _card.State == CardState.Hidden)
        {
            _isHovered = true;
            Invalidate();
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHovered = false;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        var rect = new Rectangle(2, 2, Width - 5, Height - 5);
        int cornerRadius = 14;

        if (_card == null || _card.State == CardState.Hidden)
        {
            DrawCardBack(g, rect, cornerRadius);
        }
        else
        {
            DrawCardFront(g, rect, cornerRadius, _card.Sign, _card.State == CardState.Matched);
        }
    }

    private void DrawCardBack(Graphics g, Rectangle rect, int radius)
    {
        using var path = CreateRoundedRectanglePath(rect, radius);

        // Dark celestial gradient
        Color topColor = _isHovered ? Color.FromArgb(30, 27, 75) : Color.FromArgb(15, 23, 42);
        Color botColor = _isHovered ? Color.FromArgb(49, 46, 129) : Color.FromArgb(30, 41, 59);

        using (var brush = new LinearGradientBrush(rect, topColor, botColor, 60f))
        {
            g.FillPath(brush, path);
        }

        // Card border
        Color borderColor = _isHovered ? Color.FromArgb(251, 191, 36) : Color.FromArgb(217, 119, 6);
        int borderWidth = _isHovered ? 2 : 1;
        using (var pen = new Pen(borderColor, borderWidth))
        {
            g.DrawPath(pen, path);
        }

        // Inner decorative border
        var innerRect = Rectangle.Inflate(rect, -7, -7);
        if (innerRect.Width > 0 && innerRect.Height > 0)
        {
            using var innerPath = CreateRoundedRectanglePath(innerRect, radius - 4);
            using var innerPen = new Pen(Color.FromArgb(50, 251, 191, 36), 1) { DashStyle = DashStyle.Dash };
            g.DrawPath(innerPen, innerPath);
        }

        // Corner star accents
        using (var starFont = new Font("Segoe UI Symbol", 8, FontStyle.Regular))
        using (var starBrush = new SolidBrush(Color.FromArgb(180, 251, 191, 36)))
        {
            g.DrawString("✦", starFont, starBrush, rect.Left + 8, rect.Top + 8);
            g.DrawString("✦", starFont, starBrush, rect.Right - 18, rect.Top + 8);
            g.DrawString("✦", starFont, starBrush, rect.Left + 8, rect.Bottom - 20);
            g.DrawString("✦", starFont, starBrush, rect.Right - 18, rect.Bottom - 20);
        }

        // Central mystic symbol
        float centerX = rect.Left + rect.Width / 2f;
        float centerY = rect.Top + rect.Height / 2f - 8;

        // Glowing circle in center
        int circleSize = Math.Min(rect.Width, rect.Height) / 3;
        var circleRect = new Rectangle((int)(centerX - circleSize / 2), (int)(centerY - circleSize / 2), circleSize, circleSize);
        using (var circleBrush = new SolidBrush(Color.FromArgb(_isHovered ? 40 : 25, 251, 191, 36)))
        {
            g.FillEllipse(circleBrush, circleRect);
        }
        using (var circlePen = new Pen(Color.FromArgb(90, 251, 191, 36), 1))
        {
            g.DrawEllipse(circlePen, circleRect);
        }

        using (var symbolFont = new Font("Segoe UI Symbol", 20, FontStyle.Bold))
        using (var symbolBrush = new SolidBrush(_isHovered ? Color.FromArgb(254, 240, 138) : Color.FromArgb(251, 191, 36)))
        {
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("✧", symbolFont, symbolBrush, centerX, centerY, sf);
        }

        // "BURÇ" text at bottom
        using (var textFont = new Font("Segoe UI", 8, FontStyle.Bold))
        using (var textBrush = new SolidBrush(Color.FromArgb(160, 251, 191, 36)))
        {
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("ZODIAC", textFont, textBrush, centerX, rect.Bottom - 18, sf);
        }
    }

    private void DrawCardFront(Graphics g, Rectangle rect, int radius, ZodiacSign sign, bool isMatched)
    {
        using var path = CreateRoundedRectanglePath(rect, radius);

        // Rich element gradient
        Color topColor = sign.SecondaryColor;
        Color botColor = Color.FromArgb(15, 23, 42); // deep dark base

        using (var brush = new LinearGradientBrush(rect, topColor, botColor, 90f))
        {
            g.FillPath(brush, path);
        }

        // Glow & Border
        Color borderColor = isMatched ? Color.FromArgb(52, 211, 153) : sign.PrimaryColor;
        int borderWidth = isMatched ? 3 : 2;
        using (var pen = new Pen(borderColor, borderWidth))
        {
            g.DrawPath(pen, path);
        }

        // Header: Element Tag & Emoji
        using (var tagFont = new Font("Segoe UI", 7.5f, FontStyle.Bold))
        using (var tagBrush = new SolidBrush(Color.FromArgb(220, 255, 255, 255)))
        {
            string elementTag = $"{sign.ElementSymbol} {sign.ElementName}";
            g.DrawString(elementTag, tagFont, tagBrush, rect.Left + 8, rect.Top + 8);
        }

        // Matched badge in top right
        if (isMatched)
        {
            using var badgeFont = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            using var badgeBrush = new SolidBrush(Color.FromArgb(52, 211, 153));
            var sfRight = new StringFormat { Alignment = StringAlignment.Far };
            g.DrawString("✓", badgeFont, badgeBrush, rect.Right - 8, rect.Top + 8, sfRight);
        }

        // Center: Huge Zodiac Symbol
        float centerX = rect.Left + rect.Width / 2f;
        float centerY = rect.Top + rect.Height * 0.44f;

        using (var symbolFont = new Font("Segoe UI Symbol", 32, FontStyle.Bold))
        using (var symbolBrush = new SolidBrush(Color.White))
        {
            var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(sign.Symbol, symbolFont, symbolBrush, centerX, centerY, sfCenter);
        }

        // Name
        using (var nameFont = new Font("Segoe UI", 11, FontStyle.Bold))
        using (var nameBrush = new SolidBrush(Color.White))
        {
            var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(sign.Name.ToUpperInvariant(), nameFont, nameBrush, centerX, rect.Bottom - 38, sfCenter);
        }

        // Date Range
        using (var dateFont = new Font("Segoe UI", 7, FontStyle.Regular))
        using (var dateBrush = new SolidBrush(Color.FromArgb(200, 203, 213, 225)))
        {
            var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(sign.DateRange, dateFont, dateBrush, centerX, rect.Bottom - 18, sfCenter);
        }

        // If matched, overlay a delicate subtle matched sheen
        if (isMatched)
        {
            using var sheenBrush = new SolidBrush(Color.FromArgb(25, 52, 211, 153));
            g.FillPath(sheenBrush, path);
        }
    }

    private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
