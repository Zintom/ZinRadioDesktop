using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zin_Radio
{
    public class ZinMenuBarControl : Control
    {
        /// <summary>
        /// Do Not Use Items.Clear use ZinMenuBarControl.ClearAll() instead.
        /// </summary>
        public static List<ZinMenuButton> Items = new List<ZinMenuButton>();

        public void ClearAll()
        {
            // Remove All Items
            for (int i = 0; i < Items.Count; i++)
            {
                ZinMenuButton item = Items[i];
                this.Controls.Remove(item);
                item.Dispose();
            }
            Items.Clear();
        }

        public ZinMenuBarControl()
        {
            this.Size = new Size(400, 25);
            this.BackColor = Color.White;
            CreateDefault();
        }

        /// <summary>
        /// Generates the Default Menu
        /// </summary>
        public void CreateDefault()
        {
            //ZinMenuButton ZMB = new ZinMenuButton("FILE", ButtonType.Main, this);
            //ZinMenuButton ZMB2 = new ZinMenuButton("EDIT", ButtonType.Secondary, this);
            //ZinMenuButton ZMB3 = new ZinMenuButton("ABOUT", ButtonType.Secondary, this);

            //this.Controls.Add(ZMB);
            //this.Controls.Add(ZMB2);
            //this.Controls.Add(ZMB3);
        }

    }

    public enum ButtonType
    {
        /// <summary>
        /// Main specifies the Blue Button that is always at the begining of the menu.
        /// </summary>
        Main,
        /// <summary>
        /// Secondary specifies the other buttons next to the "Main" button.
        /// </summary>
        Secondary
    }
    public class ZinMenuButton : Button
    {
        public ContextMenuStrip cm = new ContextMenuStrip();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        /// <param name="parent">The ZinMenuBarControl Parent of this Button</param>
        public ZinMenuButton(string text, ButtonType type, ZinMenuBarControl parent)
        {
            // 
            // Create Button
            // 

            if (type == ButtonType.Main)
                CreateButtonMain(text);
            else
                CreateButtonOthers(text);

            parent.Controls.Add(this);
            this.MouseDown += ZinMenuButton_MouseDown;

            ZinMenuBarControl.Items.Add(this);
        }

        void ZinMenuButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                cm.Show(this, new Point(0, this.Height));
            }
        }

        public void CreateButtonMain(string text)
        {
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(100)))), ((int)(((byte)(178)))));
            this.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(100)))), ((int)(((byte)(178)))));
            this.FlatAppearance.BorderSize = 0;
            this.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(80)))), ((int)(((byte)(158)))));
            this.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(120)))), ((int)(((byte)(198)))));
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Location = new Point(0, 0);
            this.Name = text;
            this.Size = new System.Drawing.Size(120, 25);
            this.TabIndex = 0;
            this.Text = text;
            this.UseVisualStyleBackColor = false;
        }

        public void CreateButtonOthers(string text)
        {
            Point location = new Point(0, 0);
            // Add Up Locations
            for (int i = 0; i < ZinMenuBarControl.Items.Count; i++)
            {
                location.X += ZinMenuBarControl.Items[i].Width;
            }

            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(100)))), ((int)(((byte)(178)))));
            this.FlatAppearance.BorderSize = 0;
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Location = location;
            this.Name = text;
            this.Size = new System.Drawing.Size(76, 25);
            this.TabIndex = 1;
            this.Text = text;
            this.UseVisualStyleBackColor = false;
        }
    }
}