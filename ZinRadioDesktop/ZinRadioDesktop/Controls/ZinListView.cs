using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ZinRadioDesktop.Controls
{
    public class ZinListView : Control
    {

        public List<RadioStation> Items = new List<RadioStation>();

        public void Add(RadioStation item)
        {
            Items.Add(item);
            this.Invalidate();

            scrollBar.Maximum = Items.Count;
        }

        //
        // Summary:
        //     Adds an array of items to the list of items for a System.Windows.Forms.ListBox.
        //
        // Parameters:
        //   items:
        //     An array of objects to add to the list.
        public void AddRange(RadioStation[] range)
        {
            for (int i = 0; i < range.Length; i++)
            {
                Items.Add(range[i]);
            }
            this.Invalidate();

            scrollBar.Maximum = Items.Count;
        }

        public int rowWidthOffset = 0;
        /// <summary>
        /// Will pull all items in by this many pixels each side.
        /// </summary>
        public virtual int RowWidthOffset { get { return rowWidthOffset; } set { rowWidthOffset = value; } }

        public int rowDistance = 0;
        public virtual int RowDistance { get { return rowDistance; } set { rowDistance = value; } }

        public int rowHeight = 32;
        public virtual int RowHeight { get { return rowHeight; } set { rowHeight = value; } }

        public delegate void DrawItemEvent(DrawItemState state, Graphics g, int index, Rectangle bounds);
        /// <summary>
        /// Occurs when an item is ready to be drawn.
        /// </summary>
        public event DrawItemEvent DrawItem;
        public delegate void ItemClickedEvent(int index);
        /// <summary>
        /// Occurs when an item is clicked.
        /// </summary>
        public event ItemClickedEvent ItemClicked;

        private int selected = -1;
        [Browsable(false)]
        public int Selected { get { return selected; } set { selected = value; } }
        private int scrollAmount = 0;

        // Graphics
        Bitmap buffer;
        Graphics graphics;

        VScrollBar scrollBar;

        public ZinListView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            Resize += _Resize;
            MouseWheel += _MouseWheel;
            MouseClick += _Click;
            _Resize(null, null);


            scrollBar = new VScrollBar();
            scrollBar.Scroll += _BarScroll;
            scrollBar.Left = this.Width - scrollBar.Width + 1;
            scrollBar.Height = this.Height;
            this.Controls.Add(scrollBar);
        }

        public virtual void OnDrawItem(DrawItemState state, Graphics g, int index, Rectangle bounds)
        {
            g.DrawString(Items[index].ToString(), Font, new SolidBrush(ForeColor), bounds.Location);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (graphics == null)
                return;

            graphics.Clear(BackColor);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Draw items via user-code
            if (DrawItem != null)
            {
                if (Items != null)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        int yPos = (i * (rowHeight + rowDistance)) + scrollAmount + rowDistance;
                        if (yPos >= -rowHeight && yPos < this.Height)
                            DrawItem?.Invoke(i == selected ? DrawItemState.Selected : DrawItemState.Default, graphics, i, GetItemBounds(i));
                    }
                }
            }
            else // Draw default items
            {
                if (Items != null)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        int yPos = (i * (rowHeight + rowDistance)) + scrollAmount + rowDistance;
                        if (yPos >= -rowHeight && yPos < this.Height)
                            OnDrawItem(i == selected ? DrawItemState.Selected : DrawItemState.Default, graphics, i, GetItemBounds(i));
                    }
                }
            }

            e.Graphics.DrawImageUnscaled(buffer, Point.Empty);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        private void _Click(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                //int yPos = (i * (rowHeight + rowDistance)) + scrollAmount + rowDistance;
                //Rectangle itemBounds = new Rectangle(rowWidthOffset, yPos, buffer.Width - scrollBar.Width - (rowWidthOffset * 2), rowHeight);
                if (GetItemBounds(i).Contains(e.Location))
                {
                    selected = i;
                    Invalidate();
                    Application.DoEvents();
                    ItemClicked?.Invoke(i);
                    return;
                }
            }
        }

        private Rectangle GetItemBounds(int index)
        {
            int yPos = (index * (rowHeight + rowDistance)) + scrollAmount + rowDistance;
            return new Rectangle(rowWidthOffset, yPos, buffer.Width - scrollBar.Width - (rowWidthOffset * 2), rowHeight);
        }

        private void _MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                // Scroll up
                if (scrollAmount < 0)
                {
                    scrollAmount += 28;
                    Invalidate();
                }
            }
            else if (e.Delta < 0)
            {
                // Scroll down
                if (Math.Abs(scrollAmount) < Items.Count * (rowHeight + rowDistance) - this.Height)
                {
                    scrollAmount -= 28;
                    Invalidate();
                }
            }
        }

        private void _BarScroll(object sender, ScrollEventArgs e)
        {
            scrollAmount = -(e.NewValue * (rowHeight + rowDistance));
            Invalidate();
        }

        private void _Resize(object sender, EventArgs e)
        {
            if (ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                buffer = new Bitmap(ClientSize.Width, ClientSize.Height);
                graphics = Graphics.FromImage(buffer);
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                scrollBar.Left = this.Width - scrollBar.Width + 1;
                scrollBar.Height = buffer.Height;

                this.Invalidate();
            }
        }

        /// <summary>
        /// Calculate maximum items on screen
        /// </summary>
        /// <returns></returns>
        private int MaxItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (i * (rowHeight + rowDistance) > this.Height)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}