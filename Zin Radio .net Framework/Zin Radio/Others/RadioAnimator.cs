using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zin_Radio
{
    public class RadioAnimator : Control
    {
        Timer AnimationSync = new Timer();
        int AnimationStage = 0;

        Graphics g;
        Bitmap buffer = new Bitmap(128, 128);

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(MainForm.StaticForm.BackColor);
            e.Graphics.DrawImage(buffer, new Rectangle(0, 0, this.Size.Width, this.Size.Height));
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        public RadioAnimator()
        {
            this.Size = new Size(128, 128);
            this.Location = new Point(113, 123);
            this.Anchor = AnchorStyles.None;

            buffer = Properties.Resources.Radio_128_animation_2_dark;
            AnimationSync.Tick += AnimationSync_Tick;
            AnimationSync.Interval = 1000;

            this.Invalidate();
        }

        public void Start()
        {
            AnimationSync.Enabled = true;
        }

        public void Stop()
        {
            AnimationSync.Enabled = false;
            AnimationSync.Stop();
        }

        void AnimationSync_Tick(object sender, EventArgs e)
        {
            if (AnimationStage == 0)
            {
                buffer = Properties.Resources.Radio_128_animation_2_dark;
                AnimationStage = 1;
            }
            else if (AnimationStage == 1)
            {
                buffer = Properties.Resources.Radio_128_animation_1_dark;
                AnimationStage = 2;
            }
            else if (AnimationStage == 2)
            {
                buffer = Properties.Resources.Radio_128_dark;
                AnimationStage = 0;
            }

            this.Invalidate();
        }

    }
}
