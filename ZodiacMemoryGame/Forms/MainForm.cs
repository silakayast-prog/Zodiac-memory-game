using System.Drawing;
using System.Drawing.Drawing2D;
using ZodiacMemoryGame.Controls;
using ZodiacMemoryGame.Models;
using ZodiacMemoryGame.Services;

namespace ZodiacMemoryGame.Forms;

public class MainForm : Form
{
    private readonly HighScoreService _highScoreService;
    private readonly SoundService _soundService;
    private readonly GameEngine _engine;
    private readonly System.Windows.Forms.Timer _gameTimer;

    // UI Controls
    private Panel _pnlHeader = null!;
    private Panel _pnlDashboard = null!;
    private Panel _pnlBoardContainer = null!;
    private TableLayoutPanel _gridBoard = null!;
    private ComboBox _cmbDifficulty = null!;
    private Label _lblTimer = null!;
    private Label _lblMoves = null!;
    private Label _lblScore = null!;
    private Label _lblCombo = null!;
    private Label _lblHighScore = null!;
    private Button _btnNewGame = null!;
    private Button _btnGuide = null!;
    private Button _btnSound = null!;

    private readonly List<CardControl> _cardControls = new();

    public MainForm()
    {
        _highScoreService = new HighScoreService();
        _soundService = new SoundService();
        _engine = new GameEngine(_highScoreService, _soundService);

        _gameTimer = new System.Windows.Forms.Timer { Interval = 1000 };
        _gameTimer.Tick += (_, _) => _engine.TickTimer();

        // Wire engine events
        _engine.StateChanged += OnGameStateChanged;
        _engine.GameWon += OnGameWon;

        SetupForm();
        InitializeCustomComponents();

        // Start first game
        _engine.StartNewGame(DifficultyLevel.Orta);
        BuildCardGrid();
        UpdateDashboard();
    }

    private void SetupForm()
    {
        Text = "Burçlar Diyarı - Hafıza Kartı Oyunu";
        Size = new Size(1060, 840);
        MinimumSize = new Size(880, 680);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(15, 23, 42); // slate-900
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        DoubleBuffered = true;
    }

    private void InitializeCustomComponents()
    {
        // 1. Header Panel
        _pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 65,
            BackColor = Color.FromArgb(15, 23, 42),
            Padding = new Padding(20, 10, 20, 5)
        };

        var lblAppTitle = new Label
        {
            Text = "✦  BURÇLAR DİYARI : HAFIZA OYUNU  ✦",
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            ForeColor = Color.FromArgb(251, 191, 36), // amber-400
            Dock = DockStyle.Top,
            Height = 30,
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblAppSub = new Label
        {
            Text = "Astrolojik Burç Sembollerini Eşleştir • Hafızanı ve Sezgilerini Test Et",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(148, 163, 184), // slate-400
            Dock = DockStyle.Top,
            Height = 20,
            TextAlign = ContentAlignment.MiddleCenter
        };

        _pnlHeader.Controls.Add(lblAppSub);
        _pnlHeader.Controls.Add(lblAppTitle);

        // 2. Dashboard Panel
        _pnlDashboard = new Panel
        {
            Dock = DockStyle.Top,
            Height = 74,
            BackColor = Color.FromArgb(30, 41, 59), // slate-800
            Padding = new Padding(15, 12, 15, 12)
        };

        // Left section: Difficulty Selection
        var lblDiffTitle = new Label
        {
            Text = "Zorluk:",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(203, 213, 225),
            Location = new Point(16, 15),
            AutoSize = true
        };

        _cmbDifficulty = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            BackColor = Color.FromArgb(15, 23, 42),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Location = new Point(68, 12),
            Width = 140
        };
        _cmbDifficulty.Items.AddRange(new object[] { "Kolay (12 Kart)", "Orta (16 Kart)", "Zor (24 Kart)" });
        _cmbDifficulty.SelectedIndex = 1; // Orta default
        _cmbDifficulty.SelectedIndexChanged += OnDifficultyChanged;

        // Middle section: Stat chips
        int chipX = 230;
        int chipY = 10;
        int chipSpacing = 95;

