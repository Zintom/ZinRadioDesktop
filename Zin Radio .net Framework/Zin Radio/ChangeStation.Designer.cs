namespace Zin_Radio
{
    partial class ChangeStation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.MediaURL = new System.Windows.Forms.TextBox();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.title = new Zin_Radio.TransparentLabel();
            this.zinListView1 = new Zin_Radio.ZinListView();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 20F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(56, 161);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Change Media Link";
            this.label1.Visible = false;
            // 
            // MediaURL
            // 
            this.MediaURL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.MediaURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MediaURL.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MediaURL.ForeColor = System.Drawing.Color.White;
            this.MediaURL.Location = new System.Drawing.Point(85, 223);
            this.MediaURL.Name = "MediaURL";
            this.MediaURL.Size = new System.Drawing.Size(161, 29);
            this.MediaURL.TabIndex = 1;
            this.MediaURL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MediaURL.Visible = false;
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchBox.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBox.ForeColor = System.Drawing.Color.DimGray;
            this.searchBox.Location = new System.Drawing.Point(-2, 74);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(384, 26);
            this.searchBox.TabIndex = 3;
            this.searchBox.Text = "Search for a station";
            this.searchBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            this.searchBox.Enter += new System.EventHandler(this.textBox1_Enter);
            this.searchBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.searchBox_KeyPress);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pictureBox1.Location = new System.Drawing.Point(0, 66);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(384, 42);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // title
            // 
            this.title.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.title.Font = new System.Drawing.Font("Segoe UI Light", 20F);
            this.title.ForeColor = System.Drawing.Color.White;
            this.title.Location = new System.Drawing.Point(143, 14);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(104, 37);
            this.title.TabIndex = 0;
            this.title.Text = "Stations";
            // 
            // zinListView1
            // 
            this.zinListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zinListView1.BackColor = System.Drawing.Color.White;
            this.zinListView1.Location = new System.Drawing.Point(0, 108);
            this.zinListView1.Name = "zinListView1";
            this.zinListView1.RowDistance = 12;
            this.zinListView1.RowHeight = 44;
            this.zinListView1.RowWidthOffset = 40;
            this.zinListView1.Selected = -1;
            this.zinListView1.Size = new System.Drawing.Size(384, 516);
            this.zinListView1.TabIndex = 2;
            this.zinListView1.Text = "zinListView1";
            this.zinListView1.DrawItem += new Zin_Radio.ZinListView.DrawItemEvent(this.zinListView1_DrawItem);
            this.zinListView1.ItemClicked += new Zin_Radio.ZinListView.ItemClickedEvent(this.zinListView1_ItemClicked);
            // 
            // ChangeStation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(100)))), ((int)(((byte)(178)))));
            this.ClientSize = new System.Drawing.Size(384, 624);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.title);
            this.Controls.Add(this.zinListView1);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeStation";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Change Station";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChangeStation_FormClosing);
            this.Load += new System.EventHandler(this.ChangeStation_Load);
            this.Move += new System.EventHandler(this.ChangeStation_Move);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox MediaURL;
        private TransparentLabel title;
        private ZinListView zinListView1;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}