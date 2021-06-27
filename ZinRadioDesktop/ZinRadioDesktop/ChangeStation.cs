using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZinRadioDesktop
{
    public partial class ChangeStation : Form
    {
        private Form _mainForm;
        private bool _linkWindows = false;

        private RadioStation? _currentStation;
        private readonly IDisplayCurrentStation _displayer;

        #region Colour Brush Definitions
        public SolidBrush ItemBackground = new SolidBrush(Color.FromArgb(230, 230, 230));
        public SolidBrush SelectedColour = new SolidBrush(Color.FromArgb(240, 240, 240));

        public Font TextFont = new Font("Segoe UI", 12f, FontStyle.Regular);
        public Font SubTextFont = new Font("Segoe UI", 8f, FontStyle.Bold);
        public SolidBrush TextColour = new SolidBrush(Color.Black);
        public SolidBrush SubTextColour = new SolidBrush(Color.FromArgb(251, 96, 80));

        public SolidBrush EditorsChoiceColour = new SolidBrush(Color.FromArgb(255, 100, 149, 237));
        #endregion

        public ChangeStation(IDisplayCurrentStation displayer, Form mainForm)
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            _displayer = displayer;
            _mainForm = mainForm;

            GetStationList().ContinueWith((stationList) => this.SafeInvokeIfRequired(() => ApplyStations(stationList.Result)), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void ChangeStation_Load(object sender, EventArgs e)
        {
            AnimateShow();

            // Redo entire caching system.
            //string cache = Properties.Settings.Default.StationCache;
            //if (cache != "")
            //{
            //    ApplyStations(cache);
            //    Console.WriteLine("Applied station cache.");
            //}
            //else
            //    Console.WriteLine("No station cache.");

            ResetSearch();

            if (_currentStation != null)
                stationListView.Selected = stationListView.Items.IndexOf(_currentStation);
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

        private void ChangeStation_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            AnimateHide();
        }

        public void AnimateShow()
        {
            this.BringToFront();

            WindowOrchestrator.AnimateOpacity(this, 1, new Action(() => { _linkWindows = true; }));
        }

        public void AnimateHide()
        {
            _linkWindows = false;
            this.Hide();
            this.Opacity = 0;
        }

        private async void StationListView_ItemClicked(int index)
        {
            RadioStation selected = stationListView.Items[index];
            if (_currentStation == null || selected.URL != _currentStation.URL)
            {
                _currentStation = selected;
                _displayer.UpdateCurrentStation(selected);

                AnimateHide();

                await WebAudioPlayer.Instance.Stop();
                WebAudioPlayer.Instance.Play(selected.URL);
            }
            else
            {
                AnimateHide();
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
            if (_linkWindows && this.ContainsFocus)
            {
                _mainForm.Left = this.Left - _mainForm.Width - 8;
                _mainForm.Top = this.Top + (this.Height / 2) - (_mainForm.Height / 2);
            }
        }
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