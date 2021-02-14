using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zin_Radio
{
    public partial class ExtendedListBox : ListBox
    {
        public int _MouseIndex = -1;
        public SolidBrush ItemBackground = new SolidBrush(Color.FromArgb(64, 64, 64));
        public SolidBrush HighLight = new SolidBrush(Color.FromArgb(150, 100, 149, 237));
        //public SolidBrush HighLight = new SolidBrush(Color.FromArgb(175, 41, 128, 185));
        public SolidBrush TextColour = new SolidBrush(Color.White);

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //}

        //protected override void OnPaintBackground(PaintEventArgs pevent)
        //{
        //}

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (Items.Count > 0)
            {
                //e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.;

                RadioStation station = (RadioStation)Items[e.Index];
                station.Bounds = e.Bounds;
                Items[e.Index] = station;

                // Draw Background
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    e.Graphics.FillRectangle(Brushes.CornflowerBlue, e.Bounds);
                else if (e.Index == _MouseIndex)
                    e.Graphics.FillRectangle(Brushes.DimGray, e.Bounds);
                else
                    e.Graphics.FillRectangle(ItemBackground, e.Bounds);

                // Draw seperator
                e.Graphics.DrawLine(Pens.DimGray, e.Bounds.X, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

                int textXOffset = 0;
                if (station.EditorsChoice)
                {
                    textXOffset += 8;
                    e.Graphics.FillRectangle(HighLight, new Rectangle(e.Bounds.X, e.Bounds.Y, 8, e.Bounds.Height));
                }

                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    // genre
                    e.Graphics.DrawString(station.Genre, new Font("Segoe UI", 8f, FontStyle.Bold), Brushes.White, new Point(e.Bounds.X + 6 + textXOffset, e.Bounds.Y + 24));
                }
                else
                {
                    // genre
                    e.Graphics.DrawString(station.Genre, new Font("Segoe UI", 8f, FontStyle.Bold), Brushes.Orange, new Point(e.Bounds.X + 6 + textXOffset, e.Bounds.Y + 24));
                }

                // Draw text items
                e.Graphics.DrawString(station.Name, new Font("Segoe UI", 12f, FontStyle.Regular), TextColour, new Point(e.Bounds.X + 4 + textXOffset, e.Bounds.Y + 2));

                //e.DrawFocusRectangle();
            }
        }

    }
}