        _lblTimer = CreateStatChip("⏱ SÜRE", "00:00", ref chipX, chipY, chipSpacing);
        _lblMoves = CreateStatChip("🔄 HAMLE", "0", ref chipX, chipY, chipSpacing);
        _lblScore = CreateStatChip("🌟 PUAN", "0", ref chipX, chipY, chipSpacing);
        _lblCombo = CreateStatChip("🔥 SERİ", "0x", ref chipX, chipY, chipSpacing);
        _lblHighScore = CreateStatChip("🏆 REKOR", "0", ref chipX, chipY, 110);

        // Right section: Buttons
        _btnNewGame = new Button
        {
            Text = "🔄 Yeni Oyun",
            Size = new Size(110, 38),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(_pnlDashboard.Width - 360, 10),
            BackColor = Color.FromArgb(79, 70, 229), // indigo-600
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        _btnNewGame.FlatAppearance.BorderSize = 0;
        _btnNewGame.Click += (_, _) => RestartGame();

        _btnGuide = new Button
        {
            Text = "📖 Burçlar",
            Size = new Size(100, 38),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(_pnlDashboard.Width - 240, 10),
            BackColor = Color.FromArgb(51, 65, 85), // slate-700
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        _btnGuide.FlatAppearance.BorderSize = 0;
        _btnGuide.Click += (_, _) =>
        {
            using var guideForm = new ZodiacGuideForm();
            guideForm.ShowDialog(this);
        };

        _btnSound = new Button
        {
            Text = "🔊 Ses: Açık",
            Size = new Size(110, 38),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(_pnlDashboard.Width - 130, 10),
            BackColor = Color.FromArgb(51, 65, 85),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        _btnSound.FlatAppearance.BorderSize = 0;
        _btnSound.Click += ToggleSound;

        _pnlDashboard.Controls.Add(lblDiffTitle);
        _pnlDashboard.Controls.Add(_cmbDifficulty);
        _pnlDashboard.Controls.Add(_btnNewGame);
        _pnlDashboard.Controls.Add(_btnGuide);
        _pnlDashboard.Controls.Add(_btnSound);

        // Resize behavior for right buttons
        _pnlDashboard.Resize += (_, _) =>
        {
            _btnSound.Left = _pnlDashboard.Width - 130;
            _btnGuide.Left = _pnlDashboard.Width - 240;
            _btnNewGame.Left = _pnlDashboard.Width - 360;
        };

        // 3. Board Container Panel
        _pnlBoardContainer = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(15, 23, 42),
            AutoScroll = true
        };
        _pnlBoardContainer.Resize += (_, _) => CenterGrid();

        // 4. Grid Board
        _gridBoard = new TableLayoutPanel
        {
            AutoSize = false,
            BackColor = Color.Transparent,
            Anchor = AnchorStyles.Top | AnchorStyles.Left
        };

        _pnlBoardContainer.Controls.Add(_gridBoard);

        Controls.Add(_pnlBoardContainer);
        Controls.Add(_pnlDashboard);
        Controls.Add(_pnlHeader);
    }

    private Label CreateStatChip(string header, string initialVal, ref int startX, int startY, int width)
    {
        var panel = new Panel
        {
            Size = new Size(width, 42),
            Location = new Point(startX, startY),
            BackColor = Color.FromArgb(15, 23, 42),
            Padding = new Padding(4)
        };

        var lblHeader = new Label
        {
            Text = header,
            Font = new Font("Segoe UI", 7, FontStyle.Bold),
            ForeColor = Color.FromArgb(148, 163, 184),
            Dock = DockStyle.Top,
            Height = 14,
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblValue = new Label
        {
            Text = initialVal,
            Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(251, 191, 36),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        panel.Controls.Add(lblValue);
        panel.Controls.Add(lblHeader);
        _pnlDashboard.Controls.Add(panel);

        startX += width + 10;
        return lblValue;
    }

    private void BuildCardGrid()
    {
        _gridBoard.SuspendLayout();
        _gridBoard.Controls.Clear();
        _cardControls.Clear();

        var (_, rows, cols) = DifficultyConfig.GetDimensions(_engine.Difficulty);

        _gridBoard.RowCount = rows;
        _gridBoard.ColumnCount = cols;
        _gridBoard.RowStyles.Clear();
        _gridBoard.ColumnStyles.Clear();

        for (int r = 0; r < rows; r++)
            _gridBoard.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));

        for (int c = 0; c < cols; c++)
            _gridBoard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / cols));

