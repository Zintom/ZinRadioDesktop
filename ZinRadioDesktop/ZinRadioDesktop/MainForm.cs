using Nito.AsyncEx;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ZinRadioDesktop.Controls;

namespace ZinRadioDesktop
{
    public interface IDisplayCurrentStation
    {
        void UpdateCurrentStation(RadioStation station);
    }

    public partial class MainForm : Form, IDisplayCurrentStation
    {
        AudioVisualizerControl _audioVisualizerControl;
        public ChangeStation _changeStationScreen;
        public string Theme { get; set; } = "Light";

        public RadioAnimator RadioStationLogo;
        ZinMenuBarControl MainMenuBarControl;

        public MainForm()
        {
            InitializeComponent();

            _audioVisualizerControl = new AudioVisualizerControl();
            _audioVisualizerControl.Top = ChannelNameArea.Bottom;
            _audioVisualizerControl.Width = ClientSize.Width;
            _audioVisualizerControl.Height = ClientSize.Height - ChannelNameArea.Bottom;
            Controls.Add(_audioVisualizerControl);

            RadioStationLogo = new RadioAnimator(BackColor);
            RadioStationLogo.Location = new Point(ClientSize.Width / 2 - RadioStationLogo.Width / 2, ClientSize.Height / 2 - RadioStationLogo.Height / 2);
            Controls.Add(RadioStationLogo);

            SetupMenuStrip();

            _changeStationScreen = new ChangeStation(this, this);
        }

        [MemberNotNull(nameof(MainMenuBarControl))]
        private void SetupMenuStrip()
        {
            MainMenuBarControl = new ZinMenuBarControl();
            MainMenuBarControl.Width = this.ClientSize.Width;
            MainMenuBarControl.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);

            this.Controls.Add(MainMenuBarControl);
            ZinMenuButton ZMB = new ZinMenuButton("Change Station", ButtonType.Main, MainMenuBarControl);
            ZinMenuButton ZMB2 = new ZinMenuButton("Exit", ButtonType.Secondary, MainMenuBarControl);
            ZinMenuBarControl.Items[0].Click += ShowChangeStationDialog;
            ZinMenuBarControl.Items[1].Click += (o, i) => { Close(); };

            MainMenuBarControl.BringToFront();
        }

