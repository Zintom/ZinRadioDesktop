using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zin_Radio
{
    public partial class MainForm : Form
    {
        public static RadioStation CurrentStation = null;

        public static MainForm StaticForm;
        public static ChangeStation changeStationScreen;
        public static string Theme { get; set; }

        public bool formClosing = false;
        public bool linkWindows = false;

        public static RadioAnimator RadioStationLogo;
        ZinMenuBarControl MainMenuBarControl;

        [DllImport("user32.dll")]
        static extern bool LockWindowUpdate(IntPtr hWndLock);

        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            SetupMenuStrip();
            Theme = "Light";

            RadioStationLogo = new RadioAnimator();
            this.Controls.Add(RadioStationLogo);

            // Change Form Icon
            this.Icon = Properties.Resources.radio;

            StaticForm = this;

            changeStationScreen = new ChangeStation();
            changeStationScreen.Show(this);
        }

        public void SetupMenuStrip()
        {
            MainMenuBarControl = new ZinMenuBarControl();
            MainMenuBarControl.Width = this.ClientSize.Width;
            MainMenuBarControl.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);

            this.Controls.Add(MainMenuBarControl);
            ZinMenuButton ZMB = new ZinMenuButton("Change Station", ButtonType.Main, MainMenuBarControl);
            ZinMenuButton ZMB2 = new ZinMenuButton("Exit", ButtonType.Secondary, MainMenuBarControl);
            ZinMenuBarControl.Items[0].Click += Menu_ChangeStation_Click;
            ZinMenuBarControl.Items[1].Click += (o, i) => { Close(); };

            MainMenuBarControl.BringToFront();
        }

        void Menu_ChangeStation_Click(object sender, EventArgs e)
        {
            ChangeStationDialog();
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

        public Task EnlargeWindow()
        {
            return Task.Run(() =>
            {
                //
                // Animation Event
                //

                while (this.Opacity > 0)
                {
                    this.Opacity -= 0.05;

                    Thread.Sleep(5);
                }

                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.MinimumSize = new Size(370, 420);
                this.ClientSize = new Size(370, 420);

                this.Location = new Point(SystemInformation.WorkingArea.Width / 2 - 370 / 2, SystemInformation.WorkingArea.Height / 2 - 420 / 2);
                this.MinimizeBox = true;
                this.MaximizeBox = false;
                this.TopMost = false;

                while (this.Opacity < 1)
                {
                    this.Opacity += 0.05;

                    Thread.Sleep(5);
                }
            });
        }
        public Task ShrinkWindow()
        {
            return Task.Run(() =>
            {
                //
                // Animation Event
                //

                this.WindowState = FormWindowState.Normal;

                Thread.Sleep(50);

                //this.Opacity = 0;
                while (this.Opacity > 0)
                {
                    this.Opacity -= 0.05;

                    Thread.Sleep(5);
                }

                Application.DoEvents();
                Application.DoEvents();

                this.FormBorderStyle = FormBorderStyle.Fixed3D;
                this.MinimumSize = Size.Empty;
                this.ClientSize = new Size(370, 71);

                this.Location = new Point(SystemInformation.WorkingArea.Width - this.Size.Width,
                                          SystemInformation.WorkingArea.Height - this.Size.Height);
                this.MinimizeBox = false;
                this.MaximizeBox = true;
                //this.TopMost = true;

                Application.DoEvents();
                Application.DoEvents();

                while (this.Opacity < 1)
                {
                    this.Opacity += 0.05;

                    Thread.Sleep(5);
                }
            });
        }

        //
        // Play / Pause the Stream
        //
        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (WebStreamPlayer.WebAudioPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                WebStreamPlayer.WebAudioPlayer.controls.pause();

                AutoUpdateThemedControls();
            }
            else if (WebStreamPlayer.WebAudioPlayer.playState == WMPLib.WMPPlayState.wmppsPaused)
            {
                WebStreamPlayer.WebAudioPlayer.controls.play();

                AutoUpdateThemedControls();
            }
        }

        public void UpdateTitleText()
        {
            ChannelNameLabel.Text = CurrentStation.Name;
            ChannelNameLabel.Left = ChannelNameArea.Width / 2 - ChannelNameLabel.Width / 2;
        }

        // Change Channel Triggers
        private void ChangeChannelButton_Click(object sender, EventArgs e)
        {
            ChangeStationDialog();
        }
        private void ChannelNameLabel_Click(object sender, EventArgs e)
        {
            ChangeStationDialog();
        }
        private void ChannelNameArea_Click(object sender, EventArgs e)
        {
            ChangeStationDialog();
        }

        /// <summary>
        /// Displays a Dialog to change the station
        /// </summary>
        public Task ChangeStationDialog()
        {
            return Task.Run(() =>
            {
                if (changeStationScreen.Opacity < 1 && !changeStationScreen.Visible)
                {
                    changeStationScreen.Top = this.Top + (this.Height / 2) - (changeStationScreen.Height / 2);
                    changeStationScreen.Left = this.Left < 8 ? 8 : this.Left;

                    // If the station screen is over the screen edge width.
                    if (changeStationScreen.Right + 8 > Screen.PrimaryScreen.WorkingArea.Width)
                    {
                        changeStationScreen.Left = Screen.PrimaryScreen.WorkingArea.Width - changeStationScreen.Width - 8;
                        this.Left = changeStationScreen.Left;
                    }

                    // If the station screen is over the screen edge height.
                    if (changeStationScreen.Bottom + 8 > Screen.PrimaryScreen.WorkingArea.Height)
                    {
                        changeStationScreen.Top = Screen.PrimaryScreen.WorkingArea.Height - changeStationScreen.Height - 8;
                    }

                    if (this.Bottom + 8 > Screen.PrimaryScreen.WorkingArea.Height)
                    {
                        this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 8;
                    }

                    // If the station screen is over the screen edge Y.
                    if (changeStationScreen.Top + 8 < 0)
                    {
                        this.Top = 8;
                        changeStationScreen.Top = 8;
                    }

                    changeStationScreen.Show();
                    Application.DoEvents();
                    changeStationScreen.ShowMe();

                    slideLeft();
                    MainForm.StaticForm.linkWindows = true;
                    changeStationScreen.linkWindows = true;
                }
            });
        }

        public void slideLeft()
        {
            double xOffset = this.Left;
            double target = this.Left - this.Width - 8;

            if (target < 8)
                target = 8;

            while (xOffset - 2 > target)
            {
                xOffset = Program.CosineInterpolate(xOffset, target, 0.25);
                this.Left = (int)xOffset;
                Thread.Sleep(8);
            }
            this.Left = (int)target;
        }

        public void slideRight()
        {
            changeStationScreen.linkWindows = false;

            double xOffset = this.Left;
            double target = changeStationScreen.Left;

            while (xOffset < target - 2)
            {
                xOffset = Program.CosineInterpolate(xOffset, target, 0.25);
                this.Left = (int)xOffset;
                Thread.Sleep(8);
            }
            this.Left = (int)target;
        }

        /// <summary>
        /// Updates all the Colors for the Themed Controls
        /// </summary>
        public void AutoUpdateThemedControls()
        {
            if (WebStreamPlayer.WebAudioPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                RadioStationLogo.Start();
                if (Theme == "Light")
                    PlayButton.BackgroundImage = Properties.Resources.pause_dark;
                else if (Theme == "Dark")
                    PlayButton.BackgroundImage = Properties.Resources.pause_light;
            }
            else
            {
                RadioStationLogo.Stop();
                if (Theme == "Light")
                    PlayButton.BackgroundImage = Properties.Resources.play_dark;
                else if (Theme == "Dark")
                    PlayButton.BackgroundImage = Properties.Resources.play_light;
            }
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
                    PlayButton.BackgroundImage = Properties.Resources.pause_dark;
                else if (Theme == "Dark")
                    PlayButton.BackgroundImage = Properties.Resources.pause_light;
            }
            else
            {
                RadioStationLogo.Stop();
                if (Theme == "Light")
                    PlayButton.BackgroundImage = Properties.Resources.play_dark;
                else if (Theme == "Dark")
                    PlayButton.BackgroundImage = Properties.Resources.play_light;
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            RadioStationLogo.Select();
            RadioStationLogo.Focus();
        }

        private void MainForm_Move(object sender, EventArgs e)
        {
            if (linkWindows && this.ContainsFocus)
            {
                changeStationScreen.Left = this.Right + 8;
                changeStationScreen.Top = this.Top + (this.Height / 2) - (changeStationScreen.Height / 2);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            formClosing = true;
        }
    }
}