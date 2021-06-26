using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZinRadioDesktop
{
    public partial class ChangeStation : Form, ILinkWindowClient
    {

        private RadioStation? _currentStation;
        private readonly IDisplayCurrentStation _displayer;
        private readonly ILinkWindowHost _linkWindowHost;

        #region Colour Brush Definitions
        public SolidBrush ItemBackground = new SolidBrush(Color.FromArgb(230, 230, 230));
        public SolidBrush SelectedColour = new SolidBrush(Color.FromArgb(240, 240, 240));

        public Font TextFont = new Font("Segoe UI", 12f, FontStyle.Regular);
        public Font SubTextFont = new Font("Segoe UI", 8f, FontStyle.Bold);
        public SolidBrush TextColour = new SolidBrush(Color.Black);
        public SolidBrush SubTextColour = new SolidBrush(Color.FromArgb(251, 96, 80));

        public SolidBrush EditorsChoiceColour = new SolidBrush(Color.FromArgb(255, 100, 149, 237));
        #endregion

        private ChangeStation(IDisplayCurrentStation displayer, ILinkWindowHost linkWindowHost)
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            _displayer = displayer;
            _linkWindowHost = linkWindowHost;
        }

        public static async Task<ChangeStation> CreateAsync(IDisplayCurrentStation displayer, ILinkWindowHost linkWindowHost)
        {
            ChangeStation form = new ChangeStation(displayer, linkWindowHost);

            // Load stations.
            string stationList = await GetStationList();

            // Create the list.
            form.ApplyStations(stationList);

            return form;
        }

        private void ChangeStation_Load(object sender, EventArgs e)
        {
            //Application.DoEvents();

            // Redo entire caching system.
            //string cache = Properties.Settings.Default.StationCache;
            //if (cache != "")
            //{
            //    ApplyStations(cache);
            //    Console.WriteLine("Applied station cache.");
            //}
            //else
            //    Console.WriteLine("No station cache.");
        }

        private static async Task<string> GetStationList()
        {
            string raw = await NetworkHelper.DefaultHttpClient.GetStringAsync(Program.StationListUrl);

            // Store in cache
            Properties.Settings.Default.StationCache = raw;
            Properties.Settings.Default.Save();

            return raw;
        }

        private void ApplyStations(string rawData)
        {
            string[] rawStations = rawData.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            List<RadioStation> stations = new List<RadioStation>();
            for (int i = 0; i < rawStations.Length; i++)
            {
                stations.Add(RadioStation.FromString(rawStations[i]));
            }

            stations.Sort(new StationSort());
            stationListView.Items.Clear();
            stationListView.AddRange(stations.ToArray());

            stationListView.Invalidate();
        }

        public void ShowMe()
        {
            new Thread(new ThreadStart(() =>
            {
                if (_currentStation != null)
                    stationListView.Selected = stationListView.Items.IndexOf(_currentStation);

                BringToFront();

                while (Opacity < 1)
                {
                    Opacity += 0.1;
                    Thread.Sleep(8);
                }
            }))
            {
                Priority = ThreadPriority.BelowNormal
            }.Start();
        }

        public void HideMe()
        {
            _linkWindowHost.LinkWindowsOn = false;
            this.Hide();
            this.Opacity = 0;

            _linkWindowHost.ResumePosition();
            ResetSearch();
        }

        private void ChangeStation_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            HideMe();
        }

        private async void StationListView_ItemClicked(int index)
        {
            RadioStation selected = stationListView.Items[index];
            if (_currentStation == null || selected.URL != _currentStation.URL)
            {
                _currentStation = selected;
                _displayer.UpdateCurrentStation(selected);

                HideMe();

                await WebAudioPlayer.Instance.Stop();
                WebAudioPlayer.Instance.Play(selected.URL);
            }
            else
            {
                HideMe();
            }
        }

        private void StationListView_DrawItem(DrawItemState state, Graphics g, int index, Rectangle bounds)
        {
            if (stationListView.Items.Count > 0)
            {
                RadioStation station = (RadioStation)stationListView.Items[index];

                // Draw Background
                if (state == DrawItemState.Selected)
                    g.FillRectangle(SelectedColour, bounds);
                else
                    g.FillRectangle(ItemBackground, bounds);

                int textXOffset = 0;
                if (station.EditorsChoice)
                {
                    textXOffset += 4;
                    g.FillRectangle(EditorsChoiceColour, new Rectangle(bounds.X, bounds.Y, 4, bounds.Height));
                }

                SizeF s = g.MeasureString(station.Genre, SubTextFont);
                g.DrawString(station.Genre, SubTextFont, SubTextColour, new Point((bounds.X + bounds.Width / 2) - (int)(s.Width / 2), bounds.Y + 25));

                // Draw text items
                s = g.MeasureString(station.Name, TextFont);
                g.DrawString(station.Name, TextFont, TextColour, new Point((bounds.X + bounds.Width / 2) - (int)(s.Width / 2), bounds.Y + 2));

                if (!station.ValidSearch)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(150, 255, 255, 255)), bounds);
            }
        }

        private void SearchBox_Enter(object sender, EventArgs e)
        {
            // Clear the search box text Hint Text.
            searchBox.Text = null;
            searchBox.ForeColor = Color.Black;
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            if (searchBox.Text != "")
            {
                stationListView.Items.Sort(new StationSortSearch(searchBox.Text));
                stationListView.Invalidate();
            }
            else
            {
                stationListView.Items.Sort(new StationSort());
                stationListView.Invalidate();
            }
        }

        private void SearchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                if (searchBox.Text == "")
                {
                    stationListView.Select();
                    stationListView.Focus();
                    searchBox.Text = "Search for a station";
                    searchBox.ForeColor = Color.DimGray;
                }
            }
            //else if (e.KeyChar == (char)Keys.Back)
            //{
            //    string t = searchBox.Text;

            //    if (t.Length > 0)
            //    {
            //        t = t.Substring(0, t.Length - 1);
            //    }

            //    if (t == "")
            //    {
            //        zinListView1.GetItems().Sort(new StationSort());
            //        zinListView1.Invalidate();
            //    }
            //}
        }

        public void ResetSearch()
        {
            stationListView.Select();
            stationListView.Focus();
            searchBox.Text = "Search for a station";
            searchBox.ForeColor = Color.DimGray;

            stationListView.Items.Sort(new StationSort());
            stationListView.Invalidate();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var parms = base.CreateParams;
                parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
                return parms;
            }
        }

        private void ChangeStation_Move(object sender, EventArgs e)
        {
            if (_linkWindowHost.LinkWindowsOn && this.ContainsFocus)
            {
                _linkWindowHost.Left = this.Left - _linkWindowHost.Width - 8;
                _linkWindowHost.Top = this.Top + (this.Height / 2) - (_linkWindowHost.Height / 2);
            }
        }

        //const int WM_SYSCOMMAND = 0x0112;
        //const int WM_NCMOUSEMOVE = 160;
        //const int SC_MOVE = 0xF010;
        //protected override void WndProc(ref Message m)
        //{
        //    Console.WriteLine(m.Msg);
        //    switch (m.Msg)
        //    {
        //        case WM_NCMOUSEMOVE:
        //            if (this.Bottom + 8 > Screen.PrimaryScreen.WorkingArea.Height)
        //            {
        //                this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 8;
        //            }
        //            if (this.Right + 8 > Screen.PrimaryScreen.WorkingArea.Width + 8)
        //            {
        //                this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width - 8;
        //            }
        //            break;
        //    }

        //    base.WndProc(ref m);
        //}
    }

    public class RadioStation
    {
        public Rectangle Bounds = Rectangle.Empty;

        public string Name = "";
        public string URL = "";
        public string Genre = "";
        public bool Favorite = false;
        public bool EditorsChoice = false;

        public bool ValidSearch = true;

        public RadioStation(string name, string url, string genre, bool editorsChoice, bool favorite)
        {
            Name = name;
            URL = url;
            Genre = genre;
            Favorite = favorite;
            EditorsChoice = editorsChoice;
        }

        public static RadioStation FromString(string data)
        {
            string[] raw = data.Split(new string[] { "\"," }, StringSplitOptions.RemoveEmptyEntries);
            return new RadioStation(raw[0], raw[1], raw[2], raw[3] == "t", false);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class StationSort : Comparer<RadioStation>
    {
        public override int Compare(RadioStation x, RadioStation y)
        {
            x.ValidSearch = true;
            y.ValidSearch = true;

            int compare = y.EditorsChoice.CompareTo(x.EditorsChoice);
            if (compare == 0)
            {
                return x.Name.CompareTo(y.Name);
            }

            return compare;
        }
    }

    public class StationSortSearch : Comparer<RadioStation>
    {
        private string Search = "";
        public StationSortSearch(string search)
        {
            Search = search.ToLower().Trim();
        }

        public override int Compare(RadioStation x, RadioStation y)
        {
            x.ValidSearch = x.Name.Trim().ToLower().StartsWith(Search);
            y.ValidSearch = y.Name.Trim().ToLower().StartsWith(Search);

            int compare = y.ValidSearch.CompareTo(x.ValidSearch);

            if(compare == 0)
            {
                return x.Name.Length.CompareTo(y.Name.Length);
            }

            return compare;
        }
    }

}