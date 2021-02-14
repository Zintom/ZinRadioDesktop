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

namespace Zin_Radio
{
    public partial class ChangeStation : Form
    {

        public bool linkWindows = false;

        #region Colour Brush Definitions
        public SolidBrush ItemBackground = new SolidBrush(Color.FromArgb(230, 230, 230));
        public SolidBrush SelectedColour = new SolidBrush(Color.FromArgb(240, 240, 240));

        public Font TextFont = new Font("Segoe UI", 12f, FontStyle.Regular);
        public Font SubTextFont = new Font("Segoe UI", 8f, FontStyle.Bold);
        public SolidBrush TextColour = new SolidBrush(Color.Black);
        public SolidBrush SubTextColour = new SolidBrush(Color.FromArgb(251, 96, 80));

        public SolidBrush EditorsChoiceColour = new SolidBrush(Color.FromArgb(255, 100, 149, 237));
        #endregion

        [DllImport("user32.dll")]
        static extern bool LockWindowUpdate(IntPtr hWndLock);

        public ChangeStation()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        private void ChangeStation_Load(object sender, EventArgs e)
        {
            Application.DoEvents();
            WaitAndHide();

            string cache = Properties.Settings.Default.StationCache;
            if (cache != "")
            {
                ApplyStations(cache);
                Console.WriteLine("Applied station cache.");
            }
            else
                Console.WriteLine("No station cache.");

            SyncStations();
        }

        public Task WaitAndHide()
        {
            return Task.Run(() =>
            {
                this.Opacity = 0;
                Thread.Sleep(100);
                this.Hide();
            });
        }

        public Task SyncStations()
        {
            return Task.Run(() =>
            {
                List<RadioStation> stations = new List<RadioStation>();
                using (WebClient web = new WebClient())
                {
                    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    web.Encoding = Encoding.UTF8;
                    web.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    string raw = web.DownloadString(Program.WebsiteHost + "/zinradio/stations.txt?cache=" + unixTimestamp.ToString());

                    // Store in cache
                    Properties.Settings.Default.StationCache = raw;
                    Properties.Settings.Default.Save();

                    // Process new data.
                    ApplyStations(raw);

                    Console.WriteLine("Synced stations.");
                }
            });
        }

        public void ApplyStations(string rawData)
        {
            string[] rawStations = rawData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<RadioStation> stations = new List<RadioStation>();
            for (int i = 0; i < rawStations.Length; i++)
            {
                stations.Add(RadioStation.FromString(rawStations[i]));
            }

            // Push loaded stations into global scope.
            //Stations = stations;
            stations.Sort(new StationSort());
            //stationListBox.Items.AddRange(stations.ToArray());
            zinListView1.Items.Clear();
            zinListView1.AddRange(stations.ToArray());

            zinListView1.Invalidate();
        }

        public Task ShowMe()
        {
            return Task.Run(() =>
            {
                if (MainForm.CurrentStation != null)
                    zinListView1.Selected = zinListView1.Items.IndexOf(MainForm.CurrentStation);

                BringToFront();

                while (Opacity < 1)
                {
                    Opacity += 0.1;
                    Thread.Sleep(8);
                }
            });
        }

        public Task HideMe()
        {
            return Task.Run(() =>
            {
                MainForm.StaticForm.linkWindows = false;
                this.Hide();

                Thread.Sleep(150);

                if (MainForm.StaticForm.formClosing)
                {
                    MainForm.StaticForm.Close();
                }

                this.Opacity = 0;
                MainForm.StaticForm.slideRight();
                ResetSearch();
            });
        }

        private void ChangeStation_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!MainForm.StaticForm.formClosing)
            {
                e.Cancel = true;
                HideMe();
            }
        }

        private void zinListView1_ItemClicked(int index)
        {
            RadioStation selected = zinListView1.Items[index];
            if (MainForm.CurrentStation == null || selected.URL != MainForm.CurrentStation.URL)
            {
                MainForm.CurrentStation = selected;
                MainForm.StaticForm.UpdateTitleText();
                MainForm.RadioStationLogo.Start();

                WebStreamPlayer.PlayStream(selected.URL);
                MainForm.StaticForm.UpdateThemedControls(true);
                HideMe();
            }
            else
            {
                HideMe();
            }
        }

        private void zinListView1_DrawItem(DrawItemState state, Graphics g, int index, Rectangle bounds)
        {
            if (zinListView1.Items.Count > 0)
            {
                RadioStation station = (RadioStation)zinListView1.Items[index];

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

        private void textBox1_Enter(object sender, EventArgs e)
        {
            searchBox.Text = null;
            searchBox.ForeColor = Color.Black;
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            if (searchBox.Text != "")
            {
                zinListView1.Items.Sort(new StationSortSearch(searchBox.Text));
                zinListView1.Invalidate();
            }
            else
            {
                zinListView1.Items.Sort(new StationSort());
                zinListView1.Invalidate();
            }
        }

        private void searchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                if (searchBox.Text == "")
                {
                    zinListView1.Select();
                    zinListView1.Focus();
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
            zinListView1.Select();
            zinListView1.Focus();
            searchBox.Text = "Search for a station";
            searchBox.ForeColor = Color.DimGray;

            zinListView1.Items.Sort(new StationSort());
            zinListView1.Invalidate();
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
            if (linkWindows && this.ContainsFocus)
            {
                MainForm.StaticForm.Left = this.Left - MainForm.StaticForm.Width - 8;
                MainForm.StaticForm.Top = this.Top + (this.Height / 2) - (MainForm.StaticForm.Height / 2);
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
            return new RadioStation(raw[0], raw[1], raw[2], raw[3] == "t" ? true : false, false);
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