using System.Drawing;
using System.Drawing.Drawing2D;
using ZodiacMemoryGame.Models;

namespace ZodiacMemoryGame.Forms;

public class ZodiacGuideForm : Form
{
    public ZodiacGuideForm()
    {
        Text = "Burçlar Rehberi";
        Size = new Size(760, 620);
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
        // Title Panel
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.FromArgb(30, 41, 59),
            Padding = new Padding(20, 10, 20, 10)
        };

        var lblTitle = new Label
        {
            Text = "✦ 12 BURÇ REHBERİ VE ÖZELLİKLERİ ✦",
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            ForeColor = Color.FromArgb(251, 191, 36), // amber-400
            Dock = DockStyle.Top,
            Height = 28,
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblSubtitle = new Label
        {
            Text = "Hafıza kartlarındaki burçların sembolleri, elementleri ve tarih aralıkları",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(148, 163, 184), // slate-400
            Dock = DockStyle.Top,
            Height = 20,
            TextAlign = ContentAlignment.MiddleCenter
        };

        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Controls.Add(lblTitle);

        // Bottom close button
        var pnlFooter = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 55,
            BackColor = Color.FromArgb(30, 41, 59),
            Padding = new Padding(20, 10, 20, 10)
        };

        var btnClose = new Button
        {
            Text = "Kapat",
            Size = new Size(110, 34),
            BackColor = Color.FromArgb(79, 70, 229), // indigo-600
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.None
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Location = new Point((pnlFooter.Width - btnClose.Width) / 2, 10);
        btnClose.Click += (_, _) => Close();
        pnlFooter.Controls.Add(btnClose);

        // Scrollable flow panel for 12 sign cards
        var flowPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20),
            WrapContents = true,
            BackColor = Color.FromArgb(15, 23, 42)
        };

        foreach (var sign in ZodiacSign.GetAllSigns())
        {
            flowPanel.Controls.Add(CreateSignCard(sign));
        }

        Controls.Add(flowPanel);
        Controls.Add(pnlHeader);
        Controls.Add(pnlFooter);
    }

    private Control CreateSignCard(ZodiacSign sign)
    {
        var panel = new Panel
        {
            Size = new Size(220, 115),
            Margin = new Padding(8),
            BackColor = Color.FromArgb(24, 33, 53),
            Padding = new Padding(12)
        };

        panel.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(sign.PrimaryColor, 1.5f);
            var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
            e.Graphics.DrawRectangle(pen, rect);
        };

        var lblSymbol = new Label
        {
            Text = sign.Symbol,
            Font = new Font("Segoe UI Symbol", 24, FontStyle.Bold),
            ForeColor = sign.PrimaryColor,
            Size = new Size(45, 45),
            Location = new Point(10, 10),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblName = new Label
        {
            Text = $"{sign.Name} ({sign.LatinName})",
            Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(60, 10),
            Size = new Size(150, 22)
        };

        var lblDates = new Label
        {
            Text = $"📅 {sign.DateRange}",
            Font = new Font("Segoe UI", 8, FontStyle.Regular),
            ForeColor = Color.FromArgb(203, 213, 225),
            Location = new Point(60, 32),
            Size = new Size(150, 18)
        };

        var lblElement = new Label
        {
            Text = $"{sign.ElementSymbol} {sign.ElementName} Elementi",
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = sign.PrimaryColor,
            Location = new Point(60, 50),
            Size = new Size(150, 18)
        };

        var lblTraits = new Label
        {
            Text = $"✨ {sign.Traits}",
            Font = new Font("Segoe UI", 8f, FontStyle.Italic),
            ForeColor = Color.FromArgb(148, 163, 184),
            Location = new Point(10, 78),
            Size = new Size(200, 30)
        };

        panel.Controls.Add(lblSymbol);
        panel.Controls.Add(lblName);
        panel.Controls.Add(lblDates);
        panel.Controls.Add(lblElement);
        panel.Controls.Add(lblTraits);

        return panel;
    }
}