        // Card size depending on grid density
        int cardW = cols > 5 ? 110 : 125;
        int cardH = cols > 5 ? 145 : 160;

        _gridBoard.Size = new Size(cols * (cardW + 10), rows * (cardH + 10));

        int index = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (index < _engine.Cards.Count)
                {
                    var card = _engine.Cards[index++];
                    var cardCtrl = new CardControl
                    {
                        Card = card,
                        Size = new Size(cardW, cardH),
                        Dock = DockStyle.Fill
                    };

                    cardCtrl.Click += async (_, _) =>
                    {
                        await _engine.SelectCardAsync(cardCtrl.Card!);
                    };

                    _cardControls.Add(cardCtrl);
                    _gridBoard.Controls.Add(cardCtrl, c, r);
                }
            }
        }

        _gridBoard.ResumeLayout();
        CenterGrid();
    }

    private void CenterGrid()
    {
        if (_gridBoard != null && _pnlBoardContainer != null)
        {
            int x = Math.Max(20, (_pnlBoardContainer.ClientSize.Width - _gridBoard.Width) / 2);
            int y = Math.Max(20, (_pnlBoardContainer.ClientSize.Height - _gridBoard.Height) / 2);
            _gridBoard.Location = new Point(x, y);
        }
    }

    private void RestartGame()
    {
        _gameTimer.Stop();
        var selectedDiff = _cmbDifficulty.SelectedIndex switch
        {
            0 => DifficultyLevel.Kolay,
            1 => DifficultyLevel.Orta,
            2 => DifficultyLevel.Zor,
            _ => DifficultyLevel.Orta
        };

        _engine.StartNewGame(selectedDiff);
        BuildCardGrid();
        UpdateDashboard();
    }

    private void OnDifficultyChanged(object? sender, EventArgs e)
    {
        RestartGame();
    }

    private void ToggleSound(object? sender, EventArgs e)
    {
        _soundService.IsMuted = !_soundService.IsMuted;
        _btnSound.Text = _soundService.IsMuted ? "🔇 Ses: Kapalı" : "🔊 Ses: Açık";
        _btnSound.BackColor = _soundService.IsMuted ? Color.FromArgb(75, 85, 99) : Color.FromArgb(51, 65, 85);
    }

    private void OnGameStateChanged()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(OnGameStateChanged));
            return;
        }

        if (_engine.IsActive && !_gameTimer.Enabled && !_engine.IsGameOver)
        {
            _gameTimer.Start();
        }
        else if (_engine.IsGameOver)
        {
            _gameTimer.Stop();
        }

        UpdateDashboard();

        // Refresh cards
        foreach (var ctrl in _cardControls)
        {
            ctrl.Invalidate();
        }
    }

    private void UpdateDashboard()
    {
        int mm = _engine.ElapsedSeconds / 60;
        int ss = _engine.ElapsedSeconds % 60;
        _lblTimer.Text = $"{mm:D2}:{ss:D2}";
        _lblMoves.Text = _engine.Moves.ToString();
        _lblScore.Text = _engine.Score.ToString("N0");
        _lblCombo.Text = _engine.Combo > 1 ? $"{_engine.Combo}x 🔥" : $"{_engine.Combo}x";
        _lblHighScore.Text = _highScoreService.GetHighScore(_engine.Difficulty).ToString("N0");
    }

    private void OnGameWon(GameEndEventArgs args)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<GameEndEventArgs>(OnGameWon), args);
            return;
        }

        _gameTimer.Stop();

        using var victory = new VictoryDialog(args);
        victory.ShowDialog(this);

        if (victory.RequestRestart)
        {
            RestartGame();
        }
    }
}