        // Detect WindowState Change
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MINIMIZE = 0xf020;
        private const int SC_MAXIMIZE = 0xF030;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam.ToInt32() == SC_MINIMIZE)
                {
                    ShrinkWindow();
                    m.Result = IntPtr.Zero;
                    return;
                }
                else if (m.WParam.ToInt32() == SC_MAXIMIZE)
                {
                    EnlargeWindow();
                    m.Result = IntPtr.Zero;
                    return;
                }
            }

            base.WndProc(ref m);
        }

        private readonly object _windowEnlargementLock = new();

        private void EnlargeWindow()
        {
            WindowOrchestrator.AnimateOpacity(this, 0, new Action(() =>
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.MinimumSize = new Size(370, 420);
                this.ClientSize = new Size(370, 420);

                this.Location = new Point(SystemInformation.WorkingArea.Width / 2 - 370 / 2, SystemInformation.WorkingArea.Height / 2 - 420 / 2);
                this.MinimizeBox = true;
                this.MaximizeBox = false;
                this.TopMost = false;

                WindowOrchestrator.AnimateOpacity(this, 1);
            }), _windowEnlargementLock);
        }

        private void ShrinkWindow()
        {
            WindowOrchestrator.AnimateOpacity(this, 0, new Action(() =>
            {
                this.FormBorderStyle = FormBorderStyle.Fixed3D;
                this.MinimumSize = Size.Empty;
                this.ClientSize = new Size(370, 80);

                this.Location = new Point(SystemInformation.WorkingArea.Width - this.Size.Width,
                                          SystemInformation.WorkingArea.Height - this.Size.Height);
                this.MinimizeBox = false;
                this.MaximizeBox = true;

                WindowOrchestrator.AnimateOpacity(this, 1);
            }), _windowEnlargementLock);
        }

        private async void PlayButton_Click(object sender, EventArgs e)
        {
            if (WebAudioPlayer.Instance.AudioPlaybackState == WebAudioPlayer.PlaybackState.Playing)
            {
                await WebAudioPlayer.Instance.PauseAsync();
            }
            else if (WebAudioPlayer.Instance.AudioPlaybackState == WebAudioPlayer.PlaybackState.Paused)
            {
                await WebAudioPlayer.Instance.ResumeAsync();
            }

            UpdateThemedControls(WebAudioPlayer.Instance.AudioPlaybackState == WebAudioPlayer.PlaybackState.Playing);
        }

        private readonly AsyncLock _showStationLocker = new();
        private readonly AsyncLock _showStationAnimationLocker = new();

        private async void ShowChangeStationDialog(object? sender, EventArgs e)
        {
            using (await _showStationLocker.LockAsync())
            {
                _changeStationScreen.Top = this.Top + (this.Height / 2) - (_changeStationScreen.Height / 2);
                _changeStationScreen.Left = this.Left < 8 ? 8 : this.Left;

                // If the secondary screen is over the screen width.
                if (_changeStationScreen.Right + 8 > Screen.PrimaryScreen.WorkingArea.Width)
                {
                    _changeStationScreen.Left = Screen.PrimaryScreen.WorkingArea.Width - _changeStationScreen.Width - 8;
                    this.Left = _changeStationScreen.Left;
                }

                // If the secondary screen is over the screen height.
                if (_changeStationScreen.Bottom + 8 > Screen.PrimaryScreen.WorkingArea.Height)
                {
                    _changeStationScreen.Top = Screen.PrimaryScreen.WorkingArea.Height - _changeStationScreen.Height - 8;
                }

                // If the main screen bottom is below the screen height.
                if (this.Bottom + 8 > Screen.PrimaryScreen.WorkingArea.Height)
                {
                    this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 8;
                }

                // If the secondary screen is over the screen Top.
                if (_changeStationScreen.Top + 8 < 0)
                {
                    this.Top = 8;
                    _changeStationScreen.Top = 8;
                }

                int newMainFormX = _changeStationScreen.Left - this.Width - 8;
                if (newMainFormX < 8) { newMainFormX = 8; }
                await WindowOrchestrator.SlideFormX(this, newMainFormX, _showStationAnimationLocker);

                this.TopMost = true;
                _changeStationScreen.ShowDialog(this);
                this.TopMost = false;

                await WindowOrchestrator.SlideFormX(this, _changeStationScreen.Left, _showStationAnimationLocker);
            }
        }

        public void UpdateCurrentStation(RadioStation station)
        {
            RadioStationLogo.Start();
            UpdateTitleText(station.Name);
            UpdateThemedControls(true);
        }

        private void UpdateTitleText(string text)
        {
            ChannelNameLabel.Text = text;
            ChannelNameLabel.Left = ChannelNameArea.Width / 2 - ChannelNameLabel.Width / 2;
        }

        /// <summary>
        /// Updates all the Colors for the Themed Controls
        /// </summary>
        public void UpdateThemedControls(bool playing)
        {
            if (playing)
            {
                RadioStationLogo.Start();
                if (Theme == "Light")
                    PlayButton.BackgroundImage = Properties.Resources.media_pause_8x;
                else if (Theme == "Dark")
                    PlayButton.BackgroundImage = Properties.Resources.media_pause_8x;
            }
            else
            {
                RadioStationLogo.Stop();
                if (Theme == "Light")
                    PlayButton.BackgroundImage = Properties.Resources.media_play_8x;
                else if (Theme == "Dark")
                    PlayButton.BackgroundImage = Properties.Resources.media_play_8x;
            }
        }

        //private void MainForm_Move(object sender, EventArgs e)
        //{
        //    //if (this.ContainsFocus)
        //    //{
        //    //    changeStationScreen.Left = this.Right + 8;
        //    //    changeStationScreen.Top = this.Top + (this.Height / 2) - (changeStationScreen.Height / 2);
        //    //}
        //}

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}