using System.Drawing;
using System.Windows.Forms;

namespace SpotiKnob
{
    public partial class ModeOverlayForm
    {
        private System.ComponentModel.IContainer components = null;
        private RoundedPanel cardPanel;
        private Label titleLabel;
        private Label brandLabel;
        private Timer animationTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            this.cardPanel = new SpotiKnob.RoundedPanel();
            this.brandLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.cardPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // animationTimer
            // 
            this.animationTimer.Interval = 16;
            // 
            // cardPanel
            // 
            this.cardPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cardPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(147)))), ((int)(((byte)(197)))), ((int)(((byte)(253)))));
            this.cardPanel.BorderThickness = 1;
            this.cardPanel.Controls.Add(this.brandLabel);
            this.cardPanel.Controls.Add(this.titleLabel);
            this.cardPanel.CornerRadius = 20;
            this.cardPanel.GradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cardPanel.GradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cardPanel.Location = new System.Drawing.Point(0, 0);
            this.cardPanel.Name = "cardPanel";
            this.cardPanel.Size = new System.Drawing.Size(365, 106);
            this.cardPanel.TabIndex = 0;
            this.cardPanel.UseGradient = false;
            // 
            // brandLabel
            // 
            this.brandLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.brandLabel.AutoSize = true;
            this.brandLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.brandLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(255)))));
            this.brandLabel.Location = new System.Drawing.Point(266, 71);
            this.brandLabel.Name = "brandLabel";
            this.brandLabel.Size = new System.Drawing.Size(78, 19);
            this.brandLabel.TabIndex = 1;
            this.brandLabel.Text = "Spotiknob";
            // 
            // titleLabel
            // 
            this.titleLabel.Font = new System.Drawing.Font("Consolas", 18F);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.titleLabel.Location = new System.Drawing.Point(22, 21);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(312, 50);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Spotify Mode";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ModeOverlayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(12)))), ((int)(((byte)(24)))));
            this.ClientSize = new System.Drawing.Size(360, 105);
            this.Controls.Add(this.cardPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ModeOverlayForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ModeOverlayForm";
            this.TopMost = true;
            this.cardPanel.ResumeLayout(false);
            this.cardPanel.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
