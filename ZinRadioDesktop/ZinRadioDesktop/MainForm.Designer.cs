namespace ZinRadioDesktop
{
    partial class MainForm
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
            this.PlayButton = new System.Windows.Forms.Button();
            this.ChannelNameArea = new System.Windows.Forms.PictureBox();
            this.ChannelNameLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ChannelNameArea)).BeginInit();
            this.SuspendLayout();
            // 
            // PlayButton
            // 
            this.PlayButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PlayButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PlayButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PlayButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.PlayButton.FlatAppearance.BorderSize = 0;
            this.PlayButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.PlayButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.PlayButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PlayButton.Location = new System.Drawing.Point(189, 321);
            this.PlayButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(37, 37);
            this.PlayButton.TabIndex = 3;
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayButton_Click);
            // 
            // ChannelNameArea
            // 
            this.ChannelNameArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChannelNameArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(100)))), ((int)(((byte)(178)))));
            this.ChannelNameArea.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChannelNameArea.Location = new System.Drawing.Point(0, 25);
            this.ChannelNameArea.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ChannelNameArea.Name = "ChannelNameArea";
            this.ChannelNameArea.Size = new System.Drawing.Size(413, 55);
            this.ChannelNameArea.TabIndex = 5;
            this.ChannelNameArea.TabStop = false;
            this.ChannelNameArea.Click += new System.EventHandler(this.ChannelNameArea_Click);
            // 
            // ChannelNameLabel
            // 
            this.ChannelNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ChannelNameLabel.AutoSize = true;
            this.ChannelNameLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(100)))), ((int)(((byte)(178)))));
            this.ChannelNameLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChannelNameLabel.Font = new System.Drawing.Font("Segoe UI Light", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ChannelNameLabel.ForeColor = System.Drawing.Color.White;
            this.ChannelNameLabel.Location = new System.Drawing.Point(154, 36);
            this.ChannelNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ChannelNameLabel.Name = "ChannelNameLabel";
            this.ChannelNameLabel.Size = new System.Drawing.Size(105, 30);
            this.ChannelNameLabel.TabIndex = 6;
            this.ChannelNameLabel.Text = "No Station";
            this.ChannelNameLabel.Click += new System.EventHandler(this.ChannelNameLabel_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(413, 441);
            this.Controls.Add(this.ChannelNameLabel);
            this.Controls.Add(this.ChannelNameArea);
            this.Controls.Add(this.PlayButton);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(429, 479);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Zin Radio Desktop 2017";
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Move += new System.EventHandler(this.MainForm_Move);
            ((System.ComponentModel.ISupportInitialize)(this.ChannelNameArea)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.PictureBox ChannelNameArea;
        private System.Windows.Forms.Label ChannelNameLabel;
    }
}

