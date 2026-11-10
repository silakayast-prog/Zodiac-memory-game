using System.Drawing;
using System.Drawing.Drawing2D;
using ZodiacMemoryGame.Services;

namespace ZodiacMemoryGame.Forms;

public class VictoryDialog : Form
{
    private readonly GameEndEventArgs _args;
    public bool RequestRestart { get; private set; }

    public VictoryDialog(GameEndEventArgs args)
    {
        _args = args;

        Text = "Zafer!";
        Size = new Size(460, 480);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(15, 23, 42); // slate-900
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        InitializeComponents();
    }

    private void InitializeComponents()
    {
        var mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24)
        };

        // Header Title
        var lblTrophy = new Label
        {
            Text = "🏆",
            Font = new Font("Segoe UI Emoji", 36),
            Size = new Size(60, 60),
            Location = new Point((Width - 60) / 2 - 10, 15),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblTitle = new Label
        {
            Text = "TEBRİKLER!",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(251, 191, 36), // amber-400
            Size = new Size(400, 30),
            Location = new Point((Width - 400) / 2 - 10, 75),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblSub = new Label
        {
            Text = "Tüm burç çiftlerini başarıyla eşleştirdin!",
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(203, 213, 225),
            Size = new Size(400, 22),
            Location = new Point((Width - 400) / 2 - 10, 105),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Stars
        string starsStr = _args.Stars switch
        {
            3 => "★ ★ ★",
            2 => "★ ★ ☆",
            _ => "★ ☆ ☆"
        };
        var lblStars = new Label
        {
            Text = starsStr,
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(250, 204, 21), // yellow-400
            Size = new Size(400, 40),
            Location = new Point((Width - 400) / 2 - 10, 130),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // New High Score Badge
        var lblHighScore = new Label
        {
            Text = _args.IsNewHighScore ? "✨ YENİ REKOR KIRILDI! ✨" : "",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(52, 211, 153), // emerald-400
            Size = new Size(400, 22),
            Location = new Point((Width - 400) / 2 - 10, 172),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Stats Card Panel
        var pnlStats = new Panel
        {
            Size = new Size(380, 130),
            Location = new Point((Width - 380) / 2 - 10, 200),
            BackColor = Color.FromArgb(30, 41, 59),
            Padding = new Padding(15)
        };
        pnlStats.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(71, 85, 105), 1);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlStats.Width - 1, pnlStats.Height - 1);
        };

        string timeStr = $"{_args.ElapsedSeconds / 60:D2}:{_args.ElapsedSeconds % 60:D2}";

        AddStatRow(pnlStats, "🌟 Toplam Puan:", $"{_args.FinalScore:N0}", 15, Color.FromArgb(251, 191, 36));
        AddStatRow(pnlStats, "⏱ Geçen Süre:", timeStr, 42, Color.White);
        AddStatRow(pnlStats, "🔄 Toplam Hamle:", $"{_args.Moves}", 69, Color.White);
        AddStatRow(pnlStats, "🔥 En Yüksek Seri:", $"{_args.MaxCombo}x Eşleşme", 96, Color.FromArgb(244, 114, 182));

        // Action Buttons
        var btnPlayAgain = new Button
        {
            Text = "🔄 Tekrar Oyna",
            Size = new Size(150, 42),
            Location = new Point(55, 360),
            BackColor = Color.FromArgb(79, 70, 229), // indigo-600
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnPlayAgain.FlatAppearance.BorderSize = 0;
        btnPlayAgain.Click += (_, _) =>
        {
            RequestRestart = true;
            Close();
        };

        var btnClose = new Button
        {
            Text = "Kapat",
            Size = new Size(120, 42),
            Location = new Point(235, 360),
            BackColor = Color.FromArgb(51, 65, 85), // slate-700
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            Cursor = Cursors.Hand
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Click += (_, _) => Close();

        mainPanel.Controls.Add(lblTrophy);
        mainPanel.Controls.Add(lblTitle);
        mainPanel.Controls.Add(lblSub);
        mainPanel.Controls.Add(lblStars);
        mainPanel.Controls.Add(lblHighScore);
        mainPanel.Controls.Add(pnlStats);
        mainPanel.Controls.Add(btnPlayAgain);
        mainPanel.Controls.Add(btnClose);

        Controls.Add(mainPanel);
    }

    private static void AddStatRow(Panel panel, string label, string value, int top, Color valColor)
    {
        var lbl = new Label
        {
            Text = label,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(148, 163, 184),
            Location = new Point(20, top),
            Size = new Size(160, 22)
        };

        var val = new Label
        {
            Text = value,
            Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
            ForeColor = valColor,
            Location = new Point(190, top - 2),
            Size = new Size(170, 22),
            TextAlign = ContentAlignment.MiddleRight
        };

        panel.Controls.Add(lbl);
        panel.Controls.Add(val);
    }
}
