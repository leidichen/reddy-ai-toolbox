using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeepSeekDeskBand
{
    /// <summary>
    /// 简洁余额显示 —— 无图标、无标题，只有余额数字
    /// </summary>
    public partial class DeskBandControl : UserControl
    {
        private readonly DeskBand _deskBand;
        private readonly DeepSeekApiClient _apiClient;

        private Color _dotColor = Color.Yellow;
        private Timer _refreshTimer;
        private string _displayText = "...";

        private BalanceResult? _currentBalance;
        private bool _isLoading;
        private string? _apiKey;

        private readonly Color _white = Color.FromArgb(240, 240, 240);
        private readonly Color _panel = Color.FromArgb(47, 54, 61);
        private readonly Color _panelBorder = Color.FromArgb(68, 78, 87);
        private readonly Color _green = Color.FromArgb(16, 185, 129);
        private readonly Color _yellow = Color.FromArgb(250, 204, 21);
        private readonly Color _red = Color.FromArgb(248, 113, 113);
        private readonly Color _redBg = Color.FromArgb(180, 40, 40);

        private Color _bgColor;

        public DeskBandControl(DeskBand deskBand)
        {
            _deskBand = deskBand;
            _apiClient = new DeepSeekApiClient();
            InitializeControl();
        }

        private void InitializeControl()
        {
            _bgColor = _panel;
            this.BackColor = _panel;
            this.MinimumSize = new Size(120, 40);
            this.Size = new Size(180, 40);
            this.Cursor = Cursors.Hand;
            this.Margin = new Padding(0);

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, false);
            this.TabStop = false;

            this.Click += (s, e) =>
                _deskBand.ShowFlyout(_currentBalance ?? BalanceResult.NoApiKey());

            _refreshTimer = new Timer { Interval = 30000 };
            _refreshTimer.Tick += async (s, e) => await RefreshBalance();
        }

        public void Start()
        {
            _apiKey = CredentialManager.LoadApiKey();
            if (string.IsNullOrEmpty(_apiKey))
                SetState("设置 API Key", _yellow);
            else
                _ = RefreshBalance();
            _refreshTimer.Start();
        }

        public async Task RefreshBalance()
        {
            if (_isLoading) return;
            _apiKey = CredentialManager.LoadApiKey();
            if (string.IsNullOrEmpty(_apiKey)) { SetState("设置 API Key", _yellow); return; }

            _isLoading = true;

            try
            {
                _currentBalance = await _apiClient.GetBalanceAsync(_apiKey);
                if (_currentBalance.IsSuccess)
                {
                    bool lowBal = decimal.TryParse(_currentBalance.TotalBalance,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out decimal balVal)
                        && balVal < 10m;
                    _bgColor = lowBal ? _redBg : _panel;
                    SetState($"¥ {_currentBalance.TotalBalance ?? "—"}", lowBal ? Color.White : _green);
                }
                else
                {
                    _bgColor = _panel;
                    SetState(ShortError(_currentBalance), _red);
                }
            }
            catch (Exception ex)
            {
                _currentBalance = BalanceResult.Error(ex.Message);
                SetState("Error", _red);
            }
            finally { _isLoading = false; }
        }

        public async Task ReloadAndRefresh()
        {
            _apiKey = CredentialManager.LoadApiKey();
            await RefreshBalance();
        }

        public BalanceResult? CurrentBalance => _currentBalance;

        private void SetState(string text, Color dotColor)
        {
            if (InvokeRequired) { Invoke(new Action(() => SetState(text, dotColor))); return; }
            _displayText = text;
            _dotColor = dotColor;
            Invalidate();
        }

        private static string ShortError(BalanceResult r)
        {
            if (r.Status == BalanceResult.BalanceStatus.NoApiKey) return "无 Key";
            if (r.Status == BalanceResult.BalanceStatus.NetworkError) return "网络错误";
            return "错误";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var textBounds = new Rectangle(8, 4, this.Width - 28, 32);
            TextRenderer.DrawText(
                e.Graphics,
                _displayText,
                this.Font,
                textBounds,
                _white,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            using Brush brush = new SolidBrush(_dotColor);
            e.Graphics.FillEllipse(brush, this.Width - 16, 16, 8, 8);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(_bgColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using GraphicsPath path = CreateRoundedPath(new Rectangle(0, 0, Width - 1, Height - 1), 6);
            using SolidBrush brush = new SolidBrush(_bgColor);
            using Pen pen = new Pen(_panelBorder, 1f);
            e.Graphics.FillPath(brush, path);
            e.Graphics.DrawPath(pen, path);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _refreshTimer?.Dispose(); _apiClient?.Dispose(); }
            base.Dispose(disposing);
        }
    }
}

